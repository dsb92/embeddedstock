using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ReserveController : Controller
    {
        //
        // GET: /Reserve/
        public ActionResult Index(string componentName)
        {
            var myDataUtil = new ComponentDataUtil();



            if (myDataUtil.ComponentReservation(componentName, Session["User"].ToString()))
            {
                ViewBag.Message = "Du har reserveret "+componentName;
                return View();
            }
            ViewBag.Message= "Ikke muligt at reservere komponent, prøv igen";
            return View();
        }

        public ActionResult Remove(Component component)
        {
            var loanDataUtil = new LoanDataUtil();
            var loanInfo = loanDataUtil.GetLoanInformation(component);

            if (Session["User"] == null)
                return RedirectToAction("Index", "Home");

            if (Session["User"].ToString() == loanInfo.ReservationID)
            {
                loanDataUtil.RemoveReservation(component.ComponentID); 
            }
            
            return RedirectToAction("Reserve","Account");
        }

	}
}