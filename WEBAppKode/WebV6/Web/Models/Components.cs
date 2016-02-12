using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebPages;


namespace Web.Models
{
    public class Components
    {
        public List<Component> _components = new List<Component>();
        public List<LoanInformation> _LoanInformations=new List<LoanInformation>(); 

        public int GetAvailableQuantity(string ComponentNavn)
        {
            int Count = 0;

            Parallel.ForEach(_components, component =>
            {


                foreach (var loanInformation in _LoanInformations)
                {
                    if (loanInformation.Component == component.ComponentID)
                    {
                        if (component.ComponentName == ComponentNavn && loanInformation.OwnerID.IsEmpty() &&
                            loanInformation.ReservationID.IsEmpty())
                            Interlocked.Increment(ref Count);
                        break;
                    }
                }

            });
            return Count;
        }

        public List<string> GetAllCategories()
        {
            var categories = new List<string>();
            foreach (var component in _components)
            {
                categories.Add(component.Category);

            }
            return categories.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
        }


        public int GetTotalQuantity(string Serienummer)
        {
            int Count = 0;
            foreach (Component component in _components)
            {
                if (component.SerieNr == Serienummer)
                    Count++;
            }
            return Count;
        }
        public List<Component> GetByCategory(string Category)
        {
            var ComponentsInCategory = new List<Component>();
            foreach (Component component in _components)
            {
                if (component.Category == Category)
                    ComponentsInCategory.Add(component);

            }
            return ComponentsInCategory;
        }


    }
}