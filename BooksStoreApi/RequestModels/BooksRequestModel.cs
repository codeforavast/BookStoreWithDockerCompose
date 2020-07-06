using System;

namespace BooksStore.Api.RequestModels
{
    public class BooksRequestModel
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Author { get; set; }
        public DateTime PublishedOn { get; set; }
    }
}
