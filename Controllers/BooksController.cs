using Microsoft.AspNetCore.Mvc;
using BooksApi.Models;

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

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            return Ok(_books);
        }

        [HttpGet("{id}")]
        public ActionResult<Book> GetBookById(int id)
        {
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            newBook.Id = _books.Any() ? _books.Max(b => b.Id) + 1 : 1;

            _books.Add(newBook);

            return CreatedAtAction(nameof(GetBookById), new { id = newBook.Id }, newBook);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] Book updatedBook)
        {
            if (id != updatedBook.Id)
            {
                return BadRequest("ID книги в URL та в тілі запиту не співпадають.");
            }

            var existingBook = _books.FirstOrDefault(b => b.Id == id);
            if (existingBook == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.Year = updatedBook.Year;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var bookToRemove = _books.FirstOrDefault(b => b.Id == id);
            if (bookToRemove == null)
            {
                return NotFound();
            }

            _books.Remove(bookToRemove);

            return NoContent();
        }
    }
}
