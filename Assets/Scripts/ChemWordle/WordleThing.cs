using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordleThing : MonoBehaviour
{

    public TMPro.TextMeshProUGUI parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setText(string text)
    {
        parent.text = text;
    }

}
