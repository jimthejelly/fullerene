using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChemWordle
{
    
    /// <summary>
    /// This is where all chemical data is cached and managed.
    /// </summary>
    public class GeneralDataController : MonoBehaviour
    {

        /// <summary>
        /// A list of all data types the game requests by default when looking up a chemical online (plus the CID).
        /// These must exactly match the names of data types in the PubChem database.
        /// </summary>
        public static readonly List<string> DataTypes = new()
        {
            "Title",
            "MolecularFormula",
            "MolecularWeight",
            "Charge"
        };

        /// <summary>
        /// This is where all the chemical data objects are stored.
        /// Any retrieved chemicals from online should be cached here!
        /// </summary>
        private readonly List<ChemicalData> _allData = new();

        /// <summary>
        /// Looks up the given property name/value in the list of known chemicals.
        /// Does *not* query the PubChem database if nothing is found.
        /// </summary>
        /// <param name="propertyName"> The name of the property to look for (i.e. "Title"). </param>
        /// <param name="propertyValue"> The value to look for (i.e. "Water"). </param>
        /// <returns> The first chemical found that matches the given value for the given property,
        /// or <c>null</c> if no such chemical is found. </returns>
        public ChemicalData GetChemicalWithProperty(string propertyName, string propertyValue)
        {

            // TODO: could easily redo this to be more efficient
            // not sure if there'd be any benefit though

            // make the value case-insensitive beforehand just for convenience
            var valueLower = propertyValue.ToLower();

            // loop through each chemical
            foreach (var data in _allData)
            {
                // excuse me..? why is this possible?
                if (data == null) continue;

                try
                {
                    if (data.GetProperty(propertyName) == null)
                    {
                        Debug.LogError(data);
                        continue;
                    }

                    // check if this chemical matches the requirements.
                    // if it does match, return it!
                    var dataValue = data.GetProperty(propertyName);
                    if (dataValue != null && dataValue.ToLower().Equals(valueLower))
                        return data;
                }
                // TODO: Think of a more serious way to handle exceptions.
                catch (Exception e)
                {
                    // oh well :)
                    Debug.LogError(e);
                }
            }

            // seems like no chemicals matched. return null
            return null;

        }

        /// <summary>
        /// Adds the given chemical data object to the internal list.
        /// This should be called for every new chemical retrieved from PubChem.
        /// </summary>
        /// <param name="chemicalData"> The data to add. </param>
        public void RegisterChemicalData(ChemicalData chemicalData) => _allData.Add(chemicalData);

    }

}
