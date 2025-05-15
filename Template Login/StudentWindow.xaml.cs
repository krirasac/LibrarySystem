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
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        private Student loggedInStudent;
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NULibraryConnectionString1);

        public StudentWindow(Student student)
        {
            InitializeComponent();
            loggedInStudent = student;
            LoadStudentData();
            LoadTransactionHistory();
            LoadOverdueFines();
        }
        private void LoadStudentData()
        {
            nameTextBlock.Text = $"Name: {loggedInStudent.Name}";
            contactTextBlock.Text = $"Contact: {loggedInStudent.ContactNumber}";
            emailTextBlock.Text = $"Email: {loggedInStudent.StudentEmail}";
            courseTextBlock.Text = $"Course ID: {loggedInStudent.CourseID}";
        }

        private void LoadTransactionHistory()
        {
            var transactions = from t in db.Transactions
                               where t.StudentID == loggedInStudent.StudentID
                               select new
                               {
                                   t.TransactionID,
                                   t.BookID,
                                   t.BorrowedDate,
                                   t.ReturnDate,
                                   t.ActualReturnDate
                               };

            transactionDataGrid.ItemsSource = transactions.ToList();
        }

        private void LoadOverdueFines()
        {
            var fines = from f in db.Fines
                        join t in db.Transactions on f.TransactionID equals t.TransactionID
                        where t.StudentID == loggedInStudent.StudentID && f.StatusID == "Overdue"
                        select new
                        {
                            f.FineAmount,
                            f.DateOfPayment
                        };

            finesDataGrid.ItemsSource = fines.ToList();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {

            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }
    }
}
