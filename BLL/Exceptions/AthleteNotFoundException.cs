using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class AthleteNotFoundException : Exception
{
    public AthleteNotFoundException(string message = "Athlete not found for the authenticated user.") : base(message) { }
}
