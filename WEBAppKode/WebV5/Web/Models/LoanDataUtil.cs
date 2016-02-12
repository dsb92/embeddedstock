using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace Web.Models
{
    public class LoanDataUtil
    {
        private LoanInformation _loanInformation;
        private readonly SqlConnection _conn;

        # region functions
        public LoanDataUtil()
        {
            // Instantiate the connection

            _conn = new SqlConnection(@"Data Source=10.29.0.29;Initial Catalog=F14I4SemProj4Gr3;User ID=F14I4SemProj4Gr3; Password=F14I4SemProj4Gr3");
            // Virker: Data Source=(localdb)\Projects;Initial Catalog=Opgave1;Integrated Security=True
            //new SqlConnection("Data Source=(localdb)\\Projects;Initial Catalog=Opgave1;Integrated Security=True")
            //"Data Source=(local);Initial Catalog=Northwind;Integrated Security=SSPI");
            //"Data Source=webhotel10.iha.dk;Initial Catalog=F14I4DABH0Gr16;User ID=F14I4DABH0Gr16; Password=F14I4DABH0Gr16");
        }

        public bool CreateLoanInformation(LoanInformation loan, Component c)
        {
            try
            {
                // Open the connection
                _conn.Open();

                // prepare command string using paramters in string and returning the given identity

                string insertStringParam =
                    @"INSERT INTO [LoanInformation] (Component, AdminComment, UserComment, LoanDate, ReturnDate, IsEmailSend)
                                                    OUTPUT INSERTED.LoanID
                                                    VALUES (@Data1, @Data2,@Data3,@Data4,@Data5,@Data6)";


                object adminCommentParam = DBNull.Value;
                object userCommentParam = DBNull.Value;
                object loanDateParam = DBNull.Value;
                object returnDateParam = DBNull.Value;
                object isemailsend = DBNull.Value;

                loan.Component = c.ComponentID;

                using (SqlCommand cmd = new SqlCommand(insertStringParam, _conn))
                {
                    // Get your parameters ready 
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data1";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data2";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data3";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data4";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data5";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data6";

                    cmd.Parameters["@Data1"].Value = loan.Component;
                    cmd.Parameters["@Data2"].Value = loan.AdminComment ?? adminCommentParam;
                    cmd.Parameters["@Data3"].Value = loan.UserComment ?? userCommentParam;
                    cmd.Parameters["@Data4"].Value = loan.LoanDate != DateTime.MinValue ? loan.LoanDate : loanDateParam;
                    cmd.Parameters["@Data5"].Value = loan.ReturnDate != DateTime.MinValue ? loan.ReturnDate : returnDateParam;
                    cmd.Parameters["@Data6"].Value = loan.IsEmailSend ?? isemailsend; 

                    //var id 
                    loan.LoanID = (int) cmd.ExecuteScalar(); //Returns the identity of the new tuple/record

                    //hv.HID = (int)cmd.ExecuteNonQuery(); //Does not workReturns row affected and not the identity of the new tuple/record

                    CurrentLoanInformation = loan;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            finally
            {
                // Close the connection
                if (_conn != null)
                {
                    _conn.Close();
                }
            }
            return true;
        }

        public bool UpdateLoanInformation(int id, LoanInformation loan)
        {
            try
            {
                CurrentLoanInformationHelper(id);

                // Open the connection
                _conn.Open();

                // prepare command string
                string updateString =
                    @"UPDATE LoanInformation
                        SET Component= @Data2, AdminComment = @Data3, UserComment = @Data4, LoanDate = @Data5, ReturnDate = @Data6, IsEmailSend = @Data7
                        WHERE LoanID = @Data1";

                using (SqlCommand cmd = new SqlCommand(updateString, _conn))
                {
                    // Get your parameters ready 
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data1";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data2";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data3";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data4";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data5";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data6";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data7";

                    // Hvis man ikke ?dre p?en attribut, s?s?tes den lig med CurrentInformation attribut.
                    cmd.Parameters["@Data1"].Value = loan.LoanID != 0 ? loan.LoanID : CurrentLoanInformation.LoanID;
                    cmd.Parameters["@Data2"].Value = loan.Component != 0 ? loan.Component : CurrentLoanInformation.Component;
                    cmd.Parameters["@Data3"].Value = loan.AdminComment ?? CurrentLoanInformation.AdminComment;
                    cmd.Parameters["@Data4"].Value = loan.UserComment ?? CurrentLoanInformation.UserComment;
                    cmd.Parameters["@Data5"].Value = loan.LoanDate != DateTime.MinValue ? loan.LoanDate : CurrentLoanInformation.LoanDate;
                    cmd.Parameters["@Data6"].Value = loan.ReturnDate != DateTime.MinValue ? loan.ReturnDate : CurrentLoanInformation.ReturnDate;
                    cmd.Parameters["@Data7"].Value = loan.IsEmailSend ?? CurrentLoanInformation.IsEmailSend;

                    cmd.ExecuteNonQuery();
                }

                //// 1. Instantiate a new command with command text only
                //SqlCommand cmd = new SqlCommand(updateString);

                //// 2. Set the Connection property
                //cmd.Connection = conn;

                //// 3. Call ExecuteNonQuery to send command
                //cmd.ExecuteNonQuery();
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            finally
            {
                // Close the connection
                if (_conn != null)
                {
                    _conn.Close();
                }
            }

            return true;
        }

        public LoanInformation GetLoanInformation(Component cp)
        {
            SqlDataReader rdr = null;

            try
            {
                // Open the connection
                _conn.Open();

                // prepare command string using paramters in string and returning the given identity

                string insertStringParam =
                    @"SELECT * FROM LoanInformation WHERE (Component = @Data1)";

                SqlCommand cmd = new SqlCommand(insertStringParam, _conn);
                cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data1";
                cmd.Parameters["@Data1"].Value = cp.ComponentID;
                cmd.ExecuteNonQuery();

                using (rdr = cmd.ExecuteReader())
                {

                    while (rdr.Read())
                    {
                        var loanInfo = new LoanInformation();

                        loanInfo.LoanID = (int) rdr["LoanID"];

                        loanInfo.Component = (int) rdr["Component"];

                        if (rdr["AdminComment"] != DBNull.Value)
                            loanInfo.AdminComment = (string) rdr["AdminComment"];
                        else
                        {
                            loanInfo.AdminComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["UserComment"] != DBNull.Value)
                            loanInfo.UserComment = (string) rdr["UserComment"];
                        else
                        {
                            loanInfo.UserComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["LoanDate"] != DBNull.Value)
                            loanInfo.LoanDate = (DateTime) rdr["LoanDate"];
                        else
                        {
                            loanInfo.LoanDate = DateTime.MinValue;
                        }

                        if (rdr["ReturnDate"] != DBNull.Value)
                            loanInfo.ReturnDate = (DateTime) rdr["ReturnDate"];
                        else
                        {
                            loanInfo.ReturnDate = loanInfo.ReturnDate = DateTime.MinValue;
                        }

                        if (rdr["IsEmailSend"] != DBNull.Value)
                            loanInfo.IsEmailSend = (string)rdr["IsEmailSend"];
                        else
                        {
                            loanInfo.IsEmailSend = DBNull.Value.ToString(CultureInfo.InvariantCulture);

                        }

                        CurrentLoanInformation = loanInfo;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Close the connection
                if (_conn != null)
                {
                    _conn.Close();
                }
            }
            return CurrentLoanInformation;
        }

        public List<Component> GetLoanedComponents()
        {
            var compUtil = new ComponentDataUtil();

            var compList = new List<Component>();

            foreach (var comp in compUtil.GetAllComponents())
            {
                if (GetLoanInformation(comp).LoanDate != DateTime.MinValue &&
                    GetLoanInformation(comp).ReturnDate != DateTime.MinValue)
                {
                    compList.Add(comp);
                }
            }

            return compList;
        }

        //public List<Component> GetStudentIdComponent
        //{
        //    var compUtil = new ComponentDataUtil();
        //}

        
  
        # endregion

        # region Properties

        public LoanInformation CurrentLoanInformation
        {
            get { return _loanInformation; }
            set { _loanInformation = value; }
        }

        # endregion

        # region Helpers

        private void CurrentLoanInformationHelper(int id)
        {
            SqlDataReader rdr = null;

            try
            {
                // Open the connection
                _conn.Open();

                // 1. Instantiate a new command with a query and connection
                SqlCommand cmd = new SqlCommand("SELECT * FROM LoanInformation WHERE (Component ='" + id + "')", _conn);

                // 2. Call Execute reader to get query results
                rdr = cmd.ExecuteReader();

                // transfer data from result set to local model

                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0]);
                    CurrentLoanInformation = new LoanInformation();
                    CurrentLoanInformation.LoanID = (int)rdr["LoanID"];
                    CurrentLoanInformation.Component = (int)rdr["Component"];

                    if (rdr["AdminComment"] != DBNull.Value)
                        CurrentLoanInformation.AdminComment = (string)rdr["AdminComment"];
                    else
                    {
                        CurrentLoanInformation.AdminComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    if (rdr["UserComment"] != DBNull.Value)
                        CurrentLoanInformation.UserComment = (string)rdr["UserComment"];
                    else
                    {
                        CurrentLoanInformation.LoanDate = DateTime.MinValue;
                    }
                    if (rdr["LoanDate"] != DBNull.Value)
                        CurrentLoanInformation.LoanDate = (DateTime)rdr["LoanDate"];
                    else
                    {
                        CurrentLoanInformation.LoanDate = DateTime.MinValue;
                    }
                    if (rdr["ReturnDate"] != DBNull.Value)
                        CurrentLoanInformation.ReturnDate = (DateTime)rdr["ReturnDate"];
                    else
                    {
                        CurrentLoanInformation.ReturnDate = DateTime.MinValue;
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Error: LoanInformation with component ID " + id + " does not excist");
            }
            finally
            {
                // close the reader
                if (rdr != null)
                {
                    rdr.Close();
                }

                // Close the connection
                if (_conn != null)
                {
                    _conn.Close();
                }
            }
        }

        # endregion
    }
}
