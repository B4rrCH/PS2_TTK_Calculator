using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly List<Weapon> weaponList;
        private Weapon selectedWeapon;
        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public WindowWeaponSelection()
        {
            InitializeComponent();
            weaponList = CensusAPI.GetWeaponList();
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

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
            CollectionViewSource.GetDefaultView(lvWeaponList.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        private void LvWeaponColumn_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }
    }
}
