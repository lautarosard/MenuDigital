namespace Application.Exceptions
{
    [Serializable]
    public class InvalidateParameterException : Exception
    {
        public InvalidateParameterException()
        {
        }

        public InvalidateParameterException(string? message) 
            : base(message) {}

        public InvalidateParameterException(string? message, Exception? innerException) 
            : base(message, innerException) {}
    }
}