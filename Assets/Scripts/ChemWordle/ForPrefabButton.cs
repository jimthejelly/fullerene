using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChemWordle
{

    /// <summary>
    /// Controls the dynamically spawned buttons which represent any previously guessed chemicals.
    /// These currently live on the right side of the screen in that big scrolling list.
    /// </summary>
    public class ForPrefabButton : MonoBehaviour
    {

        private GeneralDataController _generalDataController;

        void Start()
        {
            _generalDataController = FindObjectOfType<GeneralDataController>();
        }

        /// <summary>
        /// Holds the CID of the chemical this button corresponds to.
        /// </summary>
        private int _cid;
        public void SetCID(int cid) => _cid = cid;

        /// <summary>
        /// Sets the text displayed on the button.
        /// This will probably just be the title of whatever chemical the button is for.
        /// </summary>
        public void SetText(string text) =>
            GetComponentInChildren<TextMeshProUGUI>().text = text;
        
        /// <summary>
        /// Called when this button is pressed.
        /// Puts this button's associated chemical back onto the main interface.
        /// </summary>
        public void OnPressed()
        {
            
            var wordleManager = FindObjectOfType<WordleManager>();
            
            // find the chemical with this CID
            var data = _generalDataController
                .GetChemicalWithProperty("CID", _cid.ToString());
            
            // load that chemical into the interface
            wordleManager.SetGuessingChemical(data);
            
        }
    
        /// <summary>
        /// Called when the "Play Again" button is pressed in the victory menu.
        /// Restarts the minigame by reloading the Unity scene.
        /// It's very important that the method stays spelled this way.
        /// </summary>
        public void PLayAgian()
        {
            // TODO: move this method elsewhere
            
            // just reload the scene :)
            SceneManager.LoadScene("ChemicalWordle", LoadSceneMode.Single);
        }

        /// <summary>
        /// Called when the "Quit to Main" button is pressed in the victory menu.
        /// Sends the user back to the main menu.
        /// </summary>
        public void QuitToMain()
        {
            // TODO: move this method elsewhere
            // also why are there two methods to quit to main??
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }

    }
    
}
