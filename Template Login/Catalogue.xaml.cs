using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Interaction logic for Catalogue.xaml
    /// </summary>
    public partial class Catalogue : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NULibraryConnectionString2);
        private LibraryCatalog catalog;
        string staff;

        public Catalogue(string id)
        {
            InitializeComponent();
            LoadBooks();
            staff = id;
        }

        private void LoadBooks()
        {
            var books = db.LibraryCatalogs.ToList();
            BooksDataGrid.ItemsSource = books;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchBox.Text.Trim().ToLower();
            bool onlyAvailable = AvailabilityFilter.IsChecked == true;

            var books = db.LibraryCatalogs
                .Where(b =>
                    (b.Title.ToLower().Contains(keyword) ||
                     b.Author.ToLower().Contains(keyword) ||
                     b.ISBN.ToLower().Contains(keyword)));

            BooksDataGrid.ItemsSource = books.ToList();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            LibrarianWindow librarian = new LibrarianWindow(staff);
            librarian.Show();
            this.Close();
        }
    }
}
    

