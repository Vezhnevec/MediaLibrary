using MediaLibrary.Interface;

namespace MediaLibrary.Models
{
    public record ImportResult(
        int LineNumber,
        IMedia? Item,
        string? ErrorMessage
        )
    {
        public static ImportResult Success(int lineNumber, IMedia item)
            => new(lineNumber, item, null);

        public static ImportResult Failure(int lineNumber, string errorMessage)
            => new(lineNumber, null, errorMessage);

        public bool IsSuccess => Item != null;
    }
}
