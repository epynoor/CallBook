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
    public class HrUserRepository : IHrUserRepository
    {
        private readonly string _connectionString;

        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(HrUser)}";
        private readonly Func<CacheTech, ICacheService> _cacheService;
        public HrUserRepository(IConfiguration configuration, Func<CacheTech, ICacheService> cacheService)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
            _cacheService = cacheService;
        }

        public async Task<HrUser> GetUserInfo(string userId)
        {
            try
            {
                var sql = DatabaseProcedure.CallBookProcedure.SP_Hr_Mob_GetUserInfo;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", userId);
                    parameters.Add("@actionType", ActionType.GetById.ToString());
                    var result = await connection.QueryAsync<HrUser>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<HrUser>> GetAllUser(string supervisorId)
        {
            if (!_cacheService(cacheTech).TryGet(cacheKey, out IReadOnlyList<HrUser> cachedList))
            {
                var sql = DatabaseProcedure.CallBookProcedure.SP_Hr_Mob_GetUserInfo;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@actionType", ActionType.GetAll.ToString());
                    cachedList = (await connection.QueryAsync<HrUser>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                _cacheService(cacheTech).Set(cacheKey, cachedList);
            }

            if (!string.IsNullOrEmpty(supervisorId))
            {
                cachedList = cachedList.Where(p => p.SuperVisorId == supervisorId).ToList();
            }

            return cachedList;
        }

        public async Task<IReadOnlyList<HrUser>> GetAllUserBySupervisorId(string supervisorId)
        {

            //if (!_cacheService(cacheTech).TryGet(cacheKey, out IReadOnlyList<HrUser> cachedList))
            //{
            var sql = DatabaseProcedure.CallBookProcedure.SP_Hr_Mob_GetUserInfo;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", supervisorId);
                parameters.Add("@actionType", ActionType.GetAllBySupervisorId.ToString());
                var cachedList = (await connection.QueryAsync<HrUser>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();
                return cachedList;
            }
            //    _cacheService(cacheTech).Set(cacheKey, cachedList);
            //}

            //if (!string.IsNullOrEmpty(supervisorId))
            //{
            //    cachedList = cachedList.Where(p => p.SuperVisorId == supervisorId).ToList();
            //}


        }

        public async Task<HrUser> GetUserInfoByUserIdAndPassword(string userId, string password)
        {
            try
            {
                var sql = DatabaseProcedure.CallBookProcedure.SP_Hr_Mob_GetUserAndPassInfo;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserId", userId);
                    parameters.Add("@Password", password);
                    var result = await connection.QueryAsync<HrUser>(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task RefreshCache()
        {


            IReadOnlyList<HrUser> cachedList = null;

            var sql = DatabaseProcedure.CallBookProcedure.SP_Hr_Mob_GetUserInfo;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@actionType", ActionType.GetAll.ToString());
                cachedList = (await connection.QueryAsync<HrUser>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();
            }
            _cacheService(cacheTech).Set(cacheKey, cachedList);


        }

    }
}
