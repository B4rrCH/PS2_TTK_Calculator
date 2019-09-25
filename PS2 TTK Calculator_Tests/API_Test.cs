using NUnit.Framework;
using System.Collections.Generic;
using PS2_TTK_calculator;



namespace PS2_TTK_calculator_Tests
{
    [TestFixture]
    internal class API_Test
    {

        #region MockObjectCreation
        private Weapon CreateMockWeapon()
        {
            Weapon _weapon = new Weapon
            {
                damageMax = 200,
                damageMin = 100,
                damageMaxRange = 10,
                damageMinRange = 50,
                fireRateMs = 200,
                headshotMultiplier = 2,
                magazineSize = 50
            };

            return _weapon;
        }
        private Weapon CreateMockMagshot()
        {
            Weapon magshot = CensusAPI.GetWeapon(2);
            return magshot;
        }
        private Weapon CreateMockBetelgeuse()
        {
            Weapon betelgeuse = CensusAPI.GetWeapon(1894);
            return betelgeuse;
        }
        #endregion

        [Test]
        public void Test_RawDamageAtRanges()
        {
            Weapon weapon = CreateMockWeapon();
            // point blank //
            Assert.AreEqual(weapon.RawDamagePerShot(weapon.damageMinRange), weapon.damageMin);
            // far //
            Assert.AreEqual(weapon.RawDamagePerShot(weapon.damageMaxRange), weapon.damageMax);
            // medium //
            double mediumDamageRange = (weapon.damageMaxRange + weapon.damageMinRange) / 2;
            int mediumDamage = (weapon.damageMax + weapon.damageMin) / 2;
            Assert.AreEqual(weapon.RawDamagePerShot(mediumDamageRange), mediumDamage);
        }

        [Test]
        public void Test_BetelegeuseHSDamage()
        {
            Weapon betelgeuse = CreateMockBetelgeuse();
            Target target = new Target();
            Assert.AreEqual(286, target.DamagePerHeadShot(betelgeuse));
        }

        [Test]
        public void Test_BodyshotsToKill()
        {
            Weapon weapon = CreateMockWeapon();
            Target target = new Target();
            Assert.AreEqual(target.BodyshotsToKill(weapon), 5);

            target = new Target(0, false, false, true);
            Assert.AreEqual(target.BodyshotsToKill(weapon), 7);

            target = new Target(0, false, true, false);
            Assert.AreEqual(target.BodyshotsToKill(weapon), 5);

            target = new Target(0, true, false, true);
            Assert.AreEqual(target.BodyshotsToKill(weapon), 10);

            target = new Target(100, true, false, true);
            Assert.AreEqual(target.BodyshotsToKill(weapon), 19);
        }

        [Test]
        public void Test_HeadshotsToKill()
        {
            Weapon weapon = CreateMockWeapon();
            Target target = new Target();
            Assert.AreEqual(target.HeadshotsToKill(weapon), 3);

            target = new Target(0, false, false, true);
            Assert.AreEqual(target.HeadshotsToKill(weapon), 3);

            target = new Target(0, false, true, false);
            Assert.AreEqual(target.HeadshotsToKill(weapon), 3);

            target = new Target(0, true, false, true);
            Assert.AreEqual(target.HeadshotsToKill(weapon), 4);

            target = new Target(100, true, false, true);
            Assert.AreEqual(target.HeadshotsToKill(weapon), 8);
        }

        [Test]
        public void Test_IsKilled()
        {
            Weapon betelgeuse = CreateMockBetelgeuse();
            Target target = new Target();
            Assert.IsTrue(target.IsEnoughToKill(betelgeuse, 0, 4));
            Assert.IsTrue(target.IsEnoughToKill(betelgeuse, 1, 3));
            Assert.IsFalse(target.IsEnoughToKill(betelgeuse, 4, 1));
            Assert.IsFalse(target.IsEnoughToKill(betelgeuse, 0, 0));
        }

        [Test]
        public void Test_BS_TTK()
        {
            Weapon magShot = CreateMockWeapon();
            Target target = new Target();
            Assert.AreEqual(target.TimeToKillBS(magShot), 800);

            target = new Target(0, false, false, true);
            Assert.AreEqual(target.TimeToKillBS(magShot), 1200);

            target = new Target(0, false, true, false);
            Assert.AreEqual(target.TimeToKillBS(magShot), 800);

            target = new Target(0, true, false, true);
            Assert.AreEqual(target.TimeToKillBS(magShot), 1800);

            target = new Target(100, true, false, true);
            Assert.AreEqual(target.TimeToKillBS(magShot), 3600);
        }

        [Test]
        public void Test_HS_TTK()
        {
            Weapon magShot = CreateMockWeapon();
            Target target = new Target();
            Assert.AreEqual(400, target.TimeToKillHS(magShot));

            target = new Target(0, false, false, true);
            Assert.AreEqual(400, target.TimeToKillHS(magShot));

            target = new Target(0, false, true, false);
            Assert.AreEqual(400, target.TimeToKillHS(magShot));

            target = new Target(0, true, false, true);
            Assert.AreEqual(target.TimeToKillHS(magShot), 600);

            target = new Target(100, true, false, true);
            Assert.AreEqual(target.TimeToKillHS(magShot), 1400);

            target = new Target(100, true, false, false);
            Assert.AreEqual(target.TimeToKillHS(magShot), 1400);
        }

        [Test]
        public void Test_HS_multiplier()
        {
            Weapon betelgeuse = CreateMockBetelgeuse();
            Assert.AreEqual(betelgeuse.headshotMultiplier, 2.0);
        }
        [Test]
        public void Test_API()
        {
            Weapon magshot = CreateMockMagshot();

            Assert.AreEqual(magshot.fireRateMs, 171);
            Assert.AreEqual(magshot.damageMax, 200);

            Weapon betelgeuse = CreateMockBetelgeuse();
            Assert.AreEqual(betelgeuse.damageMinRange, 65);
            Assert.AreEqual(betelgeuse.damageMax, 143);
            Assert.AreEqual(betelgeuse.fireRateMs, 80);
        }

        [Test]
        public void Test_APIandCalc()
        {
            Weapon betelgeuse = CreateMockBetelgeuse();
            Weapon magshot = CreateMockMagshot();
            Target target = new Target(0, true, false, true);

            Assert.AreEqual(960, target.TimeToKillBS(betelgeuse));
            Assert.AreEqual(513, target.TimeToKillHS(magshot));
        }

        [Test]
        public void Test_WeaponList()
        {
            List<Weapon> WeaponList = CensusAPI.GetWeaponList();
            Assert.AreEqual(CensusAPI.GetWeapon("Merc"), WeaponList[1]);
        }
    }
}