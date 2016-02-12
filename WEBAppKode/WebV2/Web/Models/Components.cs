using System.Collections.Generic;
using System.Linq;


namespace Web.Models
{
    public class Components
    {
        public List<Component> _components = new List<Component>();

        public int GetAvailableQuantity(string Serienummer)
        {
            int Count = 0;
            foreach (Component component in _components)
            {
                if (component.SerieNr == Serienummer && component.OwnerID == 0)
                    Count++;

            }
            return Count;
        }

        public List<string> GetAllCategories()
        {
            var categories = new List<string>();
            foreach (Component component in _components)
            {
                categories.Add(component.Category);

            }
            return categories.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
        }


        public int GetLendOutQuantity(string ComponentName)
        {
            int Count = 0;
            foreach (Component component in _components)
            {
                if (component.ComponentName == ComponentName && component.OwnerID != 0)
                    Count++;

            }
            return Count;
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