using UnityEngine;

namespace ChemWordle
{
    public class WordleGuessScrollArea : MonoBehaviour
    {
        public RectTransform rect;

        public GameObject parent;

        public int spacing;


        public void ExpandToAccommodate(int numGuesses)
        {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (numGuesses *300 ) + 850);


            for (int i = 0; i < numGuesses - 1; i++)
            {
                parent.transform.GetChild(i).transform.position += new Vector3(0, -spacing, 0);
            }

            transform.position = new Vector3(transform.position.x, spacing/2.0f * numGuesses, transform.position.z);


        }

        public void scroll(Vector2 amt)
        {

        }

    }
}
