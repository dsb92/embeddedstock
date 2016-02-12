using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PCApplikationMVVM;

namespace DataAccesLogicLib
{
    public class ComponentDataUtil
    {
        # region Global Functions

        public ComponentDataUtil()
        {
            // Instantiate the connection

            _conn = new SqlConnection(@"Data Source=10.29.0.29;Initial Catalog=F14I4SemProj4Gr3;User ID=F14I4SemProj4Gr3; Password=F14I4SemProj4Gr3");
            // Virker: Data Source=(localdb)\Projects;Initial Catalog=Opgave1;Integrated Security=True
            // new SqlConnection("Data Source=(localdb)\\Projects;Initial Catalog=Opgave1;Integrated Security=True")
            //"Data Source=(local);Initial Catalog=Northwind;Integrated Security=SSPI");
            //"Data Source=webhotel10.iha.dk;Initial Catalog=F14I4DABH0Gr16;User ID=F14I4DABH0Gr16; Password=F14I4DABH0Gr16");
        }

        public bool CreateComponent(Component cp)
        {
            try
            {
                // Open the connection
                _conn.Open();

                // Prepare command string using paramters in string and returning the given identity
                const string insertStringParam =
                    @"INSERT INTO [Component] (ComponentNumber, SerieNr, ComponentName, ComponentInfo, Category, Datasheet, Image, ManufacturerLink, AdminComment, UserComment)
                                                    OUTPUT INSERTED.ComponentID
                                                VALUES (@Data1, @Data2,@Data3,@Data4,@Data5,@Data6,@Data7,@Data8, @Data9, @Data10)";

                object componentInfoParam = DBNull.Value;
                object categoryParam = DBNull.Value;
                object datasheetParam = DBNull.Value;
                object imageParam = DBNull.Value;
                object adminParam = DBNull.Value;
                object userParam = DBNull.Value;
                object serieNrParam = DBNull.Value;
                object manufacturParam = DBNull.Value;

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
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data9";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data10";
                    cmd.Parameters["@Data1"].Value = cp.ComponentNumber;
                    cmd.Parameters["@Data2"].Value = cp.SerieNr ?? serieNrParam;
                    cmd.Parameters["@Data3"].Value = cp.ComponentName;
                    // Hvis parameterne er null, så brug DBNULL.Value.
                    cmd.Parameters["@Data4"].Value = cp.ComponentInfo ?? componentInfoParam;
                    cmd.Parameters["@Data5"].Value = cp.Category ?? categoryParam;
                    cmd.Parameters["@Data6"].Value = cp.Datasheet ?? datasheetParam;
                    cmd.Parameters["@Data7"].Value = cp.Image ?? imageParam;
                    cmd.Parameters["@Data8"].Value = cp.ManufacturerLink ?? manufacturParam;
                    cmd.Parameters["@Data9"].Value = cp.AdminComment ?? adminParam;
                    cmd.Parameters["@Data10"].Value = cp.UserComment ?? userParam;

                    //var id 
                    cp.ComponentID = (int) cmd.ExecuteScalar(); //Returns the identity of the new tuple/record

                    CurrentComponent = cp;
                    var loanInfo = new LoanInformation();
                    var loan = new LoanDataUtil();
                    loan.CreateLoanInformation(loanInfo, cp);
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

        public bool DeleteComponent(int id)
        {
            try
            {
                CurrentComponentHelper(id);

                // Open the connection
                _conn.Open();

                // prepare command string
                const string deleteString = @"DELETE FROM Component
                        WHERE (ComponentID = @Data1)";
                using (var cmd = new SqlCommand(deleteString, _conn))
                {
                    // Get your parameters ready 
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data1";
                    cmd.Parameters["@Data1"].Value = CurrentComponent.ComponentID;
                    cmd.ExecuteNonQuery();
                    CurrentComponent = null;
                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Component with " + id + " does not excist!");
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

        public List<Component> GetComponents(List<string> keywords)
        {
            var compList = new List<Component>();
            var allComponents = GetAllComponents();
            var allLoanInformation = new LoanDataUtil().GetAllLoanInformation();
            try
            {
                var c = keywords.Count();
                var key = new List<string>();
                var nkey = new List<string>();
                foreach (var keyword in keywords)
                {
                    key.Add(keyword.ToLower());
                    nkey.Add(keyword.ToLower());
                }
                foreach (var comp in allComponents)
                {
                    var compCur = new Component
                    {
                        ComponentInfo = comp.ComponentInfo.ToLower(),
                        ComponentName = comp.ComponentName.ToLower()
                    };

                    var owner = Convert.ToString(allLoanInformation[allComponents.IndexOf(comp)].OwnerID);
                    var reservation = Convert.ToString(allLoanInformation[allComponents.IndexOf(comp)].ReservationID);

                    var serieNr = Convert.ToString(comp.SerieNr);

                    for (var i = 0; i < keywords.Count; i++)
                    {
                        if (c > 1 && !keywords.Contains("+"))
                        {
                            var check = key[i] + @"\b";
                            var compInfo = Regex.IsMatch(compCur.ComponentInfo, check);
                            var compName = Regex.IsMatch(compCur.ComponentName, check);
                            if (compInfo || owner.Contains(key[i]) || reservation.Contains(key[i]) ||
                                compName ||
                                serieNr.Contains(key[i]))
                            {
                                if (!compList.Contains(comp))
                                    compList.Add(comp);
                            }
                        }
                        if (c == 1)
                        {
                            if (compCur.ComponentInfo.Contains(key[i]) || owner.Contains(key[i]) || reservation.Contains(key[i]) ||
                                compCur.ComponentName.Contains(key[i]) ||
                                serieNr.Contains(key[i]))
                            {
                                if (!compList.Contains(comp))
                                    compList.Add(comp);
                            }

                        }

                        if (keywords.Contains("+"))
                        {
                            nkey.Remove("+");

                            if (i < nkey.Count - 1)
                            {
                                if (compCur.ComponentInfo.Contains(nkey[i]) && compCur.ComponentInfo.Contains(nkey[i + 1])
                                    || owner.Contains(nkey[i]) && compCur.ComponentName.Contains(nkey[i+1])
                                    || owner.Contains(nkey[i+1]) && compCur.ComponentName.Contains(nkey[i])
                                    || reservation.Contains(nkey[i]) && compCur.ComponentName.Contains(nkey[i+1])
                                    || reservation.Contains(nkey[i+1]) && compCur.ComponentName.Contains(nkey[i])
                                    || compCur.ComponentName.Contains(nkey[i]) && compCur.ComponentName.Contains(nkey[i + 1])
                                    || serieNr.Contains(nkey[i]))
                                {
                                    if (!compList.Contains(comp))
                                        compList.Add(comp);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return compList;
        }

        public List<Component> GetAllComponents()
        {
            var compList = new List<Component>();
            SqlDataReader rdr = null;
            try
            {
                // Open the connection
                _conn.Open();

                // prepare command string using paramters in string and returning the given identity

                const string insertStringParam = @"SELECT * FROM Component";
                //Alternative// SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]

                var oCmd = new SqlCommand(insertStringParam, _conn);

                using (rdr = oCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var newComp = new Component
                        {
                            ComponentID = (int) rdr["ComponentID"],
                            ComponentNumber = (int) rdr["ComponentNumber"],
                            ComponentName = (string) rdr["ComponentName"]
                        };

                        if (rdr["SerieNr"] != DBNull.Value)
                            newComp.SerieNr = (string)rdr["SerieNr"];
                        else
                        {
                            newComp.SerieNr = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["ComponentInfo"] != DBNull.Value)
                            newComp.ComponentInfo = (string) rdr["ComponentInfo"];
                        else
                        {
                            newComp.ComponentInfo = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["Category"] != DBNull.Value)
                            newComp.Category = (string) rdr["Category"];
                        else
                        {
                            newComp.Category = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["Datasheet"] != DBNull.Value)
                            newComp.Datasheet = (string) rdr["Datasheet"];
                        else
                        {
                            newComp.Datasheet = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["Image"] != DBNull.Value)
                            newComp.Image = (string) rdr["Image"];
                        else
                        {
                            newComp.Image = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["ManufacturerLink"] != DBNull.Value)
                            newComp.ManufacturerLink = (string) rdr["ManufacturerLink"];
                        else
                        {
                            newComp.ManufacturerLink = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }
                        if (rdr["AdminComment"] != DBNull.Value)
                            newComp.AdminComment = (string) rdr["AdminComment"];
                        else
                        {
                            newComp.AdminComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        if (rdr["UserComment"] != DBNull.Value)
                            newComp.UserComment = (string)rdr["UserComment"];
                        else
                        {
                            newComp.UserComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                        }

                        compList.Add(newComp);
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Could not get components:" + " " + e.Message);
                return null;
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
            return compList;
        }


        public bool UpdateComponent(int id, Component cp)
        {
            try
            {
                CurrentComponentHelper(id);
               
                // Open the connection
                _conn.Open();

                // prepare command string
                const string updateString = @"UPDATE Component
                        SET ComponentNumber= @Data2, SerieNr = @Data3, ComponentName = @Data4, ComponentInfo = @Data5, Category = @Data6, Datasheet = @Data7, Image = @Data8, ManufacturerLink = @Data9,  AdminComment = @Data10, UserComment = @Data11
                        WHERE ComponentID = @Data1";


                using (var cmd = new SqlCommand(updateString, _conn))
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
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data9";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data10";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data11";

                    // Hvis man ikke ændre på en attribut, så sættes den lig med CurrentComponents attribut.
                    cmd.Parameters["@Data1"].Value = cp.ComponentID != 0 ? cp.ComponentID : CurrentComponent.ComponentID;
                    cmd.Parameters["@Data2"].Value = cp.ComponentNumber != 0 ? cp.ComponentNumber : CurrentComponent.ComponentNumber;
                    cmd.Parameters["@Data3"].Value = cp.SerieNr ?? CurrentComponent.SerieNr;
                    cmd.Parameters["@Data4"].Value = cp.ComponentName ?? CurrentComponent.ComponentName;
                    cmd.Parameters["@Data5"].Value = cp.ComponentInfo ?? CurrentComponent.ComponentInfo;
                    cmd.Parameters["@Data6"].Value = cp.Category ?? CurrentComponent.Category;
                    cmd.Parameters["@Data7"].Value = cp.Datasheet ?? CurrentComponent.Datasheet;
                    cmd.Parameters["@Data8"].Value = cp.Image ?? CurrentComponent.Image;
                    cmd.Parameters["@Data9"].Value = cp.ManufacturerLink ?? CurrentComponent.ManufacturerLink;
                    cmd.Parameters["@Data10"].Value = cp.AdminComment ?? CurrentComponent.AdminComment;
                    cmd.Parameters["@Data11"].Value = cp.UserComment ?? CurrentComponent.UserComment;

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

        public List<Component> LoanTimeExceeded()
        {
            var compList = new List<Component>();

            try
            {
                var nlist = GetAllComponents();

                foreach (var comp in nlist)
                {
                    var loanDatautil = new LoanDataUtil();
                    var lf = loanDatautil.GetLoanInformation(comp);

                    if (lf.ReturnDate != null && lf.LoanDate != null && !(String.IsNullOrEmpty(lf.OwnerID)))
                    {
                        if (lf.ReturnDate.Value < DateTime.Now)
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

        public bool ComponentReservation(string cName, string studienumber)
        {
            try
            {
                var key = new List<string> {cName};
                foreach (var comp in GetComponents(key))
                {
                    var ldu = new LoanDataUtil();
                    LoanInformation loan = ldu.GetLoanInformation(comp);

                    string owner = Convert.ToString(loan.OwnerID);
                    string reservation = Convert.ToString(loan.ReservationID);

                    if (owner == "" && reservation == "")
                    {
                        loan.ReservationID = studienumber;
                        loan.ReservationDate = DateTime.Now;
                        ldu.UpdateLoanInformation(comp.ComponentID, loan);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        public void CheckReservations()
        {
            var ldu = new LoanDataUtil();
            var loans = ldu.GetAllLoanInformation();
            foreach (var loan in loans)
            {
                if (loan.ReservationDate != null)
                {
                    var dt = DateTime.Now - (DateTime?) loan.ReservationDate;

                    if (dt.Value.Days > 5)
                    {
                        loan.ReservationDate = new SqlDateTime();
                        loan.ReservationID = "";
                        ldu.UpdateLoanInformation(loan.Component, loan);
                    }
                }
            }
        }

        # endregion

        # region Private Variables

        private Component _locComponent;
        private readonly SqlConnection _conn;

        # endregion 

        # region Properties

        public Component CurrentComponent
        {
            get { return _locComponent; }
            set { _locComponent = value; }
        }

        # endregion

        # region Helpers

        private void CurrentComponentHelper(int id)
        {
            SqlDataReader rdr = null;

            try
            {
                // Open the connection
                _conn.Open();

                // 1. Instantiate a new command with a query and connection
                SqlCommand cmd = new SqlCommand("SELECT * FROM Component WHERE (ComponentID ='" + id + "')", _conn);

                // 2. Call Execute reader to get query results
                rdr = cmd.ExecuteReader();

                // transfer data from result set to local model

                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0]);
                    CurrentComponent = new Component();
                    CurrentComponent.ComponentID = (int)rdr["ComponentID"];
                    CurrentComponent.ComponentNumber = (int)rdr["ComponentNumber"];

                    if (rdr["SerieNr"] != DBNull.Value)
                        CurrentComponent.SerieNr = (string)rdr["SerieNr"];
                    else
                    {
                        CurrentComponent.SerieNr = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }

                    CurrentComponent.ComponentName = (string)rdr["ComponentName"];

                    if (rdr["ComponentInfo"] != DBNull.Value)
                        CurrentComponent.ComponentInfo = (string)rdr["ComponentInfo"];
                    else
                    {
                        CurrentComponent.ComponentInfo = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    if (rdr["Category"] != DBNull.Value)
                        CurrentComponent.Category = (string)rdr["Category"];
                    else
                    {
                        CurrentComponent.Category = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    if (rdr["Datasheet"] != DBNull.Value)
                        CurrentComponent.Datasheet = (string)rdr["Datasheet"];
                    else
                    {
                        CurrentComponent.Datasheet = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    if (rdr["Image"] != DBNull.Value)
                        CurrentComponent.Image = (string)rdr["Image"];
                    else
                    {
                        CurrentComponent.Image = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    if (rdr["ManufacturerLink"] != DBNull.Value)
                        CurrentComponent.ManufacturerLink = (string)rdr["ManufacturerLink"];
                    else
                    {
                        CurrentComponent.ManufacturerLink = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    if (rdr["AdminComment"] != DBNull.Value)
                        CurrentComponent.AdminComment = (string)rdr["AdminComment"];
                    else
                    {
                        CurrentComponent.AdminComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }

                    if (rdr["UserComment"] != DBNull.Value)
                        CurrentComponent.UserComment = (string)rdr["UserComment"];
                    else
                    {
                        CurrentComponent.UserComment = DBNull.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Error: Component with " + id + " does not excist");
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
