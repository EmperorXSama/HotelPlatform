using HotelPlatform.Domain.Common.Errors;

namespace HotelPlatform.Domain.Common.ValueObjects;

public sealed record Money
{
    public decimal Amount { get;}
    public Currency Currency { get;}
    private Money()
    { }
    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static ErrorOr<Money> Create(decimal amount, Currency currency)
    {
        if (amount < 0)
            return DomainErrors.Money.NegativeAmount;
        
        return new Money(amount, currency);
    }

    public static Money Zero(Currency currency) => new(0, currency);

    public ErrorOr<Money> Add(Money other)
    {
        if (Currency != other.Currency)
            return DomainErrors.Money.CurrencyMismatch;
        
        return Create(Amount + other.Amount, Currency);
    }

    public ErrorOr<Money> Subtract(Money other)
    {
        if (Currency != other.Currency)
            return DomainErrors.Money.CurrencyMismatch;
        
        var result = Amount - other.Amount;
        if (result < 0)
            return DomainErrors.Money.NegativeAmount;
        
        return Create(Amount - other.Amount, Currency);
    }
    public Money MultiplyBy(decimal multiplier)
    {
        var result = Amount * multiplier;
        return new Money(Math.Max(0, result), Currency);
    }
    
    public override string ToString() => $"{Currency.Symbol}{Amount:N2}";
}