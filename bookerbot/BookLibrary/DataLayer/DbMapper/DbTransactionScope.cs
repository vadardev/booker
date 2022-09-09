using System.Transactions;
using Npgsql;

namespace BookLibrary.DataLayer.DbMapper
{
    public class DbTransactionScope : IDbTransactionScope
    {
        private readonly TransactionScope _scope;

        public DbTransactionScope(TransactionScope scope)
        {
            _scope = scope;
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public void Commit()
        {
            try
            {
                _scope.Complete();
            }
            catch (PostgresException ex) when (ex.SqlState == "40001")
            {
                throw new TransactionSerializationException(ex.Message, ex);
            }
            catch (TransactionAbortedException ex) when (ex.InnerException is PostgresException {SqlState: "40001"})
            {
                throw new TransactionSerializationException(ex.Message, ex);
            }
        }
    }
}