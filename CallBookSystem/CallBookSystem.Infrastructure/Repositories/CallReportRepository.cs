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
    public class CallReportRepository : ICallReportRepository
    {
        private readonly string _connectionString;

        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(CallReportActivity)}";
        private readonly Func<CacheTech, ICacheService> _cacheService;

        public CallReportRepository(IConfiguration configuration, Func<CacheTech, ICacheService> cacheService)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
            _cacheService = cacheService;
        }

        public async Task<IReadOnlyList<CallReport>> GetCallReportAsync(string empPin,  string processId, string callId,
            string pageSize, string skip, string actionType)
        {
            IReadOnlyList<CallReport> callReport = new List<CallReport>();

            var sql = DatabaseProcedure.CallBookProcedure.Sp_CallReport;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@EmpPin", empPin);
                parameters.Add("@ProcessId", processId);
                parameters.Add("@CallId", callId);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@skip", skip);
                parameters.Add("@actionType", actionType);
                callReport = (await connection.QueryAsync<CallReport>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();
            }
            return callReport;
        }

        public async Task<IReadOnlyList<CallReport>> GetCallReportAsyncByTeam(string empPin,String teamId, string processId, string callId,
           string pageSize, string skip, string actionType)
        {
            IReadOnlyList<CallReport> callReport = new List<CallReport>();

            var sql = DatabaseProcedure.CallBookProcedure.Sp_CallReport;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@EmpPin", empPin);
                parameters.Add("@ProcessId", processId);
                parameters.Add("@CallId", callId);
                parameters.Add("@pageSize", pageSize);
                parameters.Add("@skip", skip);
                parameters.Add("@selectedPin", teamId);
                parameters.Add("@actionType", actionType);
                callReport = (await connection.QueryAsync<CallReport>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();
            }
            return callReport;
        }

        public async Task<IReadOnlyList<CallReportActivity>> GetCallActivityDetailsAsync(CallReportActivity callActvitydetails, string actionType)
        {
            try
            {
                IReadOnlyList<CallReportActivity> callReportActivity = new List<CallReportActivity>();
                var sql = DatabaseProcedure.CallBookProcedure.Sp_CallReport;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@CallId", callActvitydetails.CallId);
                    //parameters.Add("@BatchNo", callPlan.BatchNo);
                    //parameters.Add("@CallPlanId", callPlan.CallPlanId);
                    //parameters.Add("@EmpPin", callPlan.EmpPin);
                    //parameters.Add("@ProcessId", callPlan.ProcessId);
                    //parameters.Add("@CategoryId", callPlan.CategoryId);
                    //parameters.Add("@CallTypeId", callPlan.CallTypeId);
                    //parameters.Add("@Status", callPlan.Status);
                    //parameters.Add("@TentativeDate", callPlan.strTentativeDate);

                    parameters.Add("@actionType", actionType);
                    callReportActivity = (await connection.QueryAsync<CallReportActivity>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                return callReportActivity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<IList<CallingOfficer>> GetCallingOfficerAsync(string callId)
        {
            var cachedList = new List<CallingOfficer>();


            var sql = DatabaseProcedure.CallBookProcedure.Sp_CallingOfficer;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@CallId", callId);
                parameters.Add("@actionType", ActionType.GetByCallId.ToString());
                cachedList = (await connection.QueryAsync<CallingOfficer>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();
            }
            
            return cachedList;
        }


        public async Task<Tuple<string, string>> SaveCallReportAsync(CallReportActivity activity, IList<CallReportContactPerson> contactPeople, IList<CallingOfficer> callingOfficers, string actionType)
        {
            Tuple<string, string> tuple = null;
            var sql = DatabaseProcedure.CallBookProcedure.Sp_CallReportEntry;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    
                    #region Call Report Entry

                    var parameters = new DynamicParameters();
                    parameters.Add("@CallId", activity.CallId, direction: ParameterDirection.InputOutput, size: 10);
                    parameters.Add("@CallDate", activity.CallDate);
                    parameters.Add("@PlanBatchNo", activity.PlanBatchNo);
                    parameters.Add("@PlanCallId", activity.PlanCallId);
                    parameters.Add("@UnitId", activity.UnitId);
                    parameters.Add("@LocationId", activity.LocationId);
                    parameters.Add("@EmpPin", activity.EmpPin);
                    parameters.Add("@ProcessId", activity.ProcessId);
                    parameters.Add("@CategoryId", activity.CategoryId);
                    parameters.Add("@CallTypeId", activity.CallTypeId);
                    parameters.Add("@Subject", activity.Subject);
                    parameters.Add("@ClientGroup", activity.ClientGroup);
                    parameters.Add("@CIF", activity.CIF);
                    parameters.Add("@Name", activity.Name);
                    parameters.Add("@FirstCallDate", activity.FirstCallDate);
                    parameters.Add("@NextFollowUpDate", activity.NextFollowUpDate);
                    parameters.Add("@FollowUpNote", activity.FollowUpNote);
                    parameters.Add("@Status", activity.Status);
                    parameters.Add("@Summary", activity.Summary);
                    parameters.Add("@ActionPlan", activity.ActionPlan);
                    parameters.Add("@ContactPerson", activity.ContactPerson);
                    parameters.Add("@Address", activity.Address);
                    parameters.Add("@activityId", activity.ActivityId, direction: ParameterDirection.InputOutput, size: 10);
                    parameters.Add("@actionType", actionType);
                    var done = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);

                    string callId = parameters.Get<string>("@CallId");
                    string activityId = parameters.Get<string>("@activityId");

                    

                    #endregion

                    #region Save ContactPerson

                    if (contactPeople != null)
                    {
                        foreach (var cp in contactPeople)
                        {
                            if (string.IsNullOrEmpty(cp.Id))
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("@CallId", callId);
                                parameters.Add("@ActivityId", activityId);
                                parameters.Add("@ContactPerson", cp.ContactPerson);
                                parameters.Add("@ContactNumber", cp.ContactNumber);
                                parameters.Add("@Relation", cp.Relation);
                                parameters.Add("@Address", cp.Address);
                                parameters.Add("@userId", activity.EmpPin);
                                parameters.Add("@actionType", ActionType.Insert.ToString());

                                await connection.ExecuteAsync(DatabaseProcedure.CallBookProcedure.Sp_CallContactPerson, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            }
                        }
                    }

                    #endregion

                    #region Save Calling Officer

                    if(callingOfficers != null)
                    {
                        foreach (var co in callingOfficers)
                        {
                            if (string.IsNullOrEmpty(co.Id))
                            {
                                parameters = new DynamicParameters();
                                parameters.Add("@CallId", callId);
                                parameters.Add("@ActivityId", activityId);
                                parameters.Add("@EmpPin", co.EmpPin);
                                parameters.Add("@userId", activity.EmpPin);
                                parameters.Add("@actionType", ActionType.Insert.ToString());

                                await connection.ExecuteAsync(DatabaseProcedure.CallBookProcedure.Sp_CallingOfficer, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                            }
                        }
                    }
                    

                    #endregion


                    transaction.Commit();

                    tuple= new Tuple<string, string>(callId,activityId);
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    connection.Close();
                }  
            }

            return tuple;
        }


        public async Task<bool> SaveUploadedFile(IList<CallReportFile> callReportFiles, string userName)
        {
            bool done = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {

                    #region Save Uploaded File

                    var parameters = new DynamicParameters();

                    foreach (var file in callReportFiles)
                    {
                        if (string.IsNullOrEmpty(file.Id))
                        {
                            parameters = new DynamicParameters();
                            parameters.Add("@Id", file.Id);
                            parameters.Add("@ProcessId", file.ProcessId);
                            parameters.Add("@CallId", file.CallId);
                            parameters.Add("@ActivityId", file.ActivityId);
                            parameters.Add("@FileName", file.FileName);
                            parameters.Add("@FileLocation", file.FileLocation);
                            parameters.Add("@userId", userName);
                            parameters.Add("@actionType", ActionType.Insert.ToString());

                            await connection.ExecuteAsync(DatabaseProcedure.CallBookProcedure.Sp_CallReportFiles, parameters, commandType: CommandType.StoredProcedure, transaction: transaction);
                        }
                    }

                    #endregion


                    transaction.Commit();
                    done = true;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }

            return done;
        }

        public async Task<IReadOnlyList<CallReport>> GetFollowupCallScheduleAsync(string empPin, string processId)
        {
            IReadOnlyList<CallReport> callReport = new List<CallReport>();

            var sql = DatabaseProcedure.CallBookProcedure.Sp_GetFollowupCallSchedule;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@EmpPin", empPin);
                parameters.Add("@ProcessId", processId);
                callReport = (await connection.QueryAsync<CallReport>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();
            }
            return callReport;
        }

        public async Task<IReadOnlyList<ChartData>> GetChartData(string firstDate, string lastDate, string actionType)
        {
            IReadOnlyList<ChartData> data = new List<ChartData>();

            var sql = DatabaseProcedure.CallBookProcedure.SP_TeamDashboard;
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@FromDate", firstDate);
                parameters.Add("@Todate", lastDate);
                parameters.Add("@actionType", actionType);
                data = (await connection.QueryAsync<ChartData>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                connection.Close();
            }
            return data;
        }

    }
}
