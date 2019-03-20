using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShoppingListPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public ShoppingListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            //show results of table

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private void Results()
        {

            conn.CreateTable<ShoppingList>();
            var query1 = conn.Table<ShoppingList>();
            ShoppingListView.ItemsSource = query1.ToList(); //this view is added in the xaml code

        }

        //handle addButton event
        public async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ItemNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("You have not entered an item name", "Oh dear!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ShoppingList>();
                    conn.Insert(new ShoppingList
                    {
                        NameOfItem = ItemNameTextBox.Text,
                        PriceQuoted = Convert.ToDouble(ItemPriceTextBox.Text),
                        ShopName = ShopNameTextBox.Text,
                        ShoppingDate = ShoppingDatePicker.Date.DateTime
                    });

                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid Amount", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Item Name already exists, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }

            }
        }


        //handle delete button click
        public async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedItem = ((ShoppingList)ShoppingListView.SelectedItem).NameOfItem;
                if (selectedItem == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ShoppingList>();
                    var query1 = conn.Table<ShoppingList>();
                    var query2 = conn.Query<ShoppingList>("DELETE FROM ShoppingList WHERE NameOfItem ='" + selectedItem + "'");
                    ShoppingListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }

        }
    }
}
