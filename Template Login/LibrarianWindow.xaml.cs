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
        private string staff;

        public LibrarianWindow(string id)
        {
            InitializeComponent();
            staff = id;
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
