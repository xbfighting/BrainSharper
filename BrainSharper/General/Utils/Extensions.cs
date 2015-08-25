namespace BrainSharper.General.Utils
{
    using System;

    public static class Extensions
    {
        public static double NextDoubleInRange(this Random randomizer, double min, double max)
        {
            return (randomizer.NextDouble() * (max - min)) + min;
        }
    }
}
