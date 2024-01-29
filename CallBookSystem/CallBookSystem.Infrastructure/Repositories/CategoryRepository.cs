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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(Category)}";
        private readonly Func<CacheTech, ICacheService> _cacheService;

        public CategoryRepository(IConfiguration configuration, Func<CacheTech, ICacheService> cacheService)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
            _cacheService = cacheService;
        }

        public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
        {
            if (!_cacheService(cacheTech).TryGet(cacheKey, out IReadOnlyList<Category> cachedList))
            {
                var sql = DatabaseProcedure.CallBookProcedure.Sp_Category;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@actionType", ActionType.GetAll.ToString());
                    cachedList = (await connection.QueryAsync<Category>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                _cacheService(cacheTech).Set(cacheKey, cachedList);
            }
            return cachedList;
        }

        public async Task<IReadOnlyList<Category>> GetAllCategoriesByProcessIdAsync(string processId)
        {
            if (!_cacheService(cacheTech).TryGet(cacheKey, out IReadOnlyList<Category> cachedList))
            {
                var sql = DatabaseProcedure.CallBookProcedure.Sp_Category;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@actionType", ActionType.GetAll.ToString());
                    cachedList = (await connection.QueryAsync<Category>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                _cacheService(cacheTech).Set(cacheKey, cachedList);
            }
            if (processId != "-1")
            {
                cachedList = cachedList.Where(c => c.ProcessId == processId).ToList();
            }
            return cachedList;
        }

        public async Task<Category> GetCategoryAsync(string id, string categoryName, string processId, string actionType)
        {
            Category category = null;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_Category;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@CategoryName", categoryName);
                parameters.Add("@ProcessId", processId);
                parameters.Add("@actionType", actionType);
                category = (await connection.QueryAsync<Category>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                connection.Close();
            }
            return category;
        }

        public async Task<int> SaveCategoryAsync(Category category, string userId, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_Category;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@Id", category.Id);
                parameters.Add("@CategoryName", category.CategoryName);
                parameters.Add("@ProcessId", category.ProcessId);
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
