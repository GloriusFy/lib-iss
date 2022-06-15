using Newtonsoft.Json;
using System;

namespace lib_iis.Models
{
    public class Book 
    {
        [JsonProperty("id")]
        public uint BookId { get; set; }
        [JsonProperty("isbn")]
        public string ISBN { get; set; }
        [JsonProperty("plainisbn")]
        public string PlainISBN { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("authors")]
        public string Authors { get; set; }
        [JsonProperty("publishyear")]
        public ushort PublishingYear { get; set; }
        [JsonProperty("dateadded")]
        public DateTime DateAdded { get; set; }
        [JsonProperty("totalcopies")]
        public uint TotalCopies { get; set; }
        [JsonProperty("avaliblecopies")]
        public uint AvailableCopies { get; set; }
        [JsonProperty("totalcheckouts")]
        public uint TotalCheckOuts { get; set; }
        public Book ClearBookDetails()
        {
            Title = string.Empty;
            Authors = string.Empty;
            ISBN = string.Empty;
            PlainISBN = string.Empty;
            TotalCopies = 0;
            PublishingYear = 0;
            return this;
        }

    }
}