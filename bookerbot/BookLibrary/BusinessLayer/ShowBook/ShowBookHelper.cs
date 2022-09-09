using BookLibrary.DataLayer.Repositories.Book;

namespace BookLibrary.BusinessLayer.ShowBook;

public class ShowBookHelper
{
    private Dictionary<Guid, List<ShowBookModel>> _showBooks = new();

    private readonly BookRepository _bookRepository;
    
    public ShowBookHelper(BookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<ShowBookModel?> GetShowBook(Guid userId)
    {
        if (!_showBooks.TryGetValue(userId, out var showBooks))
        {
            IEnumerable<BookView> books = await _bookRepository.GetBooksForExchange(userId);
            showBooks = books.Select(x => new ShowBookModel
            {
                BookId = x.Id,
                Isbn = x.Isbn,
                Title = $"{x.Title}. {x.Authors}. {x.Isbn}",
                PhotoUrl = x.PhotoUrl,
            }).ToList();
        }
        ShowBookModel? showBook = showBooks.FirstOrDefault();

        if (showBook != null)
        {
            var books = showBooks.Where(x => x.BookId != showBook.BookId).ToList();

            _showBooks.Remove(userId);
            
            if (books.Any())
            {
                _showBooks.TryAdd(userId, books.ToList());
            }

            return showBook;
        }
        else
        {
            _showBooks.Remove(userId);
        }

        return null;
    }
}