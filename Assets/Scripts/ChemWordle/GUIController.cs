using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChemWordle
{
    
    /// <summary>
    /// Handles most of the GUI code.
    /// </summary>
    public class GUIController : MonoBehaviour
    {
        
        /// <summary>
        /// A reference to the <see cref="GeneralDataController">GeneralDataController</see>.
        /// </summary>
        private GeneralDataController _generalDataController;
        
        /// <summary>
        /// A reference to the <see cref="WordleManager">WordleManager</see>.
        /// </summary>
        private WordleManager _wordleManager;

        private void Start()
        {
            _generalDataController = FindObjectOfType<GeneralDataController>();
            _wordleManager = FindObjectOfType<WordleManager>();
            guessButton.enabled = false;
        }

        /// <summary>
        /// Text for the chemical's title.
        /// </summary>
        public TMPro.TMP_InputField titleText;

        /// <summary>
        /// Text for the chemical's molecular formula.
        /// </summary>
        public TMPro.TMP_InputField formulaText;

        /// <summary>
        /// Text for the chemical's molecular weight.
        /// </summary>
        public TMPro.TMP_InputField weightText;

        /// <summary>
        /// Text for the chemical's charge.
        /// </summary>
        public TMPro.TMP_InputField chargeText;

        /// <summary>
        /// The guess button. :p
        /// </summary>
        public Button guessButton;
        
        /// <summary>
        /// Enables or disables the guess button.
        /// The button should only be enabled when pressing it
        /// would actually guess a new chemical.
        /// </summary>
        /// <param name="_enabled"> Whether the button should be enabled.
        /// For the avoidance of all doubt, <c>true</c> => enabled,
        /// and <c>false</c> => disabled. </param>
        public void SetGuessButtonEnabled(bool _enabled) => guessButton.enabled = _enabled;

        /// <summary>
        /// Displays molecular formula feedback when a guess is made.
        /// </summary>
        public TMPro.TextMeshProUGUI formulaFeedback;
        public void SetFormulaFeedback(string feedback) => formulaFeedback.text = feedback;

        /// <summary>
        /// Displays molecular weight feedback when a guess is made.
        /// </summary>
        public TMPro.TextMeshProUGUI weightFeedback;
        public void SetWeightFeedback(string feedback) => weightFeedback.text = feedback;

        /// <summary>
        /// Displays charge feedback when a guess is made.
        /// </summary>
        public TMPro.TextMeshProUGUI chargeFeedback;
        public void SetChargeFeedback(string feedback) => chargeFeedback.text = feedback;


        /// <summary>
        /// A reference to the <see cref="ForPrefabButton">guess label prefab</see>.
        /// </summary>
        public GameObject guessObjectPrefab;

        /// <summary>
        /// A reference to the victory prefab.
        /// </summary>
        public GameObject victoryPrefab;

        /// <summary>
        /// The vertical scroll box holding all the guess objects.
        /// </summary>
        public GameObject guessesListedHere;

        /// <summary>
        /// The top-level GUI. Everything except the menu is descended from here.
        /// </summary>
        public GameObject gui;

        
        // Functions corresponding directly to Unity text field actions
        // TODO: understand the cursed async behavior here
        
        public void OnTitleValueChanged() { }
        public void OnTitleEndEdit() => _ = FindAndSelectChemicalWithProperty(
            "Title", "name",titleText.text);
        public void OnTitleSelect() { }
        public void OnTitleDeselect() { }


        public void OnFormulaValueChanged() { }
        public void OnFormulaEndEdit() => _ = FindAndSelectChemicalWithProperty(
            "MolecularFormula", "fastformula",formulaText.text);
        public void OnFormulaSelect() { }
        public void OnFormulaDeselect() { }
        
        
        /// <summary>
        /// Searches for a chemical with the given property,
        /// and sets it as the currently selected chemical if successful.
        /// </summary>
        /// <param name="wordlePropertyName"></param>
        /// <param name="pubchemPropertyName"></param>
        /// <param name="propertyValue"></param>
        private async Task FindAndSelectChemicalWithProperty(
            string wordlePropertyName,
            string pubchemPropertyName,
            string propertyValue
        ) {
            var data = _generalDataController.GetChemicalWithProperty(
                wordlePropertyName, titleText.text);
            if (data == null)
            {
                // guess we're looking online
                var cids = await PubChemAPIManager.requestCIDsWithProperty(
                    pubchemPropertyName, propertyValue, 1);
                data = (await PubChemAPIManager.RequestChemicals(cids, GeneralDataController.DataTypes))?[0];
                if (data != null) _generalDataController.RegisterChemicalData(data);
            }

            if (data == null) return;
            _wordleManager.SetGuessingChemical(data);
        }

        /// <summary>
        /// Updates all the text displays to reflect the given chemical.
        /// </summary>
        /// <param name="chemicalData"> The chemical to display. </param>
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

        /// <summary>
        /// Exits the minigame and returns the user to the main menu.
        /// </summary>
        public void ExitGame() {
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }
        
    }
}
