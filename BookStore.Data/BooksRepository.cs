using BookStore.Data.Abstractions;
using BookStore.Data.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public class BooksRepository : IBooksRepository
    {
        private readonly IDbConnection _dbConnection;

        public BooksRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<Book> CreateAsync(Book book)
        {
            var sql = "Insert into dbo.[Books] (ISBN, Title, Genre, Author, PublishedOn) VALUES (@ISBN, @Title, @Genre, @Author, @PublishedOn);";
            var parameters = new DynamicParameters();
            parameters.Add("@ISBN", book.ISBN, DbType.Guid);
            parameters.Add("@Title", book.Title, DbType.String);
            parameters.Add("@Genre", book.Genre, DbType.String);
            parameters.Add("@Author", book.Author, DbType.String);
            parameters.Add("@PublishedOn", book.PublishedOn, DbType.Date);

            await _dbConnection.ExecuteAsync(sql, parameters);
            return book;
        }

        public async Task DeleteAsync(Guid isbn)
        {
            var sql = "Delete from dbo.[Books] where ISBN = @ISBN;";
            var parameters = new DynamicParameters();
            parameters.Add("@ISBN", isbn, DbType.Guid);

            await _dbConnection.ExecuteAsync(sql, parameters);
        }

        public async Task<Book> GetAsync(Guid isbn)
        {
            var sql = "select ISBN, Title, Genre, Author, PublishedOn from dbo.[Books] where ISBN =  @ISBN;";
            var parameters = new DynamicParameters();
            parameters.Add("@ISBN", isbn, DbType.Guid);

            return await _dbConnection.QuerySingleOrDefaultAsync<Book>(sql, parameters);
        }

        public async Task<List<Book>> GetAllAsync()
        {
            var sql = "select ISBN, Title, Genre, Author, PublishedOn from dbo.[Books];";

            return (await _dbConnection.QueryAsync<Book>(sql)).AsList();
        }

        public async Task<Book> UpdateAsync(Guid isbn, Book book)
        {
            var sql = @"update Books
                            set
                            Title = @Title, Genre = @Genre, Author = @Author, PublishedOn = PublishedOn
                            where ISBN = @ISBN";
            var parameters = new DynamicParameters();
            parameters.Add("@ISBN", isbn, DbType.Guid);
            parameters.Add("@Title", book.Title, DbType.String);
            parameters.Add("@Genre", book.Genre, DbType.String);
            parameters.Add("@Author", book.Author, DbType.String);
            parameters.Add("@PublishedOn", book.PublishedOn, DbType.Date);

            await _dbConnection.ExecuteAsync(sql, parameters);
            return book;
        }
    }
}
