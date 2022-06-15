using lib_iis.Models;

namespace lib_iis
{
    public interface IValidation
    {
        bool ValidateBook(Book book);

        bool IsValidISBN(string iSBN);

        bool BookValidForCheckOut(Book book);

        bool BookValidForCheckIn(Book book);

        bool ValidateCustomer(Customer customer);

        bool IsValidEmail(string email);
    }
}
