using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;
using System.Linq;


/** Handles the main game logic, like making and evaluating guesses. */
public class WordleManager : MonoBehaviour
{

    public PubChemAPIManager pubChemAPIManager;

    public WordleGuessScrollArea wordleGuessScrollArea;

    int i2 = 0;

    public GameObject prefab;

    public GameObject VictoryPrefab;

    public GameObject guessesListedHere;

    public GameObject GUI;

    /** Adds the given guess to the list of all guesses. */
    public void AddGuess(ChemicalData guess)
    {

        // Create a label to represent the guess
        GameObject guessObject = Instantiate(prefab, guessesListedHere.transform.GetChild(0));
        guessObject.GetComponent<ForPrefabButton>().SetCID(Int32.Parse(guess.GetProperty("CID")));
        guessObject.GetComponent<ForPrefabButton>().text.text = guess.GetProperty("Title");
        RectTransform rect2 = guessObject.GetComponent<RectTransform>();
        RectTransform rect3 = guessesListedHere.GetComponent<RectTransform>();
        //rect2.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect3.rect.width);
        //rect2.offsetMin = new Vector2(0, 0);
        //rect2.offsetMax = new Vector2(0, 0);

        i2++;

        // Move it to the appropriate position
        guessObject.transform.position += new Vector3(0, 1, 0) * i2 * wordleGuessScrollArea.spacing;// + new Vector3(0, -2000, 0);

        wordleGuessScrollArea.ExpandToAccommodate(i2);

    }

    public GUIController guiController;


    public void set(ChemicalData chemicalData, bool iGuessedThisAlready)
    {
        guiController.SetGuessingChemical(chemicalData);
        guiController.set(chemicalData, iGuessedThisAlready);
    }

    private ChemicalData mysteryChemical;
    public ChemicalData GetMysteryChemical()
    {
        return mysteryChemical;
    }
    public void SetMysteryChemical(ChemicalData[] mysteryChemicals)
    {

        //double minWeight = 9999999;
        ChemicalData minChemical = mysteryChemicals[0];
        /*
        foreach(ChemicalData data in mysteryChemicals)
        {
            double weight = Double.Parse(data.GetProperty("MolecularWeight"));
            if (weight < minWeight)
            {
                minWeight = weight;
                minChemical = data;
            }
        }
        */

        Random rng = new Random();
        

        this.mysteryChemical = mysteryChemicals[rng.Next(mysteryChemicals.Length)];
        //this.mysteryChemical = minChemical;
    }

    public void chooseMysteryChemical()
    {
        int cidBeginning = UnityEngine.Random.Range(500, 1000);
        string builder = "";
        for (int i = cidBeginning; i < cidBeginning + 200; i++)
        {
            builder += i.ToString();
            if (i != cidBeginning + 199) builder += ",";
        }
        
        pubChemAPIManager.MakeAPIRequest(builder, pubChemAPIManager.generalDataController.dataTypes, "set_mystery_chemical");
    }

    void Start()
    {
        chooseMysteryChemical();
    }

    public void VICTORY()
    {
        GameObject DoubleU = Instantiate(VictoryPrefab, GUI.transform);
        
        DoubleU.transform.localPosition = new Vector3(0, 0, 0);
        DoubleU.transform.localScale.Set(2, 2, 1);
    }


    /** Guesses the currently entered chemical. */
    public void MakeGuess()
    {

        ChemicalData guessingChemical = guiController.GetGuessingChemical();

        AddGuess(guessingChemical);
        EvaluateGuess(guessingChemical);

        // Debug.Log(titleText.text);
        // Debug.Log(formulaText.text);
        // Debug.Log(weightText.text);
        // Debug.Log(chargeText.text);
        // Debug.Log(bpText.text);
    }

    private bool IsNumeric(char c)
    {
        return "0123456789".Contains(c);
    }
    private bool IsLowercase(char c)
    {
        return "abcdefghijklmnopqrstuvwxyz".Contains(c);
    }

    public void EvaluateGuess(ChemicalData guessing)
    {

        string feedback = "";
        int guessCharge = Convert.ToInt32(guessing.GetProperty("Charge")), actualCharge = Convert.ToInt32(mysteryChemical.GetProperty("Charge"));
        double guessWeight = Convert.ToDouble(guessing.GetProperty("MolecularWeight")), actualWeight = Convert.ToDouble(mysteryChemical.GetProperty("MolecularWeight"));
        string guessingFormula = guessing.GetProperty("MolecularFormula");
        string actualFormula = mysteryChemical.GetProperty("MolecularFormula");
        //print(mysteryChemical.GetProperty("BoilingPoint"));
        print(mysteryChemical.GetProperty("MolecularFormula"));

        for (int i = 0; i < guessingFormula.Length; i++)
        {
            char c = guessingFormula[i];
            if (IsNumeric(c) || c == '-' || c == '+' || IsLowercase(c)) continue;
            string elementWordBuilder = "" + c;
            for (int j = i + 1; j < guessingFormula.Length; j++)
            {
                if (IsLowercase(guessingFormula[j])) elementWordBuilder += guessingFormula[j];
                else break;
            }
            if (actualFormula.Contains(elementWordBuilder))
            {
                int count1 = 0;
                for (int j = i + elementWordBuilder.Length; j < guessingFormula.Length; j++)
                {
                    char c2 = guessingFormula[j];
                    if (!IsNumeric(c2)) break;
                    count1 = count1 * 10 + Int32.Parse("" + c2);
                }
                if (count1 == 0) count1 = 1;
                int count2 = 0;
                for (int j = actualFormula.IndexOf(elementWordBuilder) + elementWordBuilder.Length; j < actualFormula.Length; j++)
                {
                    char c2 = actualFormula[j];
                    if (!IsNumeric(c2)) break;
                    count2 = count2 * 10 + Int32.Parse("" + c2);
                }
                if (count2 == 0) count2 = 1;
                if (count1 > count2) feedback += "You have too many of " + elementWordBuilder + "\n";
                else if (count1 < count2) feedback += "You have too few of " + elementWordBuilder + "\n";
                else feedback += "You have the right amount of " + elementWordBuilder + "!" + "\n";
            }
            else
            {
                feedback += elementWordBuilder + " shouldn't be present" + "\n";
            }
        }

        for (int i = 0; i < actualFormula.Length; i++)
        {
            char c = actualFormula[i];
            if (IsNumeric(c) || c == '-' || c == '+') { 
                continue;
            }
            if (!guessingFormula.Contains(c))
            {
                feedback += "An element is missing" + "\n";
            }
        }
        if (actualWeight == guessWeight)
        {
            feedback += "You have the correct Weight\n";
        }
        else if (actualWeight > guessWeight)
        {
            feedback += "You have too little Weight\n";
        }
        else
        {
            feedback += "You have too much Weight\n";
        }
        Debug.Log(actualCharge + "\t" + guessCharge);
        if (actualCharge == guessCharge)
        {
            feedback += "You have the correct Charge\n";
        } else if (actualCharge > guessCharge) {
            feedback += "You have too little Charge\n";
        } else
        {
            feedback += "You have too much Charge\n";
        }
        if (actualFormula == guessingFormula)
        {
            VICTORY();
        }
        guiController.SetFeedback(feedback);

    }

}
