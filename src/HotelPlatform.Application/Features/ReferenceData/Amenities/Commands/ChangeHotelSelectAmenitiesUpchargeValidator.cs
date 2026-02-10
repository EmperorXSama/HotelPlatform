using FluentValidation;
using HotelPlatform.Application.Common.Interfaces.Repositories;

namespace HotelPlatform.Application.Features.ReferenceData.Amenities.Commands;

public class ChangeHotelSelectAmenitiesUpchargeValidator : AbstractValidator<ChangeHotelSelectedAmenityUpcharge>
{
    
    public ChangeHotelSelectAmenitiesUpchargeValidator(IHotelRepository hotelRepository)
    {
        RuleFor(h => h.HotelId)
            .NotEmpty()
            .WithMessage("Please specify a valid hotel id")
            .MustAsync(async (id, cancellationToken) =>
            {
                var result = await hotelRepository.GetByIdAsync((HotelId)id, cancellationToken);
                
                return result != null;
            }).WithMessage("Please specify a valid hotel id");
    }
}