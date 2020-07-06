using BookStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Abstractions
{
    public interface IBooksRepository
    {
        Task<Book> CreateAsync(Book book);
        Task<Book> UpdateAsync(Guid isbn, Book book);
        Task DeleteAsync(Guid isbn);
        Task<Book> GetAsync(Guid isbn);
        Task<List<Book>> GetAllAsync();
    }
}
