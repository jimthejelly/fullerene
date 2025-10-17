using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChemWordle
{

    /** The centralized place where all chemical data is cached and managed. */
    public class GeneralDataController : MonoBehaviour
    {

        /** An array of all data types desired by the game.
         * These must match data type names in the PubChem database. */
        public static readonly List<string> DataTypes = new()
        {
            "Title",
            "MolecularFormula",
            "MolecularWeight",
            "Charge"
        };

        /** The list itself. You should be honored. */
        private readonly List<ChemicalData> _allData = new();

        /** Returns a chemical, if any exist, that matches the given value for the given property name.
         * Returns null otherwise. */
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
                    if (data.GetProperty(propertyName).ToLower().Equals(valueLower))
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

        /** Adds the given chemical data to THE list. */
        public void RegisterChemicalData(ChemicalData chemicalData) => _allData.Add(chemicalData);

    }

}
