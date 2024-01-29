using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Enums
{
    public enum ActionType
    {
        GetAll,
        GetAllBySupervisorId,
        GetById,
        GetByName,
        GetListByProcessId,
        Insert,
        Update,
        Approve,
        Delete,
        Dismiss,
        DeleteUpdateBatch,

        GetAllBatchByEmpPin,
        GetByEmpPin,
        GetByBatchNo,

        GetByTentativeDate,

        GetCallReportByPaging,
        GetCallReportBySubordinate,
        GetByStatus,
        GetByStatusAndSupervisorId,
        GetByStatusAndSupervisorIdByPage,
        GetBySubordinateId,
        PlanNotExcepted,
        GetByActivityCallId,
        GetChartDasboard,

        GetByCallId,
        NewCallEntry
    }
}
