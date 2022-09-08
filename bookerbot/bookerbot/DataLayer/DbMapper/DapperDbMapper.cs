using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using bookerbot.DataLayer.DbMapper;
using Dapper;
using Npgsql;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Vesta.DbMapper
{
    public abstract class DapperDbMapper : IDbMapper
    {
        private readonly Func<string> _connectionStringGetter;

        protected DapperDbMapper(Func<string> connectionStringGetter)
        {
            _connectionStringGetter = connectionStringGetter;
        }

        protected abstract IDbConnection GetConnection(string connectionString);

        private void Execute(Action<IDbConnection> action)
        {
            using var connection = GetConnection(_connectionStringGetter());
            try
            {
                action(connection);
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

        private T Execute<T>(Func<IDbConnection, T> action)
        {
            using var connection = GetConnection(_connectionStringGetter());
            try
            {
                return action(connection);
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

        private async Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> action)
        {
            using var connection = GetConnection(_connectionStringGetter());
            try
            {
                return await action(connection);
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

        public int Execute(string query, object? param = null, int? timeout = null)
        {
            return Execute(connection => connection.Execute(query, param, commandTimeout: timeout));
        }

        public Task<int> ExecuteAsync(string query, object? param = null, int? timeout = null)
        {
            return ExecuteAsync(connection => connection.ExecuteAsync(query, param, commandTimeout: timeout));
        }

        public IEnumerable<T> Query<T>(string query, object? param = null, bool buffered = true, int? timeout = null)
        {
            return Execute(connection =>
                connection.Query<T>(query, param, buffered: buffered, commandTimeout: timeout));
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string query, object? param = null, bool buffered = true,
            int? timeout = null)
        {
            return ExecuteAsync(connection => connection.QueryAsync<T>(new CommandDefinition(query, param,
                commandTimeout: timeout,
                flags: buffered ? CommandFlags.Buffered : CommandFlags.None)));
        }

        public T QueryFirst<T>(string query, object? param = null, int? timeout = null)
        {
            return Execute(connection => connection.QueryFirst<T>(query, param, commandTimeout: timeout));
        }

        public T QueryFirstOrDefault<T>(string query, object? param = null, int? timeout = null)
        {
            return Execute(connection => connection.QueryFirstOrDefault<T>(query, param, commandTimeout: timeout));
        }

        public Task<T> QueryFirstAsync<T>(string query, object? param = null, int? timeout = null)
        {
            return ExecuteAsync(connection => connection.QueryFirstAsync<T>(query, param, commandTimeout: timeout));
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string query, object? param = null, int? timeout = null)
        {
            return ExecuteAsync(connection =>
                connection.QueryFirstOrDefaultAsync<T>(query, param, commandTimeout: timeout));
        }

        public IDbTransactionScope BeginTransaction(EIsolationLevel level = EIsolationLevel.ReadCommited)
        {
            var scope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions {IsolationLevel = ConvertTransactionLevel(level)},
                TransactionScopeAsyncFlowOption.Enabled);

            return new DbTransactionScope(scope);
        }

        public async Task<TResult> Aggregate<TValue, TResult>(string query, Func<TResult, TValue, TResult> func,
            TResult initialValue = default, object? param = null)
        {
            using var connection = GetConnection(_connectionStringGetter());
            IEnumerable<TValue> result;
            try
            {
                result = await connection.QueryAsync<TValue>(new CommandDefinition(query, param, flags: CommandFlags.None));
            }
            catch (PostgresException ex) when (ex.SqlState == "40001")
            {
                throw new TransactionSerializationException(ex.Message, ex);
            }
            catch (TransactionAbortedException ex) when (ex.InnerException is PostgresException {SqlState: "40001"})
            {
                throw new TransactionSerializationException(ex.Message, ex);
            }

            return result == null ? initialValue : result.Aggregate(initialValue, func);
        }

        private IsolationLevel ConvertTransactionLevel(EIsolationLevel level)
        {
            return level switch
            {
                EIsolationLevel.ReadUncommited => IsolationLevel.ReadUncommitted,
                EIsolationLevel.ReadCommited => IsolationLevel.ReadCommitted,
                EIsolationLevel.RepeatableRead => IsolationLevel.RepeatableRead,
                EIsolationLevel.Serializable => IsolationLevel.Serializable,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        }
    }
}