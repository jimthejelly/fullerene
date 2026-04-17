using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemWordle
{
    
    /// <summary>
    /// Represents, and stores all information regarding, one chemical in the PubChem database.
    /// </summary>
    public class ChemicalData
    {

        /// <summary>
        /// The field where all chemical properties are stored.
        /// Keys correspond to property names (i.e. Title) and values correspond to property values.
        /// Note that no property, except the CID (internal pubchem id for this chemical),
        /// is guaranteed to be present!
        /// </summary>
        private readonly Dictionary<string, string> properties = new();
        
        /// <summary>
        /// Looks up the given property in the chemical, and returns its value if present, or <c>null</c> otherwise.
        /// </summary>
        /// <param name="propertyName"> The name of the property to be retrieved. </param>
        /// <returns> The value of the property, or <c>null</c> if this chemical
        /// does not have the desired property set. </returns>
        public string GetProperty(string propertyName) =>
            properties.GetValueOrDefault(propertyName, null);
        
        /// <summary>
        /// Sets the given property to the given value.
        /// If the property is not present, it is added to the chemical data.
        /// </summary>
        /// <param name="propertyName"> The name of the property to set. </param>
        /// <param name="propertyValue"> The value with which to set the property. </param>
        public void SetProperty(string propertyName, string propertyValue) =>
            properties[propertyName] = propertyValue;
        
        /// <summary>
        /// Converts this ChemicalData to a string.
        /// </summary>
        /// <returns> The generated string representing this ChemicalData. </returns>
        public override string ToString()
        {
            
            // example string:
            // ChemicalData{"CID":"100","Title":"Water"}
            
            StringBuilder sb = new("ChemicalData{");
            
            // loop through all properties and add them to sb
            for (var pIndex = 0; pIndex < properties.Count; pIndex++)
            {
                // " <key> " : " <value> "
                var pKey = properties.ElementAt(pIndex).Key;
                var pValue = properties.ElementAt(pIndex).Value;
                sb.Append("\"").Append(pKey).Append("\":\"").Append(pValue).Append("\"");
                
                // add a comma unless at the very end
                if (pIndex != properties.Count - 1)
                    sb.Append(", ");
            }
            
            return sb.Append("}").ToString();
            
        }
    
    }
    
}
