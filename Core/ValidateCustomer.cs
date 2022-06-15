using lib_iis.Models;
using System;
using System.Net.Mail;

namespace lib_iis
{
    partial class Validation
    {
        public bool ValidateCustomer(Customer customer)
        {
            if (String.IsNullOrWhiteSpace(customer.FirstName) ||
               !IsValidEmail(customer.Email) ||
               customer.DateOfBirth.Date == DateTime.Now.Date)
            {
                return false;
            }

            return true;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch (FormatException e)
            {
                return false;
            }
        }
    }
}
