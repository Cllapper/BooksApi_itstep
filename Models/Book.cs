using System.ComponentModel.DataAnnotations;

namespace BooksApi.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва книги є обов'язковим полем.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ім'я автора є обов'язковим полем.")]
        public string Author { get; set; } = string.Empty;

        [Range(1, 2025, ErrorMessage = "Рік має бути дійсним значенням.")]
        public int Year { get; set; }
    }
}
