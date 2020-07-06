using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BooksStore.Api.Logging;
using BooksStore.Api.RequestModels;
using BooksStore.Api.ResponseModels;
using BookStore.Service.Abstraction;
using BookStore.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public BooksController(IBooksService booksService, IMapper mapper, ILoggerManager logger)
        {
            _booksService = booksService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]        
        [ProducesResponseType(typeof(List<BooksViewModel>), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var books = _mapper.Map<List<BooksViewModel>>(await _booksService.GetAllAsync());
            _logger.LogInfo($"Total records returned: {books.Count}");
            return Ok(books);
        }

        [HttpGet("{isbn}")]
        [ProducesResponseType(typeof(List<BooksViewModel>), 200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<IActionResult> Get(Guid isbn)
        {
            var book = _mapper.Map<BooksViewModel>(await _booksService.GetAsync(isbn));
            if (book == null)
            {
                _logger.LogInfo($"No data found for requested ISBN: {isbn}");
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BooksViewModel),201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Post(BooksRequestModel booksRequest)
        {
            var mapingBook = _mapper.Map<Book>(booksRequest);
            var newBook = await _booksService.CreateAsync(mapingBook);
            var response = _mapper.Map<BooksViewModel>(newBook);
            return CreatedAtAction(nameof(Get), new { isbn = response.ISBN }, response);
        }

        [HttpPut("{isbn}")]
        [ProducesResponseType(typeof(BooksViewModel), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Put(Guid isbn, BooksUpdateRequestModel booksUpdateRequest)
        {
            if (booksUpdateRequest.ISBN != isbn)
            {
                _logger.LogWarn($"Request mismatch, requested ISBN: {isbn} does not match with books ISBN: {booksUpdateRequest.ISBN}");
                return BadRequest();
            }

            var mapingBook = _mapper.Map<Book>(booksUpdateRequest);
            var updatedBook = await _booksService.UpdateAsync(isbn, mapingBook);
            var response = _mapper.Map<BooksViewModel>(updatedBook);
            _logger.LogInfo($"book created successfuly, ISBN: {response.ISBN}");
            return Ok(response);
        }

        [HttpDelete("{isbn}")]
        [ProducesResponseType(typeof(BooksViewModel), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(Guid isbn)
        {
            await _booksService.DeleteAsync(isbn);
            _logger.LogInfo($"book deleted successfuly, ISBN: {isbn}");
            return Ok();
        }

    }
}