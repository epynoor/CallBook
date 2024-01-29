using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
using CallBookSystem.Domain.Enums;
using CallBookSystem.Infrastructure.DbHelper;
using Dapper;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Infrastructure.Repositories
{
    public class CallPlanRepository : ICallPlanRepository
    {
        private readonly string _connectionString;
        private readonly string _connectionStringOracle;

        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(CallPlan)}";
        private readonly Func<CacheTech, ICacheService> _cacheService;

        public CallPlanRepository(IConfiguration configuration, Func<CacheTech, ICacheService> cacheService)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
            _connectionStringOracle = configuration.GetConnectionString(DatabaseConnection.FinConnection);
            _cacheService = cacheService;
        }

        public async Task<int> SaveAsync(string status, string userId, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.SP_CallPlan;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                        
                var parameters = new DynamicParameters();                            
                parameters.Add("@UserId", userId);
                parameters.Add("@Status", status);
                parameters.Add("@actionType", actionType);

                result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                 

                connection.Close();
            }
            _cacheService(cacheTech).Remove(cacheKey);
            //BackgroundJob.Enqueue(() => RefreshCache());

            await Task.Delay(25);
            return result;
        }
        public async Task<IReadOnlyList<CallPlan>> GetListAsync(CallPlan callPlan, string actionType, int pagesize = 0, int skip=0 )
        {
            try
            {
                IReadOnlyList<CallPlan> callPlans = new List<CallPlan>();
                var sql = DatabaseProcedure.CallBookProcedure.SP_CallPlan;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", callPlan.Id);
                    parameters.Add("@BatchNo", callPlan.BatchNo);
                    parameters.Add("@CallPlanId", callPlan.CallPlanId);
                    parameters.Add("@EmpPin", callPlan.EmpPin);
                    parameters.Add("@ProcessId", callPlan.ProcessId);
                    parameters.Add("@CategoryId", callPlan.CategoryId);
                    parameters.Add("@CallTypeId", callPlan.CallTypeId);
                    parameters.Add("@Status", callPlan.Status);
                    parameters.Add("@TentativeDate", callPlan.strTentativeDate);
                    parameters.Add("@pagesize", pagesize);
                    parameters.Add("@skip", skip);


                    parameters.Add("@actionType", actionType);
                    callPlans = (await connection.QueryAsync<CallPlan>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                return callPlans;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
      
        public async Task<IReadOnlyList<CallPlan>> GetListByTeamIdAsync(CallPlan callPlan, string teamId, string actionType)
        {
            try
            {
                IReadOnlyList<CallPlan> callPlans = new List<CallPlan>();
                var sql = DatabaseProcedure.CallBookProcedure.SP_CallPlan;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", callPlan.Id);
                    parameters.Add("@BatchNo", callPlan.BatchNo);
                    parameters.Add("@CallPlanId", callPlan.CallPlanId);
                    parameters.Add("@EmpPin", callPlan.EmpPin);
                    parameters.Add("@ProcessId", callPlan.ProcessId);
                    parameters.Add("@CategoryId", callPlan.CategoryId);
                    parameters.Add("@CallTypeId", callPlan.CallTypeId);
                    parameters.Add("@selectedPin", teamId);
                    parameters.Add("@Status", callPlan.Status);
                    parameters.Add("@TentativeDate", callPlan.strTentativeDate);

                    parameters.Add("@actionType", actionType);
                    callPlans = (await connection.QueryAsync<CallPlan>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                return callPlans;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CallPlan> GetByIdAsync(string id)
        {
            CallPlan callPlan = null;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_Process;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@actionType", ActionType.GetById.ToString());
                parameters.Add("@Id", id);
                callPlan = (await connection.QueryAsync<CallPlan>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                connection.Close();
            }
            return callPlan;
        }


        #region Temp Call Plan

        public async Task<int> SaveTempAsync(CallPlan callPlan, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.SP_TempCallPlan;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                
                             
                var parameters = new DynamicParameters();
                parameters.Add("@Id", callPlan.Id);
                parameters.Add("@EmpPin", callPlan.EmpPin??"");
                parameters.Add("@ProcessId", callPlan.ProcessId);
                parameters.Add("@CategoryId", callPlan.CategoryId);
                parameters.Add("@CallTypeId", callPlan.CallTypeId);
                parameters.Add("@Subject", callPlan.Subject);
                parameters.Add("@CIF", callPlan.cif??"");
                parameters.Add("@Name", callPlan.Name??"");
                parameters.Add("@Purpose", callPlan.Purpose??"");
                if (callPlan.TentativeDate > new DateTime(2000, 01, 01))
                {
                    parameters.Add("@TentativeDate", callPlan.TentativeDate);
                }                
                parameters.Add("@actionType", actionType);
                result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                
                connection.Close();
            }            
            return result;
        }

        public async Task<int> UpdateCallPlan(string status,CallPlan callPlan, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.SP_CallPlan;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();


                var parameters = new DynamicParameters();
                parameters.Add("@Id", callPlan.Id);
                parameters.Add("@EmpPin", callPlan.EmpPin ?? "");
                parameters.Add("@ProcessId", callPlan.ProcessId);
                parameters.Add("@CategoryId", callPlan.CategoryId);
                parameters.Add("@CallTypeId", callPlan.CallTypeId);
                parameters.Add("@Subject", callPlan.Subject);
                parameters.Add("@CIF", callPlan.cif ?? "");
                parameters.Add("@Name", callPlan.Name ?? "");
                parameters.Add("@Purpose", callPlan.Purpose ?? "");
                parameters.Add("@Status", status);
                if (callPlan.TentativeDate > new DateTime(2000, 01, 01))
                {
                    parameters.Add("@TentativeDate", callPlan.TentativeDate);
                }
                parameters.Add("@actionType", actionType);
                result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);

                connection.Close();
            }
            return result;
        }

        public async Task<int> ApproveCallPlan(string status,string id, string userId,string isAccepted, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.SP_CallPlan;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@Status", status);
                parameters.Add("@Id", id);
                parameters.Add("@UserId", userId);
                parameters.Add("@IsAccepted", isAccepted);
                parameters.Add("@actionType", actionType);

                result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);


                connection.Close();
            }
            _cacheService(cacheTech).Remove(cacheKey);
            //BackgroundJob.Enqueue(() => RefreshCache());

            await Task.Delay(25);
            return result;
        }


        public async Task<int> CallPlanStatusChange(string status, string id, string userId, string remarks, string actionType)
        {
            int result = 0;
            var sql = DatabaseProcedure.CallBookProcedure.SP_CallPlan;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@Status", status);
                parameters.Add("@id", id);
                parameters.Add("@UserId", userId);
                parameters.Add("@Remarks", remarks);
                parameters.Add("@actionType", actionType);

                result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);


                connection.Close();
            }
            _cacheService(cacheTech).Remove(cacheKey);
            //BackgroundJob.Enqueue(() => RefreshCache());

            await Task.Delay(25);
            return result;
        }

        public async Task<IReadOnlyList<CallPlan>> GetTempListByEmpPinAsync(string empPin, string actionType)
        {
            try
            {
                IReadOnlyList<CallPlan> callPlans = new List<CallPlan>();
                var sql = DatabaseProcedure.CallBookProcedure.SP_TempCallPlan;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();                    
                    parameters.Add("@EmpPin", empPin);
                    parameters.Add("@actionType", actionType);
                    callPlans = (await connection.QueryAsync<CallPlan>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                return callPlans;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<CallPlan>> GetUpdateListByBatchAsync(string batchNo, string actionType)
        
        {
            try
            {
                IReadOnlyList<CallPlan> callPlans = new List<CallPlan>();
                var sql = DatabaseProcedure.CallBookProcedure.SP_CallPlan;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@BatchNo", batchNo);
                    parameters.Add("@actionType", actionType);
                    callPlans = (await connection.QueryAsync<CallPlan>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                return callPlans;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CallType> GetCustomerNameByCifId(string stringCif)
        {
            var sql = DatabaseProcedure.CallBookProcedure.SP_GET_Name_BY_CIF;
            var parameters = new OracleDynamicParameters();
            try
            {
                using (var connection = new OracleConnection(_connectionStringOracle))
                {
                    connection.Open();
                    parameters.Add("CUR_CUSTOMER", dbType:OracleMappingType.RefCursor, direction: ParameterDirection.Output);
                    parameters.Add("P_CustID", stringCif);

                    var result = (await connection.QueryAsync<CallType>(sql, parameters, commandType: CommandType.StoredProcedure)).FirstOrDefault();
                    connection.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                var test = ex.Message;
                throw;
            }

        }

        #endregion

    }
}
