namespace Nerdudes.Models.DTO
{
    public record GeneralResponse
    {
        public string Message { get; init; } = string.Empty;
        public string Reason { get; init; } = string.Empty;
    }
}
