using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace ChemWordle
{
    
    /** Handles the main game logic, like making and evaluating guesses. */
    public class WordleManager : MonoBehaviour
    {

        /** A reference to the GeneralDataController instance. */
        private GeneralDataController _generalDataController;
        
        /** A reference to the GUIController instance. */
        private GUIController _guiController;
        
        /** A reference to the WordleGuessScrollArea instance. */
        private WordleGuessScrollArea _wordleGuessScrollArea;

        /** Start is called before the first frame update */
        private void Start()
        {
            
            // initialize script instance references
            _generalDataController = FindObjectOfType<GeneralDataController>();
            _guiController = FindObjectOfType<GUIController>();
            _wordleGuessScrollArea = FindObjectOfType<WordleGuessScrollArea>();
        
            // can't await this, i think it'll crash the whole program
            // btw jetbrains shut up about it please
            // TODO: find the correct way to run this asynchronously
#pragma warning disable CS4014
            chooseMysteryChemical();
#pragma warning restore CS4014
        
        }


        private static bool MysteryChemicalSelectionCriterion(ChemicalData chemical)
        {
            return
                double.Parse(chemical.GetProperty("MolecularWeight")) < 200 &&
                chemical.GetProperty("Title").Length > 2 &&
                chemical.GetProperty("Title").Length < 14 &&
                chemical.GetProperty("MolecularFormula").Length < 16;
        }
    
        
        /** List of all guessed chemicals */
        private readonly List<string> _guesses = new();
        public bool AlreadyGuessed(string guess) => _guesses.Contains(guess);

        /** The chemical currently entered into the GUI */
        private ChemicalData _guessingChemical;
        public void SetGuessingChemical(ChemicalData chemical)
        {
            _guessingChemical = chemical;
            _guiController.set(chemical);
        }

        private ChemicalData _mysteryChemical;

        
        /** Queries for and selects the mystery chemical. */
        private async Task chooseMysteryChemical()
        {
        
            // make a list of a bunch of cids
            List<int> possibleCIDs = new();
            for (var i = 100; i < 5000; i++)
            {
                possibleCIDs.Add(i);
            }
            List<int> cids = new();
            for (var i = 0; i < 500; i++)
            {
                var idx = new Random().Next(0, possibleCIDs.Count);
                cids.Add(possibleCIDs[idx]);
                possibleCIDs.RemoveAt(idx);
            }

            // request and register them!
            var results = await PubChemAPIManager.RequestChemicals(cids, GeneralDataController.DataTypes);
            foreach (var data in results) _generalDataController.RegisterChemicalData(data);
            
            // and then select a random chemical, below the weight limit, to be the mystery chemical.
            // TODO: work on the mystery chemical selection function
            var validChemicals = (
                from data in results
                where MysteryChemicalSelectionCriterion(data)
                select data
            ).ToList();
            _mysteryChemical = validChemicals[new Random().Next(validChemicals.Count)];
            
            // sneaky debug statement
            // Debug.Log("mystery chemical: " + _mysteryChemical);
            
        }

        private void VICTORY()
        {
            var DoubleU = Instantiate(_guiController.victoryPrefab, _guiController.gui.transform);
        
            DoubleU.transform.localPosition = new Vector3(0, 0, 0);
            DoubleU.transform.localScale.Set(2, 2, 1);
        }

        /** Guesses the currently entered chemical. */
        public void MakeGuess()
        {

            // Create a label to represent the guess
            var guessObject = Instantiate(_guiController.guessObjectPrefab, _guiController.guessesListedHere.transform.GetChild(0));
            guessObject.GetComponent<ForPrefabButton>().SetCID(int.Parse(_guessingChemical.GetProperty("CID")));
            guessObject.GetComponent<ForPrefabButton>().SetText(_guessingChemical.GetProperty("Title"));

            
            _guesses.Add(_guessingChemical.GetProperty("Title"));

            // Move it to the appropriate position
            guessObject.transform.position +=
                new Vector3(0, 1, 0) * _guesses.Count * _wordleGuessScrollArea.spacing;

            _wordleGuessScrollArea.ExpandToAccommodate(_guesses.Count);
            
            EvaluateGuess(_guessingChemical);
        
            _guiController.SetGuessButtonEnabled(false);
            
        }

        private static bool IsNumeric(char c)
        {
            return "0123456789".Contains(c);
        }
        private static bool IsLowercase(char c)
        {
            return "abcdefghijklmnopqrstuvwxyz".Contains(c);
        }

        public void EvaluateGuess(ChemicalData guessing)
        {

            var feedback = "";
            int guessCharge = Convert.ToInt32(guessing.GetProperty("Charge")), actualCharge = Convert.ToInt32(_mysteryChemical.GetProperty("Charge"));
            double guessWeight = Convert.ToDouble(guessing.GetProperty("MolecularWeight")), actualWeight = Convert.ToDouble(_mysteryChemical.GetProperty("MolecularWeight"));
            var guessingFormula = guessing.GetProperty("MolecularFormula");
            var actualFormula = _mysteryChemical.GetProperty("MolecularFormula");

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
            _guiController.SetWeightFeedback("");
            _guiController.SetChargeFeedback("");
        }

    
    }
}
