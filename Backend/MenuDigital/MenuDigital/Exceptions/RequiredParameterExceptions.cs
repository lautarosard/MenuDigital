namespace MenuDigital.Exceptions
{
    [Serializable]
    public class RequiredParameterException : Exception
    {
        public RequiredParameterException()
        {
        }

        public RequiredParameterException(string? message) 
            : base(message) {}

        public RequiredParameterException(string? message, Exception? innerException) 
            : base(message, innerException) {}
    }
}