using System;
using System.Linq;
using System.Windows;

namespace Template_Login
{
    public partial class adminHome : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NULibraryConnectionString1);

        public adminHome()
        {
            InitializeComponent();
            LoadCourses();
            LoadStudents();
        }

        private void logoutButton(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void manageUserAccountsButton(object sender, RoutedEventArgs e)
        {
            UserManagementWindow userManagementWindow = new UserManagementWindow();
            userManagementWindow.Show();
            this.Close();
        }

        private void LoadCourses()
        {
            try
            {
                var courses = from c in db.Courses select c;
                coursesDataGrid.ItemsSource = courses.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading courses: " + ex.Message);
            }
        }

        private void LoadStudents()
        {
            try
            {
                var students = from s in db.Students select s;
                studentsDataGrid.ItemsSource = students.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading students: " + ex.Message);
            }
        }
    }
}
