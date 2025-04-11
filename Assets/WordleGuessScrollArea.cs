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

    public GameObject parent;
    public GameObject actualParent;

    public int spacing;

    public void ExpandToAccommodate(int numGuesses)
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, spacing * numGuesses);


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
