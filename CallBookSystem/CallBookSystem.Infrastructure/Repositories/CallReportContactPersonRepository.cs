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
    public class CallReportContactPersonRepository : ICallReportContactPersonRepository
    {
        private readonly string _connectionString;

        private readonly static CacheTech cacheTech = CacheTech.Memory;
        private readonly string cacheKey = $"{typeof(CallReportContactPerson)}";
        private readonly Func<CacheTech, ICacheService> _cacheService;

        public CallReportContactPersonRepository(IConfiguration configuration, Func<CacheTech, ICacheService> cacheService)
        {
            _connectionString = configuration.GetConnectionString(DatabaseConnection.CallBookDbConn);
            _cacheService = cacheService;
        }

        public async Task<IList<CallReportContactPerson>> GetCallReportContactPeopleAsync( string callId, string actionType)
        {
            try
            {
                IReadOnlyList<CallReportContactPerson> contactPeople = new List<CallReportContactPerson>();
                var sql = DatabaseProcedure.CallBookProcedure.Sp_CallContactPerson;
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("@CallId", callId);                  

                    parameters.Add("@actionType", actionType);
                    contactPeople = (await connection.QueryAsync<CallReportContactPerson>(sql, parameters, commandType: CommandType.StoredProcedure)).ToList();
                    connection.Close();
                }
                return contactPeople.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
