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
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly string _connectionString;

        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(UserManagement)}";
        private readonly Func<CacheTech, ICacheService> _cacheService;

        public UserManagementRepository(IConfiguration configuration, Func<CacheTech, ICacheService> cacheService)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
            _cacheService = cacheService;
        }

        public async Task<IReadOnlyList<UserManagement>> GetAllUserManagementAsync()
        {
            if (!_cacheService(cacheTech).TryGet(cacheKey, out IReadOnlyList<UserManagement> cachedList))
            {
                var sql = DatabaseProcedure.CallBookProcedure.Sp_UserManagement;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@actionType", ActionType.GetAll.ToString());
                    cachedList = (await connection.QueryAsync<UserManagement>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                _cacheService(cacheTech).Set(cacheKey, cachedList);
            }
            return cachedList;
        }

        public async Task<UserManagement> GetUserManagementAsync(string id, string userId, string processId, string actionType)
        {
            UserManagement category = null;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_UserManagement;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@UserName", userId);
                parameters.Add("@ProcessName", processId);
                parameters.Add("@actionType", actionType);
                category = (await connection.QueryAsync<UserManagement>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                connection.Close();
            }
            return category;
        }
        public async Task<int> SaveUserManagementAsync(UserManagement userManagement, string logName, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_UserManagement;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", userManagement.Id);
                parameters.Add("@ProcessName", userManagement.ProcessId);
                parameters.Add("@UserName", userManagement.UserId);
                parameters.Add("@LogName", logName);
                parameters.Add("@IsAdmin", userManagement.IsAdmin);
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
