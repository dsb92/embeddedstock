using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebGrease;
using WebMatrix.WebData;
using Web.Models;

namespace Web.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {

        // GET: /Account/Login
        //[AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        //[AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            var myAuthenticator = new Web.Models.Authenticate();
            if (ModelState.IsValid && myAuthenticator.IsAuthenticated(model.UserName,model.Password))
            {
                Session["User"] = model.UserName.Split('@')[0];
                return RedirectToLocal(returnUrl);
            }
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }
	

     private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
     // POST: /Account/LogOff

     [HttpPost]
     //[AllowAnonymous]
     public ActionResult LogOff()
     {
         Session["User"] = null;

         return RedirectToAction("Index", "Home");
     }
     //[AllowAnonymous]
     public ActionResult Reserve()
     {
         var myLoanDataUtil = new LoanDataUtil();
         var myloanedComponents = new Components();
         var myreservedComponents = new Components();
         if (Session["User"] == null)
             return RedirectToAction("Index","Home");
         // Tjekker om der er indtastet en s�gning, hvis ja, hentes en komponentliste med de indtastede tags
         if (!String.IsNullOrEmpty(Session["User"].ToString()))
         {
             var studieNr =Session["User"].ToString();

             foreach (var component in myLoanDataUtil.GetLoanedComponentFromStudentId(studieNr))
             {
                 myloanedComponents._components.Add(component);
             }

             foreach (var component in myLoanDataUtil.GetReservedComponentFromStudentId(studieNr))
             {
                 myreservedComponents._components.Add(component);
             }

             var SortedReserveList = myreservedComponents._components.OrderBy(c => c.ComponentName).ToList();
             var SortedLoanList= myloanedComponents._components.OrderBy(c => c.ComponentName).ToList();
             var SortedReserveLoanInformationList = new List<string>();
             var SortedLoanedLoanInformationList = new List<string>();


             ViewData["ReservedComponents"] = SortedReserveList;
             ViewData["LoanedComponents"] = SortedLoanList;

             foreach (var component in SortedReserveList)
             {
                 SortedReserveLoanInformationList.Add(myLoanDataUtil.GetLoanInformation(component).ReservationDate.Value.Value.ToShortDateString());
             }
             foreach (var component in SortedLoanList)
             {
                 SortedLoanedLoanInformationList.Add(myLoanDataUtil.GetLoanInformation(component).ReturnDate.Value.Value.ToShortDateString());
             }

             ViewData["ReservedLoanInformation"] = SortedReserveLoanInformationList;
             ViewData["LoanedLoanInformation"] = SortedLoanedLoanInformationList;
         }

         return View();
     }

    }
}