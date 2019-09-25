using System;

namespace PS2_TTK_calculator
{
    public class Target
    {
        public readonly int healthPoints;
        public readonly double rangeFromShooter;
        public readonly double resistanceNWA;

        public Target(double range = 0, bool hasHAShield = false, bool isInfiltrator = false, bool hasNWA = false)
        {
            rangeFromShooter = range;
            healthPoints = 1000 + (hasHAShield ? 450 : 0)-(isInfiltrator?100:0);
            resistanceNWA = hasNWA ? 0.2 : 0;
        }

        public int DamagePerBodyShot(Weapon weapon, bool resistanceApplies = false)
        {
            if (resistanceApplies)
            {
                double unrounded_damage = weapon.RawDamagePerShot(rangeFromShooter) * (1 - resistanceNWA);
                return (int)Math.Ceiling(unrounded_damage);
            }
            else { return weapon.RawDamagePerShot(rangeFromShooter); };
        }

        public int DamagePerHeadShot(Weapon weapon)
        {
            int damagePerHeadshot = (int)Math.Ceiling(DamagePerBodyShot(weapon, false) * weapon.headshotMultiplier);
            return damagePerHeadshot;
        }

        public bool IsEnoughToKill(Weapon weapon, int bodyShots, int headShots)
        {
            int remainingHealth = healthPoints - (DamagePerBodyShot(weapon,true) * bodyShots) - (DamagePerHeadShot(weapon) * headShots);
            return remainingHealth < 0;
        }

        public int BodyshotsToKill(Weapon weapon)
        {
            return (int)Math.Ceiling(healthPoints / (1.0 * DamagePerBodyShot(weapon, true)));
        }
        public int TimeToKillBS(Weapon weapon)
        {
            return (BodyshotsToKill(weapon) - 1) * weapon.refireTime;
        }

        public int HeadshotsToKill(Weapon weapon)
        {
            return (int)Math.Ceiling(healthPoints / (weapon.headshotMultiplier * DamagePerBodyShot(weapon)));
        }
        public int TimeToKillHS(Weapon weapon)
        {
            return (HeadshotsToKill(weapon) - 1) * weapon.refireTime;
        }

    };
}