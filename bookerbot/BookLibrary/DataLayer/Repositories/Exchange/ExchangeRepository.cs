using BookLibrary.DataLayer.DbMapper;

namespace BookLibrary.DataLayer.Repositories.Exchange;

public class ExchangeRepository
{
    private readonly IDbMapper _dbMapper;

    public ExchangeRepository(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task<IEnumerable<ExchangeView>> GetExchange(Guid userId)
    {
        return _dbMapper.QueryAsync<ExchangeView>(@"
        select u.userName, lb.title as likeBookTitle, ub.title as userBookTitle
		from	
			(select ub.userid as likeUserId, lb.bookid as likeBookId, ub.bookid as userBookId
			from userbooks ub
				join likebooks lb on lb.userid = ub.userid and islike = true and exists(
				select *
					from userbooks
			where userid = :userId and bookid = lb.bookid
				)
			where ub.bookid in 
			(
				select bookid
				from likebooks lb
			where lb.islike = true and userid = :userId
				)
			order by lb.createdate asc) as tb
		join users u on u.id = tb.likeUserId
		join books lb on lb.id = tb.likeBookId
		join books ub on ub.id = tb.userBooksId
", new
        {
            userId,
        });
    }
}