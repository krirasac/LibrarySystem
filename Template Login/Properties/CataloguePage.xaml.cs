using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Template_Login.Models;
using Template_Login.Data;

namespace Template_Login.Pages
{
    public partial class CataloguePage : Page
    {
        private LibraryDbContext _context;
        private Book _selectedBook;

        public CataloguePage()
        {
            InitializeComponent();
            _context = new LibraryDbContext();
            LoadBooks();
        }

        private void LoadBooks()
        {
            BooksDataGrid.ItemsSource = _context.Books.ToList();
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

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleInput.Text) || string.IsNullOrWhiteSpace(AuthorInput.Text) || string.IsNullOrWhiteSpace(ISBNInput.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Missing Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Book newBook = new Book
            {
                Title = TitleInput.Text,
                Author = AuthorInput.Text,
                ISBN = ISBNInput.Text,
                IsAvailable = AvailableInput.IsChecked == true
            };

            _context.Books.Add(newBook);
            _context.SaveChanges();
            LoadBooks();
            ClearInputs();
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBook == null)
            {
                MessageBox.Show("Please select a book to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedBook.Title = TitleInput.Text;
            _selectedBook.Author = AuthorInput.Text;
            _selectedBook.ISBN = ISBNInput.Text;
            _selectedBook.IsAvailable = AvailableInput.IsChecked == true;

            _context.SaveChanges();
            LoadBooks();
            ClearInputs();
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedBook == null)
            {
                MessageBox.Show("Please select a book to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _context.Books.Remove(_selectedBook);
            _context.SaveChanges();
            LoadBooks();
            ClearInputs();
        }

        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedBook = BooksDataGrid.SelectedItem as Book;
            if (_selectedBook != null)
            {
                TitleInput.Text = _selectedBook.Title;
                AuthorInput.Text = _selectedBook.Author;
                ISBNInput.Text = _selectedBook.ISBN;
                AvailableInput.IsChecked = _selectedBook.IsAvailable;
            }
        }

        private void ClearInputs()
        {
            TitleInput.Text = "";
            AuthorInput.Text = "";
            ISBNInput.Text = "";
            AvailableInput.IsChecked = false;
            _selectedBook = null;
        }
    }
}

