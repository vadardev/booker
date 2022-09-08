using bookerbot.DataLayer.DbMapper;

namespace bookerbot.DataLayer.Repositories.UserBook;

public class UserBookRepository
{
    private readonly IDbMapper _dbMapper;

    public UserBookRepository(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task Add(UserBookEntity entity)
    {
        return _dbMapper.ExecuteAsync(@"
insert into public.userbooks(userId, bookId, createDate)
values (:UserId, :BookId, :CreateDate)
on conflict(userId, bookId) do nothing;
", EnumToStringMapper.Map(entity));
    }

    public Task<IEnumerable<UserBookView>> GetByUserId(Guid userId)
    {
        return _dbMapper.QueryAsync<UserBookView>(@"
select b.*
from public.userBooks u
join public.books b on b.id = u.bookId
where u.userid = :userId;
", new
        {
            userId,
        });
    }

    public Task Delete(Guid userId, Guid bookId)
    {
        return _dbMapper.ExecuteAsync(@"
delete from public.userbooks
where userId = :userId and bookId = :bookId;
", new
        {
            userId,
            bookId,
        });
    }

    public Task DeleteByBookName(Guid userId, string bookName)
    {
        return _dbMapper.ExecuteAsync(@"
delete from public.userbooks
where userId = :userId and bookId in (
  select id
  from public.books
  where title = :bookName
);
", new
        {
            userId,
            bookName,
        });
    }
}