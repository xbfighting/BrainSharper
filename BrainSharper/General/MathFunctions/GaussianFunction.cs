using System;

namespace BrainSharper.General.MathFunctions
{
    public class GaussianFunction
    {
        public GaussianFunction(double sigma)
        {
            Sigma = sigma;
        }

        public double Sigma { get; }

        public double GetValue(double x)
        {
            var nominator = -1*Math.Pow(x, 2);
            var denominator = 2*Math.Pow(Sigma, 2);
            return Math.Exp(nominator/denominator);
        }
    }
}