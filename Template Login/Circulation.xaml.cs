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
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NULibraryConnectionString1);
        private Transaction transaction;


        public Circulation()
        {
            InitializeComponent();
            LoadTransaction();
            LoadStudent();
            LoadBook();
        }

        private void gridTrasactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbStudent.SelectedItem = gridTrasactions.SelectedItem;
        }

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

        private void cbStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterList(cbStudent.SelectedItem.ToString());
        }

        private void FilterList(string filter)
        {
            var transactList = from t in db.Transactions
                               join s in db.Students on t.StudentID equals s.StudentID
                               join b in db.LibraryCatalogs on t.BookID equals b.BookID
                               join ts in db.TransactionStatus on t.StatusID equals ts.StatusID
                               join st in db.Staffs on t.StaffID equals st.StaffID
                               where s.Name == filter && (t.StatusID == "TS01" || t.StatusID == "TS03")

                               select new
                               {
                                   s.Name,
                                   b.Title,
                                   t.BorrowedDate,
                                   t.ReturnDate,
                                   ts.TransactionDesc
                               };

            gridTrasactions.ItemsSource = transactList.ToList();
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
            string name = cbStudent.SelectedItem.ToString();
        }
    }
}
