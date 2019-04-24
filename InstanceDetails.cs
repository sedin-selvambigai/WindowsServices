using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServicewithTimer
{
    public class InstanceDetails
    {
        public string HCM_id { get; set; }
        public string Customer { get; set; }
        public string VendorID { get; set; }

        public string SQLInstance { get; set; }

        public string Database { get; set; }

        public bool WindowsAuthentication { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }

        public string RedirectURL { get; set; }

        public string CertificatePath { get; set; }

        public string SecretCode { get; set; }

        public string TimeDelay { get; set; }

        public bool Status { get; set; }

        public string Employee_UserName { get; set; }

        public string Employee_Pwd { get; set; }
        public string Payroll_UserName { get; set; }

        public string Payroll_Password { get; set; }
    }
}
