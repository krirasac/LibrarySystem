using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Template_Login.Models; // Assuming your models are in this namespace
using Template_Login.Data;   // Assuming you have access to your DbContext

namespace Template_Login.Pages
{
    public partial class CataloguePage : Page
    {
        private LibraryDbContext _context;

        public CataloguePage()
        {
            InitializeComponent();
            _context = new LibraryDbContext();
            LoadBooks();
        }

        private void LoadBooks()
        {
            var books = _context.Books.ToList();
            BooksDataGrid.ItemsSource = books;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchBox.Text.Trim().ToLower();
            bool onlyAvailable = AvailabilityFilter.IsChecked == true;

            var books = _context.Books
                .Where(b =>
                    (b.Title.ToLower().Contains(keyword) ||
                     b.Author.ToLower().Contains(keyword) ||
                     b.ISBN.ToLower().Contains(keyword)) &&
                    (!onlyAvailable || b.IsAvailable))
                .ToList();

            BooksDataGrid.ItemsSource = books;
        }
    }
}
