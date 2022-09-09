using BookLibrary.DataLayer.DbMapper;
using telegrambotconsole.DataLayer.Repositories.User;

namespace BookLibrary.DataLayer.Repositories.User;

public class UserRepository
{
    private readonly IDbMapper _dbMapper;

    public UserRepository(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task TryAdd(UserEntity entity)
    {
        return _dbMapper.ExecuteAsync(@"
insert into public.users(id, telegramId, userName)
values(:Id, :TelegramId, :UserName)
on conflict(telegramId) do nothing;", EnumToStringMapper.Map(entity));
    }

    public Task SetCityId(Guid userId, Guid cityId)
    {
        return _dbMapper.ExecuteAsync(@"
update public.users set cityId = :cityId
where id = :userId;
", new
        {
            userId,
            cityId,
        });
    }

    public Task<UserEntity?> GetByTelegramId(long telegramId)
    {
        return _dbMapper.QueryFirstOrDefaultAsync<UserEntity?>(@"
select *
from public.users
where telegramId = :telegramId;
", new
        {
            telegramId,
        });
    }
    
    public Task<UserEntity?> Get(Guid id)
    {
        return _dbMapper.QueryFirstOrDefaultAsync<UserEntity?>(@"
select *
from public.users
where id = :id;
", new
        {
            id,
        });
    }
}