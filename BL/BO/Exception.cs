using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;

public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message) { }
}
public class InvalidOperationException : Exception
{
    public InvalidOperationException(string message) : base(message) { }
}
public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string message) : base(message) { }
}
public class BlGeneralDatabaseException : Exception
{
    public BlGeneralDatabaseException(string? message) : base(message) { }
    public BlGeneralDatabaseException(string? message, Exception? innerException)
     : base(message, innerException) { }
}
public class BlAlreadyExistsException : Exception
    {
        public BlAlreadyExistsException(string? message, Exception ex) : base(message) { }
    }
    public class BLDoesNotExistException : Exception
    {
        public BLDoesNotExistException(string? message, Exception ex) : base(message) { }


    }

public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
}
public class BlDeletionException : Exception
{
    public BlDeletionException(string? message, Exception ex) : base(message,ex) { }
}
[Serializable]
public class BlUpdatingException : Exception
{
    public BlUpdatingException() { }

    public BlUpdatingException(string? message) : base(message) { }

    public BlUpdatingException(string? message, Exception? innerException)
        : base(message, innerException) { }
}

public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
}


// Custom Exception for invalid address input
public class InvalidAddressException : Exception
    {
        public InvalidAddressException(string address)
            : base($"The address '{address}' is invalid or could not be processed.")
        {
        }
    }

    // Custom Exception for API related errors
    public class ApiRequestException : Exception
    {
        public ApiRequestException(string message)
            : base($"API request failed: {message}")
        {
        }
    }

    // Custom Exception for missing geolocation result
    public class GeolocationNotFoundException : Exception
    {
        public GeolocationNotFoundException(string address)
            : base($"No geolocation found for the address '{address}'.")
        {
        }
    }
/// <summary>
/// Custom exception for general application errors.
/// </summary>
public class GenralInitializationExcption : Exception
{
    public GenralInitializationExcption(string message, Exception innerException)
        : base(message, innerException) { }
}
public class BlInvalidFormatException : Exception
{
    public BlInvalidFormatException(string? message) : base(message) { }
}
public class BlApiRequestException : Exception
{
    public BlApiRequestException(string? message) : base(message) { }
}
//public class BlGeolocationNotFoundException : Exception
//{
//    public BlGeolocationNotFoundException(string? message) : base(message) { }
//}
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string? message) : base(message) { }

    public BlInvalidOperationException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
