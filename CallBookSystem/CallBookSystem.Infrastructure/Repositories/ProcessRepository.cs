using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
using CallBookSystem.Domain.Enums;
using CallBookSystem.Infrastructure.DbHelper;
using Dapper;
using Hangfire;
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
    public class ProcessRepository : IProcessRepository
    {
        private readonly string _connectionString;

        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(Process)}";
        private readonly Func<CacheTech, ICacheService> _cacheService;

        public ProcessRepository(IConfiguration configuration, Func<CacheTech, ICacheService> cacheService)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
            _cacheService = cacheService;
        }

        public async Task<IReadOnlyList<Process>> GetAllProcessesAsync()
        {
            if (!_cacheService(cacheTech).TryGet(cacheKey, out IReadOnlyList<Process> cachedList))
            {
                var sql = DatabaseProcedure.CallBookProcedure.Sp_Process;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@actionType", ActionType.GetAll.ToString());
                    cachedList = (await connection.QueryAsync<Process>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();                    
                }
                _cacheService(cacheTech).Set(cacheKey, cachedList);
            }
            return cachedList;
        }

        public async Task<Process> GetProcessAsync(string id, string processName, string actionType)
        {
            Process process = null;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_Process;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@ProcessName", processName);
                parameters.Add("@actionType", actionType);
                process = (await connection.QueryAsync<Process>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                connection.Close();
            }
            return process;            
        }

        public async Task<int> SaveProcessAsync(Process process, string userId, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_Process;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", process.Id);
                parameters.Add("@ProcessName", process.ProcessName);
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

        public async Task RefreshCache()
        {
            

            IReadOnlyList<Process> cachedList = null;

            var sql = DatabaseProcedure.CallBookProcedure.Sp_Process;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@actionType", ActionType.GetAll.ToString());
                cachedList = (await connection.QueryAsync<Process>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();
            }
            _cacheService(cacheTech).Set(cacheKey, cachedList);

            
        }


    }
}
