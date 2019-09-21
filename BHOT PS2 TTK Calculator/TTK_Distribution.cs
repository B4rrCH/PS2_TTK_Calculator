namespace PS2_TTK_calculator
{
    class TTKDistribution
    {
        private double ProbabilityOfBTKlessThanOrEqual(int numberOfBullets, Weapon weapon, Target target, double[] p)
        {
            MultinomialDistribution multinomialDist = new MultinomialDistribution(numberOfBullets, p);
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
        public double[] DistributionOfBulletsToKill(Weapon weapon, Target target, double[] p)
        {
            double[] resultDist = new double[weapon.magazineSize + 1];
            resultDist[0] = 0;
            for (int numberOfBullets = 1; numberOfBullets <= weapon.magazineSize; ++numberOfBullets)
            {
                resultDist[numberOfBullets] = ProbabilityOfBTKlessThanOrEqual(numberOfBullets, weapon, target, p) - ProbabilityOfBTKlessThanOrEqual(numberOfBullets - 1, weapon, target, p);
            }
            return resultDist;
        }
    }
}
