namespace Application.Currencies.CompareCurrency.Dtos
{
    public record ConvertCurrencyResultDto(
        DateTime DateCaptured,
       string Code,
       decimal Amount);
}
