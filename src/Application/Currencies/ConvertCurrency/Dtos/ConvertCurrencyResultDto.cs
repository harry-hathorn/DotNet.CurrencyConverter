namespace Application.Currencies.ConvertCurrency.Dtos
{
    public record ConvertCurrencyResultDto(
        DateTime DateCaptured,
        string Code,
        decimal Amount);
}
