using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class UserTypeNotSupportedException : Exception
{
    public UserTypeNotSupportedException(string message) : base(message) { }
}
