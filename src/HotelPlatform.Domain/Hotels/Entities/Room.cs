using HotelPlatform.Domain.Common.Errors;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Common.ValueObjects;

namespace HotelPlatform.Domain.Hotels.Entities;

public class Room : BaseEntity<RoomId>
{
    private readonly List<RoomPicture> _pictures = [];
    private readonly List<RoomSelectedAmenity> _amenities = [];

    public string Name { get; private set; } = string.Empty;
    public RoomTypeDefinitionId RoomTypeId { get; private set; }
    public int Capacity { get; private set; }
    public Money BasePrice { get; private set; } = null!;
    public string? Description { get; private set; }

    public IReadOnlyCollection<RoomPicture> Pictures => _pictures.AsReadOnly();
    public IReadOnlyCollection<RoomSelectedAmenity> Amenities => _amenities.AsReadOnly();

    private Room() : base() { }

    internal Room(
        RoomId id,
        string name,
        RoomTypeDefinitionId roomTypeId,
        int capacity,
        Money basePrice,
        string? description) : base(id)
    {
        Name = name;
        RoomTypeId = roomTypeId;
        Capacity = capacity;
        BasePrice = basePrice;
        Description = description;
    }

    internal static ErrorOr<Room> Create(
        string name,
        RoomTypeDefinitionId roomTypeId,
        int capacity,
        Money basePrice,
        string? description = null)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(DomainErrors.Room.NameRequired);

        if (capacity <= 0)
            errors.Add(DomainErrors.Room.InvalidCapacity);

        if (basePrice.Amount <= 0)
            errors.Add(DomainErrors.Room.InvalidBasePrice);

        if (errors.Count > 0)
            return errors;

        return new Room(
            RoomId.New(),
            name.Trim(),
            roomTypeId,
            capacity,
            basePrice,
            description?.Trim());
    }

    public ErrorOr<Updated> Update(
        string name,
        RoomTypeDefinitionId roomTypeId,
        int capacity,
        Money basePrice,
        string? description)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(DomainErrors.Room.NameRequired);

        if (capacity <= 0)
            errors.Add(DomainErrors.Room.InvalidCapacity);

        if (basePrice.Amount <= 0)
            errors.Add(DomainErrors.Room.InvalidBasePrice);

        if (errors.Count > 0)
            return errors;

        Name = name.Trim();
        RoomTypeId = roomTypeId;
        Capacity = capacity;
        BasePrice = basePrice;
        Description = description?.Trim();

        return Result.Updated;
    }

    #region Pictures

    public ErrorOr<Updated> AddPicture(RoomPicture picture)
    {
        if (_pictures.Count == 0)
        {
            _pictures.Add(picture.AsMain());
        }
        else
        {
            _pictures.Add(picture.AsSecondary());
        }

        return Result.Updated;
    }

    public ErrorOr<Updated> RemovePicture(StoredFileId storedFileId)
    {
        var picture = _pictures.FirstOrDefault(p => p.StoredFileId == storedFileId);
        if (picture is null)
            return DomainErrors.Room.PictureNotFound;

        _pictures.Remove(picture);

        if (picture.IsMain && _pictures.Count > 0)
        {
            var newMain = _pictures[0].AsMain();
            _pictures[0] = newMain;
        }

        return Result.Updated;
    }

    public ErrorOr<Updated> SetMainPicture(StoredFileId storedFileId)
    {
        var pictureIndex = _pictures.FindIndex(p => p.StoredFileId == storedFileId);
        if (pictureIndex == -1)
            return DomainErrors.Room.PictureNotFound;

        for (int i = 0; i < _pictures.Count; i++)
        {
            _pictures[i] = i == pictureIndex
                ? _pictures[i].AsMain()
                : _pictures[i].AsSecondary();
        }

        return Result.Updated;
    }

    #endregion

    #region Amenities

    public ErrorOr<Updated> AddAmenity(RoomSelectedAmenity amenity)
    {
        if (_amenities.Any(a => a.AmenityDefinitionId == amenity.AmenityDefinitionId))
            return DomainErrors.Room.DuplicateAmenity;

        _amenities.Add(amenity);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveAmenity(RoomAmenityDefinitionId amenityDefinitionId)
    {
        var amenity = _amenities.FirstOrDefault(a => a.AmenityDefinitionId == amenityDefinitionId);
        if (amenity is null)
            return DomainErrors.Room.AmenityNotFound;

        _amenities.Remove(amenity);
        return Result.Updated;
    }

    public ErrorOr<Updated> UpdateAmenityUpcharge(
        RoomAmenityDefinitionId amenityDefinitionId,
        Upcharge newUpcharge)
    {
        var index = _amenities.FindIndex(a => a.AmenityDefinitionId == amenityDefinitionId);
        if (index == -1)
            return DomainErrors.Room.AmenityNotFound;

        _amenities[index] = _amenities[index].WithUpcharge(newUpcharge);
        return Result.Updated;
    }

    #endregion

    public bool HasPicture(StoredFileId storedFileId) =>
        _pictures.Any(p => p.StoredFileId == storedFileId);
}