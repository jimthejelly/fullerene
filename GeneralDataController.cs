using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralDataController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public PubChemAPIManager pubChemAPIManager;

    /** Gets a crazy number of chemicals from the PubChem database. */
    public void GetAllChemicals()
    {

        for (int i = 100; i < 101; i++)
        {
            // get the chemical with cid i

            Debug.Log("Making request...");

            // will this work? let's find out
            ChemicalData data = new ChemicalData();
            pubChemAPIManager.GetProperty(i, data);

        }

    }

}
