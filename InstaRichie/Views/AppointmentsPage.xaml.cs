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
    public sealed partial class AppointmentsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public string selectedItem = "";

        public AppointmentsPage()
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
            conn.CreateTable<Appointments>();
            var query1 = conn.Table<Appointments>();
            AppointmentsView.ItemsSource = query1.ToList(); //this view is added in the xaml code

        }

        public void ClearFields()
        {
            FirstNametxtBox.Text = "";
            LastNametxtBox.Text = "";
        }

        // Add button
        private async void AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //check user has entered a name
                if (FirstNametxtBox.Text.ToString() == "" || LastNametxtBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Please enter First and Last Name", "Oh dear!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //connect to table and add information
                    conn.CreateTable<Appointments>();
                    String AppointmentDateString;
                    AppointmentDateString = DOAdatePicker.Date.Day.ToString();
                    AppointmentDateString += "/" + DOAdatePicker.Date.Month.ToString();
                    AppointmentDateString += "/" + DOAdatePicker.Date.Year.ToString();
                    String AppointmentTimeString = TOAtimePicker.Time.ToString();
                    conn.Insert(new Appointments
                    {
                        FirstName = FirstNametxtBox.Text,
                        LastName = LastNametxtBox.Text,
                        DateOfAppointment = AppointmentDateString,
                        TimeOfAppointmant = AppointmentTimeString
                    });
                    //clear fields
                    ClearFields();
                    //reload results to reflect new data
                    Results();
                }
            }
            catch (Exception ex)
            {
                //if (ex is FormatException)
                //{   //if user enters text or symbols in the amount field
                //    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid Amount", "Oh dear.!");
                //    await dialog.ShowAsync();
                //}
                //else if (ex is SQLiteException)//this exception should never activate, as Item IDs autoincrement and are unique. Duplicate item names will not cause exception.
                //{
                //    MessageDialog dialog = new MessageDialog("Item Name already exists, Try Different Name", "Oh dear..!");
                //    await dialog.ShowAsync();
                //}

            }
        }

        //delete button
        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (selectedItem == "")
                {

                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //UPDATED: Changed to handle item ID
                    conn.CreateTable<Appointments>();
                    var query1 = conn.Table<Appointments>();

                    var query2 = conn.Query<Appointments>("DELETE FROM Appointments WHERE AppointmentID ='" + selectedItem + "'");
                    AppointmentsView.ItemsSource = query1.ToList();
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

        //update button
        private async void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {//add code set fields to selected item values
                string newFirstName = FirstNametxtBox.Text;
                string newLastName = LastNametxtBox.Text;

                DateTime newDOA = DOAdatePicker.Date.DateTime;
                string DOAstring;
                DOAstring = DOAdatePicker.Date.Day.ToString() + "/" + DOAdatePicker.Date.Month.ToString() + "/" + DOAdatePicker.Date.Year.ToString();

                string newTOA = TOAtimePicker.Time.ToString();



                if (selectedItem == "")
                {
                    MessageDialog dialog = new MessageDialog("You need to select the item first.", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //UPDATED: Changed to handle item ID
                    conn.CreateTable<Appointments>();
                    var query1 = conn.Table<Appointments>();
                    //update first name
                    var query2 = conn.Query<Appointments>("UPDATE Appointments SET FirstName = '" + newFirstName + "' WHERE AppointmentID ='" + selectedItem + "'");
                    //update last name
                    var query3 = conn.Query<Appointments>("UPDATE Appointments SET LastName = '" + newLastName + "' WHERE AppointmentID ='" + selectedItem + "'");
                    //update date of appointment
                    var query4 = conn.Query<Appointments>("UPDATE Appointments SET DateOfAppointment = '" + DOAstring + "' WHERE AppointmentID ='" + selectedItem + "'");
                    //update time of appointment 
                    var query5 = conn.Query<Appointments>("UPDATE Appointments SET TimeOfAppointmant = '" + newTOA + "' WHERE AppointmentID ='" + selectedItem + "'");

                    AppointmentsView.ItemsSource = query1.ToList(); //this will deselect the item
                    selectedItem = "";//will prevent updating last selected item.
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
                if (AppointmentsView.SelectedItem != null)
                    if (((Appointments)AppointmentsView.SelectedItem).AppointmentID.ToString() != null)
                    {
                        selectedItem = ((Appointments)AppointmentsView.SelectedItem).AppointmentID.ToString();
                        //update the fields
                        FirstNametxtBox.Text = ((Appointments)AppointmentsView.SelectedItem).FirstName;
                        LastNametxtBox.Text = ((Appointments)AppointmentsView.SelectedItem).LastName;
                        //ItemPriceTextBox.Text = ((ShoppingList)ShoppingListView.SelectedItem).PriceQuoted.ToString();

                        DOAdatePicker.Date = DateTime.Parse(((Appointments)AppointmentsView.SelectedItem).DateOfAppointment.ToString());
                        //TOAtimePicker.Time = 
                        //TOAtimePicker.Time = AppointmentsView.SelectedItem.ToString();
                        //TOAtimePicker.Time = Convert.ToDateTime(((Appointments)AppointmentsView.SelectedItem).DateOfAppointment.ToString());
                        //TOAtimePicker.Time = Convert.ChangeType(AppointmentsView.SelectedItem, DateTime);
                        //TOAtimePicker.Time = "12,00,00";
                        //DOAdatePicker
                        TOAtimePicker.Time = TimeSpan.Parse(((Appointments)AppointmentsView.SelectedItem).TimeOfAppointmant.ToString());






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
