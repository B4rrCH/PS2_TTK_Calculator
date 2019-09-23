using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PS2_TTK_calculator
{
    public class TTKDistribution
    {
        private double ProbabilityOfBTKlessThanOrEqual(int numberOfBullets, Weapon weapon, Target target, double[] probabilities)
        {
            MultinomialDistribution multinomialDist = new MultinomialDistribution(numberOfBullets, probabilities);
            double[,] pmf = multinomialDist.GetDist();
            double result = 0;

            for (int BSs = 0; BSs < pmf.GetLength(0); ++BSs)
            {
                for (int HSs = 0; HSs < pmf.GetLength(1) - BSs; ++HSs)
                {
                    if (target.IsEnoughToKill(weapon, BSs, HSs))
                    {
                        result += pmf[BSs, HSs];
                    }
                }
            }
            return result;
        }
        public double[] DistributionOfBulletsToKill(Weapon weapon, Target target, double[] probabilities)
        {
            List<double> cummulativeResultDist = new List<double>();
            for (int index = 0; index <= weapon.magazineSize; index++)
            {
                cummulativeResultDist.Add(0);
            };
            ParallelLoopResult parallelLoopResult =
            Parallel.For(0, weapon.magazineSize + 1, numberOfBullets =>
            {
                cummulativeResultDist[numberOfBullets] = ProbabilityOfBTKlessThanOrEqual(numberOfBullets, weapon, target, probabilities);
            });
            while (!parallelLoopResult.IsCompleted)
            { }

            double[] resultDist = new double[weapon.magazineSize + 1];
            resultDist[0] = 0;

            for (int numberOfBullets = 1; numberOfBullets <= weapon.magazineSize; ++numberOfBullets)
            {
                resultDist[numberOfBullets] = cummulativeResultDist[numberOfBullets] - cummulativeResultDist[numberOfBullets-1];
            }

            return resultDist;
        }
    }
}
