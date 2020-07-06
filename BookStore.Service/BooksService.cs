using AutoMapper;
using BookStore.Data.Abstractions;
using Db = BookStore.Data.Models;
using BookStore.Service.Abstraction;
using BookStore.Service.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Service
{
    public class BooksService : IBooksService
    {
        private readonly IBooksRepository _booksRepository;
        private readonly IMapper _mapper;

        public BooksService(IBooksRepository booksRepository, IMapper mapper)
        {
            _booksRepository = booksRepository;
            _mapper = mapper;
        }

        public async Task<Book> CreateAsync(Book book)
        {
            var dbBook = _mapper.Map<Db.Book>(book);
            var result = await _booksRepository.CreateAsync(dbBook);
            return book;
        }

        public async Task DeleteAsync(Guid isbn)
        {
            await _booksRepository.DeleteAsync(isbn);
        }

        public async Task<Book> GetAsync(Guid isbn)
        {
            var book = _mapper.Map<Book>(await _booksRepository.GetAsync(isbn));
            return book;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            var books = _mapper.Map<List<Book>>(await _booksRepository.GetAllAsync());
            return books;
        }

        public async Task<Book> UpdateAsync(Guid isbn, Book book)
        {
            await _booksRepository.UpdateAsync(isbn, _mapper.Map<Db.Book>(book));
            return book;
        }
    }
}
