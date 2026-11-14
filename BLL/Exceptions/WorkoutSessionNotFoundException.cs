using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Exceptions;

public class WorkoutSessionNotFoundException : Exception
{
    public WorkoutSessionNotFoundException(string message) : base(message) { }
}
