using SQLite.Net;
using StartFinance.Models;
using System;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetailsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public string selectedItem = "";
        public ContactDetailsPage()
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
            conn.CreateTable<ContactDetails>();
            var query1 = conn.Table<ContactDetails>();
            ContactDetailsListView.ItemsSource = query1.ToList(); //this view is added in the xaml code

        }

    public void ClearFields()
    {
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            CompanyNameTextBox.Text = "";
            MobilePhoneTextBox.Text = "";
    }

    //handle addButton event
    public async void AddItem_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            //check user has entered a name
            if (FirstNameTextBox.Text.ToString() == "")
            {
                MessageDialog dialog = new MessageDialog("You have not entered first name");
                await dialog.ShowAsync();
            }
            else
            {
                //connect to table and add information
                conn.CreateTable<ContactDetails>();
                conn.Insert(new ContactDetails
                {
                    FirstName = FirstNameTextBox.Text,
                    LastName = LastNameTextBox.Text,
                    CompanyName = CompanyNameTextBox.Text,
                    MobilePhone=MobilePhoneTextBox.Text
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
                MessageDialog dialog = new MessageDialog("Format Error!!","Oops..!");
                await dialog.ShowAsync();
            }
            else if (ex is SQLiteException)//this exception should never activate, as Item IDs autoincrement and are unique. Duplicate item names will not cause exception.
            {
                MessageDialog dialog = new MessageDialog("Contact already exists, Try again","Oops..!");
                await dialog.ShowAsync();
            }

        }
    }


    //handle delete button click
    public async void DeleteItem_Click(object sender, RoutedEventArgs e)
    {
        try
        {

            string selectedItem = ((ContactDetails)ContactDetailsListView.SelectedItem).ContactID.ToString();
            if (selectedItem == "")
            {
                //
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
            else
            {
                //UPDATED: Changed to handle item ID
                conn.CreateTable<ContactDetails>();
                var query1 = conn.Table<ContactDetails>();
                var query2 = conn.Query<ContactDetails>("DELETE FROM ContactDetails WHERE ContactID ='" + selectedItem + "'");

                ContactDetailsListView.ItemsSource = query1.ToList();

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
        {
                //add code set fields to selected item values
                string firstName = FirstNameTextBox.Text;
                string lastName = LastNameTextBox.Text;
                string companyName = CompanyNameTextBox.Text;
                string mobilePhone = MobilePhoneTextBox.Text;

                string selectedItem = ((ContactDetails)ContactDetailsListView.SelectedItem).ContactID.ToString();
                if (selectedItem == "")
            {
                MessageDialog dialog = new MessageDialog("You need to select the item first.", "Oops..!");
                await dialog.ShowAsync();
            }
            else
            {
                //UPDATED: Changed to handle contact ID
                    conn.CreateTable<ContactDetails>();
                    var query1 = conn.Table<ContactDetails>();

                    //update first name
                    var query2 = conn.Query<ContactDetails>("UPDATE ContactDetails SET FirstName = '" + firstName + "' WHERE ContactID ='" + selectedItem + "'");
                    
                    //update last name
                    var query3 = conn.Query<ContactDetails>("UPDATE ContactDetails SET LastName = '" + lastName + "' WHERE ContactID ='" + selectedItem + "'");
                    
                    //update company name
                    var query4 = conn.Query<ContactDetails>("UPDATE ContactDetails SET CompanyName = '" + companyName + "' WHERE ContactID ='" + selectedItem + "'");
                    
                    //update mobile phone
                    var query5 = conn.Query<ContactDetails>("UPDATE ContactDetails SET MobilePhone = '" + mobilePhone + "' WHERE ContactID ='" + selectedItem + "'");

                ContactDetailsListView.ItemsSource = query1.ToList(); //this will deselect the item
                                                                
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
            if (ContactDetailsListView.SelectedItem != null)
                if (((ContactDetails)ContactDetailsListView.SelectedItem).ContactID.ToString() != null)
                {
                    selectedItem = ((ContactDetails)ContactDetailsListView.SelectedItem).ContactID.ToString();
                    //update the fields
                    FirstNameTextBox.Text = ((ContactDetails)ContactDetailsListView.SelectedItem).FirstName;
                    LastNameTextBox.Text = ((ContactDetails)ContactDetailsListView.SelectedItem).LastName;
                    CompanyNameTextBox.Text = ((ContactDetails)ContactDetailsListView.SelectedItem).CompanyName;
                    MobilePhoneTextBox.Text = ((ContactDetails)ContactDetailsListView.SelectedItem).MobilePhone;


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
