using System.Collections.Generic;
using System.Linq;

namespace ChemWordle
{
    
    /** Stores all the information for one chemical in the PubChem database. */
    public class ChemicalData
    {

        /** Where the data actually lives.
         * Set up for any number of properties. */
        private readonly Dictionary<string, string> properties = new();
        public string GetProperty(string propertyName) =>
            properties.GetValueOrDefault(propertyName, "<NULL>");
        public void SetProperty(string propertyName, string propertyValue) =>
            properties[propertyName] = propertyValue;

        
        public override string ToString()
        {
            return properties.Keys.Aggregate("Chemical Data[",
                (current, propertyName) =>
                    current + $"{propertyName}={properties[propertyName]}, ")[..^1] + "]";
        }

    }
}