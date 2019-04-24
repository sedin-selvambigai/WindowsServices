using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServicewithTimer
{
    public class Employee
    {
        public int MASTER_OU { get; set; }
        public DateTime RUN_DATE { get; set; }
        public string GUID { get; set; }
        public string XML_FILE { get; set; }
        public string EMPLOYEE_STATUS { get; set; }
        public DateTime RESPONSE_DATE { get; set; }
        public string XML_FILE_RESPONSE { get; set; }
        public string ERROR_DESCRIPTION { get; set; }
        public Int32 ERROR_CODE { get; set; }
        public string xml_formation { get; set; }
        public string EMPLOYEE_TYPE { get; set; }
        public string Gatewayid { get; set; }
        public string SubmissionkeyId { get; set; }
    }
}
