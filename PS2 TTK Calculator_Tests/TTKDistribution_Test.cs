using NUnit.Framework;
using PS2_TTK_calculator;



namespace PS2_TTK_calculator_Tests
{
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

            Assert.AreEqual(0, TTKDistribution.DistributionOfBulletsToKill(betelgeuse, target, probabilities)[3]);
            Assert.LessOrEqual(0, TTKDistribution.DistributionOfBulletsToKill(betelgeuse, target, probabilities)[4]);
            Assert.AreEqual(0, TTKDistribution.DistributionOfBulletsToKill(magShot, target, probabilities)[2]);
            Assert.LessOrEqual(0, TTKDistribution.DistributionOfBulletsToKill(magShot, target, probabilities)[3]);
        }

        [Test]
        public void Test_BTKprobabilitiesPositiveAndLessThanOne()
        {
            Weapon betelgeuse = CreateMockBetelgeuse();
            Weapon magShot = CreateMockMagshot();
            Target target = CreateMockStandardTarget();

            double[] distBetelgeuse = TTKDistribution.DistributionOfBulletsToKill(betelgeuse, target, probabilities);
            foreach (double ProbabilityOfNumberOfShots in distBetelgeuse)
            {
                Assert.GreaterOrEqual(ProbabilityOfNumberOfShots, 0);
                Assert.LessOrEqual(ProbabilityOfNumberOfShots,1);
            }

            double[] distBetelgeuseFullHS = TTKDistribution.DistributionOfBulletsToKill(betelgeuse, target, new double[] { 0, 1 });
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

            double pTotal = 0;
            double[] BG_TTK = TTKDistribution.DistributionOfBulletsToKill(betelgeuse, target, probabilities);
            for (int i = 0; i < BG_TTK.Length; ++i)
            {
                pTotal += BG_TTK[i];
            }
            Assert.LessOrEqual(pTotal, 1);
            Assert.GreaterOrEqual(pTotal, 0.9);

            pTotal = 0;
            double[] MagShot_TTK = TTKDistribution.DistributionOfBulletsToKill(magShot, target, probabilities);
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

            double[] V10_TTK= TTKDistribution.DistributionOfBulletsToKill(V10, target, new double[] { 0, 1 });
            Assert.AreEqual(1, V10_TTK[1]);
        }
    }
}