using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class LogoutFailedException : Exception
{
    public LogoutFailedException(string message) : base(message)
    {
    }
}
