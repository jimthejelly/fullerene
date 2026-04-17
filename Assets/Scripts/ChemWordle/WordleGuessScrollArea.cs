using UnityEngine;

namespace ChemWordle
{
    
    /// <summary>
    /// Code for the GUI object that holds all of the prior guesses.
    /// Allows the user to scroll through their guesses, and dynamically
    /// resizes to accommodate new guesses.
    /// </summary>
    public class WordleGuessScrollArea : MonoBehaviour
    {
        
        /// <summary>
        /// A reference to the <see cref="ForPrefabButton">guess object prefab</see>.
        /// </summary>
        private ForPrefabButton _guessObject;
        
        /// <summary>
        /// The layout bounds of this GUI object.
        /// This class needs access to its bounds because it
        /// programmatically resizes itself.
        /// </summary>
        public RectTransform rect;

        /// <summary>
        /// The vertical spacing between guess objects.
        /// Measured as a fraction of the guess object size,
        /// since that varies with screen size.
        /// This is set in-engine.
        /// </summary>
        public float spacing;

        /// <summary>
        /// Stores where the object's parent transform was originally located.
        /// Used in expanding calculations to avoid the transform drifting over time.
        /// </summary>
        private Vector3 initialParentPosition;

        void Start()
        {
            _guessObject = FindObjectOfType<GUIController>().guessObjectPrefab.GetComponent<ForPrefabButton>();
            initialParentPosition = rect.position;
        }
        

        /// <summary>
        /// Does a bunch of unintuitive math to resize enough to fit the given number of guesses.
        /// </summary>
        /// <param name="numGuesses"> How many guesses are going to be in the scroll area. </param>
        public void ExpandToAccommodate(int numGuesses)
        {

            // TODO: guess object is created in the wrong position based on monitor size
            
            double guessObjectHeight = 400;
            Debug.Log(guessObjectHeight);
            
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (int)(numGuesses * (guessObjectHeight + spacing)) + 850);

            rect.position = initialParentPosition + new Vector3(0, -spacing/2.0f * numGuesses, 0);

            transform.position = new Vector3(transform.position.x, spacing/2.0f * numGuesses, transform.position.z);


        }

        public void scroll(Vector2 amt)
        {

        }

    }
}
