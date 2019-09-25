using System;
using System.Collections.Generic;
using System.Linq;

namespace PS2_TTK_calculator
{
    public class MultinomialDistribution
    {
        private readonly int numberOfTrials;
        private readonly double[] probabilities;

        public MultinomialDistribution(int numberOfTrials, double[] probabilities_without_failure)
        {
            this.numberOfTrials = numberOfTrials;
            this.probabilities = AddFailureRateToProbabilities(probabilities_without_failure);
        }

        private double[] AddFailureRateToProbabilities(double[] probabilities_without_failure)
        {
            List<double> result = new List<double>(probabilities_without_failure);
            result.Add(1 - probabilities_without_failure.Sum());
            return result.ToArray();
        }

        public static double Binomial(int n, int k)
        {
            if (n < k)
            {
                string error_message = string.Format("Binomial(n,k) only defined for n>=k. n={0}, k={1}", n, k);
                DivideByZeroException exception = new DivideByZeroException(error_message);
                throw exception;
            }
            if (k == 0)
            {
                return 1;
            }
            if (k > n - k)
            {
                k = n - k;
            }
            return (n * Binomial(n - 1, k - 1)) / k;
        }



        private static int[] stripZeroes(int[] sequence)
        {
            return sequence.Where(i => i != 0).ToArray(); ;
        }
        public static double Multinomial(int[] sequence)
        {
            int[] zerolessSequence = stripZeroes(sequence);

            if (zerolessSequence.Any(i => i < 0))
            {
                return 0;
            }
            else
            {
                double result = Factorial(zerolessSequence.Sum());
                foreach(int k in zerolessSequence)
                {
                    result /= Factorial(k);
                }
                return result;
            }
        }


        private static double Factorial(int n)
        {
            if (n == 0 || n == 1)
            {
                return 1;
            }
            else
            {
                return n * Factorial(n - 1);
            }
        }

        private double ProbabilityOfMultinomialEvent(int[] k, double[] p)
        {
            if (k.GetLength(0) != p.GetLength(0))
            {
                ArgumentException e = new ArgumentException("Lengths don't match");
                throw e;
            }
            double pow = 1;
            for (int i = 0; i < k.GetLength(0); ++i)
            {
                pow *= Math.Pow(p[i], k[i]);
            }
            return Multinomial(k) * pow;
        }

        public double[,] GetDist()
        {
            double[,] pmf = new double[numberOfTrials + 1, numberOfTrials + 1];
            for (int i = 0; i <= numberOfTrials; ++i)
            {
                for (int j = 0; j <= numberOfTrials; ++j)
                {
                    int[] k = { i, j, numberOfTrials - i - j };
                    pmf[i, j] = ProbabilityOfMultinomialEvent(k, probabilities);
                }
            }
            return pmf;
        }
    }
}
