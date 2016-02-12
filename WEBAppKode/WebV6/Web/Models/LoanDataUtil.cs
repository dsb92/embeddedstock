using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;

namespace Web.Models
{
    public class LoanDataUtil
    {
        # region Functions
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

                const string insertStringParam = @"INSERT INTO [LoanInformation] (Component, LoanDate, ReturnDate, IsEmailSend, ReservationDate, MobilNr, OwnerID, ReservationID)
                                                    OUTPUT INSERTED.LoanID
                                                    VALUES (@Data1, @Data2,@Data3,@Data4,@Data5,@Data6,@Data7,@Data8)";

                object loanDateParam = DBNull.Value;
                object returnDateParam = DBNull.Value;
                object reservationDateParam = DBNull.Value;
                object isEmailsendParam = DBNull.Value;
                object mobilNrParam = DBNull.Value;
                object ownerIdParam = DBNull.Value;
                object resIdParam = DBNull.Value;

                loan.Component = c.ComponentID;

                using (var cmd = new SqlCommand(insertStringParam, _conn))
                {
                    // Get your parameters ready 
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data1";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data2";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data3";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data4";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data5";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data6";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data7";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data8";

                    cmd.Parameters["@Data1"].Value = loan.Component;
                    cmd.Parameters["@Data2"].Value = loan.LoanDate ?? loanDateParam;
                    cmd.Parameters["@Data3"].Value = loan.ReturnDate ?? returnDateParam;
                    cmd.Parameters["@Data4"].Value = loan.IsEmailSend ?? isEmailsendParam;
                    cmd.Parameters["@Data5"].Value = loan.ReservationDate ?? reservationDateParam;
                    cmd.Parameters["@Data6"].Value = loan.MobilNr ?? mobilNrParam;
                    cmd.Parameters["@Data7"].Value = loan.OwnerID ?? ownerIdParam;
                    cmd.Parameters["@Data8"].Value = loan.ReservationID ?? resIdParam;

                    //var id 
                    loan.LoanID = (int)cmd.ExecuteScalar(); //Returns the identity of the new tuple/record

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

        public bool UpdateLoanInformation(int compId, LoanInformation loan)
        {
            try
            {
                CurrentLoanInformationHelper(compId);

                // Open the connection
                _conn.Open();

                // prepare command string
                const string updateString = @"UPDATE LoanInformation
                        SET Component= @Data2, LoanDate = @Data3, ReturnDate = @Data4, IsEmailSend = @Data5, ReservationDate = @Data6, MobilNr = @Data7, OwnerID = @Data8, ReservationID = @Data9
                        WHERE LoanID = @Data1";

                using (var cmd = new SqlCommand(updateString, _conn))
                {
                    // Get your parameters ready 
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data1";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data2";
                    cmd.Parameters.Add("@Data3", SqlDbType.DateTime);
                    cmd.Parameters.Add("@Data4", SqlDbType.DateTime);
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data5";
                    cmd.Parameters.Add("@Data6", SqlDbType.DateTime);
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data7";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data8";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data9";

                    // Hvis man ikke ?dre p?en attribut, s?s?tes den lig med CurrentInformation attribut.
                    cmd.Parameters["@Data1"].Value = loan.LoanID != 0 ? loan.LoanID : CurrentLoanInformation.LoanID;
                    cmd.Parameters["@Data2"].Value = loan.Component != 0 ? loan.Component : CurrentLoanInformation.Component;
                    cmd.Parameters["@Data3"].Value = loan.LoanDate ?? CurrentLoanInformation.LoanDate;
                    cmd.Parameters["@Data4"].Value = loan.ReturnDate ?? CurrentLoanInformation.ReturnDate;
                    cmd.Parameters["@Data5"].Value = loan.IsEmailSend ?? CurrentLoanInformation.IsEmailSend;
                    cmd.Parameters["@Data6"].Value = loan.ReservationDate ?? CurrentLoanInformation.ReservationDate;
                    cmd.Parameters["@Data7"].Value = loan.MobilNr ?? CurrentLoanInformation.MobilNr;
                    cmd.Parameters["@Data8"].Value = loan.OwnerID ?? CurrentLoanInformation.OwnerID;
                    cmd.Parameters["@Data9"].Value = loan.ReservationID ?? CurrentLoanInformation.ReservationID;

                    cmd.ExecuteNonQuery();
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

        public LoanInformation GetLoanInformation(Component cp)
        {
            try
            {
                // Open the connection
                _conn.Open();

                // prepare command string using paramters in string and returning the given identity

                const string insertStringParam = @"SELECT * FROM LoanInformation WHERE (Component = @Data1)";

                SqlCommand cmd = new SqlCommand(insertStringParam, _conn);
                cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data1";
                cmd.Parameters["@Data1"].Value = cp.ComponentID;
                cmd.ExecuteNonQuery();

                SqlDataReader rdr;
                using (rdr = cmd.ExecuteReader())
                {
                    var loanInfo = new LoanInformation();

                    while (rdr.Read())
                    {
                        loanInfo.LoanID = (int)rdr["LoanID"];

                        loanInfo.Component = (int)rdr["Component"];

                        if (rdr["LoanDate"] != DBNull.Value)
                            loanInfo.LoanDate = (DateTime)rdr["LoanDate"];
                        else
                        {
                            loanInfo.LoanDate = null;
                        }

                        if (rdr["ReturnDate"] != DBNull.Value)
                            loanInfo.ReturnDate = (DateTime)rdr["ReturnDate"];
                        else
                        {
                            loanInfo.ReturnDate = null;
                        }

                        if (rdr["IsEmailSend"] != DBNull.Value)
                            loanInfo.IsEmailSend = (string)rdr["IsEmailSend"];
                        else
                        {
                            loanInfo.IsEmailSend = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["ReservationDate"] != DBNull.Value)
                            loanInfo.ReservationDate = (DateTime)rdr["ReservationDate"];
                        else
                        {
                            loanInfo.ReservationDate = null;
                        }

                        if (rdr["MobilNr"] != DBNull.Value)
                            loanInfo.MobilNr = (string)rdr["MobilNr"];
                        else
                        {
                            loanInfo.MobilNr = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["OwnerID"] != DBNull.Value)
                            loanInfo.OwnerID = (string)rdr["OwnerID"];
                        else
                        {
                            loanInfo.OwnerID = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["ReservationID"] != DBNull.Value)
                            loanInfo.ReservationID = (string)rdr["ReservationID"];
                        else
                        {
                            loanInfo.ReservationID = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                    }

                    CurrentLoanInformation = loanInfo;
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

        public List<LoanInformation> GetAllLoanInformation()
        {
            var loanList = new List<LoanInformation>();

            try
            {
                // Open the connection
                _conn.Open();

                const string insertStringParam = @"SELECT * FROM LoanInformation";

                SqlCommand cmd = new SqlCommand(insertStringParam, _conn);
                cmd.ExecuteNonQuery();

                SqlDataReader rdr;
                using (rdr = cmd.ExecuteReader())
                {
                    

                    while (rdr.Read())
                    {
                        var loanInfo = new LoanInformation();
                        loanInfo.LoanID = (int)rdr["LoanID"];

                        loanInfo.Component = (int)rdr["Component"];

                        if (rdr["LoanDate"] != DBNull.Value)
                            loanInfo.LoanDate = (DateTime)rdr["LoanDate"];
                        else
                        {
                            loanInfo.LoanDate = null;
                        }

                        if (rdr["ReturnDate"] != DBNull.Value)
                            loanInfo.ReturnDate = (DateTime)rdr["ReturnDate"];
                        else
                        {
                            loanInfo.ReturnDate = null;
                        }

                        if (rdr["IsEmailSend"] != DBNull.Value)
                            loanInfo.IsEmailSend = (string)rdr["IsEmailSend"];
                        else
                        {
                            loanInfo.IsEmailSend = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["ReservationDate"] != DBNull.Value)
                            loanInfo.ReservationDate = (DateTime)rdr["ReservationDate"];
                        else
                        {
                            loanInfo.ReservationDate = null;
                        }

                        if (rdr["MobilNr"] != DBNull.Value)
                            loanInfo.MobilNr = (string)rdr["MobilNr"];
                        else
                        {
                            loanInfo.MobilNr = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["OwnerID"] != DBNull.Value)
                            loanInfo.OwnerID = (string)rdr["OwnerID"];
                        else
                        {
                            loanInfo.OwnerID = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["ReservationID"] != DBNull.Value)
                            loanInfo.ReservationID = (string)rdr["ReservationID"];
                        else
                        {
                            loanInfo.ReservationID = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        loanList.Add(loanInfo);
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
            return loanList;
        }

        public List<Component> GetLoanedComponents()
        {
            var compList = new List<Component>();

            try
            {
                var compUtil = new ComponentDataUtil();

                foreach (var comp in compUtil.GetAllComponents())
                {
                    if (GetLoanInformation(comp).LoanDate != null
                        && !(String.IsNullOrEmpty(GetLoanInformation(comp).OwnerID)))
                    {
                        compList.Add(comp);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return compList;
        }

        public List<Component> GetLoanedComponentFromStudentId(string id)
        {
            var compList = new List<Component>();

            try
            {
                // Open the connection
                _conn.Open();

                // prepare command string using paramters in string and returning the given identity

                var cmd = new SqlCommand
                    ("SELECT dbo.Component.ComponentID, dbo.Component.ComponentNumber, dbo.Component.SerieNr, dbo.Component.ComponentName, " +
                     "dbo.Component.ComponentInfo, dbo.Component.Category, dbo.Component.Datasheet, " +
                     "dbo.Component.Image, dbo.Component.AdminComment, dbo.Component.UserComment, " +
                     "dbo.LoanInformation.OwnerID FROM dbo.Component INNER JOIN dbo.LoanInformation " +
                     "ON dbo.Component.ComponentID = dbo.LoanInformation.Component " +
                     "WHERE (dbo.[LoanInformation].OwnerID ='" + id + "')", _conn);

                SqlDataReader rdr;
                using (rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var comp = new Component();

                        comp.ComponentID = (int)rdr["ComponentID"];

                        comp.ComponentNumber = (int)rdr["ComponentNumber"];

                        if (rdr["SerieNr"] != DBNull.Value)
                            comp.SerieNr = (string)rdr["SerieNr"];
                        else
                        {
                            comp.SerieNr = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        comp.ComponentName = (string)rdr["ComponentName"];

                        if (rdr["ComponentInfo"] != DBNull.Value)
                            comp.ComponentInfo = (string)rdr["ComponentInfo"];
                        else
                        {
                            comp.ComponentInfo = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["Category"] != DBNull.Value)
                            comp.Category = (string)rdr["Category"];
                        else
                        {
                            comp.Category = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["Datasheet"] != DBNull.Value)
                            comp.Datasheet = (string)rdr["Datasheet"];
                        else
                        {
                            comp.Datasheet = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["Image"] != DBNull.Value)
                            comp.Image = (string)rdr["Image"];
                        else
                        {
                            comp.Image = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["AdminComment"] != DBNull.Value)
                            comp.AdminComment = (string)rdr["AdminComment"];
                        else
                        {
                            comp.AdminComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["UserComment"] != DBNull.Value)
                            comp.UserComment = (string)rdr["UserComment"];
                        else
                        {
                            comp.UserComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }


                        compList.Add(comp);
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

            return compList;
        }

        public List<Component> GetReservedComponentFromStudentId(string id)
        {
            var compList = new List<Component>();

            try
            {
                // Open the connection
                _conn.Open();

                // prepare command string using paramters in string and returning the given identity


                var cmd = new SqlCommand
                    ("SELECT dbo.Component.ComponentID, dbo.Component.ComponentNumber, dbo.Component.SerieNr, dbo.Component.ComponentName, " +
                     "dbo.Component.ComponentInfo, dbo.Component.Category, dbo.Component.Datasheet, " +
                     "dbo.Component.Image, dbo.Component.AdminComment, dbo.Component.UserComment, " +
                     "dbo.LoanInformation.OwnerID FROM dbo.Component INNER JOIN dbo.LoanInformation " +
                     "ON dbo.Component.ComponentID = dbo.LoanInformation.Component " +
                     "WHERE (dbo.[LoanInformation].ReservationID ='" + id + "')", _conn);

                SqlDataReader rdr;
                using (rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var comp = new Component();

                        comp.ComponentID = (int)rdr["ComponentID"];

                        comp.ComponentNumber = (int)rdr["ComponentNumber"];

                        if (rdr["SerieNr"] != DBNull.Value)
                            comp.SerieNr = (string)rdr["SerieNr"];
                        else
                        {
                            comp.SerieNr = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        comp.ComponentName = (string)rdr["ComponentName"];

                        if (rdr["ComponentInfo"] != DBNull.Value)
                            comp.ComponentInfo = (string)rdr["ComponentInfo"];
                        else
                        {
                            comp.ComponentInfo = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["Category"] != DBNull.Value)
                            comp.Category = (string)rdr["Category"];
                        else
                        {
                            comp.Category = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["Datasheet"] != DBNull.Value)
                            comp.Datasheet = (string)rdr["Datasheet"];
                        else
                        {
                            comp.Datasheet = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["Image"] != DBNull.Value)
                            comp.Image = (string)rdr["Image"];
                        else
                        {
                            comp.Image = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["AdminComment"] != DBNull.Value)
                            comp.AdminComment = (string)rdr["AdminComment"];
                        else
                        {
                            comp.AdminComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["UserComment"] != DBNull.Value)
                            comp.UserComment = (string)rdr["UserComment"];
                        else
                        {
                            comp.UserComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        compList.Add(comp);
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

            return compList;
        }

        public void RemoveReservation(int compId)
        {
            try
            {
                var loan = new LoanInformation { ReservationID = "", ReservationDate = new SqlDateTime() };
                UpdateLoanInformation(compId, loan);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        # endregion

        # region Private Variables

        private LoanInformation _loanInformation;
        private readonly SqlConnection _conn;

        # endregion

        # region Properties

        public LoanInformation CurrentLoanInformation
        {
            get { return _loanInformation; }
            set { _loanInformation = value; }
        }

        # endregion

        # region Helpers

        private void CurrentLoanInformationHelper(int compId)
        {
            SqlDataReader rdr = null;

            try
            {
                // Open the connection
                _conn.Open();

                // 1. Instantiate a new command with a query and connection
                SqlCommand cmd = new SqlCommand("SELECT * FROM LoanInformation WHERE (Component ='" + compId + "')", _conn);

                // 2. Call Execute reader to get query results
                rdr = cmd.ExecuteReader();

                // transfer data from result set to local model

                while (rdr.Read())
                {
                    //Console.WriteLine(rdr[0]);
                    CurrentLoanInformation = new LoanInformation();
                    CurrentLoanInformation.LoanID = (int)rdr["LoanID"];
                    CurrentLoanInformation.Component = (int)rdr["Component"];

                    if (rdr["LoanDate"] != DBNull.Value)
                        CurrentLoanInformation.LoanDate = (DateTime)rdr["LoanDate"];
                    else
                    {
                        CurrentLoanInformation.LoanDate = SqlDateTime.Null;
                    }

                    if (rdr["ReturnDate"] != DBNull.Value)
                        CurrentLoanInformation.ReturnDate = (DateTime)rdr["ReturnDate"];
                    else
                    {
                        CurrentLoanInformation.ReturnDate = SqlDateTime.Null;
                    }

                    if (rdr["IsEmailSend"] != DBNull.Value)
                        CurrentLoanInformation.IsEmailSend = (string)rdr["IsEmailSend"];
                    else
                    {
                        CurrentLoanInformation.IsEmailSend = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }

                    if (rdr["ReservationDate"] != DBNull.Value)
                        CurrentLoanInformation.ReservationDate = (DateTime)rdr["ReservationDate"];
                    else
                    {
                        CurrentLoanInformation.ReservationDate = SqlDateTime.Null;
                    }

                    if (rdr["MobilNr"] != DBNull.Value)
                        CurrentLoanInformation.MobilNr = (string)rdr["MobilNr"];
                    else
                    {
                        CurrentLoanInformation.MobilNr = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }

                    if (rdr["OwnerID"] != DBNull.Value)
                        CurrentLoanInformation.OwnerID = (string)rdr["OwnerID"];
                    else
                    {
                        CurrentLoanInformation.OwnerID = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }

                    if (rdr["ReservationID"] != DBNull.Value)
                        CurrentLoanInformation.ReservationID = (string)rdr["ReservationID"];
                    else
                    {
                        CurrentLoanInformation.ReservationID = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
