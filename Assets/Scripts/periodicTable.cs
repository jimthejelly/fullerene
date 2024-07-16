using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class periodicTable : MonoBehaviour
{
    public GameObject lanthanide;
    public GameObject actinide;

    public bool showedLanthanide;
    public bool showedActinide;

    // Start is called before the first frame update
    void Start()
    {
        lanthanide.SetActive(false);
        showedLanthanide = false;
        actinide.SetActive(false);
        showedActinide = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void lanthanideToggle()
    {
        if (showedLanthanide)
        {
            lanthanide.SetActive(false);
            showedLanthanide = false;
        }
        else
        {
            lanthanide.SetActive(true);
            showedLanthanide = true;
        }
    }

    public void actinideToggle()
    {
        if (showedActinide)
        {
            actinide.SetActive(false);
            showedActinide = false;
        }
        else
        {
            actinide.SetActive(true);
            showedActinide = true;
        }
    }
}