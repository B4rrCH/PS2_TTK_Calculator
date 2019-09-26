using System.Collections.Generic;
using System.Linq;

namespace PS2_TTK_calculator
{
    public class Loadout
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

        public double[] ProbWinsAgainst(Loadout enemyLoadout)
        {
            Weapon enemyWeapon = enemyLoadout.weapon;
            Target enemyTarget = enemyLoadout.target;
            double[] enemyProbabilities = enemyLoadout.probabilities;

            List<double> BTK = new List<double>(BTKDistribution.DistributionOfBulletsToKill(weapon, enemyTarget, probabilities));
            BTK.Add(1 - BTK.Sum());
            List<double> BTD = new List<double>(BTKDistribution.DistributionOfBulletsToKill(enemyWeapon, target, enemyProbabilities));
            BTD.Add(1 - BTD.Sum());

            int[] bulletTravelTime = { (int)enemyTarget.rangeFromShooter / weapon.muzzleVelocity, (int)target.rangeFromShooter/ enemyWeapon.muzzleVelocity };

            double[] ProbabilitiesOfPlayerWinning = { 0, 0, 0 };

            for (int btk = 1; btk < BTK.Count; ++btk)
            {
                for (int btd = 1; btd < BTD.Count; ++btd)
                {
                    // Both players still have ammo
                    if (btk < BTK.Count - 1 && btd < BTD.Count - 1)
                    {
                        if ((btk - 1)
                            * weapon.refireTime
                            +bulletTravelTime[0]
                            < (btd - 1) 
                            * enemyWeapon.refireTime
                            +bulletTravelTime[1])
                        {
                            ProbabilitiesOfPlayerWinning[0] += BTK[btk] * BTD[btd];
                        }
                        else if ((btk - 1)
                            * weapon.refireTime
                            + bulletTravelTime[0]
                            > (btd - 1)
                            * enemyWeapon.refireTime
                            + bulletTravelTime[1])
                        {
                            ProbabilitiesOfPlayerWinning[1] += BTK[btk] * BTD[btd];
                        }
                        else { ProbabilitiesOfPlayerWinning[2] += BTK[btk] * BTD[btd]; };
                    }
                    // Enemy target has run out of bullets, this target still has ammo
                    else if (btk < BTK.Count - 1 && btd == BTD.Count - 1)
                    {
                        ProbabilitiesOfPlayerWinning[0] += BTK[btk] * BTD[btd];
                    }
                    // This target has run out of bullets, enemy still has ammo
                    else if (btk == BTK.Count - 1 && btd < BTD.Count - 1)
                    {
                        ProbabilitiesOfPlayerWinning[1] += BTK[btk] * BTD[btd];
                    }
                    // Both players are out of bullets
                    else
                    {
                        // Do nothing, not a kill trade
                    }
                }
            }
            return ProbabilitiesOfPlayerWinning;
        }

        public double[] ExpectedTTKandTTD(Loadout enemyLoadout)
        {
            Weapon enemyWeapon = enemyLoadout.weapon;
            Target enemyTarget = enemyLoadout.target;
            double[] enemyProbabilities = enemyLoadout.probabilities;

            List<double> BTK = new List<double>(BTKDistribution.DistributionOfBulletsToKill(weapon, enemyTarget, probabilities));
            BTK.Add(1 - BTK.Sum());
            List<double> BTD = new List<double>(BTKDistribution.DistributionOfBulletsToKill(enemyWeapon, target, enemyProbabilities));
            BTD.Add(1 - BTD.Sum());

            int[] bulletTravelTime = { (int)enemyTarget.rangeFromShooter / weapon.muzzleVelocity, (int)target.rangeFromShooter / enemyWeapon.muzzleVelocity };

            double[] expectedTTKandTTD = { 0, 0 };

            for (int btk = 0; btk < BTK.Count; ++btk)
            {
                expectedTTKandTTD[0] += (weapon.refireTime
                                         * (btk - 1)
                                         * BTK[btk])
                                        + (bulletTravelTime[0]);
            }
            for (int btd = 0; btd < BTD.Count; ++btd)
            {
                expectedTTKandTTD[1] += (enemyWeapon.refireTime 
                                         * (btd - 1) 
                                         * BTD[btd])
                                        + (bulletTravelTime[1]);
            }
            return expectedTTKandTTD;
        }

}
}
