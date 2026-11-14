using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class UserUpdateFailedException : Exception
{
    public UserUpdateFailedException(string message) : base(message) { }
}
