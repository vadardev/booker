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
on confict(isbn) do nothing;
", new
        {
            entity,
        });
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
}