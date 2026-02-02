
using HotelPlatform.Domain.Common.Errors;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Common.ValueObjects;
using HotelPlatform.Domain.Enums;
using HotelPlatform.Domain.Hotels.Entities;
using HotelPlatform.Domain.Hotels.Events;

namespace HotelPlatform.Domain.Hotels;

public class Hotel : AggregateRoot<HotelId>
{
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 5000;

    private readonly List<HotelPicture> _pictures = [];
    private readonly List<HotelSelectedAmenity> _amenities = [];
    private readonly List<Room> _rooms = [];

    public UserId OwnerId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public HotelStatus Status { get; private set; }
    public Address? Address { get; private set; }
    public AggregatedRating AggregatedRating { get; private set; } = AggregatedRating.Empty;

    public IReadOnlyCollection<HotelPicture> Pictures => _pictures.AsReadOnly();
    public IReadOnlyCollection<HotelSelectedAmenity> Amenities => _amenities.AsReadOnly();
    public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();

    private Hotel() : base() { }

    private Hotel(
        HotelId id,
        UserId ownerId,
        string name,
        string? description) : base(id)
    {
        OwnerId = ownerId;
        Name = name;
        Description = description;
        Status = HotelStatus.Draft;
    }

    public static ErrorOr<Hotel> Create(
        UserId ownerId,
        string name,
        string? description = null)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(DomainErrors.Hotel.NameRequired);
        else if (name.Length > NameMaxLength)
            errors.Add(DomainErrors.Hotel.NameTooLong(NameMaxLength));

        if (errors.Count > 0)
            return errors;

        var hotel = new Hotel(
            HotelId.New(),
            ownerId,
            name.Trim(),
            description?.Trim());

        hotel.AddDomainEvent(new HotelCreatedEvent(hotel.Id, hotel.OwnerId));

        return hotel;
    }

    #region Core Updates

    public ErrorOr<Updated> UpdateDetails(string name, string? description)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(DomainErrors.Hotel.NameRequired);
        else if (name.Length > NameMaxLength)
            errors.Add(DomainErrors.Hotel.NameTooLong(NameMaxLength));

        if (errors.Count > 0)
            return errors;

        Name = name.Trim();
        Description = description?.Trim();
        SetUpdated();

        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateAddress(Address address)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        Address = address;
        SetUpdated();

        return Result.Updated;
    }

    #endregion

    #region Status Management

    public ErrorOr<Updated> Publish()
    {
        if (Status == HotelStatus.Published)
            return DomainErrors.Hotel.AlreadyPublished;

        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.InvalidStatusTransition(
                HotelStatus.Archived.ToString(),
                HotelStatus.Published.ToString());

        if (!CanPublish())
            return DomainErrors.Hotel.CannotPublishWithoutRequirements;

        Status = HotelStatus.Published;
        SetUpdated();

        AddDomainEvent(new HotelPublishedEvent(Id, OwnerId));

        return Result.Updated;
    }

    public ErrorOr<Updated> Archive()
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.AlreadyArchived;

        if (Status == HotelStatus.Draft)
            return DomainErrors.Hotel.InvalidStatusTransition(
                HotelStatus.Draft.ToString(),
                HotelStatus.Archived.ToString());

        Status = HotelStatus.Archived;
        SetUpdated();

        AddDomainEvent(new HotelArchivedEvent(Id, OwnerId));

        return Result.Updated;
    }

    private bool CanPublish()
    {
        return !string.IsNullOrWhiteSpace(Name)
               && Address is not null
               && _pictures.Count > 0;
    }

    #endregion

    #region Pictures

    public ErrorOr<Updated> AddPicture(HotelPicture picture)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        if (_pictures.Count == 0)
        {
            _pictures.Add(picture.AsMain());
        }
        else
        {
            _pictures.Add(picture.AsSecondary());
        }

        SetUpdated();
        return Result.Updated;
    }

    public ErrorOr<Updated> RemovePicture(StoredFileId storedFileId)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        var picture = _pictures.FirstOrDefault(p => p.StoredFileId == storedFileId);
        if (picture is null)
            return DomainErrors.Hotel.PictureNotFound;

        _pictures.Remove(picture);

        if (picture.IsMain && _pictures.Count > 0)
        {
            var newMain = _pictures[0].AsMain();
            _pictures[0] = newMain;
        }

        SetUpdated();
        return Result.Updated;
    }

    public ErrorOr<Updated> SetMainPicture(StoredFileId storedFileId)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        var pictureIndex = _pictures.FindIndex(p => p.StoredFileId == storedFileId);
        if (pictureIndex == -1)
            return DomainErrors.Hotel.PictureNotFound;

        for (int i = 0; i < _pictures.Count; i++)
        {
            _pictures[i] = i == pictureIndex
                ? _pictures[i].AsMain()
                : _pictures[i].AsSecondary();
        }

        SetUpdated();
        return Result.Updated;
    }

    #endregion

    #region Amenities

    public ErrorOr<Updated> AddAmenity(HotelSelectedAmenity amenity)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        if (_amenities.Any(a => a.AmenityDefinitionId == amenity.AmenityDefinitionId))
            return DomainErrors.Hotel.DuplicateAmenity;

        _amenities.Add(amenity);
        SetUpdated();

        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveAmenity(HotelAmenityDefinitionId amenityDefinitionId)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        var amenity = _amenities.FirstOrDefault(a => a.AmenityDefinitionId == amenityDefinitionId);
        if (amenity is null)
            return DomainErrors.Hotel.AmenityNotFound;

        _amenities.Remove(amenity);
        SetUpdated();

        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateAmenityUpcharge(
        HotelAmenityDefinitionId amenityDefinitionId,
        Upcharge newUpcharge)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        var index = _amenities.FindIndex(a => a.AmenityDefinitionId == amenityDefinitionId);
        if (index == -1)
            return DomainErrors.Hotel.AmenityNotFound;

        _amenities[index] = _amenities[index].WithUpcharge(newUpcharge);
        SetUpdated();

        return Result.Updated;
    }

    #endregion

    #region Rooms

    public ErrorOr<Room> AddRoom(
        string name,
        RoomTypeDefinitionId roomTypeId,
        int capacity,
        Money basePrice,
        string? description = null)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        var roomResult = Room.Create(name, roomTypeId, capacity, basePrice, description);
        if (roomResult.IsError)
            return roomResult.Errors;

        _rooms.Add(roomResult.Value);
        SetUpdated();

        return roomResult.Value;
    }

    public ErrorOr<Updated> RemoveRoom(RoomId roomId)
    {
        if (Status == HotelStatus.Archived)
            return DomainErrors.Hotel.CannotModifyArchivedHotel;

        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room is null)
            return DomainErrors.Room.NotFound(roomId.Value);

        _rooms.Remove(room);
        SetUpdated();

        return Result.Updated;
    }

    public Room? GetRoom(RoomId roomId) => _rooms.FirstOrDefault(r => r.Id == roomId);

    #endregion

    #region Rating

    public void UpdateAggregatedRating(decimal averageScore, int totalCount)
    {
        AggregatedRating = AggregatedRating.Create(averageScore, totalCount);
        SetUpdated();
    }

    #endregion

    #region Query Helpers

    public bool HasPicture(StoredFileId storedFileId) =>
        _pictures.Any(p => p.StoredFileId == storedFileId);

    public bool HasPictureInUse(StoredFileId storedFileId) =>
        HasPicture(storedFileId) || _rooms.Any(r => r.HasPicture(storedFileId));

    public HotelPicture? GetMainPicture() =>
        _pictures.FirstOrDefault(p => p.IsMain);

    #endregion
}