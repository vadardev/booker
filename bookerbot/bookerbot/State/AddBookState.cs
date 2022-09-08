using System.Net.Http.Headers;
using bookerbot.Context;
using bookerbot.DataLayer.Repositories.Book;
using bookerbot.DataLayer.Repositories.UserBook;
using Newtonsoft.Json;

namespace bookerbot.State;

public class AddBookState : IUserState
{
    public static string Back = "◀️ Назад";
    
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly BookRepository _bookRepository;
    private readonly UserBookRepository _userBookRepository;

    public AddBookState(IHttpClientFactory httpClientFactory,
        BookRepository bookRepository,
        UserBookRepository userBookRepository)
    {
        _httpClientFactory = httpClientFactory;
        _bookRepository = bookRepository;
        _userBookRepository = userBookRepository;
    }

    public async Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        if (message != Back)
        {
            string isbn = message.Trim();

            BookEntity? book = await _bookRepository.GetByIsbn(isbn);

            if (book == null)
            {
                NewBook? newBook = await GetBook(isbn);

                if (newBook == null)
                {
                    return await ProfileState.GetResponseMessage(userContext, "Не удалось найти книгу");
                }

                book = new BookEntity
                {
                    Id = Guid.NewGuid(),
                    Isbn = isbn,
                    Title = newBook.Title,
                    Authors = newBook.Meta.Author.FirstOrDefault()?.Name ?? "",
                    PhotoUrl = newBook.Meta.Image,
                    SiteUrl = newBook.Link,
                    Price = newBook.Meta.Price,
                    CreateDate = DateTime.UtcNow,
                };
                await _bookRepository.Add(book);
            }

            await _userBookRepository.Add(new UserBookEntity
            {
                UserId = userContext.UserId,
                BookId = book.Id,
                CreateDate = DateTime.UtcNow,
            });

            return await ProfileState.GetResponseMessage(userContext, "Книга успешно добавлена!");
        }

        return await ProfileState.GetResponseMessage(userContext);
    }

    public static async Task<ResponseMessage> GetResponseMessage(UserContext context)
    {
        context.State = EContextState.AddBook;

        return new ResponseMessage
        {
            Text = "Укажите ISBN книги:",
            UpButtons = new List<string> { AddBookState.Back },
            ResponseMessageType = EResponseMessageType.Text
        };
    }


    private async Task<NewBook?> GetBook(string isbn)
    {
        var httpClient = _httpClientFactory.CreateClient("books");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        string uri = $"api/v1/catalog/search/suggests/?q={isbn}&limit_suggest=1&limit_product=1&limit_section=3&limit_podborki=0&SESSID=92633865-91fe-436b-ba34-9e7bb0bab239";
        NewBookResponse? response = JsonConvert.DeserializeObject<NewBookResponse>(await httpClient.GetStringAsync(uri));

        if (response?.Books != null && response.Books.Any())
        {
            NewBook book = response.Books.First();

            var image = await httpClient.GetByteArrayAsync(book.Meta.Image);

            await using (FileStream fs = new FileStream($"bookerbot/Images/{isbn}.jpg", FileMode.Create))
            {
               await fs.WriteAsync(image, 0, image.Length);
            }
         
        }

        return null;
    }
}

public class NewBook
{
    public string Title { get; set; }
    public string Link { get; set; }
    public Meta Meta { get; set; }
}
    
public class Meta
{
    public string Image { get; set; }
    public decimal Price { get; set; }
    public List<Author> Author { get; set; }
}

public class Author
{
    public string Name { get; set; }
}
    
public class NewBookResponse
{
    [JsonProperty("Data")] public List<NewBook>? Books { get; set; }
}