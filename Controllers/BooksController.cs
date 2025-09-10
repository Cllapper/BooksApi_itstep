using Microsoft.AspNetCore.Mvc;
using BooksApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace BooksApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private static List<Book> _books = new List<Book>
        {
            new Book { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", Year = 1840 },
            new Book { Id = 2, Title = "Лісова пісня", Author = "Леся Українка", Year = 1911 },
            new Book { Id = 3, Title = "Захар Беркут", Author = "Іван Франко", Year = 1883 }
        };

        private readonly IMemoryCache _cache;
        private readonly ILogger<BooksController> _logger;
        private const string AllBooksCacheKey = "AllBooksCache";

        public BooksController(IMemoryCache cache, ILogger<BooksController> logger)
        {
            _cache = cache;
            _logger = logger;
        }


        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            _logger.LogInformation("Отримано запит на GetAllBooks.");

            if (_cache.TryGetValue(AllBooksCacheKey, out IEnumerable<Book> books))
            {
                _logger.LogInformation(">>> Дані знайдено в кеші! Повертаємо кешовану версію.");
                return Ok(books);
            }

            _logger.LogWarning("!!! Кеш порожній. Завантажуємо дані з 'бази даних'.");
            books = _books.ToList();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(45))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(AllBooksCacheKey, books, cacheOptions);

            return Ok(books);
        }


        [HttpGet("{id}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public ActionResult<Book> GetBookById(int id)
        {
            _logger.LogInformation("Отримано запит GetBookById для id={id}. Цей лог НЕ з'явиться, якщо відповідь взято з HTTP кешу.", id);

            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }


        [HttpPost]
        public ActionResult<Book> CreateBook([FromBody] Book newBook)
        {
            newBook.Id = _books.Any() ? _books.Max(b => b.Id) + 1 : 1;
            _books.Add(newBook);

            _logger.LogWarning("Книгу додано. ІНВАЛІДАЦІЯ (видалення) кешу '{key}'", AllBooksCacheKey);
            _cache.Remove(AllBooksCacheKey);

            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
        {
            var existingBook = _books.FirstOrDefault(b => b.Id == id);
            if (existingBook == null) return NotFound();

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.Year = updatedBook.Year;

            _logger.LogWarning("Книгу оновлено. ІНВАЛІДАЦІЯ (видалення) кешу '{key}'", AllBooksCacheKey);
            _cache.Remove(AllBooksCacheKey);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var bookToRemove = _books.FirstOrDefault(b => b.Id == id);
            if (bookToRemove == null) return NotFound();

            _books.Remove(bookToRemove);

            _logger.LogWarning("Книгу видалено. ІНВАЛІДАЦІЯ (видалення) кешу '{key}'", AllBooksCacheKey);
            _cache.Remove(AllBooksCacheKey);

            return NoContent();
        }
    }
}
