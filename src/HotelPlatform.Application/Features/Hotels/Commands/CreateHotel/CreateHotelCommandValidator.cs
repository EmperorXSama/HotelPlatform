// Application/Features/Hotels/Commands/CreateHotel/CreateHotelCommandValidator.cs
using FluentValidation;
using HotelPlatform.Domain.Hotels;

namespace HotelPlatform.Application.Features.Hotels.Commands.CreateHotel;

public sealed class CreateHotelCommandValidator : AbstractValidator<CreateHotelCommand>
{
    public CreateHotelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Hotel name is required.")
            .MaximumLength(Hotel.NameMaxLength)
            .WithMessage($"Hotel name cannot exceed {Hotel.NameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(Hotel.DescriptionMaxLength)
            .WithMessage($"Description cannot exceed {Hotel.DescriptionMaxLength} characters.")
            .When(x => x.Description is not null);

        // Pictures validation
        RuleFor(x => x.Pictures)
            .NotEmpty()
            .WithMessage("At least one picture is required.");

        RuleFor(x => x.Pictures)
            .Must(HaveExactlyOneMainPicture)
            .WithMessage("Exactly one picture must be marked as main.")
            .When(x => x.Pictures is not null && x.Pictures.Count > 0);

        RuleForEach(x => x.Pictures)
            .ChildRules(picture =>
            {
                picture.RuleFor(p => p.StoredFileId)
                    .NotEmpty()
                    .WithMessage("StoredFileId is required for each picture.");

                picture.RuleFor(p => p.AltText)
                    .MaximumLength(500)
                    .When(p => p.AltText is not null);
            });

        // Address validation
        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.Street)
                .NotEmpty()
                .WithMessage("Street is required when providing an address.")
                .MaximumLength(500);

            RuleFor(x => x.Address!.City)
                .NotEmpty()
                .WithMessage("City is required when providing an address.")
                .MaximumLength(200);

            RuleFor(x => x.Address!.Country)
                .NotEmpty()
                .WithMessage("Country is required when providing an address.")
                .MaximumLength(100);

            RuleFor(x => x.Address!.PostalCode)
                .MaximumLength(20)
                .When(x => x.Address!.PostalCode is not null);

            RuleFor(x => x.Address!.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.")
                .When(x => x.Address!.Latitude.HasValue);

            RuleFor(x => x.Address!.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.")
                .When(x => x.Address!.Longitude.HasValue);

            RuleFor(x => x.Address!.Longitude)
                .NotNull()
                .WithMessage("Longitude is required when latitude is provided.")
                .When(x => x.Address!.Latitude.HasValue);

            RuleFor(x => x.Address!.Latitude)
                .NotNull()
                .WithMessage("Latitude is required when longitude is provided.")
                .When(x => x.Address!.Longitude.HasValue);
        });
    }

    private static bool HaveExactlyOneMainPicture(List<CreateHotelPictureDto> pictures)
    {
        return pictures.Count(p => p.IsMain) == 1;
    }
}