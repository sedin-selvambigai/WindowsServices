using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServicewithTimer
{
    public class Payroll
    {
        public string MASTEROU { get; set; }
        public string GUID { get; set; }
        public string REQUEST_ID { get; set; }
        public string REQ_STATUS { get; set; }
        public DateTime REQ_DATE { get; set; }
        public string SUBMISSION_KEY { get; set; }
        public string LINE_NUMBER { get; set; }
        public string EMPLOYEE_IRD_NUMBER { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string TAX_CODE { get; set; }
        public DateTime PAY_PERIOD_START_DATE { get; set; }
        public DateTime PAY_PERIOD_END_DATE { get; set; }
        public DateTime EMPLOYMENT_START_DAT { get; set; }
        public string EMPLOYEE_PAY_FREQUENCY { get; set; }
        public decimal GROSS_EARNINGS { get; set; }
        public decimal EARNINGS_NOT_LIABLE_ACC { get; set; }
        public string LUMPSUM_INDICATOR { get; set; }
        public decimal PAYE_SCHEDULAR_TAX_DEDUCTIONS { get; set; }
        public string CHILD_SUPPORT_CODE { get; set; }
        public decimal CHILD_SUPPORT_DEDUCTIONS { get; set; }
        public decimal STUDENT_LOAN_DEDUCTIONS { get; set; }
        public decimal KIWI_SAVER_EMPLOYER_CONTRIBUTIONS { get; set; }
        public decimal KIWI_SAVER_DEDUCTIONS { get; set; }
        public decimal TAX_CREDIT_PAYROLL_DONATIONS { get; set; }
        public decimal ESCT_DEDUCTED { get; set; }
        public decimal Family_Tax_Credits { get; set; }
        public string created_by { get; set; }
        public string modified_by { get; set; }
    }
}
