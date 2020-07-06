using System;

namespace BookStore.Data.Models
{
    public class Book
    {
        public Guid ISBN { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public DateTime PublishedOn { get; set; }
    }
}
