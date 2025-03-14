using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/** Handles all the Wordle stuff. */
public class WordleManager : MonoBehaviour
{

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
        for (int i = 0; i < 3; i++)
        {
            AddGuess(ChemicalData.LoadFromPUGREST(100));
        }
    }

    // Update is called once per frame
    void Update()
    {
        titleText.text = "hi";
    }


    int i2 = 0;

    public GameObject prefab;

    public GameObject guessesListedHere;


    /** Adds the given guess to the list of all guesses. */
    void AddGuess(ChemicalData guess)
    {

        // Create a label to represent the guess
        GameObject guessObject = Instantiate(prefab, guessesListedHere.transform);

        // Move it to the appropriate position
        guessObject.transform.position += new Vector3(0, 1, 0) * i2 * 50;
        i2++;


    }

}
