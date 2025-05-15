using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Template_Login
{
    /// <summary>
    /// Interaction logic for LibrarianWindow.xaml
    /// </summary>
    public partial class LibrarianWindow : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NULibraryConnectionString1);
        private string staff;
        private Staff librarian;

        public LibrarianWindow(string id)
        {
            InitializeComponent();
            staff = id;
            librarian = db.Staffs.FirstOrDefault(s => s.StaffID == staff);
            lbLibrarian.Content = $"Welcome, {librarian.Name}";


        }

        private void btnTransaction_Click(object sender, RoutedEventArgs e)
        {
            Circulation transact = new Circulation(staff);
            transact.Show();
            this.Close();
        }

        private void btnCatalogue_Click(object sender, RoutedEventArgs e)
        {
            Catalogue library = new Catalogue(staff);
            library.Show();
            this.Close();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}
