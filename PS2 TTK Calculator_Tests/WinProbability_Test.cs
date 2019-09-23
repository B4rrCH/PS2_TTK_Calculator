using NUnit.Framework;
using PS2_TTK_calculator;



namespace PS2_TTK_calculator_Tests
{

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