using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BooksStore.Api.ResponseModels
{
    public class ApiResponseModel
    {
        public bool Status { get; set; }
        public ModelStateDictionary ModelState { get; set; }
    }
}
