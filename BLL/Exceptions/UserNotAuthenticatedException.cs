using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class UserNotAuthenticatedException : Exception
{
    public UserNotAuthenticatedException() : base("User is not authenticated.") { }
}
