using Newtonsoft.Json;
using System;

namespace lib_iis.Models
{
    public class Customer 
    {
        [JsonProperty("id")]
        public uint CustomerId { get; set; }
        [JsonProperty("firstname")]
        public string FirstName { get; set; }
        [JsonProperty("lastname")]
        public string LastName { get; set; }
        [JsonProperty("fullname")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("birthdate")]
        public DateTime DateOfBirth { get; set; }
        [JsonProperty("accountcreateddate")]
        public DateTime AccountCreatedOn { get; set; }

        public Customer ClearCustomerDetails()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            DateOfBirth = DateTime.Now.Date;
            return this;
        }
    }

}
