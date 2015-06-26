using System;

namespace BrainSharper.General.Exceptions.Data
{
    public class InvalidColumnNamesException : Exception
    {
        public const string Name = "Invalid column names passed to data frame";

        public InvalidColumnNamesException() : base(Name)
        {
        }
    }
}
