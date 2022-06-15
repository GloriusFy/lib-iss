using lib_iis.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lib_iis.Controls
{
    public partial class BookCardControl : UserControl
    {
        public Book Book
        {
            get { return (Book)GetValue(BookProperty); }
            set { SetValue(BookProperty, value); }
        }

        public static readonly DependencyProperty BookProperty =
            DependencyProperty.Register("Book", typeof(Book), typeof(BookCardControl), new PropertyMetadata(null, SetText));

        private static void SetText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BookCardControl bookCard = d as BookCardControl;

            if (bookCard != null)
            {
                var newValue = e.NewValue as Book;

                bookCard.bookTitle.Text = newValue.Title;
                bookCard.bookAuthors.Text = string.Format("Author(s) : {0}", newValue.Authors);
                bookCard.bookPublishingYear.Text = string.Format("Publishing Year : {0}", newValue.PublishingYear);
                bookCard.bookAddedDate.Text = string.Format("Date Added : {0}", newValue.DateAdded.ToString("dd/MM/yyyy"));
                bookCard.bookTotalCopies.Text = string.Format("Total Copies : {0}", newValue.TotalCopies.ToString());
                bookCard.bookAvailableCopies.Text = string.Format("Available Copies : {0}", newValue.AvailableCopies.ToString());

                var imageUrl = String.Format(@"http://covers.openlibrary.org/b/isbn/{0}-L.jpg", newValue.PlainISBN);
                bookCard.bookCover.Source = new BitmapImage(new Uri(imageUrl, UriKind.Absolute));
            }
        }
        public BookCardControl()
        {
            InitializeComponent();
        }
    }
}
