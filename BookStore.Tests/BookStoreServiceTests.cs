using AutoMapper;
using BookStore.Data.Abstractions;
using DataModels = BookStore.Data.Models;
using BookStore.Service;
using ServiceModel = BookStore.Service.Models;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System;
using BookStore.Service.Mappers;
using BookStore.Service.Models;

namespace BookStore.Tests
{
    public class BookStoreServiceTests
    {
        private Mock<IBooksRepository> _booksRepositoryMock;
        private List<Guid> _isbns;

        public BookStoreServiceTests()
        {
            _booksRepositoryMock = new Mock<IBooksRepository>();
            _isbns = new List<Guid>();
            _isbns.AddRange(new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_AllRecordsIfAnyExists()
        {
            //arrange
            var resultSet = MockBooksRepositoryReturn();
            _booksRepositoryMock.Setup(x => x.GetAllAsync()).Returns(resultSet).Verifiable();
            var sut = new BooksService(_booksRepositoryMock.Object, GetMapper());

            
            //act
            var result = await sut.GetAllAsync();

            //assert
            Assert.NotEmpty(result);
            Assert.IsType<List<ServiceModel.Book>>(result);
            Assert.Equal(result.Count, resultSet.Result.Count);
            for (var i = 0; i < result.Count; i++)
            {
                AssertDataModelBook_ServiceModelBook(resultSet.Result[i], result[i]);
            }

            _booksRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        

        [Fact]
        public async Task GetAllAsync_ShouldReturn_EmptyIfNoRecordsExists()
        {
            //arrange            
            _booksRepositoryMock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(new List<DataModels.Book>())).Verifiable();
            var sut = new BooksService(_booksRepositoryMock.Object, GetMapper());
                       
            //act
            var result = await sut.GetAllAsync();

            //assert
            Assert.Empty(result);
            Assert.IsType<List<ServiceModel.Book>>(result);
            _booksRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Fact]
        public async Task GetAsync_ShouldReturnNull_IfNoMatchingISBNExist()
        {
            //arrange         
            var isbn = Guid.NewGuid();
            _booksRepositoryMock.Setup(x => x.GetAsync(isbn)).Returns(Task.FromResult<DataModels.Book>(null)).Verifiable();
            var sut = new BooksService(_booksRepositoryMock.Object, GetMapper());

            //act
            var result = await sut.GetAsync(isbn);

            //assert
            Assert.Null(result);
            _booksRepositoryMock.Verify(x => x.GetAsync(isbn), Times.Once());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetAsync_ShouldReturnBookFor_ISBN1(int recordNum)
        {
            //arrange
            var isbn = _isbns[recordNum];
            var bookFromRepo = MockBooksRepositoryReturn().Result.Find(x => x.ISBN == isbn);
            _booksRepositoryMock.Setup(x => x.GetAsync(isbn)).ReturnsAsync(bookFromRepo).Verifiable();
            var sut = new BooksService(_booksRepositoryMock.Object, GetMapper());

            //act
            var result = await sut.GetAsync(isbn);

            //assert
            Assert.NotNull(result);
            Assert.IsType<ServiceModel.Book>(result);
            AssertDataModelBook_ServiceModelBook(bookFromRepo, result);
            _booksRepositoryMock.Verify(x => x.GetAsync(isbn), Times.Once());
        }


        [Fact]
        public async Task CreateAsync_ShouldCorrectlyReturn_CreatedObject()
        {
            //arrange
            var book = new DataModels.Book { ISBN = Guid.NewGuid(), PublishedOn = DateTime.Now, Author = "Test Author", Genre = "Test Genre", Title = "New Title" };
            _booksRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<DataModels.Book>())).ReturnsAsync(book).Verifiable();
            var sut = new BooksService(_booksRepositoryMock.Object, GetMapper());
            var serviceModelBook = GetMapper().Map<ServiceModel.Book>(book);

            //act
            var result = await sut.CreateAsync(serviceModelBook);

            //assert
            Assert.NotNull(result);
            Assert.IsType<ServiceModel.Book>(result);
            AssertServiceModelBook(serviceModelBook, result);
            _booksRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<DataModels.Book>()), Times.Once());
        }

        [Fact]
        public async Task UpdateAsync_ShouldCorrectlyReturn_UpdatedObject()
        {
            //arrange
            var book = new DataModels.Book { ISBN = Guid.NewGuid(), PublishedOn = DateTime.Now, Author = "Test Author", Genre = "Test Genre", Title = "New Title" };
            _booksRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<DataModels.Book>())).ReturnsAsync(book).Verifiable();
            var sut = new BooksService(_booksRepositoryMock.Object, GetMapper());
            var serviceModelBook = GetMapper().Map<ServiceModel.Book>(book);

            //act
            var result = await sut.UpdateAsync(book.ISBN, serviceModelBook);

            //assert
            Assert.NotNull(result);
            Assert.IsType<ServiceModel.Book>(result);
            AssertServiceModelBook(serviceModelBook, result);
            _booksRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<DataModels.Book>()), Times.Once());
        }



        private Task<List<DataModels.Book>> MockBooksRepositoryReturn()
        {
            var list = new List<DataModels.Book>();

            list.Add(new DataModels.Book { Title = "Title 1", Author = "Author 1", Genre = "Genre 1", ISBN = _isbns[0], PublishedOn = new DateTime(2010,01,21) });
            list.Add(new DataModels.Book { Title = "Title 2", Author = "Author 2", Genre = "Genre 2", ISBN = _isbns[1], PublishedOn = new DateTime(2011,02,12) });
            list.Add(new DataModels.Book { Title = "Title 3", Author = "Author 3", Genre = "Genre 3", ISBN = _isbns[2], PublishedOn = new DateTime(2012,03,2) });
            list.Add(new DataModels.Book { Title = "Title 4", Author = "Author 4", Genre = "Genre 4", ISBN = _isbns[3], PublishedOn = new DateTime(2013,04,5) });
            return Task.FromResult(list);
        }

        private IMapper GetMapper()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingModels());
            });
            var mapper = mockMapper.CreateMapper();
            return mapper;
        }

        private void AssertDataModelBook_ServiceModelBook(DataModels.Book dataModelBook, ServiceModel.Book serviceModelBook)
        {
            Assert.Equal(dataModelBook.Author, serviceModelBook.Author);
            Assert.Equal(dataModelBook.Genre, serviceModelBook.Genre);
            Assert.Equal(dataModelBook.ISBN, serviceModelBook.ISBN);
            Assert.Equal(dataModelBook.Title, serviceModelBook.Title);
            Assert.Equal(dataModelBook.PublishedOn, serviceModelBook.PublishedOn);
        }
        private void AssertServiceModelBook(Book serviceModelBook, Book result)
        {
            Assert.Equal(serviceModelBook.Author, result.Author);
            Assert.Equal(serviceModelBook.Genre, result.Genre);
            Assert.Equal(serviceModelBook.ISBN, result.ISBN);
            Assert.Equal(serviceModelBook.Title, result.Title);
            Assert.Equal(serviceModelBook.PublishedOn, result.PublishedOn);
        }

    }
}
