namespace PS2_TTK_calculator
{
    class Loadout
    {
        public Target target;
        public double[] probabilities;
        public Weapon weapon;

        public Loadout(Target target, Weapon weapon, double[] probabilities)
        {
            this.target = target;
            this.weapon = weapon;
            this.probabilities = probabilities;

        }

        public double[] ProbWins(Loadout enemyLoadout)
        {
            Weapon enemyWeapon = enemyLoadout.weapon;
            Target enemyTarget = enemyLoadout.target;
            double[] enemyProbabilities = enemyLoadout.probabilities;
            TTKDistribution dist = new TTKDistribution();
            double[] BTK = dist.DistributionOfBulletsToKill(weapon, enemyTarget, probabilities);
            double[] BTD = dist.DistributionOfBulletsToKill(enemyWeapon, target, enemyProbabilities);
            double[] ProbabilitiesOfPlayerWinning = { 0, 0 };

            for (int btk = 0; btk < BTK.Length; ++btk)
            {
                for (int btd = 0; btd < BTD.Length; ++btd)
                {
                    if (btk * weapon.fireRateMs < btd * enemyWeapon.fireRateMs)
                    {
                        ProbabilitiesOfPlayerWinning[0] += BTK[btk] * BTD[btd];
                    }
                    else if (btk * weapon.fireRateMs > btd * enemyWeapon.fireRateMs)
                    {
                        ProbabilitiesOfPlayerWinning[1] += BTK[btk] * BTD[btd];
                    }
                    else { };
                }
            }
            return ProbabilitiesOfPlayerWinning;
        }
    }
}
