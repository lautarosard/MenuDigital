namespace Domain.Exceptions
{
    public class OrderPriceException : Exception
    {
        public OrderPriceException() : base() { }
        public OrderPriceException(string message) : base(message) { }
        public OrderPriceException(string message, Exception inner) : base(message, inner) { }
        
    }
}
