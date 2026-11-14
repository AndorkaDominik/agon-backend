using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class TrainerNotFoundException : Exception
{
    public TrainerNotFoundException(string message = "Trainer not found for the authenticated user.") : base(message) { }
}
