using System;

namespace BooksStore.Api.RequestModels
{
    public class BooksUpdateRequestModel : BooksRequestModel
    {
        public Guid ISBN { get; set; }
    }
}
