using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace WindowsServicewithTimer
{
    public class callAPI
    {
        //public log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string IRUsername = "";
        public string IRPassword = "";

        public string PayUsername = "";
        public string PayPassword = "";

        public string IRURL = "";
        public string ClientID = "";

        public string CertificatePath = "";
        public string SecretCode = "";
        public string Get()
        {
            try
            {
                //log.Info("starts running");
                var jsonData = System.IO.File.ReadAllText(ConfigurationManager.AppSettings["ClientJsonChoosed"]);
                List<InstanceDetails> _InstanceDetailsList = JsonConvert.DeserializeObject<List<InstanceDetails>>(jsonData)
                                      ?? new List<InstanceDetails>();

                string response = HCMProcess(_InstanceDetailsList);

                //log.Info("HCMProcess Completed");
                return response;
            }
            catch (Exception e)
            {
                //log.Info("API Get Method" + e.InnerException);
                return "Exception";
            }
        }

        public string HCMProcess(List<InstanceDetails> _InstanceDetailsList)
        {
            try
            {
                string response = "";
                foreach (var item in _InstanceDetailsList)
                {
                    string _dataContext = "";
                    if (item.WindowsAuthentication)
                    {
                        _dataContext = "Data Source=" + item.SQLInstance + ";Initial Catalog=" + item.Database + ";Integrated Security=true;";

                    }
                    else
                    {
                        _dataContext = "Data Source=" + item.SQLInstance + ";Initial Catalog=" + item.Database + ";User Id ="
                            + EncryptLogic.Decrypt(item.Username) + ";Password = " + EncryptLogic.Decrypt(item.Password) + ";";
                    }
                    ClientID = item.VendorID;

                    IRUsername = EncryptLogic.Decrypt(item.Employee_UserName);
                    IRPassword = EncryptLogic.Decrypt(item.Employee_Pwd);
                    PayUsername = EncryptLogic.Decrypt(item.Payroll_UserName);
                    PayPassword = EncryptLogic.Decrypt(item.Payroll_Password);

                    IRURL = EncryptLogic.Decrypt(item.RedirectURL);
                    CertificatePath = item.CertificatePath;
                    SecretCode = EncryptLogic.Decrypt(item.SecretCode);

                    response = EmployeeServices(_dataContext);
                    string response_pay = EmployeeInformation(_dataContext);

                }
                return response;
            }
            catch (Exception e)
            {
                //log.Info("HCMProcess Method" + e.InnerException);
                return null;
            }

        }
        public string EmployeeServices(string _dataContext)
        {
            try
            {
                BusinessLogic objemployee = new BusinessLogic(_dataContext);

                DataSet employees = new DataSet();
                employees = objemployee.GetAllEmployees();

                //log.Info("EmployeeServices Process Started");

                if (employees.Tables[0].Rows.Count > 0)
                {

                    string geturl = string.Format(ConfigurationManager.AppSettings.Get("requesturl"), ClientID, IRURL);
                    var authcode = GetCookieValue(geturl);
                    //Console.WriteLine(authcode);

                    string posturl = ConfigurationManager.AppSettings.Get("postauthurl");
                    string redirectpath = Posturlmethod(posturl, authcode, IRUsername, IRPassword);
                    //Console.WriteLine(redirectpath);

                    int firstequ = redirectpath.IndexOf('=');
                    string autherisationcode = redirectpath.Substring(firstequ + 1);

                    //https % 3A % 2F % 2Fbhcmst.ramcouat.com % 3A4454 % 2Frvw

                    string _IRURL = System.Web.HttpUtility.UrlEncode(IRURL);
                    string _uriRedirect = string.Format(ConfigurationManager.AppSettings.Get("redirecturi"), _IRURL);

                    string redirecturl = _uriRedirect + autherisationcode;
                    string tokenurl = ConfigurationManager.AppSettings.Get("tokenurl");
                    var authtoken = Tokengen(tokenurl, redirecturl);
                    if (authtoken == "Error")
                    {
                        //log.Info("Tokengen() returns an Exception");
                        return "Error";
                    }

                    string tokenvalurl = ConfigurationManager.AppSettings.Get("tokenvalurl");
                    var value_tokenurl = Tokenvalidation(tokenvalurl, authtoken);

                    if (value_tokenurl == "false" || value_tokenurl == "Error")
                    {
                        return "Error";
                    }

                    string employeefilingurl = ConfigurationManager.AppSettings.Get("employeefilingurl");
                    XmlDocument employeedoc = new XmlDocument();

                    for (int i = 0; i <= employees.Tables[0].Rows.Count - 1; i++)
                    {
                        string xmldata = employees.Tables[0].Rows[i].ItemArray[1].ToString();
                        string guid = employees.Tables[0].Rows[i].ItemArray[2].ToString();
                        string empreqtype = employees.Tables[0].Rows[i].ItemArray[3].ToString();
                        int ouid = (int)employees.Tables[0].Rows[i].ItemArray[0];
                        int errstatus = 0;
                        string errormsg = "";
                        xmlvalidataion(xmldata);

                        employeedoc.LoadXml(xmldata);

                        // 


                        var employeeresponse = EmployeeXMLTransaction(employeefilingurl, authtoken, employeedoc, guid, ouid, "Employee", _dataContext);
                        string xmlcontents = employeeresponse.InnerXml;
                        XDocument xDoc = XDocument.Load(new StringReader(xmlcontents));
                        XNamespace soap = XNamespace.Get("http://www.w3.org/2003/05/soap-envelope");
                        XNamespace emp1 = XNamespace.Get("urn:www.ird.govt.nz/GWS:types/Employment");
                        XNamespace cre = XNamespace.Get("https://services.ird.govt.nz/GWS/Employment/:types/RetrieveListResponse");
                        XNamespace emp = XNamespace.Get("https://services.ird.govt.nz/GWS/Employment/");
                        XNamespace com = XNamespace.Get("urn:www.ird.govt.nz/GWS:types/Common.v1");
                        if (employees.Tables[0].Rows[i].ItemArray[3].ToString() != "Request")
                        {
                            //Console.WriteLine(xmlcontents);
                            IEnumerable<XElement> responses = xDoc.Descendants(com + "statusMessage");
                            foreach (XElement response in responses)
                            {
                                errstatus = (int)response.Element(com + "statusCode");
                                string err = response.Value;
                                errormsg = (string)response.Element(com + "errorMessage");
                            }
                            Employee clsEmp = new Employee();
                            clsEmp.MASTER_OU = Convert.ToInt32(employees.Tables[0].Rows[i].ItemArray[0].ToString());
                            clsEmp.XML_FILE_RESPONSE = xmlcontents;
                            clsEmp.GUID = employees.Tables[0].Rows[i].ItemArray[2].ToString();

                            if (errstatus == 0)
                            {
                                clsEmp.EMPLOYEE_STATUS = "PROCESSED";
                            }
                            else
                            {
                                clsEmp.EMPLOYEE_STATUS = "ERROR";
                            }
                            clsEmp.ERROR_CODE = errstatus;
                            clsEmp.ERROR_DESCRIPTION = errormsg;

                            objemployee.Update(clsEmp);
                        }
                        if (employees.Tables[0].Rows[i].ItemArray[3].ToString() == "Request")
                        {
                            IEnumerable<XElement> responses = xDoc.Descendants(emp1 + "employee");
                            foreach (XElement response in responses)
                            {
                                EMP_RETRIEVE clsEmp_insert = new EMP_RETRIEVE();

                                clsEmp_insert.IRD_NO = (string)response.Element(emp1 + "employeeIRD");
                                clsEmp_insert.LEGALNAME = (string)response.Element(emp1 + "employeeNameOnEILine");
                                clsEmp_insert.EMPLOYMENT_START_DATE = (DateTime)response.Element(emp1 + "employmentStartDate");
                                clsEmp_insert.DATE_OF_BIRTH = (DateTime)response.Element(emp1 + "employeeDateOfBirth");
                                clsEmp_insert.LAST_AVAILABLE_DATE = (DateTime)response.Element(emp1 + "employmentFinishDate");
                                clsEmp_insert.FIRST_NAME = (string)response.Element(emp1 + "employeeName").Element(emp1 + "nameFirst");
                                clsEmp_insert.LAST_NAME = (string)response.Element(emp1 + "employeeName").Element(emp1 + "nameSurname");
                                clsEmp_insert.TAX_CODE = (string)response.Element(emp1 + "taxCodes").Element(emp1 + "taxCode");

                                objemployee.Add(clsEmp_insert, (Guid)employees.Tables[0].Rows[i].ItemArray[2], (int)employees.Tables[0].Rows[i].ItemArray[0]);
                            }

                            IEnumerable<XElement> statusresponses = xDoc.Descendants(com + "statusMessage");
                            foreach (XElement response in statusresponses)
                            {
                                errstatus = (int)response.Element(com + "statusCode");
                                string err = response.Value;
                                errormsg = (string)response.Element(com + "errorMessage");
                            }
                            Employee clsEmp = new Employee();
                            clsEmp.MASTER_OU = Convert.ToInt32(employees.Tables[0].Rows[i].ItemArray[0].ToString());
                            clsEmp.XML_FILE_RESPONSE = xmlcontents;
                            clsEmp.GUID = employees.Tables[0].Rows[i].ItemArray[2].ToString();

                            objemployee.Update(clsEmp);
                        }
                    }

                    //log.Info("Success Message:" + "EI GetData() Function working fine");
                    return "Success";
                }
                else { return "No records found"; }

            }
            catch (Exception e)
            {
                //log.Error("Error Message: EmployeeInformation() - " + e.InnerException + e.Message);
                return "Error";
            }
        }

        static void xmlvalidataion(string xmldata)
        {
            string _xmlPath = ConfigurationManager.AppSettings["xmlPath"];
            XmlReaderSettings booksSettings = new XmlReaderSettings();
            booksSettings.Schemas.Add("urn:www.ird.govt.nz/GWS:types/Employment", @_xmlPath + "employment.xsd");
            booksSettings.Schemas.Add("urn:www.ird.govt.nz/GWS:types/Common.v1", @_xmlPath + "Common.xsd");
            booksSettings.ValidationType = ValidationType.Schema;
            booksSettings.ValidationEventHandler += new ValidationEventHandler(booksSettingsValidationEventHandler);

            //XmlReader books = XmlReader.Create(@"F:\Documents\HCM\Test\text.xml", booksSettings);
            XmlReader books = XmlReader.Create(new StringReader(xmldata), booksSettings);

            while (books.Read()) { }
        }

        static void booksSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
            }
        }
        public string EmployeeInformation(string _dataContext)
        {
            //log.Info("EmployeeInformation Process Started");
            try
            {
                BusinessLogic _objBL = new BusinessLogic(_dataContext);
                DataSet ds = new DataSet();
                ds = _objBL.GetPayrollEmployees();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string geturl = string.Format(ConfigurationManager.AppSettings.Get("requesturl"), ClientID, IRURL);
                    var authcode = GetCookieValue(geturl);
                    //Console.WriteLine(authcode);

                    string posturl = ConfigurationManager.AppSettings.Get("postauthurl");
                    string redirectpath = Posturlmethod(posturl, authcode, PayUsername, PayPassword);
                    //Console.WriteLine(redirectpath);

                    int firstequ = redirectpath.IndexOf('=');
                    string autherisationcode = redirectpath.Substring(firstequ + 1);

                    //https % 3A % 2F % 2Fbhcmst.ramcouat.com % 3A4454 % 2Frvw

                    string _IRURL = System.Web.HttpUtility.UrlEncode(IRURL);
                    string _uriRedirect = string.Format(ConfigurationManager.AppSettings.Get("redirecturi"), _IRURL);

                    string redirecturl = _uriRedirect + autherisationcode;
                    string tokenurl = ConfigurationManager.AppSettings.Get("tokenurl");
                    var authtoken = Tokengen(tokenurl, redirecturl);
                    if (authtoken == "Error")
                    {
                        //log.Info("Tokengen() returns an Exception");
                        return "Error";
                    }

                    string tokenvalurl = ConfigurationManager.AppSettings.Get("tokenvalurl");
                    var value_tokenurl = Tokenvalidation(tokenvalurl, authtoken);

                    if (value_tokenurl == "false" || value_tokenurl == "Error")
                    {
                        return "Error";
                    }

                    //Console.WriteLine(token);

                    //string toBeSearched = "access_token\":\"";
                    //int ix = token.IndexOf(toBeSearched);
                    //int startind = ix + toBeSearched.Length;
                    //int endind = token.Length - startind - 2;
                    //string authtoken = token.Substring(startind, endind);

                    string employeefilingurl = ConfigurationManager.AppSettings.Get("payrollfilingurl");
                    XmlDocument employeedoc = new XmlDocument();

                    for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                    {
                        string xmldata = ds.Tables[0].Rows[i].ItemArray[1].ToString();
                        string guid = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                        string empreqtype = ds.Tables[0].Rows[i].ItemArray[3].ToString();
                        int ouid = (int)ds.Tables[0].Rows[i].ItemArray[0];
                        int errstatus = 0;
                        string errormsg = "";
                        string gateway = "";
                        string subkey = "";
                        employeedoc.LoadXml(xmldata);
                        var employeeresponse = EmployeeXMLTransaction(employeefilingurl, authtoken, employeedoc, guid, ouid, "Payroll", _dataContext);
                        string xmlcontents = employeeresponse.InnerXml;
                        XDocument xDoc = XDocument.Load(new StringReader(xmlcontents));
                        XNamespace soap = XNamespace.Get("http://www.w3.org/2003/05/soap-envelope");
                        XNamespace emp1 = XNamespace.Get("urn:www.ird.govt.nz/GWS:types/Employment");
                        XNamespace cre = XNamespace.Get("https://services.ird.govt.nz/GWS/Employment/:types/RetrieveListResponse");
                        XNamespace emp = XNamespace.Get("https://services.ird.govt.nz/GWS/Employment/");
                        XNamespace com = XNamespace.Get("urn:www.ird.govt.nz/GWS:types/Common.v1");
                        XNamespace xmlns = XNamespace.Get("urn:www.ird.govt.nz/GWS:types/ReturnCommon.v1");
                        if (ds.Tables[0].Rows[i].ItemArray[3].ToString() == "RETURN_FILING")
                        {
                            //Console.WriteLine(xmlcontents);
                            //log.Info("RETURN_FILING Started");
                            IEnumerable<XElement> responses = xDoc.Descendants(com + "statusMessage");
                            foreach (XElement response in responses)
                            {
                                errstatus = (int)response.Element(com + "statusCode");
                                string err = response.Value;
                                errormsg = (string)response.Element(com + "errorMessage");
                            }
                            IEnumerable<XElement> responsebody = xDoc.Descendants(xmlns + "responseBody");
                            foreach (XElement response in responsebody)
                            {
                                gateway = (string)response.Element(xmlns + "gatewayId");
                                string err = response.Value;
                                subkey = (string)response.Element(xmlns + "submissionKey");
                            }

                            Employee clsEmp = new Employee();

                            clsEmp.MASTER_OU = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                            clsEmp.XML_FILE_RESPONSE = xmlcontents;
                            clsEmp.GUID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                            //if (errstatus == 0)
                            //{
                            //    clsEmp.ERROR_CODE = "PROCESSED";
                            //}
                            //else
                            //{
                            //    clsEmp.ERROR_CODE = "ERROR";
                            //}
                            clsEmp.ERROR_CODE = errstatus;
                            clsEmp.ERROR_DESCRIPTION = errormsg;
                            clsEmp.Gatewayid = gateway;
                            clsEmp.SubmissionkeyId = subkey;

                            _objBL.PayrollReturnFiling(clsEmp);
                            //log.Info("RETURN_FILING Completed");
                        }
                        if (ds.Tables[0].Rows[i].ItemArray[3].ToString() == "RETRIEVE_RETURN_REQUEST")
                        {
                            //log.Info("RETRIEVE_RETURN_REQUEST Started");
                            XNamespace xmlnsr = XNamespace.Get("urn:www.ird.govt.nz/GWS:types/ReturnEI.v1");
                            IEnumerable<XElement> retresponses = xDoc.Descendants(xmlnsr + "formFields");
                            foreach (XElement response in retresponses)
                            {
                                subkey = (string)response.Element(xmlnsr + "submissionKey");
                                string paydate = (string)response.Element(xmlnsr + "payDayDate");
                                string contactname = (string)response.Element(xmlnsr + "contactName");
                                string contactphno = (string)response.Element(xmlnsr + "contactPhoneNumber");
                                string contactemail = (string)response.Element(xmlnsr + "contactEmail");
                                double tGrossEarnings = (double)response.Element(xmlnsr + "totalGrossEarnings");
                                double tEarningsNotLiableACC = (double)response.Element(xmlnsr + "totalEarningsNotLiableACC");
                                double tChildSupportDeductions = (double)response.Element(xmlnsr + "totalChildSupportDeductions");
                                double tStudentLoansDeductions = (double)response.Element(xmlnsr + "totalStudentLoansDeductions");
                                double tKiwisaverEmployerContributions = (double)response.Element(xmlnsr + "totalKiwisaverEmployerContributions");
                                double tKiwisaverDeductions = (double)response.Element(xmlnsr + "totalKiwisaverDeductions");
                                double tTaxCreditPayrollDonations = (double)response.Element(xmlnsr + "totalTaxCreditPayrollDonations");
                                double tESCTDeducted = (double)response.Element(xmlnsr + "totalESCTDeducted");
                                double tFamilyTaxCredits = (double)response.Element(xmlnsr + "totalFamilyTaxCredits");
                                double tAmountPayable = (double)response.Element(xmlnsr + "totalAmountPayable");

                                IEnumerable<XElement> retempresponses = xDoc.Descendants(xmlnsr + "employee");
                                foreach (XElement empresponse in retempresponses)
                                {
                                    string lineno = (string)empresponse.Element(xmlnsr + "lineNumber");
                                    string irdNumber = (string)empresponse.Element(xmlnsr + "irdNumber");
                                    string employeeName = (string)empresponse.Element(xmlnsr + "employeeName");
                                    string taxCode = (string)empresponse.Element(xmlnsr + "taxCode");
                                    string payPeriodStartDate = (string)empresponse.Element(xmlnsr + "payPeriodStartDate");
                                    string payPeriodEndDate = (string)empresponse.Element(xmlnsr + "payPeriodEndDate");
                                    string employmentStartDate = (string)empresponse.Element(xmlnsr + "employmentStartDate");
                                    string employeePayFrequency = (string)empresponse.Element(xmlnsr + "employeePayFrequency");
                                    string grossEarnings = (string)empresponse.Element(xmlnsr + "grossEarnings");
                                    string earningsNotLiableACC = (string)empresponse.Element(xmlnsr + "earningsNotLiableACC");
                                    string lumpSumIndicator = (string)empresponse.Element(xmlnsr + "lumpSumIndicator");
                                    string payeSchedularTaxDeductions = (string)empresponse.Element(xmlnsr + "payeSchedularTaxDeductions");
                                    string childSupportCode = (string)empresponse.Element(xmlnsr + "childSupportCode");
                                    string childSupportDeductions = (string)empresponse.Element(xmlnsr + "childSupportDeductions");
                                    string studentLoansDeductions = (string)empresponse.Element(xmlnsr + "studentLoansDeductions");
                                    string kiwisaverEmployerContributions = (string)empresponse.Element(xmlnsr + "kiwisaverEmployerContributions");
                                    string kiwisaverDeductions = (string)empresponse.Element(xmlnsr + "kiwisaverDeductions");
                                    string taxCreditPayrollDonations = (string)empresponse.Element(xmlnsr + "taxCreditPayrollDonations");
                                    string esctDeducted = (string)empresponse.Element(xmlnsr + "esctDeducted");
                                    string familyTaxCredits = (string)empresponse.Element(xmlnsr + "familyTaxCredits");

                                    Payroll clsPay = new Payroll();

                                    clsPay.MASTEROU = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                                    clsPay.GUID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                                    clsPay.REQUEST_ID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                                    clsPay.REQ_STATUS = "PROCESSSED";
                                    clsPay.REQ_DATE = Convert.ToDateTime(ds.Tables[0].Rows[i].ItemArray[4]);
                                    clsPay.SUBMISSION_KEY = subkey;
                                    clsPay.LINE_NUMBER = lineno;
                                    clsPay.EMPLOYEE_IRD_NUMBER = irdNumber;
                                    clsPay.EMPLOYEE_NAME = employeeName;
                                    clsPay.TAX_CODE = taxCode;
                                    clsPay.PAY_PERIOD_START_DATE = Convert.ToDateTime(payPeriodStartDate);
                                    clsPay.PAY_PERIOD_END_DATE = Convert.ToDateTime(payPeriodEndDate);
                                    clsPay.EMPLOYMENT_START_DAT = Convert.ToDateTime(employmentStartDate);
                                    clsPay.EMPLOYEE_PAY_FREQUENCY = employeePayFrequency;
                                    clsPay.GROSS_EARNINGS = decimal.Parse(grossEarnings);
                                    clsPay.EARNINGS_NOT_LIABLE_ACC = decimal.Parse(earningsNotLiableACC);
                                    clsPay.LUMPSUM_INDICATOR = lumpSumIndicator;
                                    clsPay.PAYE_SCHEDULAR_TAX_DEDUCTIONS = decimal.Parse(payeSchedularTaxDeductions);
                                    clsPay.CHILD_SUPPORT_CODE = childSupportCode;
                                    clsPay.CHILD_SUPPORT_DEDUCTIONS = decimal.Parse(childSupportDeductions);
                                    clsPay.STUDENT_LOAN_DEDUCTIONS = decimal.Parse(studentLoansDeductions);
                                    clsPay.KIWI_SAVER_EMPLOYER_CONTRIBUTIONS = decimal.Parse(kiwisaverEmployerContributions);
                                    clsPay.KIWI_SAVER_DEDUCTIONS = decimal.Parse(kiwisaverDeductions);
                                    clsPay.TAX_CREDIT_PAYROLL_DONATIONS = decimal.Parse(taxCreditPayrollDonations);
                                    clsPay.ESCT_DEDUCTED = decimal.Parse(esctDeducted);
                                    clsPay.Family_Tax_Credits = decimal.Parse(familyTaxCredits);
                                    clsPay.created_by = "SYSTEM";
                                    clsPay.modified_by = "SYSTEM";
                                    _objBL.PayrollInsert(clsPay);
                                }
                            }

                            IEnumerable<XElement> statusresponses = xDoc.Descendants(com + "statusMessage");
                            foreach (XElement response in statusresponses)
                            {
                                errstatus = (int)response.Element(com + "statusCode");
                                string err = response.Value;
                                errormsg = (string)response.Element(com + "errorMessage");
                            }
                            Employee clsEmp = new Employee();

                            clsEmp.MASTER_OU = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                            clsEmp.XML_FILE_RESPONSE = xmlcontents;
                            clsEmp.GUID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                            if (errstatus == 0)
                            {
                                clsEmp.EMPLOYEE_STATUS = "PROCESSED";
                            }
                            else
                            {
                                clsEmp.EMPLOYEE_STATUS = "ERROR";
                            }
                            clsEmp.ERROR_CODE = errstatus;
                            clsEmp.ERROR_DESCRIPTION = errormsg;

                            _objBL.PayrollUpdate(clsEmp);
                            //log.Info("RETRIEVE_RETURN_REQUEST Completed");
                        }
                        if (ds.Tables[0].Rows[i].ItemArray[3].ToString() == "RETRIEVE_PRE_POP_REQUEST")
                        {
                            //log.Info("RETRIEVE_PRE_POP_REQUEST Started");
                            XNamespace xmlnsr = XNamespace.Get("urn:www.ird.govt.nz/GWS:types/ReturnEI.v1");

                            IEnumerable<XElement> retempresponses = xDoc.Descendants(xmlnsr + "employee");
                            foreach (XElement empresponse in retempresponses)
                            {
                                //string lineno = (string)empresponse.Element(xmlnsr + "lineNumber");
                                string irdNumber = (string)empresponse.Element(xmlnsr + "irdNumber");
                                string employeeName = (string)empresponse.Element(xmlnsr + "employeeName");
                                string taxCode = (string)empresponse.Element(xmlnsr + "taxCode");
                                //string payPeriodStartDate = (string)empresponse.Element(xmlnsr + "payPeriodStartDate");
                                //string payPeriodEndDate = (string)empresponse.Element(xmlnsr + "payPeriodEndDate");
                                //string employmentStartDate = (string)empresponse.Element(xmlnsr + "employmentStartDate");
                                //string employmentEndDate = (string)empresponse.Element(xmlnsr + "employmentEndDate");
                                //string employeePayFrequency = (string)empresponse.Element(xmlnsr + "employeePayFrequency");
                                //string grossEarnings = (string)empresponse.Element(xmlnsr + "grossEarnings");
                                //string earningsNotLiableACC = (string)empresponse.Element(xmlnsr + "earningsNotLiableACC");
                                //string lumpSumIndicator = (string)empresponse.Element(xmlnsr + "lumpSumIndicator");
                                //string payeSchedularTaxDeductions = (string)empresponse.Element(xmlnsr + "payeSchedularTaxDeductions");
                                //string childSupportCode = (string)empresponse.Element(xmlnsr + "childSupportCode");
                                //string childSupportDeductions = (string)empresponse.Element(xmlnsr + "childSupportDeductions");
                                //string studentLoansDeductions = (string)empresponse.Element(xmlnsr + "studentLoansDeductions");
                                //string kiwisaverEmployerContributions = (string)empresponse.Element(xmlnsr + "kiwisaverEmployerContributions");
                                //string kiwisaverDeductions = (string)empresponse.Element(xmlnsr + "kiwisaverDeductions");
                                //string taxCreditPayrollDonations = (string)empresponse.Element(xmlnsr + "taxCreditPayrollDonations");
                                //string esctDeducted = (string)empresponse.Element(xmlnsr + "esctDeducted");
                                //string familyTaxCredits = (string)empresponse.Element(xmlnsr + "familyTaxCredits");

                                PAYROLL_PREPOP clsPay = new PAYROLL_PREPOP();

                                clsPay.MASTEROU = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                                clsPay.GUID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                                clsPay.REQUEST_ID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                                clsPay.REQ_STATUS = "PROCESSSED";
                                clsPay.REQ_DATE = Convert.ToDateTime(ds.Tables[0].Rows[i].ItemArray[4]);
                                clsPay.FORM_TYPE = "";
                                clsPay.SUBMISSION_KEY = subkey;
                                clsPay.EMPLOYEE_IRD_NUMBER = irdNumber;
                                clsPay.EMPLOYEE_NAME = employeeName;
                                clsPay.TAX_CODE = taxCode;
                                //clsPay.PAY_DAY_DATE = Convert.ToDateTime(paydate);
                                //clsPay.PAY_PERIOD_END_DATE = Convert.ToDateTime(payPeriodEndDate);
                                //clsPay.EMPLOYMENT_START_DATE = Convert.ToDateTime(employmentStartDate);
                                //clsPay.EMPLOYMENT_END_DATE = Convert.ToDateTime(employmentEndDate);
                                clsPay.created_by = "SYSTEM";
                                clsPay.modified_by = "SYSTEM";
                                // clsPay.RETRIEVE_DATE = "";
                                clsPay.MOD_FLAG = "";
                                _objBL.PrePopInsert(clsPay);
                            }


                            IEnumerable<XElement> statusresponses = xDoc.Descendants(com + "statusMessage");
                            foreach (XElement response in statusresponses)
                            {
                                errstatus = (int)response.Element(com + "statusCode");
                                string err = response.Value;
                                errormsg = (string)response.Element(com + "errorMessage");
                            }
                            Employee clsEmp = new Employee();

                            clsEmp.MASTER_OU = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                            clsEmp.XML_FILE_RESPONSE = xmlcontents;
                            clsEmp.GUID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                            if (errstatus == 0)
                            {
                                clsEmp.EMPLOYEE_STATUS = "PROCESSED";
                            }
                            else
                            {
                                clsEmp.EMPLOYEE_STATUS = "ERROR";
                            }
                            clsEmp.ERROR_CODE = errstatus;
                            clsEmp.ERROR_DESCRIPTION = errormsg;

                            _objBL.PayrollUpdate(clsEmp);

                            //log.Info("RETRIEVE_PRE_POP_REQUEST Completed");
                        }
                        if (ds.Tables[0].Rows[i].ItemArray[3].ToString() == "RETURNSTATUS_REQUEST")
                        {
                            string status_info = "";
                            //log.Info("RETURNSTATUS_REQUEST Started");
                            XNamespace xmlnsr = XNamespace.Get("urn:www.ird.govt.nz/GWS:types/ReturnEI.v1");

                            IEnumerable<XElement> statusresponsebody = xDoc.Descendants(xmlns + "retrieveStatusResponse");
                            foreach (XElement response in statusresponsebody)
                            {
                                status_info = (string)response.Element(xmlns + "responseBody");
                            }
                            Employee clsEmp_returnreq = new Employee();

                            clsEmp_returnreq.MASTER_OU = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                            clsEmp_returnreq.GUID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                            clsEmp_returnreq.EMPLOYEE_STATUS = status_info;
                            _objBL.ReturnStatusRequestUpdate(clsEmp_returnreq);


                            IEnumerable<XElement> statusresponses = xDoc.Descendants(com + "statusMessage");
                            foreach (XElement response in statusresponses)
                            {
                                errstatus = (int)response.Element(com + "statusCode");
                                string err = response.Value;
                                errormsg = (string)response.Element(com + "errorMessage");
                            }
                            Employee clsEmp = new Employee();

                            clsEmp.MASTER_OU = Convert.ToInt32(ds.Tables[0].Rows[i].ItemArray[0].ToString());
                            clsEmp.XML_FILE_RESPONSE = xmlcontents;
                            clsEmp.GUID = ds.Tables[0].Rows[i].ItemArray[2].ToString();
                            if (errstatus == 0)
                            {
                                clsEmp.EMPLOYEE_STATUS = "PROCESSED";
                            }
                            else
                            {
                                clsEmp.EMPLOYEE_STATUS = "ERROR";
                            }
                            clsEmp.ERROR_CODE = errstatus;
                            clsEmp.ERROR_DESCRIPTION = errormsg;

                            _objBL.PayrollUpdate(clsEmp);

                            //log.Info("RETRIEVE_PRE_POP_REQUEST Completed");
                        }
                    }
                    //adapter.Dispose();
                    //command.Dispose();
                    //conn.Close();

                    //log.Info("Success Message:" + "EI GetData() Function working fine");
                    return "Success";
                }
                else { return "No records found"; }

            }
            catch (Exception e)
            {
                //log.Error("Error Message: EmployeeInformation() - " + e.InnerException + e.Message);
                return "Error";
            }
        }
        private List<Cookie> GetCookieValue(string url)
        {

            //log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
            try
            {
                //X509Certificate2 certificate = new X509Certificate2(@"E:\Jaga\Wildcard_RAMCOHCMPDF_com.pfx", "hcm@123");
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler();
                //handler.ClientCertificates.Add(certificate);
                handler.AllowAutoRedirect = true;
                handler.CookieContainer = cookies;
                HttpClient client = new HttpClient(handler);

                HttpResponseMessage response = client.GetAsync(url).Result;

                Uri uri = new Uri(url);
                List<Cookie> cookielist = cookies.GetCookies(uri).Cast<Cookie>().ToList<Cookie>();


                //log.Info("Success Message:" + "GetCookieValue Function working fine");

                return (cookielist);

            }
            catch (Exception e)
            {
                //log.Error("Error Message: GetCookieValue() - " + e.InnerException + e.Message);
                return null;
            }
        }
        private string Posturlmethod(string url, List<Cookie> authsession, string username, string password)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
            try
            {
                CookieContainer cookieContainer = new CookieContainer();
                foreach (Cookie cookieval in authsession)
                {
                    cookieContainer.Add(cookieval);
                }
                Uri uri = new Uri(url);
                //X509Certificate2 certificate = new X509Certificate2(@"E:\Jaga\Wildcard_RAMCOHCMPDF_com.pfx", "hcm@123");
                byte[] buffer = Encoding.ASCII.GetBytes("userid=" + username + "&password=" + password + "&login=Login");
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = buffer.Length;
                webReq.AllowAutoRedirect = true;
                //webReq.Headers["Cookie"] = authsession;
                webReq.CookieContainer = cookieContainer;
                //webReq.ClientCertificates.Add(certificate);

                Stream PostData = webReq.GetRequestStream();
                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                //string redirecturl = webResp.GetResponseHeader("Location");
                String redirecturl = webResp.ResponseUri.ToString();
                System.IO.StreamReader answer = new System.IO.StreamReader(webResp.GetResponseStream());
                string returnValue = answer.ReadToEnd();


                //log.Info("Success Message:" + "Posturlmethod Function working fine");

                return (redirecturl);
            }

            catch (Exception e)
            {
                //log.Error("Error Message: Posturlmethod() - " + e.InnerException + e.Message);
                return "Error";
            }
        }

        private string Tokengen(string url, string pathredirect)
        {
            // log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
            try
            {
                // string plaintext = "Ramco_Payroll-RT:2VFvArFC3jdFW";
                //X509Certificate2 certificate = new X509Certificate2(@"E:\Jaga\Wildcard_RAMCOHCMPDF_com.pfx", "hcm@123");
                string plaintext = "Ramco_Payroll-RT:" + SecretCode;
                string authbasic = Base64Encode(plaintext);
                Uri uri = new Uri(url);
                byte[] tokenbuffer = Encoding.ASCII.GetBytes(pathredirect);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.Headers.Add("Authorization", "Basic " + authbasic);
                webReq.ContentLength = tokenbuffer.Length;
                webReq.AllowAutoRedirect = true;
                webReq.KeepAlive = true;
                webReq.SendChunked = true;
                //webReq.ClientCertificates.Add(certificate);

                Stream PostData = webReq.GetRequestStream();
                PostData.Write(tokenbuffer, 0, tokenbuffer.Length);
                PostData.Close();
                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                //string redirecturl = webResp.GetResponseHeader("Location");
                System.IO.StreamReader answer = new System.IO.StreamReader(webResp.GetResponseStream());
                //string returnValue = answer.ReadToEnd();
                tokendetails returnValue = JsonConvert.DeserializeObject<tokendetails>(answer.ReadToEnd());



                //log.Info("Success Message:" + "Tokengen Function working fine");

                return (returnValue.access_token);
            }

            catch (Exception e)
            {
                //log.Error("Error Message: Tokengen() - " + e.InnerException + e.Message);
                return "Error";

            }
        }

        public string Tokenvalidation(string v_strURL, string authtoken)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
            try
            {
                //X509Certificate2 certificate = new X509Certificate2(@"E:\Jaga\Wildcard_RAMCOHCMPDF_com.pfx", "hcm@123");
                //string plaintext = "Ramco_Payroll-RT:gQdUWT0UmJnOW";
                string plaintext = "Ramco_Payroll-RT:" + SecretCode;
                string authbasic = Base64Encode(plaintext);
                Uri uri = new Uri(v_strURL);
                byte[] tokenbuffer = Encoding.ASCII.GetBytes("grant_type=oracle-idm:/oauth/grant-type/resource-access-token/jwt&oracle_token_action=validate&scope=MYIR.Services&assertion=" + authtoken + "&oracle_token_attrs_retrieval=prn exp");
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.Headers.Add("Authorization", "Basic " + authbasic);
                //webReq.ClientCertificates.Add(certificate);
                webReq.ContentLength = tokenbuffer.Length;
                webReq.AllowAutoRedirect = true;
                webReq.KeepAlive = true;
                webReq.SendChunked = true;

                Stream PostData = webReq.GetRequestStream();
                PostData.Write(tokenbuffer, 0, tokenbuffer.Length);
                PostData.Close();
                string reqdat = webReq.RequestUri.ToString();
                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                //string redirecturl = webResp.GetResponseHeader("Location");
                System.IO.StreamReader answer = new System.IO.StreamReader(webResp.GetResponseStream());
                string returnValue = answer.ReadToEnd();

                tokenvalidation valid = JsonConvert.DeserializeObject<tokenvalidation>(returnValue);

                //log.Info("Success Message:" + "TokenValidation is Function working fine");

                return (valid.successful.ToString());
            }

            catch (Exception e)
            {
                //log.Error("Error Message: Tokengen() - " + e.InnerException + e.Message);
                return "Error";

            }

        }
        private string Employeefiling(string url, string employepath)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));
            try
            {
                Uri uri = new Uri(url);
                byte[] tokenbuffer = Encoding.ASCII.GetBytes(employepath);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.Headers.Add("Authorization", "Basic VGVzdENsaWVudDE6VGVzdENsaWVudDFTZWNyZXQ=");
                webReq.ContentLength = tokenbuffer.Length;
                webReq.AllowAutoRedirect = true;
                webReq.KeepAlive = true;
                webReq.SendChunked = true;

                Stream PostData = webReq.GetRequestStream();
                PostData.Write(tokenbuffer, 0, tokenbuffer.Length);
                PostData.Close();
                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                //string redirecturl = webResp.GetResponseHeader("Location");
                System.IO.StreamReader answer = new System.IO.StreamReader(webResp.GetResponseStream());
                string returnValue = answer.ReadToEnd();
                //log.Info("Success Message:" + "Employeefiling Function working fine");

                return (returnValue);
            }

            catch (Exception e)
            {
                //log.Error("Error Message: Employeefiling() - " + e.InnerException);
                return "Error";

            }
        }
        private string Base64Encode(string plaintext)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public XmlDocument EmployeeXMLTransaction(string v_strURL, string authtoken, XmlDocument v_objXMLDoc, string guid, int ouid, string type, string _dataContext)
        {
            //log4net.ILog log = log4net.LogManager.GetLogger(typeof(Program));

            // SqlConnection con = new SqlConnection(_connectionString);
            BusinessLogic _objBL = new BusinessLogic(_dataContext);

            try
            {
                XmlDocument XMLResponse = null;
                HttpWebRequest objHttpWebRequest;
                HttpWebResponse objHttpWebResponse = null;
                Stream objRequestStream = null;
                Stream objResponseStream = null;

                XmlTextReader objXMLReader;
                X509Certificate2 certificate = new X509Certificate2(@CertificatePath, "kan@123");

                objHttpWebRequest = (HttpWebRequest)WebRequest.Create(v_strURL);

                try
                {
                    objHttpWebRequest.ClientCertificates.Add(certificate);
                    byte[] bytes;
                    bytes = System.Text.Encoding.ASCII.GetBytes(v_objXMLDoc.InnerXml);
                    objHttpWebRequest.Method = "POST";
                    objHttpWebRequest.ContentLength = bytes.Length;
                    objHttpWebRequest.ContentType = "application/soap+xml";
                    objHttpWebRequest.Headers.Add("Authorization", "Bearer " + authtoken);
                    if (type == "Payroll")
                    {
                        objHttpWebRequest.Headers.Add("Fastslice", "Ramco_EI");
                    }
                    else
                        objHttpWebRequest.Headers.Add("Fastslice", "Ramco_ES");


                    //Get Stream object 
                    objRequestStream = objHttpWebRequest.GetRequestStream();

                    //Writes a sequence of bytes to the current stream 
                    objRequestStream.Write(bytes, 0, bytes.Length);

                    //Close stream
                    objRequestStream.Close();

                    //---------- End HttpRequest

                    //Sends the HttpWebRequest, and waits for a response.
                    objHttpWebResponse = (HttpWebResponse)objHttpWebRequest.GetResponse();

                    //---------- Start HttpResponse
                    if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //Get response stream 
                        objResponseStream = objHttpWebResponse.GetResponseStream();

                        //Load response stream into XMLReader
                        objXMLReader = new XmlTextReader(objResponseStream);

                        //Declare XMLDocument
                        XmlDocument xmldoc = new XmlDocument();
                        xmldoc.Load(objXMLReader);

                        //Set XMLResponse object returned from XMLReader
                        XMLResponse = xmldoc;

                        //Close XMLReader
                        objXMLReader.Close();
                    }

                    //Close HttpWebResponse
                    objHttpWebResponse.Close();
                }
                catch (WebException we)
                {
                    //TODO: Add custom exception handling
                    var resp = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();

                    Employee employee = new Employee();
                    employee.MASTER_OU = ouid;
                    employee.XML_FILE_RESPONSE = "";
                    employee.GUID = guid;
                    employee.EMPLOYEE_STATUS = "ERROR";
                    employee.ERROR_CODE = 404;
                    employee.ERROR_DESCRIPTION = resp;

                    _objBL.Update(employee);
                    //log.Error("Error Message: EmployeeXMLTransaction - " + we.Message);
                }
                catch (Exception ex)
                {
                    //log.Error("Error Message: EmployeeXMLTransaction - " + ex.Message);
                    throw new Exception(ex.Message);
                }
                finally
                {
                    //Close connections
                    objRequestStream.Close();
                    objResponseStream.Close();
                    objHttpWebResponse.Close();

                    //Release objects
                    objXMLReader = null;
                    objRequestStream = null;
                    objResponseStream = null;
                    objHttpWebResponse = null;
                    objHttpWebRequest = null;
                }

                //Return

                //log.Info("Success Message:" + "EmployeeXMLTransaction Function working fine");

                return XMLResponse;
            }

            catch (Exception e)
            {
                //log.Error("Error Message: EmployeeXMLTransaction() - " + e.InnerException + e.Message);
                return null;

            }
        }
        private class tokendetails
        {
            public string expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
            public string access_token { get; set; }
        }
        private class tokenvalidation
        {
            public string successful { get; set; }
        }
    }
}
