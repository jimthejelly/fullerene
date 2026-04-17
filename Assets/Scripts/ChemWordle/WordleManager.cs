using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace ChemWordle
{
    
    /// <summary>
    /// Handles the main game logic, like making and evaluating guesses.
    /// </summary>
    public class WordleManager : MonoBehaviour
    {

        /// <summary>
        /// A reference to the <see cref="GeneralDataController">GeneralDataController</see>.
        /// </summary>
        private GeneralDataController _generalDataController;
        
        /// <summary>
        /// A reference to the <see cref="GUIController">GUIController</see>.
        /// </summary>
        private GUIController _guiController;
        
        /// <summary>
        /// A reference to the <see cref="WordleGuessScrollArea">WordleGuessScrollArea</see>.
        /// </summary>
        private WordleGuessScrollArea _wordleGuessScrollArea;

        
        void Start()
        {
            
            // initialize script instance references
            _generalDataController = FindObjectOfType<GeneralDataController>();
            _guiController = FindObjectOfType<GUIController>();
            _wordleGuessScrollArea = FindObjectOfType<WordleGuessScrollArea>();
        
            // _ = chooseMysteryChemical();

            cba();

        }

        async void cba()
        {
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    await chooseMysteryChemical();
                    Debug.Log(_mysteryChemical);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Given a random chemical, determine if it's reasonable to make this the mystery chemical.
        /// </summary>
        /// <param name="chemical"> The chemical to evaluate. </param>
        /// <returns> Whether this can be the mystery chemical. </returns>
        private static bool MysteryChemicalSelectionCriterion(ChemicalData chemical)
        {
            var molecularWeight = double.Parse(RequireProperty(chemical, "MolecularWeight"));
            var title = RequireProperty(chemical, "Title");
            var molecularFormula = RequireProperty(chemical, "MolecularFormula");
            var charge = int.Parse(RequireProperty(chemical, "Charge"));
            
            // roughly parse formula for total amount elements occur
            // just sum all numbers present in string -
            // will misrepresent some chemicals but should be good enough
            var totalNumberOfAtoms = 0;
            var intermediateString = ""; // add digits to this to build large numbers
            foreach (var c in molecularFormula)
            {
                if (IsNumeric(c))
                {
                    intermediateString += c;
                }
                else
                {
                    // register built string as number
                    if (intermediateString.Any(IsNumeric) &&
                        int.TryParse(intermediateString, out var intermediateValue))
                    {
                        totalNumberOfAtoms += intermediateValue;
                    }

                    // reset the string
                    intermediateString = "";
                }
            }

            var longestWordInTitle = 0;
            var wordTracker = 0;
            foreach (var c in title)
            {
                if (c == ' ')
                {
                    wordTracker = 0;
                }
                else
                {
                    wordTracker++;
                    if (wordTracker > longestWordInTitle)
                    {
                        longestWordInTitle = wordTracker;
                    }
                }
            }

            return
                molecularWeight < 160 &&
                longestWordInTitle is > 2 and < 10 &&
                totalNumberOfAtoms < 10 &&
                totalNumberOfAtoms > 1;
        }
    
		/// <summary>
		/// A list of the names of all guessed chemicals so far.
		/// </summary>
        private readonly List<string> _guesses = new();
        
        /// <returns> Whether the given string is already present in the list of guesses. </returns>
        public bool AlreadyGuessed(string guess) => _guesses.Contains(guess);

        /// <summary>
        /// The chemical currently displayed in the GUI.
        /// Pressing the "guess" button will guess this chemical.
        /// </summary>
        private ChemicalData _guessingChemical;
        
        /// <summary>
        /// Sets the currently guessing chemical, both internally and in the GUI.
        /// </summary>
        public void SetGuessingChemical(ChemicalData chemical)
        {
            _guessingChemical = chemical;
            _guiController.set(chemical);
        }

        /// <summary>
        /// The chemical the user is trying to guess!
        /// This should only be set when the minigame starts.
        /// </summary>
        private ChemicalData _mysteryChemical;

        /// <summary>
        /// Looks online for a suitable mystery chemical, and selects it.
        /// </summary>
        private async Task chooseMysteryChemical()
        {
        
            // make a list of a bunch of cids
            List<int> possibleCIDs = new();
            for (var i = 1; i < 5000; i++)
            {
                possibleCIDs.Add(i);
            }
            
            // break query into multiple small queries so each one fits in an http header
            // TODO: put all of the desired CIDs in the body of one request instead
            List<List<int>> cids = new();
            List<int> smallCIDs = new();
            int i2 = 0;
            for (var i = 0; i < 2000; i++)
            {
                var idx = new Random().Next(0, possibleCIDs.Count);
                smallCIDs.Add(possibleCIDs[idx]);
                possibleCIDs.RemoveAt(idx);
                i2++;
                if (i2 > 500)
                {
                    cids.Add(smallCIDs);
                    smallCIDs = new List<int>();
                    i2 -= 500;
                }
            }

            List<ChemicalData> results = new();
            // request and register them!
            foreach (var smallCIDsAgain in cids)
            {
                var theseResults = await PubChemAPIManager.RequestChemicals(smallCIDsAgain, GeneralDataController.DataTypes);
                if (theseResults == null)
                {
                    // TODO: remember how to print a list properly
                    Debug.LogError("Failed request for mystery chemical: " + smallCIDs);
                    continue;
                }
                results.AddRange(theseResults);
            }
            
            // cache everything found
            foreach (var data in results) _generalDataController.RegisterChemicalData(data);
            
            // and then select a random chemical, below the weight limit, to be the mystery chemical.
            // TODO: work on the mystery chemical selection function
            var validChemicals = (
                from data in results
                where MysteryChemicalSelectionCriterion(data)
                select data
            ).ToList();
            if (validChemicals.Count == 0)
            {
                // no valid chemicals! abort
                _mysteryChemical = null;
                throw new Exception("No valid mystery chemical found");
            }
            _mysteryChemical = validChemicals[new Random().Next(validChemicals.Count)];
            
            foreach (var data in validChemicals)
            {
                Debug.Log(data);
            }
            // sneaky debug statement
            // Debug.Log("mystery chemical: " + _mysteryChemical);
            
        }


        private void VICTORY()
        {
            var DoubleU = Instantiate(_guiController.victoryPrefab, _guiController.gui.transform);
        
            DoubleU.transform.localPosition = new Vector3(0, 0, 0);
            DoubleU.transform.localScale.Set(2, 2, 1);
        }

        /// <summary>
        /// Guesses the currently entered chemical.
        /// </summary>
        public void MakeGuess()
        {

            // Create a label to represent the guess
            var guessObject = Instantiate(_guiController.guessObjectPrefab, _guiController.guessesListedHere.transform.GetChild(0));
            guessObject.GetComponent<ForPrefabButton>().SetCID(int.Parse(RequireProperty(_guessingChemical, "CID")));
            guessObject.GetComponent<ForPrefabButton>().SetText(_guessingChemical.GetProperty("Title"));

            
            _guesses.Add(_guessingChemical.GetProperty("Title"));

            // Move it to the appropriate position
            guessObject.transform.position +=
                new Vector3(0, 1, 0) * _guesses.Count * _wordleGuessScrollArea.spacing;

            _wordleGuessScrollArea.ExpandToAccommodate(_guesses.Count);
            
            EvaluateGuess(_guessingChemical);
        
            _guiController.SetGuessButtonEnabled(false);
            
        }
        
        
        /// <summary>
        /// Searches for a chemical with the given property,
        /// and sets it as the currently selected chemical if successful.
        /// </summary>
        /// <param name="wordlePropertyName"> The Fullerene internal name for the property. </param>
        /// <param name="pubchemPropertyName"> The PubChem internal name for the property. </param>
        /// <param name="propertyValue"> The value of the property. </param>
        public async Task FindAndSelectChemicalWithProperty(
            string wordlePropertyName,
            string pubchemPropertyName,
            string propertyValue
        ) {
            var data = _generalDataController.GetChemicalWithProperty(
                wordlePropertyName, propertyValue);
            if (data == null)
            {
                // guess we're looking online
                var cids = await PubChemAPIManager.requestCIDsWithProperty(
                    pubchemPropertyName, propertyValue, 1);
                data = (await PubChemAPIManager.RequestChemicals(cids, GeneralDataController.DataTypes))?[0];
                if (data != null) _generalDataController.RegisterChemicalData(data);
            }

            if (data == null) return;
            SetGuessingChemical(data);
        }

        private static bool IsNumeric(char c)
        {
            return "0123456789".Contains(c);
        }
        private static bool IsLowercase(char c)
        {
            return "abcdefghijklmnopqrstuvwxyz".Contains(c);
        }

        /// <summary>
        /// Automatically asserts that the given property is present in the given chemical.
        /// This is mostly to prevent a million "might be null" warnings.
        /// </summary>
        /// <returns> The property value. </returns>
        /// <exception cref="Exception"> If the property actually isn't present.
        /// This should never be the case if you're calling this function. </exception>
        private static string RequireProperty([NotNull] ChemicalData chemical, string propertyName)
        {
            return chemical.GetProperty(propertyName)
                !?? throw new Exception("Missing required property \"" + propertyName + "\"");
        }

        /// <summary>
        /// Compares the given chemical against the mystery chemical,
        /// and sends the appropriate feedback to the GUI Controller.
        /// </summary>
        /// <param name="guessing"> The proposed chemical. </param>
        public void EvaluateGuess([NotNull] ChemicalData guessing)
        {

            var feedback = "";
            int guessCharge = Convert.ToInt32(RequireProperty(guessing, "Charge")),
                actualCharge = Convert.ToInt32(RequireProperty(_mysteryChemical, "Charge"));
            double guessWeight = Convert.ToDouble(RequireProperty(guessing, "MolecularWeight")),
                actualWeight = Convert.ToDouble(RequireProperty(_mysteryChemical, "MolecularWeight"));
            string guessingFormula = RequireProperty(guessing, "MolecularFormula"),
                actualFormula = RequireProperty(_mysteryChemical, "MolecularFormula");

            for (var i = 0; i < guessingFormula.Length; i++)
            {
                var c = guessingFormula[i];
                if (IsNumeric(c) || c == '-' || c == '+' || IsLowercase(c)) continue;
                var elementWordBuilder = "" + c;
                for (var j = i + 1; j < guessingFormula.Length; j++)
                {
                    if (IsLowercase(guessingFormula[j]))
                    {
                        elementWordBuilder += guessingFormula[j];
                    }
                    else break;
                }
                if (actualFormula.Contains(elementWordBuilder))
                {
                    var count1 = 0;
                    for (var j = i + elementWordBuilder.Length; j < guessingFormula.Length; j++)
                    {
                        var c2 = guessingFormula[j];
                        if (!IsNumeric(c2)) break;
                        count1 = count1 * 10 + int.Parse("" + c2);
                    }
                    if (count1 == 0) count1 = 1;
                    var count2 = 0;
                    for (var j = actualFormula.IndexOf(elementWordBuilder, StringComparison.Ordinal) + elementWordBuilder.Length; j < actualFormula.Length; j++)
                    {
                        var c2 = actualFormula[j];
                        if (!IsNumeric(c2)) break;
                        count2 = count2 * 10 + int.Parse("" + c2);
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

            feedback = actualFormula
                .Where(c => !IsNumeric(c) && c != '-' && c != '+')
                .Where(c => !guessingFormula.Contains(c) && !IsLowercase(c))
                .Aggregate(feedback, (current, _) => current + "An element is missing" + "\n");
            
            _guiController.SetFormulaFeedback(feedback);
            
            if (Math.Abs(actualWeight - guessWeight) < 0.00001f)
            {
                _guiController.SetWeightFeedback("Correct");
            }
            else if (actualWeight > guessWeight)
            {
                _guiController.SetWeightFeedback("Too Light");
            }
            else
            {
                _guiController.SetWeightFeedback("Too Heavy");
            }
            if (actualCharge == guessCharge)
            {
                _guiController.SetChargeFeedback("Correct");
            } else if (actualCharge > guessCharge) {
                _guiController.SetChargeFeedback("Too Negative");
            } else
            {
                _guiController.SetChargeFeedback("Too Positive");
            }
            if (actualFormula == guessingFormula)
            {
                VICTORY();
            }

        }

        public void clearFeedBackText()
        {
            _guiController.SetFormulaFeedback("");
            // _guiController.SetWeightFeedback("");
            // _guiController.SetChargeFeedback("");
        }
    
        /// <summary>
        /// Called when the "Play Again" button is pressed in the victory menu.
        /// Restarts the minigame by reloading the Unity scene.
        /// It's very important that the method stays spelled this way.
        /// </summary>
        public static void PLayAgian()
        {
            // TODO: move this method elsewhere
            
            // just reload the scene :)
            SceneManager.LoadScene("ChemicalWordle", LoadSceneMode.Single);
        }

        /// <summary>
        /// Called when the "Quit to Main" button is pressed in the victory menu.
        /// Sends the user back to the main menu.
        /// </summary>
        public static void QuitToMain()
        {
            // TODO: move this method elsewhere
            // also why are there two methods to quit to main??
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }

    
    }
}
