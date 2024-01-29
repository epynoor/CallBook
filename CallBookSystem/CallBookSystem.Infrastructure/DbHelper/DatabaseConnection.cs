using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Infrastructure.DbHelper
{
    public static class DatabaseConnection
    {
        public static readonly string CallBookDbConn = "CallBookDbConn";
        public static readonly string FinConnection = "FinConnection";
    }


    public static class DatabaseProcedure
    {
        internal static class CallBookProcedure
        {
            public static readonly string SP_Hr_Mob_ActivityLog = "SP_Hr_Mob_ActivityLog";
            public static readonly string SP_Hr_Mob_GetUserInfo = "SP_Hr_Mob_GetUserInfo";
            public static readonly string SP_Hr_Mob_GetUserAndPassInfo = "SP_Hr_Mob_GetUserAndPassInfo";
            public static readonly string Sp_Process = "Sp_Process";
            public static readonly string Sp_CallType = "Sp_CallType";
            public static readonly string Sp_Category = "Sp_Category";
            public static readonly string Sp_UserManagement = "Sp_UserManagement";
            public static readonly string SP_CallPlan = "SP_CallPlan";
            public static readonly string SP_TempCallPlan = "SP_TempCallPlan";
            public static readonly string Sp_CallReport = "Sp_CallReport";
            public static readonly string Sp_CallingOfficer = "Sp_CallingOfficer";
            public static readonly string Sp_CallContactPerson = "Sp_CallContactPerson";
            public static readonly string Sp_CallReportFiles = "Sp_CallReportFiles";

            public static readonly string Sp_CallReportEntry = "Sp_CallReportEntry";
            public static readonly string Sp_GetFollowupCallSchedule = "Sp_GetFollowupCallSchedule";
            public static readonly string SP_TeamDashboard = "SP_TeamDashboard";
            public static readonly string SP_GET_Name_BY_CIF = "SP_GET_Name_BY_CIF";
        }
    }
}
