using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Template_Login
{
    public partial class UserManagementWindow : Window
    {
        private readonly DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NULibraryConnectionString1);
        private User _selectedUser;

        public UserManagementWindow()
        {
            InitializeComponent();
            LoadRoles();
            LoadUsers();
        }

        private void LoadRoles()
        {
            try
            {
                var roles = db.Roles.Select(r => r.RoleDesc).ToList();
                addRoleComboBox.ItemsSource = roles;
                updateRoleComboBox.ItemsSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading roles: " + ex.Message);
            }
        }

        private void LoadUsers()
        {
            try
            {
                var users = from u in db.Users
                            join r in db.Roles on u.RoleID equals r.RoleID
                            select new
                            {
                                u.UserID,
                                u.Password,
                                Role = r.RoleDesc
                            };

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
            string selectedRole = addRoleComboBox.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(selectedRole))
            {
                MessageBox.Show("User ID, password, and role are required.");
                return;
            }

            if (db.Users.Any(u => u.UserID == userId))
            {
                MessageBox.Show($"User '{userId}' already exists.");
                return;
            }

            try
            {
                string roleId = db.Roles.First(r => r.RoleDesc == selectedRole).RoleID;

                var newUser = new User
                {
                    UserID = userId,
                    Password = password,
                    RoleID = roleId
                };

                db.Users.InsertOnSubmit(newUser);
                db.SubmitChanges();

                LoadUsers();
                ClearAddFields();
                statusTextBlock.Text = $"User '{userId}' added successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding user: " + ex.Message);
            }
        }

        private void ClearAddFields()
        {
            addUserIdTextBox.Clear();
            addPasswordTextBox.Clear();
            addRoleComboBox.SelectedIndex = -1;
        }

        private void userDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (userDataGrid.SelectedItem is null)
            {
                ClearUpdateFields();
                return;
            }

            var selectedItem = userDataGrid.SelectedItem;
            var userIdProp = selectedItem.GetType().GetProperty("UserID");
            string selectedId = userIdProp?.GetValue(selectedItem)?.ToString();

            if (string.IsNullOrEmpty(selectedId)) return;

            _selectedUser = db.Users.FirstOrDefault(u => u.UserID == selectedId);

            if (_selectedUser != null)
            {
                string roleDesc = db.Roles.First(r => r.RoleID == _selectedUser.RoleID).RoleDesc;

                updateUserIdTextBox.Text = _selectedUser.UserID;
                updatePasswordTextBox.Text = _selectedUser.Password;
                updateRoleComboBox.SelectedItem = roleDesc;

                viewUserIdTextBox.Text = _selectedUser.UserID;
                viewPasswordTextBox.Text = _selectedUser.Password;
                viewRoleTextBox.Text = roleDesc;

                saveUserButton.IsEnabled = true;
                deleteSelectedUserButton.IsEnabled = true;
            }
        }

        private void saveUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null) return;

            string newPassword = updatePasswordTextBox.Text.Trim();
            string selectedRole = updateRoleComboBox.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedRole))
            {
                MessageBox.Show("Please select a valid role.");
                return;
            }

            try
            {
                string roleId = db.Roles.First(r => r.RoleDesc == selectedRole).RoleID;

                _selectedUser.Password = newPassword;
                _selectedUser.RoleID = roleId;

                db.SubmitChanges();
                LoadUsers();
                ClearUpdateFields();
                statusTextBlock.Text = "User updated successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user: " + ex.Message);
            }
        }

        private void deleteSelectedUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser == null) return;

            var result = MessageBox.Show($"Delete user '{_selectedUser.UserID}'?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                db.Users.DeleteOnSubmit(_selectedUser);
                db.SubmitChanges();
                LoadUsers();
                ClearUpdateFields();
                statusTextBlock.Text = "User deleted.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting user: " + ex.Message);
            }
        }

        private void ClearUpdateFields()
        {
            _selectedUser = null;

            updateUserIdTextBox.Clear();
            updatePasswordTextBox.Clear();
            updateRoleComboBox.SelectedIndex = -1;

            viewUserIdTextBox.Clear();
            viewPasswordTextBox.Clear();
            viewRoleTextBox.Clear();

            saveUserButton.IsEnabled = false;
            deleteSelectedUserButton.IsEnabled = false;
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
