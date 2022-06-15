using System;

namespace lib_iis.Models
{
    public class BookTransaction  
    {
        public int TrainsactionId { get; set; }
        public Customer Customer { get; set; }
        public Book Book { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
