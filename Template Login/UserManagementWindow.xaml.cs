using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Template_Login
{
    public partial class UserManagementWindow : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NULibraryConnectionString);
        private Student _selectedUser;

        public UserManagementWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                var users = from u in db.Students select u;
                userDataGrid.ItemsSource = users.ToList();
                statusTextBlock.Text = "Users loaded successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
                statusTextBlock.Text = "Error loading users.";
            }
        }

        private void addNewUserButton_Click(object sender, RoutedEventArgs e)
        {
            string userId = addUserIdTextBox.Text.Trim();
            string password = addPasswordTextBox.Text.Trim();
            string userName = addUserNameTextBox.Text.Trim();
            string selectedRole = (addRoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password) &&
                !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(selectedRole))
            {
                if (!db.Students.Any(u => u.StudentID == userId))
                {
                    var newUser = new Student
                    {
                        StudentID = userId,
                        Password = password,
                        Name = userName,
                        UserRole = selectedRole
                    };

                    db.Students.InsertOnSubmit(newUser);

                    try
                    {
                        db.SubmitChanges();
                        LoadUsers();
                        statusTextBlock.Text = $"User '{userId}' added successfully with Role: '{selectedRole}'.";

                        addUserIdTextBox.Clear();
                        addPasswordTextBox.Clear();
                        addUserNameTextBox.Clear();
                        addRoleComboBox.SelectedIndex = -1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error adding user: " + ex.Message);
                        statusTextBlock.Text = "Error adding user.";
                    }
                }
                else
                {
                    MessageBox.Show($"User with ID '{userId}' already exists.");
                    statusTextBlock.Text = "User already exists.";
                }
            }
            else
            {
                MessageBox.Show("User ID, password, user name, and role cannot be empty.");
                statusTextBlock.Text = "All fields are required.";
            }
        }

        private void userDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (userDataGrid.SelectedItem != null && userDataGrid.SelectedItem is Student selectedUser)
            {
                _selectedUser = selectedUser;
                updateUserIdTextBox.Text = _selectedUser.StudentID;
                updatePasswordTextBox.Text = _selectedUser.Password;
                updateUserNameTextBox.Text = _selectedUser.Name;
                updateRoleComboBox.Text = _selectedUser.UserRole;

                viewUserIdTextBox.Text = _selectedUser.StudentID;
                viewPasswordTextBox.Text = _selectedUser.Password;
                viewUserNameTextBox.Text = _selectedUser.Name;
                viewRoleTextBox.Text = _selectedUser.UserRole;

                saveUserButton.IsEnabled = true;
                deleteSelectedUserButton.IsEnabled = true;
            }
            else
            {
                _selectedUser = null;
                updateUserIdTextBox.Clear();
                updatePasswordTextBox.Clear();
                updateUserNameTextBox.Clear();
                updateRoleComboBox.SelectedIndex = -1;
                viewUserIdTextBox.Clear();
                viewPasswordTextBox.Clear();
                viewUserNameTextBox.Clear();
                viewRoleTextBox.Clear();
                saveUserButton.IsEnabled = false;
                deleteSelectedUserButton.IsEnabled = false;
            }
        }

        private void saveUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser != null)
            {
                _selectedUser.Password = updatePasswordTextBox.Text.Trim();
                _selectedUser.Name = updateUserNameTextBox.Text.Trim();
                string selectedRole = (updateRoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (!string.IsNullOrEmpty(selectedRole))
                {
                    _selectedUser.UserRole = selectedRole;

                    try
                    {
                        db.SubmitChanges();
                        LoadUsers();
                        statusTextBlock.Text = $"User '{_selectedUser.StudentID}' updated successfully.";

                        updateUserIdTextBox.Clear();
                        updatePasswordTextBox.Clear();
                        updateUserNameTextBox.Clear();
                        updateRoleComboBox.SelectedIndex = -1;
                        viewUserIdTextBox.Clear();
                        viewPasswordTextBox.Clear();
                        viewUserNameTextBox.Clear();
                        viewRoleTextBox.Clear();
                        saveUserButton.IsEnabled = false;
                        _selectedUser = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating user: " + ex.Message);
                        statusTextBlock.Text = "Error updating user.";
                    }
                }
                else
                {
                    MessageBox.Show("Please select a role to update.");
                    statusTextBlock.Text = "Please select a role.";
                }
            }
            else
            {
                MessageBox.Show("No user selected for update.");
                statusTextBlock.Text = "No user selected.";
            }
        }

        private void deleteSelectedUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (userDataGrid.SelectedItem != null && userDataGrid.SelectedItem is Student selectedUser)
            {
                _selectedUser = selectedUser;
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete user '{_selectedUser.StudentID}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.Students.DeleteOnSubmit(_selectedUser);
                        db.SubmitChanges();

                        LoadUsers();
                        statusTextBlock.Text = $"User '{_selectedUser.StudentID}' deleted successfully.";

                        deleteSelectedUserButton.IsEnabled = false;
                        _selectedUser = null;

                        updateUserIdTextBox.Clear();
                        updatePasswordTextBox.Clear();
                        updateUserNameTextBox.Clear();
                        updateRoleComboBox.SelectedIndex = -1;
                        viewUserIdTextBox.Clear();
                        viewPasswordTextBox.Clear();
                        viewUserNameTextBox.Clear();
                        viewRoleTextBox.Clear();
                        saveUserButton.IsEnabled = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting user: {ex.Message}", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        statusTextBlock.Text = "Error deleting user.";
                    }
                }
            }
            else
            {
                MessageBox.Show("No user selected for deletion.");
                statusTextBlock.Text = "No user selected.";
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
