using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoreLinq;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index(string searchString, string category, int? page)
        {
           var componentsPerPage = new int?();
           var mySearcher = new Searcher();
           var myComponentDataUtil = new ComponentDataUtil();
           var myComponents = new Components();
           var emptyComponent = new Component();
           var listOfQuantity = new List<int>();

            emptyComponent.ComponentName="Ingen Komponenter Fundet";
            componentsPerPage = 20;

            // Tjekker om der er indtastet en søgning, hvis ja, hentes en komponentliste med de indtastede tags
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

            //Tjekker om man har trykket på en kategori, hvis ja, hentes en komponentliste med den kategori
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

           //Opretter liste med alle kategorier
            var listOfCategories=myComponents.GetAllCategories();

            //Sætter listen med kategorier i alfabetisk rækkefølge
            listOfCategories.Sort();
           
            //Opretter liste til hvis der ikke eksisterer nogen kategorier
            var noneFound= new List<string> {"Ingen kategorier fundet"};
            
            
            if (listOfCategories.Count != 0)
                ViewData["Categories"] = listOfCategories;
            else
                ViewData["Categories"] = noneFound;

            //Sorterer listen med komponenter, så den er i alfabetisk rækkefølge og komponenterne kun ankommer en gang.
            var sortedList = myComponents._components.OrderBy(c => c.ComponentName).DistinctBy(c => c.ComponentName).ToList();

            //Udregner hvor mange sider, der skal være på hjemmesiden
            ViewData["NumberOfPages"] = (sortedList.Count + componentsPerPage - 1) / componentsPerPage;

            //Udregner hvor mange der er af hver komponent type(taget ud af Componentname), lægger det derefter over i en ny liste

            foreach (var component in sortedList)
            {
                listOfQuantity.Add(myComponents.GetAvailableQuantity(component.ComponentName));
            }
            

            //Hvis dette tjek går igennem, er der ikke nok komponenter til at vise den ønskede side
            if (((componentsPerPage * page.GetValueOrDefault())) >= sortedList.Count || page < 0)
            {
                myComponents._components.Clear();
                myComponents._components.Add(emptyComponent);
                listOfQuantity.Clear();
                listOfQuantity.Add(0);
                return View(myComponents._components);
            }

            ViewData["AvailableQuantity"] = listOfQuantity;
                
            //Hvis dette tjek går igennem har man komponenter til overs, men ikke nok til at lave en "hel" side
            if(((componentsPerPage*page.GetValueOrDefault())+componentsPerPage)>sortedList.Count)
                return View(sortedList.GetRange((page * componentsPerPage).GetValueOrDefault(), sortedList.Count - (componentsPerPage * page).GetValueOrDefault()));
            //Returnerer componentsPerPage, startende fra den side man er kommet til
                return View(sortedList.GetRange(((page * componentsPerPage).GetValueOrDefault()), componentsPerPage.GetValueOrDefault()));
        }
	}
}