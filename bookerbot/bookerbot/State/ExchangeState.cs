using bookerbot.Context;
using telegrambotconsole.DataLayer.Repositories.LikeBook;
using telegrambotconsole.DataLayer.Repositories.User;

namespace bookerbot.State;

public class ExchangeState : IUserState
{
    public static string Like = "❤️ Нравится";
    public static string Dislike = "🚫 Не очень";
    public static string AddBook = "📕 Добавить книгу";
    public static string Profile = "💼 Профиль";
    
    private readonly LikeBookRepository _likeBookRepository;
    private readonly UserRepository _userRepository;

    public ExchangeState(LikeBookRepository likeBookRepository,
        UserRepository userRepository)
    {
        _likeBookRepository = likeBookRepository;
        _userRepository = userRepository;
    }

    public async Task<ResponseMessage> Handle(UserContext userContext, string message)
    {
        if (message == Like)
        {
            userContext.State = EContextState.Exchange;

            if (userContext.ShowBookId.HasValue)
            {
                await _likeBookRepository.Add(new LikeBookEntity
                {
                    BookId = userContext.ShowBookId.Value,
                    UserId = userContext.UserId,
                    IsLike = true,
                    CreateDate = DateTime.UtcNow,
                });
            }

            userContext.ShowBookId = Guid.NewGuid();

            return new ResponseMessage
            {
                Text = "Название книги 1",
                UpButtons = new List<string> { Like, Dislike },
                DownButtons = new List<string> {AddBook, Profile },
                PhotoUrl = "https://cdn.book24.ru/v2/ASE000000000862783/COVER/cover3d1__w220.jpg",
                ResponseMessageType = EResponseMessageType.Photo
            };
        }
        else if (message == Dislike)
        {
            userContext.State = EContextState.Exchange;
            
            if (userContext.ShowBookId.HasValue)
            {
                await _likeBookRepository.Add(new LikeBookEntity
                {
                    BookId = userContext.ShowBookId.Value,
                    UserId = userContext.UserId,
                    IsLike = false,
                    CreateDate = DateTime.UtcNow,
                });
            }

            userContext.ShowBookId = Guid.NewGuid();

            return new ResponseMessage
            {
                Text = "Название книги 2",
                UpButtons = new List<string> { Like, Dislike },
                DownButtons = new List<string> {AddBook, Profile },
                PhotoUrl = "https://ndc.book24.ru/resize/220x270/iblock/56e/56ecd4daffa4473798b46746eb77231d/e32f9baf94eac3e8377142fd6e83469f.jpg",
                ResponseMessageType = EResponseMessageType.Photo
            };
        }
        else if (message == Profile)
        {
            userContext.State = EContextState.Profile;

            return new ResponseMessage
            {
                Text = "Профиль",
                UpButtons = new List<string> { ProfileState.MyBooks, ProfileState.City, ProfileState.Back },
                ResponseMessageType = EResponseMessageType.Text
            };
        }
        else if (message == AddBook)
        {
            UserEntity? user = await _userRepository.Get(userContext.UserId);

            if (user?.CityId == null)
            {
                userContext.State = EContextState.AddCity;

                return new ResponseMessage
                {
                    Text = "Напишите ваш город:",
                    UpButtons = new List<string> { AddCityState.Back },
                    ResponseMessageType = EResponseMessageType.Text
                };
            }

            userContext.State = EContextState.AddBook;

            return new ResponseMessage
            {
                Text = "Профиль",
                UpButtons = new List<string> { ProfileState.MyBooks, ProfileState.City, ProfileState.Back },
                ResponseMessageType = EResponseMessageType.Text
            };
        }

        userContext.State = EContextState.Exchange;
        userContext.ShowBookId = Guid.NewGuid();
        
        return new ResponseMessage
        {
            Text = "Название книги",
            UpButtons = new List<string> { Like, Dislike },
            DownButtons = new List<string> {AddBook, Profile },
            PhotoUrl = "https://ndc.book24.ru/resize/220x270/iblock/56e/56ecd4daffa4473798b46746eb77231d/e32f9baf94eac3e8377142fd6e83469f.jpg",
            ResponseMessageType = EResponseMessageType.Photo
        };
    }
}