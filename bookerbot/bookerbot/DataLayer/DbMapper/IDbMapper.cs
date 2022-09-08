namespace bookerbot.DataLayer.DbMapper
{
    public interface IDbMapper
    {
        int Execute(string query, object? param = null, int? timeout = null);
        Task<int> ExecuteAsync(string query, object? param = null, int? timeout = null);
        IEnumerable<T> Query<T>(string query, object? param = null, bool buffered = true, int? timeout = null);
        Task<IEnumerable<T>> QueryAsync<T>(string query, object? param = null, bool buffered = true, int? timeout = null);
        T QueryFirst<T>(string query, object? param = null, int? timeout = null);
        T QueryFirstOrDefault<T>(string query, object? param = null, int? timeout = null);
        Task<T> QueryFirstAsync<T>(string query, object? param = null, int? timeout = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string query, object? param = null, int? timeout = null);
        IDbTransactionScope BeginTransaction(EIsolationLevel level = EIsolationLevel.ReadCommited);

        Task<TResult> Aggregate<TValue, TResult>(string query, Func<TResult, TValue, TResult> func, TResult initialValue = default,
            object? param = null);
    }
}