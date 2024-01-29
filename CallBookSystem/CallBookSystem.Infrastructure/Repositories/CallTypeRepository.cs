using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
using CallBookSystem.Domain.Enums;
using CallBookSystem.Infrastructure.DbHelper;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Infrastructure.Repositories
{
    public class CallTypeRepository : ICallTypeRepository
    {
        private readonly string _connectionString;

        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(CallType)}";
        private readonly Func<CacheTech, ICacheService> _cacheService;

        public CallTypeRepository(IConfiguration configuration, Func<CacheTech, ICacheService> cacheService)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
            _cacheService = cacheService;
        }
        public async Task<IReadOnlyList<CallType>> GetAllTypeAsync()
        {
            if (!_cacheService(cacheTech).TryGet(cacheKey, out IReadOnlyList<CallType> cachedList))
            {
                var sql = DatabaseProcedure.CallBookProcedure.Sp_CallType;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@actionType", ActionType.GetAll.ToString());
                    cachedList = (await connection.QueryAsync<CallType>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                _cacheService(cacheTech).Set(cacheKey, cachedList);
            }
            return cachedList;
        }

        public async Task<CallType> GetTypeAsync(string id, string processId, string typeName, string actionType)
        {
            CallType process = null;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_CallType;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@Name", typeName);
                parameters.Add("@ProcessId", processId);
                parameters.Add("@actionType", actionType);
                process = (await connection.QueryAsync<CallType>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                connection.Close();
            }
            return process;
        }

        public async Task<int> SaveType(CallType type, string userId, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_CallType;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", type.Id);
                parameters.Add("@Name", type.Name);
                parameters.Add("@ProcessId", type.ProcessId);
                parameters.Add("@userId", userId);
                parameters.Add("@actionType", actionType);
                result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                connection.Close();
            }
            _cacheService(cacheTech).Remove(cacheKey);
            //BackgroundJob.Enqueue(() => RefreshCache());

            await Task.Delay(25);
            return result;
        }
    }
}
