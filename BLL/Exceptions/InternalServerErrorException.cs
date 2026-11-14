using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class InternalServerErrorException : Exception
{
    public InternalServerErrorException(string message) : base(message) { }
}
