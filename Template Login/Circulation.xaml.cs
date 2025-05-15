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
    /// Interaction logic for Circulation.xaml
    /// </summary>
    public partial class Circulation : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NULibraryConnectionString2);
        private Transaction transaction;
        private Student borrower;
        private LibraryCatalog catalog;

        private string staff;


        public Circulation(string id)
        {
            InitializeComponent();
            LoadTransaction();
            LoadStudent();
            LoadBook();

            staff = id;
        }

        //Loads list (transaction, students, and books)
        private void LoadTransaction()
        {
            try
            {
                FilterList();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void LoadStudent()
        {
            var student = from t in db.Transactions
                          join s in db.Students on t.StudentID equals s.StudentID
                          select s.Name;

            cbStudent.ItemsSource = student.ToList();

        }

        private void LoadBook()
        {
            var book = from t in db.Transactions
                       join b in db.LibraryCatalogs on t.BookID equals b.BookID
                       select b.Title;

            cbBook.ItemsSource = book.ToList();
        }

        //displays based on selection
        private void gridTrasactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridTrasactions.SelectedIndex == -1) { return; };

            string student = FindItem("Name");
            string book = FindItem("Title");
            
            if (string.IsNullOrEmpty(student) || string.IsNullOrEmpty(book)) return;

            borrower = db.Students.FirstOrDefault(u => u.Name == student);
            catalog = db.LibraryCatalogs.FirstOrDefault(u => u.Title == book);
            cbStudent.SelectedItem = borrower.Name;
            cbBook.SelectedItem = catalog.Title;
            lblAvail.Content = $"Available Copies: {catalog.NoOfAvailableCopies}";

        }

        private string FindItem(string property)
        {
            var selectedItem = gridTrasactions.SelectedItem;
            var userIdProp = selectedItem.GetType().GetProperty(property);

            return userIdProp?.GetValue(selectedItem)?.ToString();
        }

        private void FilterList()
        {
            var transactList = from t in db.Transactions
                               join s in db.Students on t.StudentID equals s.StudentID
                               join b in db.LibraryCatalogs on t.BookID equals b.BookID
                               join ts in db.TransactionStatus on t.StatusID equals ts.StatusID
                               join st in db.Staffs on t.StaffID equals st.StaffID
                               where t.StatusID == "TS01" || t.StatusID == "TS03"

                               select new
                               {
                                   t.TransactionID,
                                   s.Name,
                                   b.Title,
                                   t.BorrowedDate,
                                   t.ReturnDate,
                                   ts.TransactionDesc
                               };

            gridTrasactions.ItemsSource = transactList.ToList();
        }

        private void btnBorrow_Click(object sender, RoutedEventArgs e)
        {
            if (cbStudent.SelectedIndex == -1 || cbBook.SelectedIndex == -1) { return; }

            string lastID = db.Transactions.OrderByDescending(t => t.TransactionID)
                .Select(t => t.TransactionID)
                .FirstOrDefault();

            int num = int.Parse(lastID.Substring(1)) + 1;
            string id = "T"+num;
            
            string name = cbStudent.SelectedItem.ToString();
            string book = cbBook.SelectedItem.ToString();

            catalog = db.LibraryCatalogs.FirstOrDefault(u => u.Title == book);
            borrower = db.Students.FirstOrDefault(u => u.Name == name);

            if (catalog.NoOfAvailableCopies == 0) { MessageBox.Show("There are currently no available copies of this book."); return; }

            var newTransact = new Transaction
            {
                TransactionID = id,
                StudentID = borrower.StudentID,
                BookID = catalog.BookID,
                BorrowedDate = DateTime.Now,
                ReturnDate = DateTime.Now.AddMonths(1),
                StatusID = "TS01",
                StaffID = staff
            };

            db.Transactions.InsertOnSubmit(newTransact);
            db.SubmitChanges();
            LoadTransaction();
            ClearCB();

            MessageBox.Show("A new entry has been added to the list");

        }

        private void cbBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbBook.SelectedItem == null) { return; }
            catalog = db.LibraryCatalogs.FirstOrDefault(u => u.Title == cbBook.SelectedItem.ToString());
            lblAvail.Content = $"Available Copies: {catalog.NoOfAvailableCopies}";

        }
        private void ClearCB()
        {
            cbStudent.SelectedIndex = -1;
            cbBook.SelectedIndex = -1;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            string id = FindItem("TransactionID");
            string status = FindItem("TransactionDesc");

            if (status == "TS03")
            {
                string lastID = db.Fines.OrderByDescending(t => t.FineID)
                .Select(t => t.FineID)
                .FirstOrDefault();

                int num = int.Parse(lastID.Substring(1)) + 1;
                string fineID = "F" + num;

                db.ReturnBookAndApplyFine(id,fineID);
            }

            else
            {
                transaction = db.Transactions.FirstOrDefault(u => u.TransactionID == id);
                transaction.StatusID = "TS02";
                db.SubmitChanges();
                ClearCB();
                LoadTransaction();

                MessageBox.Show("An entry has been updated");
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            LibrarianWindow librarian = new LibrarianWindow(staff);
            librarian.Show();
            this.Close();

        }
    }
}
