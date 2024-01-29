using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Domain.Enums
{
    public enum CallReportStatus
    {
        [Description("INITIATED")]
        INITIATED,
        [Description("IN PROGRESS")]
        IN_PROGRESS,
        [Description("COMPLETED")]
        COMPLETED,
        [Description("COMPLETED TO OPPORTUNITY")]
        COMPLETED_TO_OPPORTUNITY

    }
}
