using Newtonsoft.Json.Linq;
using System;

namespace PS2_TTK_calculator
{
    public class Weapon
    {
        public string weaponName;
        public int damageMax;
        public int damageMin;
        public double damageMaxRange;
        public double damageMinRange;
        public int refireTime;
        public double headshotMultiplier;
        public int magazineSize;
        public double muzzleVelocityMpMs;
        private string categoryName;

        public string CategoryName { get => categoryName;}
        public string WeaponName { get => weaponName;}
        public int DamageMax { get => damageMax;}
        public int MuzzleVelocityMpS { get => (int)(muzzleVelocityMpMs * 1000); }

        public Weapon() { }

        public Weapon(JToken weaponJToken)
        {
            try
            {
                weaponName = (string)weaponJToken["name"]["en"];

                damageMax = (weaponJToken["item_id_join_fire_mode"].Value<int?>("damage_max") ??
                            weaponJToken["item_id_join_fire_mode"].Value<int?>("damage") ??
                            1) 
                            *
                            (weaponJToken["item_id_join_fire_mode"].Value<int?>("pellets_per_shot") ??
                            1);

                damageMin = (weaponJToken["item_id_join_fire_mode"].Value<int?>("damage_min") ??
                            weaponJToken["item_id_join_fire_mode"].Value<int?>("damage") ??
                            1)
                            *
                            (weaponJToken["item_id_join_fire_mode"].Value<int?>("pellets_per_shot") ??
                            1);

                damageMaxRange = weaponJToken["item_id_join_fire_mode"].Value<double?>("damage_max_range") ?? 500;

                damageMinRange = weaponJToken["item_id_join_fire_mode"].Value<double?>("damage_min_range") ?? 501;

                refireTime = (weaponJToken["fire_mode_2"]["weapon_id_join_weapon_to_fire_group"][0]["fire_group_id_join_fire_group_to_fire_mode"][0]["fire_mode_id_join_fire_mode_2"][0].Value<int?>("fire_refire_ms") ?? 1)+
                              (weaponJToken["fire_mode_2"]["weapon_id_join_weapon_to_fire_group"][0]["fire_group_id_join_fire_group"].Value<int?>("chamber_duration_ms") ??0 );

                headshotMultiplier = 1.0 + weaponJToken["fire_mode_2"]["weapon_id_join_weapon_to_fire_group"][0]["fire_group_id_join_fire_group_to_fire_mode"][0]["fire_mode_id_join_fire_mode_2"][0].Value<double?>("damage_head_multiplier") ?? 2.0;

                magazineSize = weaponJToken["fire_mode_2"]["weapon_id_join_weapon_ammo_slot"][0].Value<int?>("clip_size") ?? 30;

                muzzleVelocityMpMs = (weaponJToken["item_id_join_fire_mode"].Value<double?>("muzzle_velocity") ?? 600) /1000;

                categoryName = weaponCategoryIDtoString(weaponJToken.Value<int?>("item_category_id")??0);
            }
            catch
            {
                string errorMessage = string.Format("{0}", weaponJToken.ToString());
                throw new FormatException(errorMessage);
            }
        }

        public override string ToString()
        {
            return this.weaponName;
        }

        private string weaponCategoryIDtoString(int weaponCategoryID)
        {
            switch (weaponCategoryID)
            {
                case 3: return "Pistol";
                case 4: return "Shotgun";
                case 5: return "SMG";
                case 6: return "LMG";
                case 7: return "Assault rifle";
                case 8: return "Carbine";
                case 11: return "Sniper rifle";
                case 12: return "Scout rifle";
                case 14: return "Heavy weapon";
                case 19: return "Battle rifle";
                case 24: return "Crossbow";

                default: return "Unknown";
            }
        }

        public override bool Equals(object value)
        {
            if (value is null)
            {
                return false;
            }
            if (Object.ReferenceEquals(this, value))
            {
                return true;
            }
            if (value.GetType() != this.GetType())
            {
                return false;
            }
            return IsEqual((Weapon)value);
        }
        public bool Equals(Weapon expectedWeapon)
        {
            if (expectedWeapon is null)
            {
                return false;
            }
            if (Object.ReferenceEquals(this, expectedWeapon))
            {
                return true;
            }
            return IsEqual(expectedWeapon);
        }
        private bool IsEqual(Weapon expectedWeapon)
        {
            bool areEqual = expectedWeapon.weaponName == weaponName &&
                expectedWeapon.damageMax == damageMax &&
                expectedWeapon.damageMin == damageMin &&
                expectedWeapon.damageMaxRange == damageMaxRange &&
                expectedWeapon.damageMinRange == damageMinRange &&
                expectedWeapon.refireTime == refireTime &&
                expectedWeapon.headshotMultiplier == headshotMultiplier;
            return areEqual;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int RawDamagePerShot(double targetRange)
        {
            if (targetRange <= damageMaxRange)
            {
                return damageMax;
            }
            else
            if (targetRange >= damageMinRange)
            {
                return damageMin;
            }
            else
            {
                double inclination = (damageMin - damageMax) / (damageMinRange - damageMaxRange);
                double rangeSurplus = targetRange - damageMaxRange;
                double unroundedDamage = damageMax + rangeSurplus * inclination;
                return (int)unroundedDamage;
            };
        }

        public string DamageModel()
        {
            string output = string.Format("{0}@{1}m - {2}@{3}m", damageMax, (int)damageMaxRange, damageMin, (int)damageMinRange);
            return output;
        }

        public string FireRateRPM()
        {
            int fireRateRPM = 60000 / refireTime;
            return string.Format("{0} RPM", fireRateRPM);
        }

        public string weaponString()
        {
            string weaponString = "";

            weaponString += weaponName + "\n";
            weaponString += categoryName + "\n";
            weaponString += DamageModel() + "\n";
            weaponString += FireRateRPM().ToString() + "\n";
            weaponString += headshotMultiplier + "\n";
            weaponString += magazineSize + "\n";
            weaponString += (muzzleVelocityMpMs * 1000) + " m/s";
            return weaponString;
        }
    };
}