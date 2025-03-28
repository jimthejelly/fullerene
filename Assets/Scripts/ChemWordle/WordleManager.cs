using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/** Handles all the Wordle stuff. */
public class WordleManager : MonoBehaviour
{

    public PubChemAPIManager pubChemAPIManager;

    public WordleGuessScrollArea wordleGuessScrollArea;

    // Text for the chemical's title
    public TMPro.TextMeshProUGUI titleText;

    // Text for the chemical's molecular formula
    public TMPro.TextMeshProUGUI formulaText;

    // Text for the chemical's molecular weight
    public TMPro.TextMeshProUGUI weightText;

    // Text for the chemical's charge
    public TMPro.TextMeshProUGUI chargeText;

    // Text for the chemical's boiling point
    public TMPro.TextMeshProUGUI bpText;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        thing += Time.deltaTime;
        if (thing > 0.5 && !thing2)
        {
            thing2 = true;
            for (int i = 0; i < 100; i++)
            {
                AddGuess(pubChemAPIManager.generalDataController.GetChemicalWithCID(100 + i));
            }
        }
    }

    double thing = 0;
    bool thing2 = false;


    int i2 = 0;

    public GameObject prefab;

    public GameObject guessesListedHere;


    /** Adds the given guess to the list of all guesses. */
    void AddGuess(ChemicalData guess)
    {

        // Create a label to represent the guess
        GameObject guessObject = Instantiate(prefab, guessesListedHere.transform.GetChild(0));
        guessObject.transform.GetChild(0).GetComponent<ForPrefabButton>().SetCID(100 + i2);
        guessObject.transform.GetChild(0).GetComponent<ForPrefabButton>().text.text = guess.GetProperty("Title");

        // Move it to the appropriate position
        guessObject.transform.position += new Vector3(0, 1, 0) * i2 * 40 + new Vector3(0, -400, 0);
        i2++;

        wordleGuessScrollArea.ExpandToAccommodate(i2);


    }


    public void set(ChemicalData chemicalData)
    {

        titleText.text = chemicalData.GetProperty("Title");
        formulaText.text = chemicalData.GetProperty("MolecularFormula");
        weightText.text = chemicalData.GetProperty("MolecularWeight");
        chargeText.text = chemicalData.GetProperty("Charge");

    }

}
