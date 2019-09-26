using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PS2_TTK_calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double range;
        Loadout loadout1, loadout2;
        ObservableCollection<KeyValuePair<int, double>> ChartableTTKDist1, ChartableTTKDist2;

        private bool initializing = false;
        public MainWindow()
        {
            initializing = true;
            InitializeComponent();
            initializing = false;


            this.Title = "Planetside 2 TTK calculator";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.loadout1 = new Loadout(new Target(), CensusAPI.GetWeapon(2), new double[] { 0, 1 });
            this.loadout2 = new Loadout(new Target(), CensusAPI.GetWeapon(3), new double[] { 0, 1 });
            this.range = 0;

            this.ChartableTTKDist1 = new ObservableCollection<KeyValuePair<int, double>>();
            this.ChartableTTKDist2 = new ObservableCollection<KeyValuePair<int, double>>();

            UpdateBasicWeaponInfoText();
            UpdateChartableTTKDist();
            LoadLineChartData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //System.Windows.Data.CollectionViewSource weaponViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("weaponViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // weaponViewSource.Source = [generic data source]
        }
        private void menuRefresh_Click(object sender, RoutedEventArgs e)
        {
            CensusAPI.UpdateWeaponDataFromDBG();
        }
        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }
        private void UpdateBasicWeaponInfoText()
        {
            txt_SelectedWeapon1Name.Text = loadout1.weapon.weaponString();
            txt_SelectedWeapon2Name.Text = loadout2.weapon.weaponString();
        }
        private void btn_OpenWeapon1Selection_Click(object sender, RoutedEventArgs e)
        {
            WindowWeaponSelection weaponSelection = new WindowWeaponSelection();
            if (weaponSelection.ShowDialog() == false)
            {
                loadout1.weapon = weaponSelection.GetSelectedWeapon() ?? loadout1.weapon;
                UpdateBasicWeaponInfoText();
                LoadLineChartData();
            }
        }
        private void btn_OpenWeapon2Selection_Click(object sender, RoutedEventArgs e)
        {
            WindowWeaponSelection weaponSelection = new WindowWeaponSelection();
            if (weaponSelection.ShowDialog() == false)
            {
                loadout2.weapon = weaponSelection.GetSelectedWeapon() ?? loadout2.weapon;
                UpdateBasicWeaponInfoText();
                LoadLineChartData();
            }
        }
        private void btn_UpdateDistribution_Click(object sender, RoutedEventArgs e)
        {
            LoadLineChartData();
        }
        private void TargetsChanged(object sender, RoutedEventArgs e)
        {
            if (initializing) return;
            range = sld_Range.Value;

            double acc1 = sld_Accuracy1.Value / 100;
            double hsr1 = sld_HSR1.Value / 100;
            double[] probabilities1 = { acc1 * (1 - hsr1), acc1 * hsr1 };

            bool hasNMS1 = cmb_Target1Class.SelectedIndex == 1;
            bool isInfil1 = cmb_Target1Class.SelectedIndex == 2;
            bool hasNWA1 = chb_NWA1.IsChecked ?? loadout1.target.resistanceNWA > 0;

            Target target1 = new Target(range, hasNMS1, isInfil1, hasNWA1);

            double acc2 = sld_Accuracy2.Value / 100;
            double hsr2 = sld_HSR2.Value / 100;
            double[] probabilities2 = { acc2 * (1 - hsr2), acc2 * hsr2 };

            bool hasNMS2 = cmb_Target2Class.SelectedIndex == 1;
            bool isInfil2 = cmb_Target2Class.SelectedIndex == 2;
            bool hasNWA2 = chb_NWA2.IsChecked ?? loadout2.target.resistanceNWA > 0;

            Target target2 = new Target(range, hasNMS2, isInfil2, hasNWA2);

            loadout1 = new Loadout(target1, loadout1.weapon, probabilities1);
            loadout2 = new Loadout(target2, loadout2.weapon, probabilities2);
        }

        private void LimitInputsToNumbers(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "100";
            }

        }

        private void UpdateChartableTTKDist()
        {
            double[] TTKdist1 = BTKDistribution.DistributionOfBulletsToKill(loadout1.weapon, loadout2.target, loadout1.probabilities);
            ChartableTTKDist1.Clear();
            if (sld_Range.Value > 0)
            {
                ChartableTTKDist1.Add(new KeyValuePair<int, double>(0, 0));
            }
            for (int i = 1; i <= loadout1.weapon.magazineSize; ++i)
            {
                ChartableTTKDist1.Add(new KeyValuePair<int, double>(((i - 1) * loadout1.weapon.refireTime) + (int)(loadout2.target.rangeFromShooterM / loadout1.weapon.muzzleVelocityMpMs),
                                                                    TTKdist1[i]));
            }

            double[] TTKdist2 = BTKDistribution.DistributionOfBulletsToKill(loadout2.weapon, loadout1.target, loadout2.probabilities);
            ChartableTTKDist2.Clear();
            if (sld_Range.Value > 0)
            {
                ChartableTTKDist2.Add(new KeyValuePair<int, double>(0, 0));
            }
            for (int i = 1; i <= loadout2.weapon.magazineSize; ++i)
            {
                ChartableTTKDist2.Add(new KeyValuePair<int, double>(((i - 1) * loadout2.weapon.refireTime) + (int)(loadout1.target.rangeFromShooterM / loadout2.weapon.muzzleVelocityMpMs),
                                                                    TTKdist2[i]));
            }
        }
        private void LoadLineChartData()
        {
            UpdateChartableTTKDist();

            ls_TTKdist1.DataContext = ChartableTTKDist1;
            ls_TTKdist2.DataContext = ChartableTTKDist2;
            double[] winningProbabilities = loadout1.ProbWinsAgainst(loadout2);
            double[] expectedTTKandTTD = loadout1.ExpectedTTKandTTD(loadout2);

            txt_WinningProbability1.Text =
                string.Format("Player 1 wins with a probability of {0}%. Their expected TTK (given they do get a kill) is {1} s.",
                              (decimal)((int)(winningProbabilities[0] * 1000)) / 10,
                              (decimal)((int)(expectedTTKandTTD[0])) / 1000);
            txt_WinningProbability2.Text = string.Format("Player 2 wins with a probability of {0}%. Their expected TTK (given they do get a kill) is {1} s.",
                              (decimal)((int)(winningProbabilities[1] * 1000)) / 10,
                              (decimal)((int)(expectedTTKandTTD[1])) / 1000);
            txt_KillTradeProbability.Text = string.Format("A kill trade occurs with probability {0}%.", (decimal)((int)(winningProbabilities[2] * 1000)) / 10);
            txt_DrawProbability.Text = string.Format("No one gets killed with probability {0}%.", (decimal)((int)((1 - winningProbabilities.Sum()) * 1000)) / 10);
        }
    }
}
