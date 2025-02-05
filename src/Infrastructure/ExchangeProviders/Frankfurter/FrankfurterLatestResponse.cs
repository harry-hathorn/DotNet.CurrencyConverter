namespace Infrastructure.ExchangeProviders.Frankfurter
{
    internal class FrankfurterLatestResponse
    {
        public float Amount { get; set; }
        public string _base { get; set; }
        public string Date { get; set; }
        public Rates Rates { get; set; }
    }

    internal class Rates
    {
        public float AUD { get; set; }
        public float BGN { get; set; }
        public float BRL { get; set; }
        public float CAD { get; set; }
        public float CHF { get; set; }
        public float CNY { get; set; }
        public float CZK { get; set; }
        public float DKK { get; set; }
        public float GBP { get; set; }
        public float HKD { get; set; }
        public float HUF { get; set; }
        public int IDR { get; set; }
        public float ILS { get; set; }
        public float INR { get; set; }
        public float ISK { get; set; }
        public float JPY { get; set; }
        public float KRW { get; set; }
        public float MXN { get; set; }
        public float MYR { get; set; }
        public float NOK { get; set; }
        public float NZD { get; set; }
        public float PHP { get; set; }
        public float PLN { get; set; }
        public float RON { get; set; }
        public float SEK { get; set; }
        public float SGD { get; set; }
        public float THB { get; set; }
        public float TRY { get; set; }
        public float USD { get; set; }
        public float ZAR { get; set; }
    }

}
