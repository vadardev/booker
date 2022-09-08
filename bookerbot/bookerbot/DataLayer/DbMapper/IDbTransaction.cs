namespace bookerbot.DataLayer.DbMapper
{
    public interface IDbTransactionScope : IDisposable
    {
        void Commit();
    }
}