using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServicewithTimer
{
    public class BusinessLogic
    {
        private string _connectionString;

        public BusinessLogic()
        {
            //_connectionString = ConfigurationManager.AppSettings["HCMIRDTest"];
        }
        public BusinessLogic(string constr)
        {
            //string _cs = "Data Source=" + cd.SQLInstance + ";Initial Catalog=" + cd.Database + ";User Id="
            //      + cd.Username + ";Password=" + cd.Password;
            _connectionString = constr;
        }
        public DataSet getLoginDetails(string Username, string Password)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM HCM_IRD_INTEGRATION_USERS WHERE UserName='" + Username + "' and Password='" + Password + "'", _connectionString);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        public int Add(EMP_RETRIEVE employee, Guid guid, int ouid)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {
                SqlCommand update = new SqlCommand("NZ_EMP_RETRIEVE_HDR_XML_INSERT", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter IRD_NO = update.Parameters.Add("@IRD_NO", SqlDbType.VarChar, 40);
                IRD_NO.Direction = ParameterDirection.Input;
                SqlParameter LEGALNAME = update.Parameters.Add("@LEGALNAME", SqlDbType.VarChar, 40);
                LEGALNAME.Direction = ParameterDirection.Input;
                SqlParameter DATE_OF_BIRTH = update.Parameters.Add("@DATE_OF_BIRTH", SqlDbType.DateTime);
                DATE_OF_BIRTH.Direction = ParameterDirection.Input;
                SqlParameter REQ_ID = update.Parameters.Add("@REQ_ID", SqlDbType.VarChar, 40);
                REQ_ID.Direction = ParameterDirection.Input;
                SqlParameter REQ_STATUS = update.Parameters.Add("@REQ_STATUS", SqlDbType.VarChar, 40);
                REQ_STATUS.Direction = ParameterDirection.Input;
                SqlParameter REQ_DATE = update.Parameters.Add("@REQ_DATE", SqlDbType.VarChar, 40);
                REQ_DATE.Direction = ParameterDirection.Input;
                SqlParameter REQ_USER = update.Parameters.Add("@REQ_USER", SqlDbType.VarChar, 40);
                REQ_USER.Direction = ParameterDirection.Input;
                SqlParameter REQ_USER_ROLE = update.Parameters.Add("@REQ_USER_ROLE", SqlDbType.VarChar, 40);
                REQ_USER_ROLE.Direction = ParameterDirection.Input;
                SqlParameter TITLE_CODE = update.Parameters.Add("@TITLE_CODE", SqlDbType.VarChar, 40);
                TITLE_CODE.Direction = ParameterDirection.Input;
                SqlParameter FIRST_NAME = update.Parameters.Add("@FIRST_NAME", SqlDbType.VarChar, 40);
                FIRST_NAME.Direction = ParameterDirection.Input;
                SqlParameter LAST_NAME = update.Parameters.Add("@LAST_NAME", SqlDbType.VarChar, 40);
                LAST_NAME.Direction = ParameterDirection.Input;
                SqlParameter MIDDLE_NAME = update.Parameters.Add("@MIDDLE_NAME", SqlDbType.VarChar, 40);
                MIDDLE_NAME.Direction = ParameterDirection.Input;
                SqlParameter TAX_CODE = update.Parameters.Add("@TAX_CODE", SqlDbType.VarChar, 40);
                TAX_CODE.Direction = ParameterDirection.Input;
                SqlParameter EMPLOYMENT_START_DATE = update.Parameters.Add("@EMPLOYMENT_START_DATE", SqlDbType.DateTime);
                EMPLOYMENT_START_DATE.Direction = ParameterDirection.Input;
                SqlParameter LAST_AVAILABLE_DATE = update.Parameters.Add("@LAST_AVAILABLE_DATE", SqlDbType.DateTime);
                LAST_AVAILABLE_DATE.Direction = ParameterDirection.Input;
                SqlParameter RETRIEVE_DATE = update.Parameters.Add("@RETRIEVE_DATE", SqlDbType.DateTime);
                RETRIEVE_DATE.Direction = ParameterDirection.Input;
                SqlParameter REQ_MASTER_OU = update.Parameters.Add("@REQ_MASTER_OU", SqlDbType.VarChar, 40);
                REQ_MASTER_OU.Direction = ParameterDirection.Input;

                IRD_NO.Value = employee.IRD_NO; ;
                LEGALNAME.Value = employee.LEGALNAME;
                EMPLOYMENT_START_DATE.Value = employee.EMPLOYMENT_START_DATE;
                DATE_OF_BIRTH.Value = employee.DATE_OF_BIRTH;
                REQ_ID.Value = guid;
                REQ_MASTER_OU.Value = ouid;
                LAST_AVAILABLE_DATE.Value = employee.LAST_AVAILABLE_DATE;
                FIRST_NAME.Value = employee.FIRST_NAME;
                LAST_NAME.Value = employee.LAST_NAME;
                TAX_CODE.Value = employee.TAX_CODE;
                REQ_STATUS.Value = "COMPLETED";

                //string sqlins = "INSERT INTO NZ_EMP_RETRIEVE_HDR_XML (IRD_NO,	LEGALNAME,	DATE_OF_BIRTH,	REQ_ID,	REQ_STATUS,	REQ_DATE,	REQ_USER,	" +
                //                                "REQ_USER_ROLE,	TITLE_CODE,	FIRST_NAME,	LAST_NAME,	MIDDLE_NAME,	TAX_CODE,	" +
                //                                "EMPLOYMENT_START_DATE,	LAST_AVAILABLE_DATE, RETRIEVE_DATE,	REQ_MASTER_OU) " +
                //                                "VALUES ('" + ird + "','" + EIline + "','" + dob + "','" + guid + "', 'COMPLETED', GETDATE(), 'SYSTEM', 'SYSTEM', NULL,'" +
                //                                firstname + "', NULL, NULL,'" + taxcode + "','" + startdate + "','" + finishdate + "', GETDATE(), " + ouid + ")";


                con.Open();
                update.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                con.Close();
            }

            //foreach (var item in employee)
            //{
            //           SqlCommand insert = new SqlCommand(sqlins, con);
            //insert.ExecuteNonQuery();
            //}
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Models.Employee, Entities.Employee>();
            //});
            //var mapper = config.CreateMapper();
            //var entityModel = mapper.Map<Models.Employee, Entities.Employee>(employee);
            //using (var unitOfWork = new UnitOfWork(new EmployeeManagementEntities()))
            //{
            //    unitOfWork.Employees.Add(entityModel);
            //    return unitOfWork.Complete();
            //}
            return 0;
        }

        public int Update(Employee employee)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {

                SqlCommand update = new SqlCommand("NZ_RESPONSE_UPDATE", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter masterou = update.Parameters.Add("@MASTEROU", SqlDbType.Int);
                masterou.Direction = ParameterDirection.Input;
                SqlParameter guidin = update.Parameters.Add("@GUID", SqlDbType.VarChar, 40);
                guidin.Direction = ParameterDirection.Input;
                SqlParameter xmlin = update.Parameters.Add("@XMLFILERESPONSE", SqlDbType.Xml);
                xmlin.Direction = ParameterDirection.Input;
                SqlParameter status = update.Parameters.Add("@STATUS", SqlDbType.VarChar, 40);
                status.Direction = ParameterDirection.Input;
                SqlParameter errorcode = update.Parameters.Add("@ERROR_CODE", SqlDbType.Int);
                errorcode.Direction = ParameterDirection.Input;
                SqlParameter errordesc = update.Parameters.Add("@ERROR_DESCRIPTION", SqlDbType.VarChar);
                errordesc.Direction = ParameterDirection.Input;
                masterou.Value = employee.MASTER_OU;
                xmlin.Value = employee.XML_FILE_RESPONSE;
                guidin.Value = employee.GUID;
                status.Value = employee.EMPLOYEE_STATUS;
                errorcode.Value = Convert.ToInt32(employee.ERROR_CODE);
                errordesc.Value = employee.ERROR_DESCRIPTION;

                con.Open();
                update.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                con.Close();
            }

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Models.Employee, Entities.Employee>();
            //});
            //var mapper = config.CreateMapper();
            //var entityModel = mapper.Map<Models.Employee, Entities.Employee>(employee);\=-    1`  
            //using (var unitOfWork = new UnitOfWork(new EmployeeManagementEntities()))
            //{
            //    unitOfWork.Employees.Add(entityModel);
            //    return unitOfWork.Complete();
            //}
            return 0;
        }

        public DataSet GetAllEmployees()
        {
            // var value = ConfigurationManager.ConnectionStrings["HCMIRDTest"].ConnectionString;

            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand select = new SqlCommand("SELECT MASTER_OU, XML_FILE, GUID, FILING_TYPE  from NZ_JOB_XML_TBL WHERE EMPLOYEE_STATUS = 'INITIATE' AND CATEGORY ='Employee'", con);
            SqlDataAdapter da = new SqlDataAdapter(select);
            DataSet ds = new DataSet();
            da.Fill(ds);

            //var empList = ds.Tables[0].AsEnumerable().Select(dataRow => new ViewModels.Employee { LEGALNAME = dataRow.Field<string>("LEGALNAME") }).ToList();

            //List<ViewModels.Employee> empList = new List<ViewModels.Employee>();
            //empList = ds.Tables[0].AsEnumerable().ToList();

            return ds;

            //var employeesList = new List<ViewModels.Employee>();
            ////AutoMapper for converting entity model into view model
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Entities.Employee, ViewModels.Employee>();
            //});
            //var mapper = config.CreateMapper();

            //using (var unitOfWork = new UnitOfWork(new EmployeeManagementEntities()))
            //{
            //    var employees = unitOfWork.Employees.GetAll();
            //    foreach (var employee in employees)
            //    {

            //        employeesList.Add(mapper.Map<Entities.Employee, ViewModels.Employee>(employee));
            //    }
            //}

            //return employeesList;
        }

        public List<Employee> GetEmployees()
        {
            // var value = ConfigurationManager.ConnectionStrings["HCMIRDTest"].ConnectionString;

            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand select = new SqlCommand("SELECT *  from NZ_JOB_XML_TBL WHERE EMPLOYEE_STATUS = 'PROCESSED' AND EMPLOYEE_TYPE='CREATE'", con);
            SqlDataAdapter da = new SqlDataAdapter(select);
            DataSet ds = new DataSet();
            da.Fill(ds);

            var empList = ds.Tables[0].AsEnumerable().Select(dataRow => new Employee { EMPLOYEE_TYPE = dataRow.Field<string>("EMPLOYEE_TYPE") }).ToList();

            //List<ViewModels.Employee> empList = new List<ViewModels.Employee>();
            //empList = ds.Tables[0].AsEnumerable().ToList();

            return empList;


        }

        public int InsertServerDetails(ServerDetails SD)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {

                SqlCommand update = new SqlCommand("NZ_RESPONSE_UPDATE", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter ServerName = update.Parameters.Add("@ServerName", SqlDbType.Int);
                ServerName.Direction = ParameterDirection.Input;
                SqlParameter DatabaseName = update.Parameters.Add("@DatabaseName", SqlDbType.VarChar, 40);
                DatabaseName.Direction = ParameterDirection.Input;
                SqlParameter Username = update.Parameters.Add("@Username", SqlDbType.Xml);
                Username.Direction = ParameterDirection.Input;
                SqlParameter Password = update.Parameters.Add("@Password", SqlDbType.VarChar, 40);
                Password.Direction = ParameterDirection.Input;
                SqlParameter Status = update.Parameters.Add("@DBStatus", SqlDbType.Int);
                Status.Direction = ParameterDirection.Input;

                ServerName.Value = SD.ServerName;
                DatabaseName.Value = SD.Database;
                Username.Value = SD.Username;
                Password.Value = SD.Password;
                Status.Value = SD.Status;

                con.Open();
                update.ExecuteNonQuery();
                con.Close();

                return 1;
            }
            catch (Exception e)
            {
                con.Close();
                return 0;
            }
        }


        //Get Payroll Data

        public DataSet GetPayrollEmployees()
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand select = new SqlCommand("SELECT MASTER_OU, XML_FILE, GUID, FILING_TYPE, RUN_DATE  from NZ_JOB_XML_TBL WHERE EMPLOYEE_STATUS = 'INITIATE' AND CATEGORY = 'Payroll'", con);
            SqlDataAdapter da = new SqlDataAdapter(select);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public int PayrollReturnFiling(Employee emp)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {

                SqlCommand update = new SqlCommand("NZ_EI_RESPONSE_UPDATE", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter masterou = update.Parameters.Add("@MASTEROU", SqlDbType.Int);
                masterou.Direction = ParameterDirection.Input;
                SqlParameter guidin = update.Parameters.Add("@GUID", SqlDbType.VarChar, 40);
                guidin.Direction = ParameterDirection.Input;
                SqlParameter xmlin = update.Parameters.Add("@XMLFILERESPONSE", SqlDbType.Xml);
                xmlin.Direction = ParameterDirection.Input;
                SqlParameter status = update.Parameters.Add("@STATUS", SqlDbType.VarChar, 40);
                xmlin.Direction = ParameterDirection.Input;
                SqlParameter errorcode = update.Parameters.Add("@ERROR_CODE", SqlDbType.Int);
                xmlin.Direction = ParameterDirection.Input;
                SqlParameter errordesc = update.Parameters.Add("@ERROR_DESCRIPTION", SqlDbType.VarChar);
                xmlin.Direction = ParameterDirection.Input;
                SqlParameter gatwayid = update.Parameters.Add("@GATEWAYID", SqlDbType.VarChar);
                xmlin.Direction = ParameterDirection.Input;
                SqlParameter submissionkey = update.Parameters.Add("@SUBMISSIONKEY", SqlDbType.VarChar);
                xmlin.Direction = ParameterDirection.Input;
                masterou.Value = emp.MASTER_OU;//ds.Tables[0].Rows[i].ItemArray[0];
                xmlin.Value = emp.XML_FILE_RESPONSE; //xmlcontents;
                guidin.Value = emp.GUID; //ds.Tables[0].Rows[i].ItemArray[2];
                if (emp.ERROR_CODE == 0)
                {
                    status.Value = "PROCESSED";
                }
                else
                {
                    status.Value = "ERROR";
                }
                gatwayid.Value = emp.Gatewayid; //gateway;
                submissionkey.Value = emp.SubmissionkeyId; //subkey;
                errorcode.Value = emp.ERROR_CODE; //errstatus;
                errordesc.Value = emp.ERROR_DESCRIPTION;
                con.Open();
                update.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                con.Close();
            }
            return 0;
        }
        public void PayrollInsert(Payroll PR)
        {
            SqlConnection con = new SqlConnection(_connectionString);

            SqlCommand eiretriveupdate = new SqlCommand("NZ_EI_RETRIEVE_RETURN_UPDATE", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter mstrou = eiretriveupdate.Parameters.Add("@MASTEROU", SqlDbType.Int);
            SqlParameter eiguid = eiretriveupdate.Parameters.Add("@GUID", SqlDbType.VarChar, 40);
            SqlParameter u_reqid = eiretriveupdate.Parameters.Add("@REQUEST_ID", SqlDbType.VarChar, 40);
            SqlParameter u_reqstatus = eiretriveupdate.Parameters.Add("@REQ_STATUS", SqlDbType.VarChar, 40);
            SqlParameter u_reqdt = eiretriveupdate.Parameters.Add("@REQ_DATE", SqlDbType.DateTime);
            SqlParameter u_skey = eiretriveupdate.Parameters.Add("@SUBMISSION_KEY", SqlDbType.VarChar, 40);
            SqlParameter u_lno = eiretriveupdate.Parameters.Add("@LINE_NUMBER", SqlDbType.VarChar, 10);
            SqlParameter u_empirdno = eiretriveupdate.Parameters.Add("@EMPLOYEE_IRD_NUMBER", SqlDbType.VarChar, 60);
            SqlParameter u_empname = eiretriveupdate.Parameters.Add("@EMPLOYEE_NAME", SqlDbType.VarChar, 160);
            SqlParameter u_taxcode = eiretriveupdate.Parameters.Add("@TAX_CODE", SqlDbType.VarChar, 20);
            SqlParameter u_payprdstdt = eiretriveupdate.Parameters.Add("@PAY_PERIOD_START_DATE", SqlDbType.DateTime);
            SqlParameter u_payprdenddt = eiretriveupdate.Parameters.Add("@PAY_PERIOD_END_DATE", SqlDbType.DateTime);
            SqlParameter u_empstartdate = eiretriveupdate.Parameters.Add("@EMPLOYMENT_START_DATE", SqlDbType.DateTime);
            SqlParameter u_emppayfrq = eiretriveupdate.Parameters.Add("@EMPLOYEE_PAY_FREQUENCY", SqlDbType.VarChar, 20);
            SqlParameter u_empgrossear = eiretriveupdate.Parameters.Add("@GROSS_EARNINGS", SqlDbType.Money);
            SqlParameter u_empearacc = eiretriveupdate.Parameters.Add("@EARNINGS_NOT_LIABLE_ACC", SqlDbType.Money);
            SqlParameter u_emplumind = eiretriveupdate.Parameters.Add("@LUMPSUM_INDICATOR", SqlDbType.VarChar, 10);
            SqlParameter emppayschded = eiretriveupdate.Parameters.Add("@PAYE_SCHEDULAR_TAX_DEDUCTIONS", SqlDbType.Money);
            SqlParameter empchildsupcode = eiretriveupdate.Parameters.Add("@CHILD_SUPPORT_CODE", SqlDbType.VarChar, 40);
            SqlParameter empchildsupded = eiretriveupdate.Parameters.Add("@CHILD_SUPPORT_DEDUCTIONS", SqlDbType.Money);
            SqlParameter empstudloanded = eiretriveupdate.Parameters.Add("@STUDENT_LOAN_DEDUCTIONS", SqlDbType.Money);
            SqlParameter empkiwisavercont = eiretriveupdate.Parameters.Add("@KIWI_SAVER_EMPLOYER_CONTRIBUTIONS", SqlDbType.Money);
            SqlParameter empkiwisaverded = eiretriveupdate.Parameters.Add("@KIWI_SAVER_DEDUCTIONS", SqlDbType.Money);
            SqlParameter emptaxcrddon = eiretriveupdate.Parameters.Add("@TAX_CREDIT_PAYROLL_DONATIONS", SqlDbType.Money);
            SqlParameter empesctded = eiretriveupdate.Parameters.Add("@ESCT_DEDUCTED", SqlDbType.Money);
            SqlParameter empfamtaxcred = eiretriveupdate.Parameters.Add("@Family_Tax_Credits", SqlDbType.Money);
            SqlParameter empcreatedby = eiretriveupdate.Parameters.Add("@created_by", SqlDbType.VarChar, 20);
            SqlParameter empmodifiedby = eiretriveupdate.Parameters.Add("@modified_by", SqlDbType.VarChar, 20);

            mstrou.Value = PR.MASTEROU;
            eiguid.Value = PR.GUID;
            u_reqid.Value = PR.REQUEST_ID;
            u_reqstatus.Value = "PROCESSSED";
            u_reqdt.Value = PR.REQ_DATE;
            u_skey.Value = PR.SUBMISSION_KEY;
            u_lno.Value = PR.LINE_NUMBER;
            u_empirdno.Value = PR.EMPLOYEE_IRD_NUMBER;
            u_empname.Value = PR.EMPLOYEE_NAME;
            u_taxcode.Value = PR.TAX_CODE;
            u_payprdstdt.Value = PR.PAY_PERIOD_START_DATE;
            u_payprdenddt.Value = PR.PAY_PERIOD_END_DATE;
            u_empstartdate.Value = PR.EMPLOYMENT_START_DAT;
            u_emppayfrq.Value = PR.EMPLOYEE_PAY_FREQUENCY;
            u_empgrossear.Value = PR.GROSS_EARNINGS;
            u_empearacc.Value = PR.EARNINGS_NOT_LIABLE_ACC;
            u_emplumind.Value = PR.LUMPSUM_INDICATOR;
            emppayschded.Value = PR.PAYE_SCHEDULAR_TAX_DEDUCTIONS;
            empchildsupcode.Value = PR.CHILD_SUPPORT_CODE;
            empchildsupded.Value = PR.CHILD_SUPPORT_DEDUCTIONS;
            empstudloanded.Value = PR.STUDENT_LOAN_DEDUCTIONS;
            empkiwisavercont.Value = PR.KIWI_SAVER_EMPLOYER_CONTRIBUTIONS;
            empkiwisaverded.Value = PR.KIWI_SAVER_DEDUCTIONS;
            emptaxcrddon.Value = PR.TAX_CREDIT_PAYROLL_DONATIONS;
            empesctded.Value = PR.ESCT_DEDUCTED;
            empfamtaxcred.Value = PR.Family_Tax_Credits;
            empcreatedby.Value = "SYSTEM";
            empmodifiedby.Value = "SYSTEM";
            con.Open();
            eiretriveupdate.ExecuteNonQuery();
            con.Close();
        }

        public void PayrollUpdate(Employee emp)
        {
            SqlConnection con = new SqlConnection(_connectionString);

            SqlCommand update = new SqlCommand("NZ_RESPONSE_UPDATE", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter masterou = update.Parameters.Add("@MASTEROU", SqlDbType.Int);
            masterou.Direction = ParameterDirection.Input;
            SqlParameter guidin = update.Parameters.Add("@GUID", SqlDbType.VarChar, 40);
            guidin.Direction = ParameterDirection.Input;
            SqlParameter xmlin = update.Parameters.Add("@XMLFILERESPONSE", SqlDbType.Xml);
            xmlin.Direction = ParameterDirection.Input;
            SqlParameter status = update.Parameters.Add("@STATUS", SqlDbType.VarChar, 40);
            xmlin.Direction = ParameterDirection.Input;
            SqlParameter errorcode = update.Parameters.Add("@ERROR_CODE", SqlDbType.Int);
            xmlin.Direction = ParameterDirection.Input;
            SqlParameter errordesc = update.Parameters.Add("@ERROR_DESCRIPTION", SqlDbType.VarChar);
            xmlin.Direction = ParameterDirection.Input;
            masterou.Value = emp.MASTER_OU;
            xmlin.Value = emp.XML_FILE_RESPONSE;
            guidin.Value = emp.GUID;
            if (emp.ERROR_CODE == 0)
            {
                status.Value = "PROCESSED";
            }
            else
            {
                status.Value = "ERROR";
            }
            errorcode.Value = emp.ERROR_CODE;
            errordesc.Value = emp.ERROR_DESCRIPTION;
            con.Open();
            update.ExecuteNonQuery();
            con.Close();
        }

        public void PrePopInsert(PAYROLL_PREPOP PR)
        {
            SqlConnection con = new SqlConnection(_connectionString);

            SqlCommand eiretriveupdate = new SqlCommand("NZ_EI_PRE_POP_INSERT", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter P_mstrou = eiretriveupdate.Parameters.Add("@MASTEROU", SqlDbType.Int);
            //SqlParameter P_PAY_DAY_DATE = eiretriveupdate.Parameters.Add("@PAY_DAY_DATE", SqlDbType.DateTime);
            SqlParameter P_REQUEST_ID = eiretriveupdate.Parameters.Add("@REQUEST_ID", SqlDbType.VarChar, 40);
            SqlParameter P_REQ_STATUS = eiretriveupdate.Parameters.Add("@REQ_STATUS", SqlDbType.VarChar, 40);
            SqlParameter P_REQ_DATE = eiretriveupdate.Parameters.Add("@REQ_DATE", SqlDbType.DateTime);
            SqlParameter P_EMPLOYEE_IRD_NUMBER = eiretriveupdate.Parameters.Add("@EMPLOYEE_IRD_NUMBER", SqlDbType.VarChar, 60);
            SqlParameter P_EMPLOYEE_NAME = eiretriveupdate.Parameters.Add("@EMPLOYEE_NAME", SqlDbType.VarChar, 160);
            SqlParameter P_TAX_CODE = eiretriveupdate.Parameters.Add("@TAX_CODE", SqlDbType.VarChar, 20);
            //SqlParameter P_EMPLOYMENT_START_DATE = eiretriveupdate.Parameters.Add("@EMPLOYMENT_START_DATE", SqlDbType.DateTime);
            //SqlParameter P_EMPLOYMENT_END_DATE = eiretriveupdate.Parameters.Add("@EMPLOYMENT_END_DATE", SqlDbType.DateTime);
            //SqlParameter P_PAY_PERIOD_END_DATE = eiretriveupdate.Parameters.Add("@PAY_PERIOD_END_DATE", SqlDbType.DateTime);
            SqlParameter P_FORM_TYPE = eiretriveupdate.Parameters.Add("@FORM_TYPE", SqlDbType.VarChar, 40);
            SqlParameter P_SUBMISSION_KEY = eiretriveupdate.Parameters.Add("@SUBMISSION_KEY", SqlDbType.VarChar, 40);
            SqlParameter P_created_by = eiretriveupdate.Parameters.Add("@created_by", SqlDbType.VarChar, 20);
            SqlParameter P_modified_by = eiretriveupdate.Parameters.Add("@modified_by", SqlDbType.VarChar, 20);
            SqlParameter P_GUID = eiretriveupdate.Parameters.Add("@GUID", SqlDbType.VarChar, 40);
            //SqlParameter P_RETRIEVE_DATE = eiretriveupdate.Parameters.Add("@RETRIEVE_DATE", SqlDbType.DateTime);
            SqlParameter P_MOD_FLAG = eiretriveupdate.Parameters.Add("@MOD_FLAG", SqlDbType.VarChar, 20);

            P_mstrou.Value = PR.MASTEROU;
            //P_PAY_DAY_DATE.Value = PR.PAY_DAY_DATE;
            P_REQUEST_ID.Value = PR.REQUEST_ID;
            P_REQ_STATUS.Value = "PROCESSSED";
            P_REQ_DATE.Value = PR.REQ_DATE;
            P_EMPLOYEE_IRD_NUMBER.Value = PR.EMPLOYEE_IRD_NUMBER;
            P_EMPLOYEE_NAME.Value = PR.EMPLOYEE_NAME;
            P_TAX_CODE.Value = PR.TAX_CODE;
            //P_EMPLOYMENT_START_DATE.Value = PR.EMPLOYMENT_START_DATE;
            //P_EMPLOYMENT_END_DATE.Value = PR.EMPLOYMENT_END_DATE;
            //P_PAY_PERIOD_END_DATE.Value = PR.PAY_PERIOD_END_DATE;
            P_FORM_TYPE.Value = PR.FORM_TYPE;
            P_SUBMISSION_KEY.Value = PR.SUBMISSION_KEY;
            P_created_by.Value = PR.created_by;
            P_modified_by.Value = PR.modified_by;
            P_GUID.Value = PR.GUID;
            //P_RETRIEVE_DATE.Value = PR.RETRIEVE_DATE;
            P_MOD_FLAG.Value = PR.MOD_FLAG;

            con.Open();
            eiretriveupdate.ExecuteNonQuery();
            con.Close();
        }

        public void PrePopUpdate(Employee emp)
        {
            SqlConnection con = new SqlConnection(_connectionString);

            SqlCommand update = new SqlCommand("NZ_PRE_POP_RESPONSE_UPDATE", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter masterou = update.Parameters.Add("@MASTEROU", SqlDbType.Int);
            masterou.Direction = ParameterDirection.Input;
            SqlParameter guidin = update.Parameters.Add("@GUID", SqlDbType.VarChar, 40);
            guidin.Direction = ParameterDirection.Input;
            //SqlParameter xmlin = update.Parameters.Add("@XMLFILERESPONSE", SqlDbType.Xml);
            //xmlin.Direction = ParameterDirection.Input;
            SqlParameter status = update.Parameters.Add("@STATUS", SqlDbType.VarChar, 40);
            status.Direction = ParameterDirection.Input;
            //SqlParameter errorcode = update.Parameters.Add("@ERROR_CODE", SqlDbType.Int);
            //xmlin.Direction = ParameterDirection.Input;
            //SqlParameter errordesc = update.Parameters.Add("@ERROR_DESCRIPTION", SqlDbType.VarChar);
            //xmlin.Direction = ParameterDirection.Input;
            masterou.Value = emp.MASTER_OU;
            //xmlin.Value = emp.XML_FILE_RESPONSE;
            guidin.Value = emp.GUID;
            if (emp.ERROR_CODE == 0)
            {
                status.Value = "PROCESSED";
            }
            else
            {
                status.Value = "ERROR";
            }
            //errorcode.Value = emp.ERROR_CODE;
            //errordesc.Value = emp.ERROR_DESCRIPTION;
            con.Open();
            update.ExecuteNonQuery();
            con.Close();
        }

        public void ReturnStatusRequestUpdate(Employee emp)
        {
            SqlConnection con = new SqlConnection(_connectionString);

            SqlCommand update = new SqlCommand("NZ_RETURNSTATUS_REQUEST_UPDATE", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            SqlParameter masterou = update.Parameters.Add("@MASTEROU", SqlDbType.Int);
            masterou.Direction = ParameterDirection.Input;
            SqlParameter guidin = update.Parameters.Add("@GUID", SqlDbType.VarChar, 40);
            guidin.Direction = ParameterDirection.Input;
            SqlParameter status = update.Parameters.Add("@STATUS", SqlDbType.VarChar, 40);
            status.Direction = ParameterDirection.Input;
            masterou.Value = emp.MASTER_OU;
            guidin.Value = emp.GUID;
            status.Value = emp.EMPLOYEE_STATUS;

            con.Open();
            update.ExecuteNonQuery();
            con.Close();
        }

        public string SQLInstance_Insert(InstanceDetails ID)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {

                SqlCommand _InstanceInsert = new SqlCommand("NZ_HRM_SQLINSTANCE_CRUD_SP", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlParameter _Customer = _InstanceInsert.Parameters.Add("@Customer", SqlDbType.VarChar);
                SqlParameter _Vendorid = _InstanceInsert.Parameters.Add("@VendorID", SqlDbType.VarChar);
                SqlParameter _SQLInstance = _InstanceInsert.Parameters.Add("@SQLInstance", SqlDbType.VarChar);
                SqlParameter _HCM_Database = _InstanceInsert.Parameters.Add("@HCM_Database", SqlDbType.VarChar);
                SqlParameter _WindowsAuthentication = _InstanceInsert.Parameters.Add("@WindowsAuthentication", SqlDbType.VarChar);
                SqlParameter _HCM_Username = _InstanceInsert.Parameters.Add("@HCM_Username", SqlDbType.VarChar);
                SqlParameter _HCM_Password = _InstanceInsert.Parameters.Add("@HCM_Password", SqlDbType.VarChar);
                SqlParameter _RedirectURL = _InstanceInsert.Parameters.Add("@RedirectURL", SqlDbType.VarChar);
                SqlParameter _CertificatePath = _InstanceInsert.Parameters.Add("@CertificatePath", SqlDbType.VarChar);
                SqlParameter _SecretCode = _InstanceInsert.Parameters.Add("@SecretCode", SqlDbType.VarChar);
                SqlParameter _TimeDelay = _InstanceInsert.Parameters.Add("@TimeDelay", SqlDbType.Int);
                SqlParameter _Status = _InstanceInsert.Parameters.Add("@Status", SqlDbType.VarChar);
                SqlParameter _Emp_Username = _InstanceInsert.Parameters.Add("@Emp_Username", SqlDbType.VarChar);
                SqlParameter _Emp_Password = _InstanceInsert.Parameters.Add("@Emp_Password", SqlDbType.VarChar);
                SqlParameter _Pay_Username = _InstanceInsert.Parameters.Add("@Pay_Username", SqlDbType.VarChar);
                SqlParameter _Pay_Password = _InstanceInsert.Parameters.Add("@Pay_Password", SqlDbType.VarChar);
                SqlParameter _createdby = _InstanceInsert.Parameters.Add("@createdby", SqlDbType.VarChar);
                SqlParameter _Action = _InstanceInsert.Parameters.Add("@Action", SqlDbType.VarChar);
                SqlParameter _HCM_ID = _InstanceInsert.Parameters.Add("@HCM_ID", SqlDbType.Int);
                _HCM_ID.Direction = ParameterDirection.Output;
                SqlParameter _ID = _InstanceInsert.Parameters.Add("@ID", SqlDbType.Int);

                _ID.Value = 0;
                _Customer.Value = ID.Customer;
                _Vendorid.Value = ID.VendorID;
                _SQLInstance.Value = ID.SQLInstance;
                _HCM_Database.Value = ID.Database;
                _WindowsAuthentication.Value = ID.WindowsAuthentication;
                _HCM_Username.Value = ID.Username;
                _HCM_Password.Value = ID.Password;
                _RedirectURL.Value = ID.RedirectURL;
                _CertificatePath.Value = ID.CertificatePath;
                _SecretCode.Value = ID.SecretCode;
                _TimeDelay.Value = ID.TimeDelay;
                _Status.Value = ID.Status;
                _Emp_Username.Value = ID.Employee_UserName;
                _Emp_Password.Value = ID.Employee_Pwd;
                _Pay_Username.Value = ID.Payroll_UserName;
                _Pay_Password.Value = ID.Payroll_Password;
                _createdby.Value = "User";
                _Action.Value = "INSERT";

                con.Open();
                _InstanceInsert.ExecuteNonQuery();

                con.Close();
                return _HCM_ID.Value.ToString();
            }
            catch (Exception ex)
            {
                con.Close();
                return "Error";
            }
        }

        public string SQLInstance_Update(InstanceDetails ID)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {

                SqlCommand _InstanceInsert = new SqlCommand("NZ_HRM_SQLINSTANCE_CRUD_SP", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlParameter _Customer = _InstanceInsert.Parameters.Add("@Customer", SqlDbType.VarChar);
                SqlParameter _Vendorid = _InstanceInsert.Parameters.Add("@VendorID", SqlDbType.VarChar);
                SqlParameter _SQLInstance = _InstanceInsert.Parameters.Add("@SQLInstance", SqlDbType.VarChar);
                SqlParameter _HCM_Database = _InstanceInsert.Parameters.Add("@HCM_Database", SqlDbType.VarChar);
                SqlParameter _WindowsAuthentication = _InstanceInsert.Parameters.Add("@WindowsAuthentication", SqlDbType.VarChar);
                SqlParameter _HCM_Username = _InstanceInsert.Parameters.Add("@HCM_Username", SqlDbType.VarChar);
                SqlParameter _HCM_Password = _InstanceInsert.Parameters.Add("@HCM_Password", SqlDbType.VarChar);
                SqlParameter _RedirectURL = _InstanceInsert.Parameters.Add("@RedirectURL", SqlDbType.VarChar);
                SqlParameter _CertificatePath = _InstanceInsert.Parameters.Add("@CertificatePath", SqlDbType.VarChar);
                SqlParameter _SecretCode = _InstanceInsert.Parameters.Add("@SecretCode", SqlDbType.VarChar);
                SqlParameter _TimeDelay = _InstanceInsert.Parameters.Add("@TimeDelay", SqlDbType.Int);
                SqlParameter _Status = _InstanceInsert.Parameters.Add("@Status", SqlDbType.VarChar);
                SqlParameter _Emp_Username = _InstanceInsert.Parameters.Add("@Emp_Username", SqlDbType.VarChar);
                SqlParameter _Emp_Password = _InstanceInsert.Parameters.Add("@Emp_Password", SqlDbType.VarChar);
                SqlParameter _Pay_Username = _InstanceInsert.Parameters.Add("@Pay_Username", SqlDbType.VarChar);
                SqlParameter _Pay_Password = _InstanceInsert.Parameters.Add("@Pay_Password", SqlDbType.VarChar);
                SqlParameter _createdby = _InstanceInsert.Parameters.Add("@createdby", SqlDbType.VarChar);
                SqlParameter _Action = _InstanceInsert.Parameters.Add("@Action", SqlDbType.VarChar);
                SqlParameter _HCM_ID = _InstanceInsert.Parameters.Add("@HCM_ID", SqlDbType.Int);
                _HCM_ID.Direction = ParameterDirection.Output;
                SqlParameter _ID = _InstanceInsert.Parameters.Add("@ID", SqlDbType.Int);


                _Customer.Value = ID.Customer;
                _Vendorid.Value = ID.VendorID;
                _SQLInstance.Value = ID.SQLInstance;
                _HCM_Database.Value = ID.Database;
                _WindowsAuthentication.Value = ID.WindowsAuthentication;
                _HCM_Username.Value = ID.Username;
                _HCM_Password.Value = ID.Password;
                _RedirectURL.Value = ID.RedirectURL;
                _CertificatePath.Value = ID.CertificatePath;
                _SecretCode.Value = ID.SecretCode;
                _TimeDelay.Value = ID.TimeDelay;
                _Status.Value = ID.Status;
                _Emp_Username.Value = ID.Employee_UserName;
                _Emp_Password.Value = ID.Employee_Pwd;
                _Pay_Username.Value = ID.Payroll_UserName;
                _Pay_Password.Value = ID.Payroll_Password;
                _createdby.Value = "User";
                _Action.Value = "UPDATE";
                _HCM_ID.Value = Convert.ToInt32(ID.HCM_id);
                _ID.Value = Convert.ToInt32(ID.HCM_id);
                con.Open();
                int rowsAffected = _InstanceInsert.ExecuteNonQuery();

                con.Close();
                return _HCM_ID.Value.ToString();
            }
            catch (Exception ex)
            {
                con.Close();
                return "Error";
            }
        }

        public DataSet SQLInstance_SelectDuplicate(InstanceDetails ID)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {
                SqlCommand _InstanceSelectDup = new SqlCommand("HRM_SqlInstance_SelectDuplicate", con)
                {
                    CommandType = CommandType.StoredProcedure
                };


                SqlParameter _Customer = _InstanceSelectDup.Parameters.Add("@Customer", SqlDbType.VarChar);
                SqlParameter _Vendorid = _InstanceSelectDup.Parameters.Add("@VendorID", SqlDbType.VarChar);
                _Customer.Value = ID.Customer;
                _Vendorid.Value = ID.VendorID;

                SqlDataAdapter da = new SqlDataAdapter(_InstanceSelectDup);

                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                con.Close();
                return null;
            }
        }
        public DataSet SQLInstance_Select()
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {

                SqlCommand _InstanceSelectDup = new SqlCommand("HRM_SqlInstance_Select", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataAdapter da = new SqlDataAdapter(_InstanceSelectDup);

                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;

            }
            catch (Exception ex)
            {
                con.Close();
                return null;
            }
        }
        public DataSet SQLInstance_SelectbyID(int id)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {

                SqlCommand _InstanceSelectDup = new SqlCommand("NZ_HRM_SqlInstance_SelectbyID_SP", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlParameter _HCMID = _InstanceSelectDup.Parameters.Add("@HCM_id", SqlDbType.Int);
                _HCMID.Value = id;
                SqlDataAdapter da = new SqlDataAdapter(_InstanceSelectDup);

                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;

            }
            catch (Exception ex)
            {
                con.Close();
                return null;
            }
        }
        public string SQLInstance_Delete(int HCMID)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            try
            {

                SqlCommand _Instancedel = new SqlCommand("HRM_SqlInstance_Delete", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlParameter _ID = _Instancedel.Parameters.Add("@ID", SqlDbType.Int);

                _ID.Value = HCMID;
                con.Open();
                int ROWAFFECTED = _Instancedel.ExecuteNonQuery();
                con.Close();
                return "Deleted";
            }
            catch (Exception ex)
            {
                con.Close();
                return "Error";
            }
        }
        public DataSet GetStatusEmployees(string Category)
        {
            SqlConnection con = new SqlConnection(_connectionString);
            SqlCommand select = new SqlCommand("SELECT *  from NZ_JOB_XML_TBL WHERE EMPLOYEE_STATUS = 'INITIATE' AND CATEGORY = '" + Category + "'", con);
            SqlDataAdapter da = new SqlDataAdapter(select);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
    }
}
