
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServicewithTimer
{
    public class EMP_RETRIEVE
    {
        public string IRD_NO { get; set; }
        public string LEGALNAME { get; set; }
        public DateTime DATE_OF_BIRTH { get; set; }
        public string REQ_ID { get; set; }
        public string REQ_STATUS { get; set; }
        public string REQ_DATE { get; set; }
        public string REQ_USER { get; set; }
        public string REQ_USER_ROLE { get; set; }
        public string TITLE_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MIDDLE_NAME { get; set; }
        public string TAX_CODE { get; set; }
        public DateTime EMPLOYMENT_START_DATE { get; set; }
        public DateTime LAST_AVAILABLE_DATE { get; set; }
        public DateTime RETRIEVE_DATE { get; set; }
        public string REQ_MASTER_OU { get; set; }
        public string XmlFileResponse { get; set; }
        public string Req_Emp_GUID { get; set; }
    }
}
