using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChemWordle
{
    
    /** Controls the dynamically spawned buttons which
     * represent any previously guessed chemicals.  */
    public class ForPrefabButton : MonoBehaviour
    {

        private GeneralDataController _generalDataController;

        private void Start()
        {
            _generalDataController = FindObjectOfType<GeneralDataController>();
        }

        /** Holds the CID of the chemical this button corresponds to. */
        private int _cid;
        public void SetCID(int cid) => _cid = cid;

        /** Sets the text displayed on the button. */
        public void SetText(string text) =>
            GetComponentInChildren<TextMeshProUGUI>().text = text;
        
        /** Called when this button is pressed.
         * Restores the associated chemical to the main interface. */
        public void OnPressed()
        {
            
            var wordleManager = FindObjectOfType<WordleManager>();
            
            // find the chemical with this CID
            var data = _generalDataController
                .GetChemicalWithProperty("CID", _cid.ToString());
            
            // load that chemical into the interface
            wordleManager.SetGuessingChemical(data);
            
        }
    
        /** Called when the "Play Again" button is pressed in the victory menu.
         * It's very important that the method stays spelled this way. */
        public void PLayAgian()
        {
            // TODO: move this method elsewhere
            
            // just reload the scene :)
            SceneManager.LoadScene("ChemicalWordle", LoadSceneMode.Single);
        }

        /** Called when the "Quit to Main" button is pressed in the victory menu. */
        public void QuitToMain()
        {
            // TODO: move this method elsewhere
            SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
        }

    }
    
}
