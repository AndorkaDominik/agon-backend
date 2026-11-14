using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class RegistrationFailedException : Exception
{
    public RegistrationFailedException(string message) : base(message) { }
}
