using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ComponentDataUtil
    {
        private Component _locComponent;
        private readonly SqlConnection _conn;
        public ComponentDataUtil()
        {
            // Instantiate the connection

            _conn = new SqlConnection(@"Data Source=10.29.0.29;Initial Catalog=F14I4SemProj4Gr3;User ID=F14I4SemProj4Gr3; Password=F14I4SemProj4Gr3");
            // Virker: Data Source=(localdb)\Projects;Initial Catalog=Opgave1;Integrated Security=True
            //new SqlConnection("Data Source=(localdb)\\Projects;Initial Catalog=Opgave1;Integrated Security=True")
            //"Data Source=(local);Initial Catalog=Northwind;Integrated Security=SSPI");
            //"Data Source=webhotel10.iha.dk;Initial Catalog=F14I4DABH0Gr16;User ID=F14I4DABH0Gr16; Password=F14I4DABH0Gr16");
        }

        // bool CreateComponent(Component cp)

        public bool CreateComponent(Component cp)
        {
            try
            {
                // Open the connection
                _conn.Open();

                // prepare command string using paramters in string and returning the given identity

                string insertStringParam =
                    @"INSERT INTO [Component] (ComponentNumber, SerieNr, ComponentName, ComponentInfo, Category, Datasheet, Image, OwnerID)
                                                    OUTPUT INSERTED.ComponentID
                                                VALUES (@Data1, @Data2,@Data3,@Data4,@Data5,@Data6,@Data7,@Data8)";
                //Alternative //    ; SELECT SCOPE_IDENTITY()";

                object componentInfoParam = DBNull.Value;
                object categoryParam = DBNull.Value;
                object datasheetParam = DBNull.Value;
                object imageParam = DBNull.Value;
                object ownerIdParam = DBNull.Value;
                object serieNrParam = DBNull.Value;

                using (SqlCommand cmd = new SqlCommand(insertStringParam, _conn))
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
                    cmd.Parameters["@Data1"].Value = cp.ComponentNumber;
                    cmd.Parameters["@Data2"].Value = cp.SerieNr ?? serieNrParam;
                    cmd.Parameters["@Data3"].Value = cp.ComponentName;
                    // Hvis parameterne er null, så brug DBNULL.Value.
                    cmd.Parameters["@Data4"].Value = cp.ComponentInfo ?? componentInfoParam;
                    cmd.Parameters["@Data5"].Value = cp.Category ?? categoryParam;
                    cmd.Parameters["@Data6"].Value = cp.Datasheet ?? datasheetParam;
                    cmd.Parameters["@Data7"].Value = cp.Image ?? imageParam;
                    cmd.Parameters["@Data8"].Value = cp.OwnerID != 0 ? cp.OwnerID : ownerIdParam;


                    //var id 
                    cp.ComponentID = (int) cmd.ExecuteScalar(); //Returns the identity of the new tuple/record

                    //hv.HID = (int)cmd.ExecuteNonQuery(); //Does not workReturns row affected and not the identity of the new tuple/record

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

        // bool DeleteComponent(int id)

        public bool DeleteComponent(int id)
        {
            try
            {
                CurrentComponentHelper(id);

                // Open the connection
                _conn.Open();

                

                // prepare command string
                string deleteString =
                    @"DELETE FROM Component
                        WHERE (ComponentID = @Data1)";
                using (SqlCommand cmd = new SqlCommand(deleteString, _conn))
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

        // List<Component> GetComponents(List<string> keywords)

        public List<Component> GetComponents(List<string> keywords)
        {
            var compList = new List<Component>();
            try
            {
                foreach (var comp in GetAllComponents())
                {
                    Component compCur = new Component();
                    compCur.ComponentInfo = comp.ComponentInfo.ToLower();
                    compCur.ComponentName = comp.ComponentName.ToLower();
                    foreach (var keyword in keywords)
                    {
                        string key = keyword.ToLower();
                        string owner = Convert.ToString(comp.OwnerID);
                        string serieNr = Convert.ToString(comp.SerieNr);
                        if (compCur.ComponentInfo.Contains(key) || owner.Contains(key) || compCur.ComponentName.Contains(key) ||
                            serieNr.Contains(key))
                        {
                            compList.Add(comp);
                        }

                        //if (compCur.ComponentInfo.Contains(key) && compCur.ComponentName.Contains(key))
                        //{
                        //    compList.Add(comp);
                        //}
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

                string insertStringParam =
                    @"SELECT * FROM Component";
                //Alternative// SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]

                SqlCommand oCmd = new SqlCommand(insertStringParam, _conn);

                using (rdr = oCmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var newComp = new Component();

                        newComp.ComponentID = (int) rdr["ComponentID"];
                        newComp.ComponentNumber = (int) rdr["ComponentNumber"];
                        newComp.ComponentName = (string) rdr["ComponentName"];

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
                        if (rdr["OwnerID"] != DBNull.Value)
                            newComp.OwnerID = (int) rdr["OwnerID"];

                        compList.Add(newComp);
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: Could not get components");
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
                string updateString =
                    @"UPDATE Component
                        SET ComponentNumber= @Data2, SerieNr = @Data3, ComponentName = @Data4, ComponentInfo = @Data5, Category = @Data6, Datasheet = @Data7, Image = @Data8, OwnerID = @Data9
                        WHERE ComponentID = @Data1";

                object ownerIdParam = DBNull.Value;
                object loanInformationParam = DBNull.Value;

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
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data8";
                    cmd.Parameters.Add(cmd.CreateParameter()).ParameterName = "@Data9";

                    // Hvis man ikke ændre på en attribut, så sættes den lig med CurrentComponents attribut.
                    cmd.Parameters["@Data1"].Value = cp.ComponentID != 0 ? cp.ComponentID : CurrentComponent.ComponentID;
                    cmd.Parameters["@Data2"].Value = cp.ComponentNumber != 0 ? cp.ComponentNumber : CurrentComponent.ComponentNumber;
                    cmd.Parameters["@Data3"].Value = cp.SerieNr ?? CurrentComponent.SerieNr;
                    cmd.Parameters["@Data4"].Value = cp.ComponentName ?? CurrentComponent.ComponentName;
                    cmd.Parameters["@Data5"].Value = cp.ComponentInfo ?? CurrentComponent.ComponentInfo;
                    cmd.Parameters["@Data6"].Value = cp.Category ?? CurrentComponent.Category;
                    cmd.Parameters["@Data7"].Value = cp.Datasheet ?? CurrentComponent.Datasheet;
                    cmd.Parameters["@Data8"].Value = cp.Image ?? CurrentComponent.Image;
                    cmd.Parameters["@Data9"].Value = cp.OwnerID != 0 ? cp.OwnerID : ownerIdParam;

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
                    CurrentComponent.SerieNr = (string)rdr["SerieNr"];
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
                    if (rdr["OwnerID"] != DBNull.Value)
                        CurrentComponent.OwnerID = (int)rdr["OwnerID"];
                    break; 
                }
            }

            catch (Exception e)
            {
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
