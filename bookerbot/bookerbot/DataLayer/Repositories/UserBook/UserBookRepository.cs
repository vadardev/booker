using bookerbot.DataLayer.DbMapper;

namespace telegrambotconsole.DataLayer.Repositories.UserBook;

public class UserBookRepository
{
    private readonly IDbMapper _dbMapper;

    public UserBookRepository(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task Add(UserBookEntity entity)
    {
        return Task.CompletedTask;
    }
}