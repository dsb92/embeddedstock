using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WEBApp;

namespace test_string_split
{
    class Program
    {
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

        public class Search
        {
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

        public class Components
        {
            private List<Component> _components;

            public Components(List<Component> components)
            {
                this._components = components;
            }

            public int GetAvaiableQuantity(string ComponentName)
            {
                int Count = 0;
                foreach (Component component in _components)
                {
                    if (component._name == ComponentName && component._ownerId == 0)
                        Count++;

                }
                return Count;
            }
            public int GetLendOutQuantity(string ComponentName)
            {
                int Count = 0;
                foreach (Component component in _components)
                {
                    if (component._name == ComponentName && component._ownerId != 0)
                        Count++;

                }
                return Count;
            }
            public int GetTotalQuantity(string ComponentName)
            {
                int Count = 0;
                foreach (Component component in _components)
                {
                    if (component._name == ComponentName)
                        Count++;

                }
                return Count;
            }
            public List<Component> GetByCategory(string Category)
            {
                var ComponentsInCategory = new List<Component>();
                foreach (Component component in _components)
                {
                    if (component._category == Category)
                        ComponentsInCategory.Add(component);

                }
                return ComponentsInCategory;
            }


        }
    }
}
