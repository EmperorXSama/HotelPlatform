using HotelPlatform.Domain.Common.Errors;
using HotelPlatform.Domain.Enums;

namespace HotelPlatform.Domain.Common.ValueObjects;

public sealed record Upcharge
{
    public UpchargeType Type { get; }
    public decimal Amount { get; }
    public Currency? Currency { get; }
    private Upcharge()
    { }
    private Upcharge(UpchargeType type, decimal amount, Currency? currency)
    {
        Type = type;
        Amount = amount;
        Currency = currency;
    }

    public static ErrorOr<Upcharge> CreateFlat(Money money)
    {
        return new Upcharge(UpchargeType.Flat, money.Amount, money.Currency);
    }
    public static ErrorOr<Upcharge> CreateFlat(decimal amount, Currency currency)
    {
        if (amount < 0)
            return DomainErrors.Upcharge.NegativeAmount;

        return new Upcharge(UpchargeType.Flat, amount, currency);
    }

    public static ErrorOr<Upcharge> CreatePercentage(decimal percentage)
    {
        if (percentage < 0 || percentage >1)
            return DomainErrors.Upcharge.InvalidPercentage;

        return new Upcharge(UpchargeType.PercentagePerNight, percentage, null);
    }

    public Money CalculateFor(Money basePrice, int nights = 1)
    {
        return Type switch
        {
            UpchargeType.Flat => Money.Create(Amount, Currency!).Value,
            UpchargeType.PercentagePerNight => basePrice.MultiplyBy(Amount * nights),
            _ => Money.Zero(basePrice.Currency)
        };
    }
    public bool IsFlat => Type == UpchargeType.Flat;
    public bool IsPercentage => Type == UpchargeType.PercentagePerNight;
    public override string ToString() => Type switch
    {
        UpchargeType.Flat => $"{Currency?.Symbol}{Amount:N2}",
        UpchargeType.PercentagePerNight => $"{Amount:P0}/night",
        _ => string.Empty
    };
}