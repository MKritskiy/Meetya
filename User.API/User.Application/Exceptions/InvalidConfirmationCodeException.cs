namespace Users.Application.Exceptions
{
    [Serializable]
    public class InvalidConfirmationCodeException : Exception
    {
        public InvalidConfirmationCodeException()
        {
        }

        public InvalidConfirmationCodeException(string? message) : base(message)
        {
        }

        public InvalidConfirmationCodeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}