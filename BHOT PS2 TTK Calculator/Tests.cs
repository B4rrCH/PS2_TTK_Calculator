#if DEBUG
using NUnit.Framework;
using System.Collections.Generic;



namespace PS2_TTK_calculator
{
    [TestFixture]
    internal class API_Tests
    {
        readonly CensusAPI MockCensusAPI;

        public API_Tests()
        {
            this.MockCensusAPI = new CensusAPI();
        }

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
            Weapon magshot = MockCensusAPI.GetWeapon(2);
            return magshot;
        }
        private Weapon CreateMockBetelgeuse()
        {
            Weapon betelgeuse = MockCensusAPI.GetWeapon(1894);
            return betelgeuse;
        }

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
            List<Weapon> WeaponList = MockCensusAPI.GetWeaponList();
            Assert.AreEqual(MockCensusAPI.GetWeapon("Merc"), WeaponList[1]);
        }
    }

    [TestFixture]
    internal class MultinomialDistribution_Test
    {
        private static readonly int MockNumberOfTrials = 20;
        private static readonly double[] MockProbabilites = { 0.3, 0.2 };
        private readonly MultinomialDistribution MockMultinomialDistribution = new MultinomialDistribution(MockNumberOfTrials, MockProbabilites);

        private int Factorial(int n)
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

        [Test]
        public void Test_Binomial()
        {
            Assert.AreEqual(2300, MockMultinomialDistribution.Binomial(25, 3));
            Assert.AreEqual(12870, MockMultinomialDistribution.Binomial(16, 8));

        }

        [Test]
        public void Test_Multinomial()
        {
            int[] k1 = { 1, 4, 4, 2 };
            Assert.AreEqual(34650, MockMultinomialDistribution.Multinomial(k1));
            int[] k2 = { 2, 4, 3, 2, 1 };
            Assert.AreEqual(831600, MockMultinomialDistribution.Multinomial(k2));
            int[] k3 = { 0, 1 };
            Assert.AreEqual(1, MockMultinomialDistribution.Multinomial(k3));
        }

        [Test]
        public void Test_MultinomialPMF()
        {
            double[,] test_multPMF = MockMultinomialDistribution.GetDist();
            Assert.AreEqual(0.006898447265625, test_multPMF[5, 1], 0.0001);
            Assert.AreEqual(0.0051099609375, test_multPMF[3, 2], 0.0001);
            Assert.AreNotEqual(0, test_multPMF[0, 20]);
            foreach (double testValue in test_multPMF)
            {
                Assert.GreaterOrEqual(testValue, 0);
            }
        }

        [Test]
        public void Test_ExtremeCases()
        {
            MultinomialDistribution multinomialDistribution = new MultinomialDistribution(50, new double[] { 0, 1 });
            double[,] test_multPMF = multinomialDistribution.GetDist();
            for (int i = 0; i < test_multPMF.GetLength(0); ++i)
            {
                for (int j = 0; j < test_multPMF.GetLength(1) - i; ++j)
                {
                    Assert.LessOrEqual(0, test_multPMF[i, j], string.Format("{0},{1}", i, j));
                    Assert.GreaterOrEqual(1, test_multPMF[i, j], string.Format("{0},{1}", i, j));
                }
            }
        }
    }

    [TestFixture]
    internal class TTKDistribution_Test
    {
        readonly CensusAPI MockCensusAPI;
        readonly double[] probabilities = new double[2];
        readonly double Accuracy = 0.4;
        readonly double HSperHit = 0.3;

        public TTKDistribution_Test()
        {
            MockCensusAPI = new CensusAPI();
            probabilities[0] = Accuracy * (1 - HSperHit);
            probabilities[1] = Accuracy * HSperHit;
        }

        private Target CreateMockStandardTarget(bool nanoWeave = false, double range = 0)
        {
            Target target = new Target(range, false, false, nanoWeave);
            return target;
        }
        private Target CreateMockInfiltratorTarget(bool nanoWeave = false, double range = 0)
        {
            Target target = new Target(range, false, true, nanoWeave);
            return target;
        }
        private Target CreateMockNMSTarget(bool nanoWeave = false, double range = 0)
        {
            Target target = new Target(range, true, false, nanoWeave);
            return target;
        }



        private Weapon CreateMockMagshot()
        {
            Weapon magshot = MockCensusAPI.GetWeapon(2);
            return magshot;
        }

        private Weapon CreateMockBetelgeuse()
        {
            Weapon betelgeuse = MockCensusAPI.GetWeapon(1894);
            return betelgeuse;
        }
        private Weapon CreateMockV10()
        {
            Weapon V10 = MockCensusAPI.GetWeapon(26003);
            return V10;
        }


        [Test]
        public void Test_BTKdist()
        {
            Weapon betelgeuse = CreateMockBetelgeuse();
            Weapon magShot = CreateMockMagshot();
            Target target = CreateMockStandardTarget();
            TTKDistribution dist = new TTKDistribution();

            Assert.AreEqual(0, dist.DistributionOfBulletsToKill(betelgeuse, target, probabilities)[3]);
            Assert.LessOrEqual(0, dist.DistributionOfBulletsToKill(betelgeuse, target, probabilities)[4]);
            Assert.AreEqual(0, dist.DistributionOfBulletsToKill(magShot, target, probabilities)[2]);
            Assert.LessOrEqual(0, dist.DistributionOfBulletsToKill(magShot, target, probabilities)[3]);
        }

        [Test]
        public void Test_BTKprobabilitiesPositiveAndLessThanOne()
        {
            Weapon betelgeuse = CreateMockBetelgeuse();
            Weapon magShot = CreateMockMagshot();
            Target target = CreateMockStandardTarget();
            TTKDistribution dist = new TTKDistribution();

            double[] distBetelgeuse = dist.DistributionOfBulletsToKill(betelgeuse, target, probabilities);
            foreach (double ProbabilityOfNumberOfShots in distBetelgeuse)
            {
                Assert.LessOrEqual(0, ProbabilityOfNumberOfShots);
                Assert.GreaterOrEqual(1, ProbabilityOfNumberOfShots);
            }

            double[] distBetelgeuseFullHS = dist.DistributionOfBulletsToKill(betelgeuse, target, new double[] { 0, 1 });
            foreach (double ProbabilityOfNumberOfShots in distBetelgeuseFullHS)
            {
                Assert.LessOrEqual(0, ProbabilityOfNumberOfShots);
                Assert.GreaterOrEqual(1, ProbabilityOfNumberOfShots);
            }
        }

        [Test]
        public void Test_BTKtotalLessThan1()
        {
            Weapon betelgeuse = CreateMockBetelgeuse();
            Weapon magShot = CreateMockMagshot();
            Target target = CreateMockStandardTarget();
            TTKDistribution dist = new TTKDistribution();

            double pTotal = 0;
            double[] BG_TTK = dist.DistributionOfBulletsToKill(betelgeuse, target, probabilities);
            for (int i = 0; i < BG_TTK.Length; ++i)
            {
                pTotal += BG_TTK[i];
            }
            Assert.LessOrEqual(pTotal, 1);
            Assert.GreaterOrEqual(pTotal, 0.9);

            pTotal = 0;
            double[] MagShot_TTK = dist.DistributionOfBulletsToKill(magShot, target, probabilities);
            for (int i = 0; i < MagShot_TTK.Length; ++i)
            {
                pTotal += MagShot_TTK[i];
            }
            Assert.LessOrEqual(pTotal, 1);
            Assert.GreaterOrEqual(pTotal, 0.2);
        }

        [Test]
        public void Test_PerfectV10GetsInstagib()
        {
            Weapon V10 = CreateMockV10();
            Target target = CreateMockStandardTarget();
            TTKDistribution dist = new TTKDistribution();

            double[] V10_TTK= dist.DistributionOfBulletsToKill(V10, target, new double[] { 0, 1 });
            Assert.AreEqual(1, V10_TTK[1]);

        }
    }

    [TestFixture]
    internal class WinProbability_Test
    {
        readonly CensusAPI MockCensusAPI;

        public WinProbability_Test()
        {
            this.MockCensusAPI = new CensusAPI();

        }


        private Target CreateMockStandardTarget(bool nanoWeave = false, double range = 0)
        {
            Target target = new Target(range, false, false, nanoWeave);
            return target;
        }
        private Target CreateMockInfiltratorTarget(bool nanoWeave = false, double range = 0)
        {
            Target target = new Target(range, false, true, nanoWeave);
            return target;
        }
        private Target CreateMockNMSTarget(bool nanoWeave = false, double range = 0)
        {
            Target target = new Target(range, true, false, nanoWeave);
            return target;
        }

        private Weapon CreateMockMagshot()
        {
            Weapon magshot = MockCensusAPI.GetWeapon(2);
            return magshot;
        }

        private Weapon CreateMockBetelgeuse()
        {
            Weapon betelgeuse = MockCensusAPI.GetWeapon(1894);
            return betelgeuse;
        }

        private Weapon CreateMockV10()
        {
            Weapon V10 = MockCensusAPI.GetWeapon(26003);
            return V10;
        }

        private Loadout CreateMockHALoadout()
        {
            double[] probabilities = new double[2];
            double Accuracy = 0.4;
            double HSperHit = 0.3;
            probabilities[0] = Accuracy * (1 - HSperHit);
            probabilities[1] = Accuracy * HSperHit;
            return new Loadout(CreateMockNMSTarget(true), CreateMockBetelgeuse(), probabilities);
        }

        private Loadout CreateMockStalkerLoadout()
        {
            double[] probabilities = new double[2];
            double Accuracy = 0.4;
            double HSperHit = 0.3;
            probabilities[0] = Accuracy * (1 - HSperHit);
            probabilities[1] = Accuracy * HSperHit;
            return new Loadout(CreateMockInfiltratorTarget(), CreateMockMagshot(), probabilities);
        }

        private Loadout CreateMockPerfectSniper()
        {
            return new Loadout(CreateMockNMSTarget(true), CreateMockV10(), new double[] { 0, 1 });
        }
        
        [Test]
        public void Test_EqualPair()
        {
            Loadout MLGproVirgin = CreateMockHALoadout();
            double[] p = MLGproVirgin.ProbWinsAgainst(MLGproVirgin);
            Assert.AreEqual(p[0], p[1], 0.00001);
            Assert.Greater(1, p[0] + p[1]);
        }
        [Test]
        public void Test_HAwinsoverStalker()
        {
            Loadout MLGproVirgin = CreateMockHALoadout();
            Loadout UselessStalker = CreateMockStalkerLoadout();
            double[] p = MLGproVirgin.ProbWinsAgainst(UselessStalker);
            Assert.Greater(p[0], p[1]);
        }

        [Test]
        public void Test_PerfectInstagibAlwaysWins()
        {
            Loadout Sykka = CreateMockPerfectSniper();
            Loadout UselessStalker = CreateMockStalkerLoadout();
            double[] p = Sykka.ProbWinsAgainst(UselessStalker);
            Assert.AreEqual(1, p[0], 0.01);
        }
    }
}

#endif