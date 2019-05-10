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
        public string selectedItem = "";

        //connect to the database upon page load
        public ShoppingListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            // Initializing a database connection
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // show results of table when page loads(whatever has been saved to database)
            Results();
        }

        private void Results()
        {
           // find results in the database and add them to the view
            conn.CreateTable<ShoppingList>();
            var query1 = conn.Table<ShoppingList>();
            ShoppingListView.ItemsSource = query1.ToList(); //this view is added in the xaml code

        }

        public void ClearFields()
        {
            ItemNameTextBox.Text = "";
            ShopNameTextBox.Text = "";
            ItemPriceTextBox.Text = "";
        }

        //handle addButton event
        public async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //check user has entered a name
                if (ItemNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("You have not entered an item name", "Oh dear!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //connect to table and add information
                    conn.CreateTable<ShoppingList>();
                    String ShoppingDateString;
                    ShoppingDateString = ShoppingDatePicker.Date.Day.ToString();
                    ShoppingDateString += "/" + ShoppingDatePicker.Date.Month.ToString();
                    ShoppingDateString += "/" + ShoppingDatePicker.Date.Year.ToString();
                    conn.Insert(new ShoppingList
                    {
                        NameOfItem = ItemNameTextBox.Text,
                        PriceQuoted = Convert.ToDouble(ItemPriceTextBox.Text),
                        ShopName = ShopNameTextBox.Text,
                        ShoppingDate = ShoppingDateString
                    });
                    //clear fields
                    ClearFields();
                    //reload results to reflect new data
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {   //if user enters text or symbols in the amount field
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid Amount", "Oh dear.!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)//this exception should never activate, as Item IDs autoincrement and are unique. Duplicate item names will not cause exception.
                {  
                    MessageDialog dialog = new MessageDialog("Item Name already exists, Try Different Name", "Oh dear..!");
                    await dialog.ShowAsync();
                }

            }
        }


        //handle delete button click
        public async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            //string selectedItemName = ((ShoppingList)ShoppingListView.SelectedItem).NameOfItem; //now handled in ItemSelected Event
            
            try
            {
                //string selectedItem = ((ShoppingList)ShoppingListView.SelectedItem).ShoppingItemID.ToString();
                if (selectedItem == "")
                {
                    //
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //UPDATED: Changed to handle item ID
                    conn.CreateTable<ShoppingList>();
                    var query1 = conn.Table<ShoppingList>();
                    //var query2 = conn.Update(ShoppingList)
                    var query2 = conn.Query<ShoppingList>("DELETE FROM ShoppingList WHERE ShoppingItemID ='" + selectedItem + "'");
                    ShoppingListView.ItemsSource = query1.ToList();
                    selectedItem = "";
                    ClearFields();
                }
            }
            catch (NullReferenceException)
            {
                //prevents a crash from pressing delete button while no item selected.
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("Have you selected an item?", "Oh dear..!");
                await dialog.ShowAsync();
            }

        }

        private async void UpdateItem_Click(object sender, RoutedEventArgs e)
        {

            try
            {//add code set fields to selected item values
                string newItemName = ItemNameTextBox.Text;
                double newPrice = Convert.ToDouble(ItemPriceTextBox.Text);
                string newShopName = ShopNameTextBox.Text;


                DateTime newShopTime = ShoppingDatePicker.Date.DateTime;
                string ShopTimeString;
                ShopTimeString = ShoppingDatePicker.Date.Day.ToString() + "/" + ShoppingDatePicker.Date.Month.ToString() + "/" + ShoppingDatePicker.Date.Year.ToString();


                //selectedItem = ((ShoppingList)ShoppingListView.SelectedItem).ShoppingItemID.ToString(); //now handled in ItemSelected event
                if (selectedItem == "")
                {
                    MessageDialog dialog = new MessageDialog("You need to select the item first.", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //UPDATED: Changed to handle item ID
                    conn.CreateTable<ShoppingList>();
                    var query1 = conn.Table<ShoppingList>();
                    //update item name
                    var query2 = conn.Query<ShoppingList>("UPDATE ShoppingList SET NameOfItem = '" + newItemName + "' WHERE ShoppingItemID ='" + selectedItem + "'");
                    //update itemPrice
                    var query3 = conn.Query<ShoppingList>("UPDATE ShoppingList SET PriceQuoted = '" + newPrice + "' WHERE ShoppingItemID ='" + selectedItem + "'");
                    //update Name of shop
                    var query4 = conn.Query<ShoppingList>("UPDATE ShoppingList SET ShopName = '" + newShopName + "' WHERE ShoppingItemID ='" + selectedItem + "'");
                    //update time of shop //Cannot Update as DateTime for some reason. 
                    //var query5 = conn.Query<ShoppingList>("UPDATE ShoppingList SET ShoppingDate = '" + newShopTime + "' WHERE ShoppingItemID ='" + selectedItem + "'");
                    var query5 = conn.Query<ShoppingList>("UPDATE ShoppingList SET ShoppingDate = '" + ShopTimeString + "' WHERE ShoppingItemID ='" + selectedItem + "'");
                    
                    ShoppingListView.ItemsSource = query1.ToList(); //this will deselect the item
                    selectedItem = "";//will prevent updating last selected item. HL 08/04/19
                    //clear Fields
                    ClearFields();
                }
            }
            catch (NullReferenceException)
            {
                //prevents a crash from pressing update button while no item selected.
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
            catch (FormatException)
            {
                MessageDialog dialog = new MessageDialog("Amount must be in number form", "Oh dear..!");
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog("Have you selected an item?", "Oh dear..!");
                await dialog.ShowAsync();
            }
        }

        private async void ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                if(ShoppingListView.SelectedItem!=null)
                if (((ShoppingList)ShoppingListView.SelectedItem).ShoppingItemID.ToString() != null)
                {
                    selectedItem = ((ShoppingList)ShoppingListView.SelectedItem).ShoppingItemID.ToString();
                    //update the fields
                    ItemNameTextBox.Text = ((ShoppingList)ShoppingListView.SelectedItem).NameOfItem;
                    ShopNameTextBox.Text = ((ShoppingList)ShoppingListView.SelectedItem).ShopName;
                    ItemPriceTextBox.Text = ((ShoppingList)ShoppingListView.SelectedItem).PriceQuoted.ToString();
                    //attempt to update date

                    ShoppingDatePicker.Date = DateTime.Parse(((ShoppingList)ShoppingListView.SelectedItem).ShoppingDate);


                }
            }
            catch (NullReferenceException)
            {
                //prevents a crash from pressing update button while no item selected.
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oh dear..!");
                await dialog.ShowAsync();
            }

        }
    }
}
