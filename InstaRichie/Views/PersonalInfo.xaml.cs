using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel.DataAnnotations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// Author: Jefferson Gomes - 31/03/2019
    /// </summary>
    public sealed partial class PersonalInfo : Page
    {
        SQLiteConnection conn; // Creating SQLite connection
        string dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite"); // Setting up the DB path

        MessageDialog msg; // Variable to handle messages

        public PersonalInfo()
        {
            this.InitializeComponent();

            conn = new SQLiteConnection(new SQLitePlatformWinRT(), dbPath); // Create a new instance of the connection
            conn.CreateTable<Person>(); // Create table in the database
            UpdateList(); // Call the method to query the database reset the page

        }

        /// <summary>
        /// Method to Reset all the buttons to Default
        /// </summary>
        public void ResetButtons() {
            // Hide all button not necessary when load for first time
            btnCancel.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;
            btnEdit.Visibility = Visibility.Collapsed;

            btnAdd.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Method to clear all the fields of the page
        /// </summary>
        public void ResetFields() {
            txtFirstName.ClearValue(TextBox.TextProperty);
            txtLastName.ClearValue(TextBox.TextProperty);
            dpDOB.ClearValue(DatePicker.DateProperty);
            cbxGender.SelectedIndex = 0;
            txtEmail.ClearValue(TextBox.TextProperty);
            txtPhone.ClearValue(TextBox.TextProperty);
        }

        /// <summary>
        /// Method to Update the list of People and Reset all the page to Default
        /// </summary>
        public void UpdateList()
        {
            var result = conn.Table<Person>(); // Connects to the Table
            lvPeople.ItemsSource = result.ToList(); // Return all the records from the Table
            ResetFields(); // Call the method to reset the fields
            ResetButtons(); // Call the method to reset the buttons
            lvPeople.IsEnabled = true; // Enable the List View in case the user clicked in Edit
        }

        /// <summary>
        /// Check if is the same person by checking FirstName, LastName and DOB
        /// </summary>
        /// <param name="pCompare">Person object to be compared</param>
        /// <returns>True if found same person. False if not</returns>
        private Boolean IsSamePerson(Person pCompare)
        {
            // Try to return a person with same Name and DOB
            var result = conn.Table<Person>().FirstOrDefault(p => p.FirstName == pCompare.FirstName && p.LastName == pCompare.LastName && p.DOB == pCompare.DOB);
            if (result != null) {
                return true; // If found return True
            }
            else {
                return false; // If not found return False
            }
        }

        /// <summary>
        /// Cast the gender to populate combobox
        /// </summary>
        /// <param name="gender">Gender as String</param>
        /// <returns>0 - Male, 1 - Female, 2 - Other</returns>
        private int getGender(string gender) {
            if (gender == "Male")
            {
                return 0;
            }
            else if (gender == "Female") {
                return 1;
            }else { return 2; }
        }

        /// <summary>
        /// Check if the email is valid or not
        /// </summary>
        /// <param name="emailAddress">Email address as String</param>
        /// <returns>True if valid, otherwise False</returns>
        private bool isEmailValid(string emailAddress) {
            return new EmailAddressAttribute().IsValid(emailAddress);
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a new Person
                Person newPerson = new Person
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    DOB = dpDOB.Date.Date.ToString("dd/MM/yyyy"),
                    Gender = cbxGender.SelectionBoxItem.ToString(),
                    Email = txtEmail.Text,
                    Phone = int.Parse(txtPhone.Text)
                };

                // Check if First Name and Last Name are filled
                if (txtFirstName.Text.Length == 0 || txtLastName.Text.Length == 0)
                {
                    msg = new MessageDialog("No value entered for First Name or Last Name", "Oops..!");
                    await msg.ShowAsync();
                }
                else if (IsSamePerson(newPerson))
                {
                    // Check if it is same person
                    msg = new MessageDialog("Person already exist, Try Different Name", "Oops..!");
                    await msg.ShowAsync();
                }
                else if (!isEmailValid(txtEmail.Text))
                {
                    // Email is not valid
                    msg = new MessageDialog("Email is not valid", "Oops..!");
                    await msg.ShowAsync();
                }
                else
                {
                    conn.Insert(newPerson); // Insert the new Person
                    UpdateList(); // Update the list and page                    
                }
            }
            catch (Exception ex)
            {
                // Check for any Formate Exception (Usualy with the Phone number)
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Phone number or entered an invalid Phone number", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Database error: " + ex.Message, "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Generic error: " + ex.Message, "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected person
                Person selectedPerson = (Person)lvPeople.SelectedItem;
                // If for any reason the person is Null do not continue
                if (selectedPerson == null)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    // If ok, delete the selected person
                    conn.Delete(selectedPerson);
                    UpdateList(); // Update the list and page
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Show the buttons Save and Cancel editing
            btnSave.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            // Hide the buttons not necessary for editing
            btnEdit.Visibility = Visibility.Collapsed;            
            btnAdd.Visibility = Visibility.Collapsed;
            btnDelete.Visibility = Visibility.Collapsed;

            lvPeople.IsEnabled = false; // Disable the List View to prevent from changing the selected item

            try
            {
                // Get the selected person 
                Person selectedPerson = (Person)lvPeople.SelectedItem;
                if (selectedPerson == null)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    // Update the fields of the selected person
                    txtFirstName.Text = selectedPerson.FirstName;
                    txtLastName.Text = selectedPerson.LastName;
                    dpDOB.Date = DateTime.Parse(selectedPerson.DOB);
                    cbxGender.SelectedIndex = getGender(selectedPerson.Gender);
                    txtEmail.Text = selectedPerson.Email;
                    txtPhone.Text = selectedPerson.Phone.ToString();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the selected person
                Person selectedPerson = (Person)lvPeople.SelectedItem;
                if (selectedPerson == null)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    // Check if First Name and Last Name are filled
                    if (txtFirstName.Text.Length == 0 || txtLastName.Text.Length == 0)
                    {
                        msg = new MessageDialog("No value entered for First Name or Last Name", "Oops..!");
                        await msg.ShowAsync();
                    }else {
                        // Update the selected person info
                        selectedPerson.FirstName = txtFirstName.Text;
                        selectedPerson.LastName = txtLastName.Text;
                        selectedPerson.DOB = dpDOB.Date.Date.ToString("dd/MM/yyyy");
                        selectedPerson.Gender = cbxGender.SelectionBoxItem.ToString();
                        selectedPerson.Email = txtEmail.Text;
                        selectedPerson.Phone = int.Parse(txtPhone.Text);

                        // Send the update to the Database
                        conn.Update(selectedPerson);

                        UpdateList(); // Update the list and page
                    }
                    
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Phone number or entered an invalid Phone number", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Database error: " + ex.Message, "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Generic error: " + ex.Message, "Oops..!");
                    await dialog.ShowAsync();
                }
            }
           
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            UpdateList(); // Cancel the editing
        }

        private void lvPeople_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When the user selects and item in the list, shows up the buttons for Editing and Deleting.
            btnDelete.Visibility = Visibility.Visible;
            btnEdit.Visibility = Visibility.Visible;           
        }
    }
}
