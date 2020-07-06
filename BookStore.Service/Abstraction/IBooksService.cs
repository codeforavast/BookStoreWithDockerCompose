using BookStore.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Service.Abstraction
{
    public interface IBooksService
    {
        Task<Book> CreateAsync(Book book);
        Task<Book> UpdateAsync(Guid isbn, Book book);
        Task DeleteAsync(Guid isbn);
        Task<Book> GetAsync(Guid isbn);
        Task<List<Book>> GetAllAsync();

    }
}
