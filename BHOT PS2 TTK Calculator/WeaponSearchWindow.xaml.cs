using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PS2_TTK_calculator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowWeaponSelection : Window
    {
        private readonly CensusAPI census = new CensusAPI();
        private readonly List<Weapon> weaponList;
        private Weapon selectedWeapon;


        public WindowWeaponSelection()
        {
            InitializeComponent();
            weaponList = census.GetWeaponList();
            lvWeaponList.ItemsSource = weaponList;
            CollectionView weaponView = (CollectionView)CollectionViewSource.GetDefaultView(lvWeaponList.ItemsSource);
            weaponView.Filter = CustomFilter;
        }

        private bool CustomFilter(object sender)
        {
            string search = sender.ToString().Trim();
            if (string.IsNullOrEmpty(txt_Search.Text))
            {
                return true;
            }
            else
            {
                return (sender as Weapon).weaponName.IndexOf(txt_Search.Text, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }
        private void txt_Search_TextChagned(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvWeaponList.ItemsSource).Refresh();
        }

        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            selectedWeapon = lvWeaponList.SelectedItem as Weapon;
            this.Close();
        }

        public void lvWeaponList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            selectedWeapon = lvWeaponList.SelectedItem as Weapon;
            this.Close();
        }

        public Weapon GetSelectedWeapon()
        {
            return selectedWeapon;
        }
    }
}
