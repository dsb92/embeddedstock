using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Models
{
    class Searcher
    {
        /*
        static void Main(string[] args)
        {
            var mySearch = new Search();
            List<string> myList= new List<string>();
            myList=mySearch.SplitString("din mor er en mand");

            foreach (string s in myList)
            {
                Console.WriteLine(s);
            }


            var component1 = new Component();
            component1._name = "myComponent";

            var component2 = new Component();
            component2._name = "myComponent";

           var myComponentsList=new List<Component>();
            myComponentsList.Add(component1);
            myComponentsList.Add(component2);
            var myComponents = new Components(myComponentsList);
            Console.WriteLine(myComponents.GetAvaiableQuantity("myComponent"));

        }
        */
            public List<string> SplitString(string searchString)
            {

                var keywords = new List<string>();
                string[] split = searchString.Split(' ');
                foreach (string s in split)
                {
                    keywords.Add(s);
                }

                return keywords;
            }

        }
}
