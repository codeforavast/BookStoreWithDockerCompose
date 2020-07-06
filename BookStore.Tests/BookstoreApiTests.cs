using AutoMapper;
using BooksStore.Api.Controllers;
using BooksStore.Api.Logging;
using BooksStore.Api.Mappers;
using BooksStore.Api.RequestModels;
using BooksStore.Api.ResponseModels;
using BookStore.Service.Abstraction;
using BookStore.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests
{
    public class BookstoreApiTests
    {
        private IMapper _mapper;
        private Mock<IBooksService> _booksServiceMock;
        private Mock<ILoggerManager> _logger;
        private List<Guid> _isbns;

        public BookstoreApiTests()
        {
            _mapper = GetMapper();
            _booksServiceMock = new Mock<IBooksService>();
            _logger = new Mock<ILoggerManager>();
            _isbns = new List<Guid>();
            _isbns.AddRange(new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });
        }

        [Fact]
        public async Task Get_ShouldReturn200_AndAllRecords()
        {
            //arrange
            var resultSet = MockBooksServiceReturn();
            _booksServiceMock.Setup(x => x.GetAllAsync()).Returns(resultSet).Verifiable();
            var controller = new BooksController(_booksServiceMock.Object, _mapper, _logger.Object);


            //act
            var result = await controller.Get();

            //assert
            var okRersult = Assert.IsType<OkObjectResult>(result);
            var resultData = (List<BooksViewModel>)okRersult.Value;
            Assert.Equal(resultSet.Result.Count, resultData.Count);
            for (var i = 0; i < resultData.Count; i++)
            {
                AssertServiceModelBook_ViewModelBook(resultSet.Result[i], resultData[i]);
            }
            _booksServiceMock.Verify(x => x.GetAllAsync(), Times.Once());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task Get_WithISBN_ShouldReturn200AndSpecificRecord(int recordNum)
        {
            //arrange
            var isbn = _isbns[recordNum];
            var resultSet = MockBooksServiceReturn().Result.Find(x => x.ISBN == isbn); ;
            _booksServiceMock.Setup(x => x.GetAsync(isbn)).ReturnsAsync(resultSet).Verifiable();
            var controller = new BooksController(_booksServiceMock.Object, _mapper, _logger.Object);


            //act
            var result = await controller.Get(isbn);

            //assert
            var okRersult = Assert.IsType<OkObjectResult>(result);
            var resultData = (BooksViewModel)okRersult.Value;
            Assert.NotNull(resultData);
            
            AssertServiceModelBook_ViewModelBook(resultSet, resultData);
            _booksServiceMock.Verify(x => x.GetAsync(isbn), Times.Once());
        }


        [Fact]
        public async Task Get_WithISBN_ShouldReturn400AndNoRecord()
        {
            //arrange
            var isbn = Guid.NewGuid();
            _booksServiceMock.Setup(x => x.GetAsync(isbn)).ReturnsAsync((Book)null).Verifiable();
            var controller = new BooksController(_booksServiceMock.Object, _mapper, _logger.Object);
            
            //act
            var result = await controller.Get(isbn);

            //assert
            var notFoundRersult = Assert.IsType<NotFoundResult>(result);
            _booksServiceMock.Verify(x => x.GetAsync(isbn), Times.Once());
        }

        [Fact]
        public async Task Post_ShouldReturn201_WithCreatedBook()
        {
            //arrange
            var requestBook = new BooksRequestModel { Title = "Title 1", Author = "Author 1", Genre = "Genre 1",  PublishedOn = new DateTime(2010, 01, 21) };
            var isbn = Guid.NewGuid();
            var serviceBook = _mapper.Map<Book>(requestBook);
            _booksServiceMock.Setup(x => x.CreateAsync(It.IsAny<Book>())).ReturnsAsync(serviceBook).Verifiable();
            var controller = new BooksController(_booksServiceMock.Object, _mapper, _logger.Object);
            

            //act
            var result = await controller.Post(requestBook);

            //assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var data = (BooksViewModel)createdAtActionResult.Value;
            AssertServiceModelBook_ViewModelBook(serviceBook, data);
            _booksServiceMock.Verify(x => x.CreateAsync(It.IsAny<Book>()), Times.Once());
        }

        [Fact]
        public async Task Put_ShouldReturn200_WithUpdatedBook()
        {
            //arrange
            var updateRequestBook = new BooksUpdateRequestModel { ISBN = Guid.NewGuid(), Title = "Title 1", Author = "Author 1", Genre = "Genre 1",  PublishedOn = new DateTime(2010, 01, 21) };
            var serviceBook = _mapper.Map<Book>(updateRequestBook);
            _booksServiceMock.Setup(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Book>())).ReturnsAsync(serviceBook).Verifiable();
            var controller = new BooksController(_booksServiceMock.Object, _mapper, _logger.Object);
            

            //act
            var result = await controller.Put(updateRequestBook.ISBN, updateRequestBook);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = (BooksViewModel)okResult.Value;
            AssertServiceModelBook_ViewModelBook(serviceBook, data);
            _booksServiceMock.Verify(x => x.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Book>()), Times.Once());
        }

        [Fact]
        public async Task Delete_ShouldReturn200_AndDeleteResource()
        {
            //arrange
            var isbn = Guid.NewGuid();
            _booksServiceMock.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).Verifiable();
            var controller = new BooksController(_booksServiceMock.Object, _mapper, _logger.Object);
            

            //act
            var result = await controller.Delete(isbn);

            //assert
            var okResult = Assert.IsType<OkResult>(result);
            _booksServiceMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Once());
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

        private Task<List<Book>> MockBooksServiceReturn()
        {
            var list = new List<Book>();

            list.Add(new Book { Title = "Title 1", Author = "Author 1", Genre = "Genre 1", ISBN = _isbns[0], PublishedOn = new DateTime(2010, 01, 21) });
            list.Add(new Book { Title = "Title 2", Author = "Author 2", Genre = "Genre 2", ISBN = _isbns[1], PublishedOn = new DateTime(2011, 02, 12) });
            list.Add(new Book { Title = "Title 3", Author = "Author 3", Genre = "Genre 3", ISBN = _isbns[2], PublishedOn = new DateTime(2012, 03, 2) });
            list.Add(new Book { Title = "Title 4", Author = "Author 4", Genre = "Genre 4", ISBN = _isbns[3], PublishedOn = new DateTime(2013, 04, 5) });
            return Task.FromResult(list);
        }

        private void AssertServiceModelBook_ViewModelBook(Book book, BooksViewModel viewModelBook)
        {
            Assert.Equal(book.Author, viewModelBook.Author);
            Assert.Equal(book.Genre, viewModelBook.Genre);
            Assert.Equal(book.ISBN, viewModelBook.ISBN);
            Assert.Equal(book.Title, viewModelBook.Title);
            Assert.Equal(book.PublishedOn, viewModelBook.PublishedOn);
        }
    }
}
