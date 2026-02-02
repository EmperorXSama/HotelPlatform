using HotelPlatform.Domain.Common.Errors;
using HotelPlatform.Domain.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HotelPlatform.Infrastructure.Data.Converters;

public class CurrencyConverter: ValueConverter<Currency, string>
{
    public CurrencyConverter()
    :base(
        currency => currency.Code,
        code => Currency.FromCode(code).Value
        )
    {
        
    }
}