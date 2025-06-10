using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour
{

    // Text for the chemical's title
    public TMPro.TMP_InputField titleText;

    // Text for the chemical's molecular formula
    public TMPro.TMP_InputField formulaText;

    // Text for the chemical's molecular weight
    public TMPro.TMP_InputField weightText;

    // Text for the chemical's charge
    public TMPro.TMP_InputField chargeText;


    public TMPro.TextMeshProUGUI feedbackText;

    private List<string> guesses = new List<string>();


    private ChemicalData guessingChemical = null;

    public GeneralDataController generalDataController;


    public void OnTitleValueChanged()
    {
        // Debug.Log("Changed: \"" + titleText.text + "\"");
    }
    public void OnTitleEndEdit()
    {
        // Debug.Log("End edit: \"" + titleText.text + "\"");
        generalDataController.pubChemAPIManager.RequestChemicalWithProperty("name", titleText.text);
        ChemicalData data = generalDataController.GetChemicalWithProperty("Title", titleText.text);
        if(data != null)
        {
            guessingChemical = data;
            set(data, GuessCheck(data));
            print(data);
        }
    }
    public void OnTitleSelect()
    {
        // Debug.Log("Select: \"" + titleText.text + "\"");
    }
    public void OnTitleDeselect()
    {
        // Debug.Log("Deselect: \"" + titleText.text + "\"");
    }


    public void OnFormulaValueChanged()
    {
        // Debug.Log("Changed: \"" + formulaText.text + "\"");
    }
    public void OnFormulaEndEdit()
    {
        // Debug.Log("End edit: \"" + formulaText.text + "\"");
        generalDataController.pubChemAPIManager.RequestChemicalWithProperty("fastformula", formulaText.text);
        ChemicalData data = generalDataController.GetChemicalWithProperty("MolecularFormula", formulaText.text);
        if (data != null)
        {
            guessingChemical = data;
            set(data, false);
            print(data);
        }
    }
    public void OnFormulaSelect()
    {
        // Debug.Log("Select: \"" + formulaText.text + "\"");
    }
    public void OnFormulaDeselect()
    {
        // Debug.Log("Deselect: \"" + formulaText.text + "\"");
    }


    public bool GuessCheck(ChemicalData guess)
    {
        if (guesses.Contains(guess.GetProperty("Title")))
        {
            return true;
        }
        guesses.Add(guess.GetProperty("Title"));
        return false;
    }

    public ChemicalData GetGuessingChemical()
    {

        return guessingChemical;

    }

    public void SetGuessingChemical(ChemicalData data)
    {
        guessingChemical = data;
    }


    public WordleManager wordleManager;


    public void set(ChemicalData chemicalData, bool iGuessedThisAlready)
    {

        titleText.text = chemicalData.GetProperty("Title");
        formulaText.text = chemicalData.GetProperty("MolecularFormula");
        weightText.text = chemicalData.GetProperty("MolecularWeight");
        chargeText.text = chemicalData.GetProperty("Charge");
        print(chargeText.text);

        if (iGuessedThisAlready) wordleManager.EvaluateGuess(chemicalData);

    }

    public void SetFeedback(string feedback)
    {
        feedbackText.text = feedback;
    }


    public void ExitGame() {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
