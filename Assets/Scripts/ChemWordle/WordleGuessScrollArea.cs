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
        /// The layout bounds of this GUI object.
        /// This class needs access to its bounds because it
        /// programmatically resizes itself.
        /// </summary>
        public RectTransform rect;

        /// <summary>
        /// The vertical spacing between guess objects.
        /// This is set in-engine.
        /// </summary>
        public int spacing;

        /// <summary>
        /// Does a bunch of unintuitive math to resize enough to fit the given number of guesses.
        /// </summary>
        /// <param name="numGuesses"> How many guesses are going to be in the scroll area. </param>
        public void ExpandToAccommodate(int numGuesses)
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, numGuesses * 300 + 850);
    

            for (int i = 0; i < numGuesses - 1; i++)
            {
                rect.parent.position += new Vector3(0, -spacing, 0);
            }

            transform.position = new Vector3(transform.position.x, spacing/2.0f * numGuesses, transform.position.z);


        }

        public void scroll(Vector2 amt)
        {

        }

    }
}
