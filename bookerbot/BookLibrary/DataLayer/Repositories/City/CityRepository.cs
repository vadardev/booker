using bookerbot.DataLayer.DbMapper;

namespace bookerbot.DataLayer.Repositories.City;

public class CityRepository
{
    private readonly IDbMapper _dbMapper;

    public CityRepository(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task Add(CityEntity entity)
    {
        return _dbMapper.ExecuteAsync(@"
insert into public.cities(id, name)
values (:Id, :Name)
on conflict(name) do nothing;
", EnumToStringMapper.Map(entity));
    }

    public Task<CityEntity?> GetByName(string name)
    {
        return _dbMapper.QueryFirstOrDefaultAsync<CityEntity?>(@"
select *
from public.cities
where name = :name;
", new
        {
            name,
        });
    }
}