using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataAccesLogicLib;

namespace UseDAL
{
    class Program
    {
        static void Main(string[] args)
        {
            /* Indsæt komponent */
            //for (int i = 2; i < 5; i++)
            //{
            //    var comUtil = new ComponentDataUtil();
            //    var cp = new Component();
            //    cp.ComponentNumber = 22;
            //    cp.SerieNr = "123FAQ";
            //    cp.ComponentName = "126 nF Kondensator";
            //    cp.ComponentInfo = "126 nF kondensator som bare virker pisse godt og som kan bruges til bluetooth";

            //    comUtil.CurrentComponent = cp;
            //    comUtil.CreateComponent(cp);
            //}

            var CompDataUtil = new ComponentDataUtil();
            CompDataUtil.CheckReservations();
            //var cp = new Component();
            //cp.ComponentNumber = 10;
            //cp.ComponentName = "Test2";
            //cp.SerieNr = "123ADC";
            //cp.Image = "hehehelol.jpg";
            ////cp.Datasheet = "https://www.elfaelektronik.dk/elfa3~dk_da/elfa/init.do?item=10-389-16&toc=0&q=arduino";
            //CompDataUtil.CurrentComponent = cp;
            //CompDataUtil.UpdateComponent(65, cp);

            //foreach (var comp in CompDataUtil.GetAllComponents())
            //{
            //    Console.WriteLine(comp.ComponentID + " " + comp.ComponentName + " " + comp.ComponentNumber);
            //}

            //CompDataUtil.DeleteComponent(3);

            //var newlist = new List<string>();
            //newlist.Add("Raspberry");
            //newlist.Add("+");
            //newlist.Add("Pi");

            //foreach (var comp in CompDataUtil.GetComponents(newlist))
            //{
            //    Console.WriteLine(comp.ComponentID + " ");
            //}


            //var u = new User();
            //u.FirstName = "David";
            //u.LastName = "Buhauer";
            //u.Email = "201270749@iha.dk";
            //userDataUtil.CurrentUser = u;
            //userDataUtil.CreateUser(u);


            //var u = new User();
            //u.MobileNr = 76647283;
            //u.FirstName = "Carl";
            //userDataUtil.UpdateUser(3, u);

            //userDataUtil.DeleteUser(1);

            //var cp = new Component();
            //cp.ComponentNumber = 10;
            //cp.ComponentName = "Test2";
            //cp.SerieNr = "123ADC";
            //cp.Image = "hehehelol.jpg";
            //cp.Datasheet = "https://www.elfaelektronik.dk/elfa3~dk_da/elfa/init.do?item=10-389-16&toc=0&q=arduino";

            //var compDataUtil = new ComponentDataUtil();
            //compDataUtil.UpdateComponent(108, cp);
            //List<string> list = new List<string>();
            //list.Add("wifi");
            //list.Add("+");
            //list.Add("201270915");
            //List<Component> cList = compDataUtil.GetComponents(list);
            //foreach (var comp in cList)
            //{
            //    Console.WriteLine(comp.ComponentID);
            //}
                
            
            //compDataUtil.CreateComponent(cp);

            //compDataUtil.DeleteComponent(143);

            //var loanDataUtil = new LoanDataUtil();
            //LoanInformation loan = new LoanInformation();
            //loan.ReservationDate = new SqlDateTime(2014, 3, 15);
            //loanDataUtil.UpdateLoanInformation(160, loan);

            //var loan = new LoanInformation();
            //loan.AdminComment = "sut mig";
            //loan.UserComment = "HEHEHE";
            //loan.LoanDate = new SqlDateTime();
            //loan.ReturnDate = new SqlDateTime();

            //loanDataUtil.UpdateLoanInformation(1, loan);

            //if (compDataUtil.LoanTimeExceeded().Count > 0)
            //{
            //    foreach (var comp in compDataUtil.LoanTimeExceeded())
            //    {
            //        Console.WriteLine(comp.ComponentID + " " + comp.ComponentName);
            //    }
            //}

            //List<Component> comps = compDataUtil.GetAllComponents();

            //loan = loanDataUtil.GetLoanInformation(comps[0]);
            //Console.WriteLine(loan.LoanID + " " + loan.Component);

            //loanDataUtil.UpdateLoanInformation(14, loan);

            //for (int i = 1; i < compDataUtil.GetAllComponents().Count; i++)
            //{
            //    loanDataUtil.UpdateLoanInformation(i, loan);
            //}

            ////compDataUtil.DeleteComponent(180);

            //compDataUtil.LoanTimeExceeded();

            //compDataUtil.ComponentReservation("Raspberry Pi Model B 120", 1);

            //foreach (var comp in loanDataUtil.GetLoanedComponents())
            //{
            //    Console.WriteLine(comp.ComponentID + " " + comp.ComponentName);
            //}

            /* Test af udlånt/reserveret komponenter for student ID med X nummer */

            //Component newcomp = new Component();
            //newcomp.OwnerID = 9;

            //compDataUtil.UpdateComponent(1, newcomp);

            //Console.WriteLine("------LÅNT-----");
            //foreach (var comp in loanDataUtil.GetLoanedComponentFromStudentId("201270749"))
            //{
            //    Console.WriteLine("---------------");
            //    Console.WriteLine(comp.ComponentID + " " + comp.ComponentNumber + " " + comp.SerieNr + " "
            //        + comp.ComponentName + " " + comp.ComponentInfo + " " + comp.Category
            //        + " " + comp.Datasheet + " " + comp.Image);
            //    Console.WriteLine("---------------");

            //}
            //Console.WriteLine("---------------");

            //Console.WriteLine();

            //Console.WriteLine("----RESERVERET----");
            //foreach (var comp in loanDataUtil.GetReservedComponentFromStudentId("201270749"))
            //{
            //    Console.WriteLine("---------------");
            //    Console.WriteLine(comp.ComponentID + " " + comp.ComponentNumber + " " + comp.SerieNr + " "
            //        + comp.ComponentName + " " + comp.ComponentInfo + " " + comp.Category
            //        + " " + comp.Datasheet + " " + comp.Image);
            //    Console.WriteLine("---------------");
            //}
            //Console.WriteLine("------------------");



            //Console.WriteLine("----RESERVERING SLETTET----");
            //loanDataUtil.RemoveReservation(1);

            /* Test af udlånt/reserveret komponenter for student ID med X nummer */

            //var loan = new LoanInformation();
            //loan.LoanDate = DateTime.Now;
            //loan.ReturnDate = new SqlDateTime(2014, 05, 02);

            //for (int i = 1; i <= 3; i++)
            //    loanDataUtil.UpdateLoanInformation(i, loan);

        }
    }
}
