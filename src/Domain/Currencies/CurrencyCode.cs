using Domain.Common;

namespace Domain.Currencies
{
    public record CurrencyCode
    {
        private static Error InvalidCodeError = new Error(ErrorCode.BadInput, "The currency code is invalid");

        public static readonly CurrencyCode Usd = new("USD", "United States Dollar");
        public static readonly CurrencyCode Eur = new("EUR", "Euro");
        public static readonly CurrencyCode Aud = new("AUD", "Australian Dollar");
        public static readonly CurrencyCode Bgn = new("BGN", "Bulgarian Lev");
        public static readonly CurrencyCode Brl = new("BRL", "Brazilian Real");
        public static readonly CurrencyCode Cad = new("CAD", "Canadian Dollar");
        public static readonly CurrencyCode Chf = new("CHF", "Swiss Franc");
        public static readonly CurrencyCode Cny = new("CNY", "Chinese Renminbi Yuan");
        public static readonly CurrencyCode Czk = new("CZK", "Czech Koruna");
        public static readonly CurrencyCode Dkk = new("DKK", "Danish Krone");
        public static readonly CurrencyCode Gbp = new("GBP", "British Pound");
        public static readonly CurrencyCode Hkd = new("HKD", "Hong Kong Dollar");
        public static readonly CurrencyCode Huf = new("HUF", "Hungarian Forint");
        public static readonly CurrencyCode Idr = new("IDR", "Indonesian Rupiah");
        public static readonly CurrencyCode Ils = new("ILS", "Israeli New Sheqel");
        public static readonly CurrencyCode Inr = new("INR", "Indian Rupee");
        public static readonly CurrencyCode Isk = new("ISK", "Icelandic Króna");
        public static readonly CurrencyCode Jpy = new("JPY", "Japanese Yen");
        public static readonly CurrencyCode Krw = new("KRW", "South Korean Won");
        public static readonly CurrencyCode Mxn = new("MXN", "Mexican Peso");
        public static readonly CurrencyCode Myr = new("MYR", "Malaysian Ringgit");
        public static readonly CurrencyCode Nok = new("NOK", "Norwegian Krone");
        public static readonly CurrencyCode Nzd = new("NZD", "New Zealand Dollar");
        public static readonly CurrencyCode Php = new("PHP", "Philippine Peso");
        public static readonly CurrencyCode Pln = new("PLN", "Polish Złoty");
        public static readonly CurrencyCode Ron = new("RON", "Romanian Leu");
        public static readonly CurrencyCode Sek = new("SEK", "Swedish Krona");
        public static readonly CurrencyCode Sgd = new("SGD", "Singapore Dollar");
        public static readonly CurrencyCode Thb = new("THB", "Thai Baht");
        public static readonly CurrencyCode Try = new("TRY", "Turkish Lira");
        public static readonly CurrencyCode Zar = new("ZAR", "South African Rand");

        private CurrencyCode(string code, string description)
        {
            Value = code;
            Description = description;
        }

        public string Value { get; init; }
        public string Description { get; init; }

        public static Result<CurrencyCode> FromCode(string code)
        {
            return All.FirstOrDefault(c => c.Value == code) ??
                   Result.Failure<CurrencyCode>(InvalidCodeError);
        }

        public static readonly IReadOnlyCollection<CurrencyCode> All = new[]
        {
            Usd,
            Eur,
            Aud,
            Bgn,
            Brl,
            Cad,
            Chf,
            Cny,
            Czk,
            Dkk,
            Gbp,
            Hkd,
            Huf,
            Idr,
            Ils,
            Inr,
            Isk,
            Jpy,
            Krw,
            Mxn,
            Myr,
            Nok,
            Nzd,
            Php,
            Pln,
            Ron,
            Sek,
            Sgd,
            Thb,
            Try,
            Zar
        };
    }
}
