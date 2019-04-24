using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServicewithTimer
{
    public class PAYROLL_PREPOP
    {
        public string MASTEROU { get; set; }
        public DateTime PAY_DAY_DATE { get; set; }
        public string REQUEST_ID { get; set; }
        public string REQ_STATUS { get; set; }
        public DateTime REQ_DATE { get; set; }
        public string REQUESTED_USER { get; set; }
        public string REQUESTED_USER_ROLE { get; set; }
        public string REQUESTED_USER_OU { get; set; }
        public string EMPLOYEE_IRD_NUMBER { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string TAX_CODE { get; set; }
        public DateTime EMPLOYMENT_START_DATE { get; set; }
        public DateTime EMPLOYMENT_END_DATE { get; set; }
        public DateTime PAY_PERIOD_END_DATE { get; set; }
        public string FORM_TYPE { get; set; }
        public string SUBMISSION_KEY { get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }
        public string GUID { get; set; }
        public string RETRIEVE_DATE { get; set; }
        public string MOD_FLAG { get; set; }
    }
}
