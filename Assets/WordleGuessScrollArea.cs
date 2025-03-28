using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordleGuessScrollArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public RectTransform rect;

    public void ExpandToAccommodate(int numGuesses)
    {
        rect.anchorMin = new Vector2(0, -1.75f * numGuesses);
        rect.anchorMax = new Vector2(1, -0.75f * numGuesses);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 120 + 20 * numGuesses);
    }

    public void scroll(Vector2 amt)
    {

    }

}
