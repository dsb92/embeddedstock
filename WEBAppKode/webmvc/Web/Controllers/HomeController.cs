using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index(string searchString, string category)
        {
           
           var mySearcher = new Searcher();
           var myComponentDataUtil = new ComponentDataUtil();
           var myComponentList = new List<Component>();
           var myComponents = new Components();


            if (!String.IsNullOrEmpty(searchString))
            {
                var SearchTags = mySearcher.SplitString(searchString);
               
                foreach (var component in  myComponentDataUtil.GetComponents(SearchTags))
                {
                    myComponents._components.Add(component);
                }
                
            }
            else
            {
                if (!String.IsNullOrEmpty(category))
                {
                    foreach (var component in myComponentDataUtil.GetAllComponents())
                    {
                        if (component.Category == category)
                            myComponents._components.Add(component);
                    }
                }
                else
                {
                    foreach (var component in myComponentDataUtil.GetAllComponents())
                    {
                            myComponents._components.Add(component);
                    }
                }
                
                
            }


            var resultList=myComponents.GetAllCategories();
            var noneFound= new List<string>();
            noneFound.Add("Ingen kategorier fundet");
            if (resultList.Count != 0)
                ViewData["Categories"] = resultList;
            else
                ViewData["Categories"] = noneFound;
            

            return View(myComponents._components);
        }
	}
}