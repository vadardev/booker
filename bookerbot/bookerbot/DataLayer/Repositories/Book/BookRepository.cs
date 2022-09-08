using bookerbot.DataLayer.DbMapper;

namespace bookerbot.DataLayer.Repositories.Book;

public class BookRepository
{
    private readonly IDbMapper _dbMapper;

    public BookRepository(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task Add(BookEntity entity)
    {
        return _dbMapper.ExecuteAsync(@"
insert into public.books(id, isbn, title, authors, photourl, siteurl, price, createdate)
values(:Id, :Isbn, :Title, :Authors, :PhotoUrl, :SiteUrl, :Price, :CreateDate)
on conflict(isbn) do nothing;
", EnumToStringMapper.Map(entity));
    }

    public Task<BookEntity?> Get(Guid id)
    {
        return _dbMapper.QueryFirstOrDefaultAsync<BookEntity?>(@"
select *
from public.books
where id = :id;
", new
        {
            id,
        });
    }

    public Task<BookEntity?> GetByIsbn(string isbn)
    {
        return _dbMapper.QueryFirstOrDefaultAsync<BookEntity?>(@"
select *
from public.books
where isbn = :isbn;
", new
        {
            isbn,
        });
    }

    public Task<IEnumerable<BookView>> GetBooksForExchange(Guid userId)
    {
        return _dbMapper.QueryAsync<BookView>(@"
select *
from books
where id in (
				select distinct bookId
				from userBooks
				where userid in (
						select id
						from users 
						where cityId = (
						select cityId
						from users 
						where id = :userId
						) and id <> :userId
					)
				)
and id not in (
				select bookid 
				from likeBooks
				where userid = :userId and islike = true
			  )
", new
        {
            userId,
        });
    }
}