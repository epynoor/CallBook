using CallBookSystem.Application.Interfaces;
using CallBookSystem.Domain.Entities;
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
    public class SysActivityLogRepository : ISysActivityLogRepository
    {
        private readonly string _connectionString;

        public SysActivityLogRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
        }

        
        public async Task ActivityLogEntry(SysActivityLog log)
        {
            BackgroundJob.Enqueue(() => SaveActivityLog(log));
        }

        public async Task SaveActivityLog(SysActivityLog log)
        {
            try
            {
                var sql = DatabaseProcedure.CallBookProcedure.SP_Hr_Mob_ActivityLog;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@ActivityLog", log.ActivityLog);
                    parameters.Add("@ProcessName",log.ProcessName);
                    parameters.Add("@EmpId", log.EmpId);
                    parameters.Add("@SurveyId", log.SurveyId);
                    var result = await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                    connection.Close();                    
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
