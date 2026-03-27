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

    }
    
}
