namespace Bookify.Domain.Shared;

public sealed record Currency
{
    public string Code { get; init; }

    private Currency(string code) => Code = code;

    internal static readonly Currency None = new(string.Empty);

    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");

    public static Currency FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code == code) ?? throw new ApplicationException("The currency code is invalid");
    }

    public static readonly IReadOnlyCollection<Currency> All = [Usd, Eur];
}
