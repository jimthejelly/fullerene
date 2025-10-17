using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChemWordle
{
    
    /** Handles most of the GUI code. */
    public class GUIController : MonoBehaviour
    {
        
        private GeneralDataController _generalDataController;
        private WordleManager _wordleManager;

        private void Start()
        {
            _generalDataController = FindObjectOfType<GeneralDataController>();
            _wordleManager = FindObjectOfType<WordleManager>();
            guessButton.enabled = false;
        }

        /** Text for the chemical's title. */
        public TMPro.TMP_InputField titleText;

        /** Text for the chemical's molecular formula. */
        public TMPro.TMP_InputField formulaText;

        /** Text for the chemical's molecular weight. */
        public TMPro.TMP_InputField weightText;

        /** Text for the chemical's charge. */
        public TMPro.TMP_InputField chargeText;

        /** The guess button. :p */
        public Button guessButton;
        public void SetGuessButtonEnabled(bool _enabled) => guessButton.enabled = _enabled;

        /** The text that tells you how close your formula was. */
        public TMPro.TextMeshProUGUI formulaFeedback;
        public void SetFormulaFeedback(string feedback) => formulaFeedback.text = feedback;

        /** The text that tells you how close your weight was. */
        public TMPro.TextMeshProUGUI weightFeedback;
        public void SetWeightFeedback(string feedback) => weightFeedback.text = feedback;

        /** The text that tells you how close your charge was. */
        public TMPro.TextMeshProUGUI chargeFeedback;
        public void SetChargeFeedback(string feedback) => chargeFeedback.text = feedback;



        public GameObject prefab;

        public GameObject victoryPrefab;

        public GameObject guessesListedHere;

        public GameObject gui;
        



        public void OnTitleValueChanged() { }
        public void OnTitleEndEdit() => _ = 
            GuiChemicalTask("Title", "name",titleText.text);
        public void OnTitleSelect() { }
        public void OnTitleDeselect() { }
        

        /** Look for a chemical with 'wordlePropertyName'/'pubchemPropertyName' set to 'propertyValue',
         * and get it from online if it's not cached right now. */
        private async Task GuiChemicalTask(
            string wordlePropertyName,
            string pubchemPropertyName,
            string propertyValue
        ) {
            var data = _generalDataController.GetChemicalWithProperty(wordlePropertyName, titleText.text);
            if (data == null)
            {
                var cids = await PubChemAPIManager.requestCIDsWithProperty(
                   pubchemPropertyName, propertyValue, 1);
                data = (await PubChemAPIManager.RequestChemicals(cids, GeneralDataController.DataTypes))[0];
                if (data != null) _generalDataController.RegisterChemicalData(data);
            }

            if (data == null) return;
            _wordleManager.SetGuessingChemical(data);
        }


        public void OnFormulaValueChanged() { }
        public void OnFormulaEndEdit() => _ = 
            GuiChemicalTask("MolecularFormula", "fastformula",formulaText.text);
        public void OnFormulaSelect() { }
        public void OnFormulaDeselect() { }

        public void set(ChemicalData chemicalData)
        {

            // update each of the labels
            titleText.text = chemicalData.GetProperty("Title");
            formulaText.text = chemicalData.GetProperty("MolecularFormula");
            weightText.text = chemicalData.GetProperty("MolecularWeight");
            chargeText.text = chemicalData.GetProperty("Charge");

            // update the wordle gui based on whether this chemical has been guessed yet
            var alreadyGuessed = _wordleManager.AlreadyGuessed(chemicalData.GetProperty("Title"));
            SetGuessButtonEnabled(!alreadyGuessed);
            if (alreadyGuessed) _wordleManager.EvaluateGuess(chemicalData);
            else _wordleManager.clearFeedBackText();

        }

        /** Called when the "Exit" button is pressed, probably. */
        public void ExitGame() {
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }
        
    }
}
