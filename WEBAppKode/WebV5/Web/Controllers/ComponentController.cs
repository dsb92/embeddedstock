using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ComponentController : Controller
    {
        //
        // GET: /Component/
        public ActionResult Index(int? componentid)
        {
           var myComponentDataUtil = new ComponentDataUtil();
           var myComponents = new Components();
           var selectedComponent = new Component();
           var sortedList = new List<Component>();

            foreach (var component in myComponentDataUtil.GetAllComponents())
            {         
                myComponents._components.Add(component);
            }

            foreach (var component in myComponents._components)
            {
                if (component.ComponentID == componentid)
                    selectedComponent = component;
            }

            foreach (var component in myComponents._components)
            {
                if (selectedComponent.ComponentName==component.ComponentName)
                    sortedList.Add(component);
            }
            if (sortedList.Count > 0)
            {
                ViewData["AvailableQuantity"] = myComponents.GetAvailableQuantity(sortedList[0].ComponentName);

                return View(sortedList[0]); 
            }
            return View(); 
        }
	}
}