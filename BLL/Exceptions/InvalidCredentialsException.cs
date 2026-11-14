using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message) : base(message) { }
}
