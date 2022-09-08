using bookerbot.DataLayer.DbMapper;

namespace telegrambotconsole.DataLayer.Repositories.LikeBook;

public class LikeBookRepository
{
    private readonly IDbMapper _dbMapper;

    public LikeBookRepository(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task Add(LikeBookEntity entity)
    {
        return _dbMapper.ExecuteAsync(@"
insert into public.likebooks (bookid, userid, islike, createdate)
values (:BookId, :UserId, :IsLike, :CreateDate)
on conflict(bookId, userId) do update
set isLike = EXCLUDED.isLike,
    createdate = EXCLUDED.createDate;
", entity);
    }
}