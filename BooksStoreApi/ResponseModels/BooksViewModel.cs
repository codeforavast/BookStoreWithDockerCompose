using System;

namespace BooksStore.Api.ResponseModels
{
    public class BooksViewModel
    {
        public Guid ISBN { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public DateTime PublishedOn { get; set; }
    }
}
