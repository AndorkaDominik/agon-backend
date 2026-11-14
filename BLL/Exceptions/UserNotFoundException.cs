using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() : base("User not found.") { }
}
