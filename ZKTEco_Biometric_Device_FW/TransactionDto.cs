using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKTEco_Biometric_Device_FW
{
    public class TransactionDto
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? TransactionTime { get; set; }
        public string TransactionDirection { get; set; }
        public string MachinSerialNo { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string DateTimeRecord { get; set; }

    }
}
