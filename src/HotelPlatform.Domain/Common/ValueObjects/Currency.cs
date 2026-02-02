using HotelPlatform.Domain.Common.Errors;

namespace HotelPlatform.Domain.Common.ValueObjects;

public sealed record Currency
{
    public string Code { get; }
    public string Symbol { get; }
    public string Name { get; }

    private Currency(string code, string symbol, string name)
    {
        Code = code;
        Symbol = symbol;
        Name = name;
    }
    private Currency()
    { }
    public static readonly Currency USD = new("USD", "US Dollar", "$");
    public static readonly Currency EUR = new("EUR", "Euro", "€");
    public static readonly Currency GBP = new("GBP", "British Pound", "£");
    public static readonly Currency MAD = new("MAD", "Moroccan Dirham", "د.م.");
    public static readonly Currency SAR = new("SAR", "Saudi Riyal", "﷼");
    public static readonly Currency AED = new("AED", "UAE Dirham", "د.إ");
    
  
    private static readonly Dictionary<string, Currency> _all = new(StringComparer.OrdinalIgnoreCase)
    {
        { USD.Code, USD },
        { EUR.Code, EUR },
        { GBP.Code, GBP },
        { MAD.Code, MAD },
        { SAR.Code, SAR },
        { AED.Code, AED }
    };
    
    public static IReadOnlyCollection<Currency> All => _all.Values.ToList().AsReadOnly();

    public static ErrorOr<Currency> FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return DomainErrors.Currency.InvalidCode(code);
        }

        return _all.TryGetValue(code, out var currency)
            ? currency
            : DomainErrors.Currency.InvalidCode(code);
    }
    public static bool IsSupported(string code) =>
        !string.IsNullOrWhiteSpace(code) && _all.ContainsKey(code);
    
    public override string ToString() => Code;
}