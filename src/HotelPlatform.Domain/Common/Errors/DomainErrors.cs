using ErrorOr;

namespace HotelPlatform.Domain.Common.Errors;

public static class DomainErrors
{
    public static class Money
    {
        public static Error NegativeAmount => Error.Validation(
            code: "Money.NegativeAmount",
            description: "Amount cannot be negative.");

        public static Error CurrencyMismatch => Error.Validation(
            code: "Money.CurrencyMismatch",
            description: "Cannot perform operation on money with different currencies.");
    }

    public static class Currency
    {
        public static Error InvalidCode(string code) => Error.Validation(
            code: "Currency.InvalidCode",
            description: $"Currency code '{code}' is not supported.");
    }

    public static class Upcharge
    {
        public static Error InvalidPercentage => Error.Validation(
            code: "Upcharge.InvalidPercentage",
            description: "Percentage must be between 0 and 1 (e.g., 0.10 for 10%).");

        public static Error NegativeAmount => Error.Validation(
            code: "Upcharge.NegativeAmount",
            description: "Upcharge amount cannot be negative.");
    }

    public static class Address
    {
        public static Error StreetRequired => Error.Validation(
            code: "Address.StreetRequired",
            description: "Street is required.");

        public static Error CityRequired => Error.Validation(
            code: "Address.CityRequired",
            description: "City is required.");

        public static Error CountryRequired => Error.Validation(
            code: "Address.CountryRequired",
            description: "Country is required.");
    }

    public static class Coordinates
    {
        public static Error InvalidLatitude => Error.Validation(
            code: "Coordinates.InvalidLatitude",
            description: "Latitude must be between -90 and 90.");

        public static Error InvalidLongitude => Error.Validation(
            code: "Coordinates.InvalidLongitude",
            description: "Longitude must be between -180 and 180.");
    }

    public static class Hotel
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            code: "Hotel.NotFound",
            description: $"Hotel with ID '{id}' was not found.");

        public static Error NameRequired => Error.Validation(
            code: "Hotel.NameRequired",
            description: "Hotel name is required.");

        public static Error NameTooLong(int maxLength) => Error.Validation(
            code: "Hotel.NameTooLong",
            description: $"Hotel name cannot exceed {maxLength} characters.");

        public static Error DuplicateAmenity => Error.Conflict(
            code: "Hotel.DuplicateAmenity",
            description: "This amenity is already added to the hotel.");

        public static Error AmenityNotFound => Error.NotFound(
            code: "Hotel.AmenityNotFound",
            description: "The specified amenity was not found on this hotel.");

        public static Error PictureNotFound => Error.NotFound(
            code: "Hotel.PictureNotFound",
            description: "The specified picture was not found on this hotel.");

        public static Error MustHaveMainPicture => Error.Validation(
            code: "Hotel.MustHaveMainPicture",
            description: "Hotel must have exactly one main picture.");

        public static Error CannotPublishWithoutRequirements => Error.Validation(
            code: "Hotel.CannotPublishWithoutRequirements",
            description: "Hotel must have a name, address, and at least one picture to be published.");

        public static Error InvalidStatusTransition(string from, string to) => Error.Validation(
            code: "Hotel.InvalidStatusTransition",
            description: $"Cannot transition from {from} to {to}.");

        public static Error AlreadyPublished => Error.Conflict(
            code: "Hotel.AlreadyPublished",
            description: "Hotel is already published.");

        public static Error AlreadyArchived => Error.Conflict(
            code: "Hotel.AlreadyArchived",
            description: "Hotel is already archived.");

        public static Error CannotModifyArchivedHotel => Error.Validation(
            code: "Hotel.CannotModifyArchivedHotel",
            description: "Cannot modify an archived hotel.");
    }

    public static class Room
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            code: "Room.NotFound",
            description: $"Room with ID '{id}' was not found.");

        public static Error NameRequired => Error.Validation(
            code: "Room.NameRequired",
            description: "Room name is required.");

        public static Error InvalidCapacity => Error.Validation(
            code: "Room.InvalidCapacity",
            description: "Room capacity must be greater than zero.");

        public static Error InvalidBasePrice => Error.Validation(
            code: "Room.InvalidBasePrice",
            description: "Room base price must be greater than zero.");

        public static Error DuplicateAmenity => Error.Conflict(
            code: "Room.DuplicateAmenity",
            description: "This amenity is already added to the room.");

        public static Error AmenityNotFound => Error.NotFound(
            code: "Room.AmenityNotFound",
            description: "The specified amenity was not found on this room.");

        public static Error PictureNotFound => Error.NotFound(
            code: "Room.PictureNotFound",
            description: "The specified picture was not found on this room.");
    }

    public static class Rating
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            code: "Rating.NotFound",
            description: $"Rating with ID '{id}' was not found.");

        public static Error InvalidScore => Error.Validation(
            code: "Rating.InvalidScore",
            description: "Rating score must be between 1 and 5.");

        public static Error AlreadyRated => Error.Conflict(
            code: "Rating.AlreadyRated",
            description: "User has already rated this hotel.");

        public static Error CannotRateOwnHotel => Error.Validation(
            code: "Rating.CannotRateOwnHotel",
            description: "Hotel owners cannot rate their own hotels.");
    }

    public static class User
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            code: "User.NotFound",
            description: $"User with ID '{id}' was not found.");

        public static Error EmailRequired => Error.Validation(
            code: "User.EmailRequired",
            description: "Email is required.");

        public static Error InvalidEmail => Error.Validation(
            code: "User.InvalidEmail",
            description: "Email format is invalid.");

        public static Error AlreadyExists => Error.Conflict(
            code: "User.AlreadyExists",
            description: "A user with this identity already exists.");
    }

    public static class StoredFile
    {
        public static Error NotFound(Guid id) => Error.NotFound(
            code: "StoredFile.NotFound",
            description: $"Stored file with ID '{id}' was not found.");

        public static Error StillInUse => Error.Conflict(
            code: "StoredFile.StillInUse",
            description: "Cannot delete file because it is still in use. Remove it from all hotels and rooms first.");

        public static Error InvalidContentType => Error.Validation(
            code: "StoredFile.InvalidContentType",
            description: "File content type is not supported.");
    }
}