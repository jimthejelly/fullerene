using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject ParagraphObject;
    TextMeshProUGUI Paragraph;

    string p1 = "This minigame will help teach the effectivness of IUPAC naming for the Molecules we're gonna throw at you.\n\nBut first, lets go over those naming rules.";
    string p2 = "To start: IUPAC has 3 core tenants:\n\n1) A root or base that indicates the major ring/chain of carbon atoms.\n2) A suffix or element that designates a group that can be present.\n" +
                                "3) Names of substituant groups that complete the structure other than hydrogen.";
    string p3 = "That just means that each molecule has 3 main parts and that we'll teach you how to discern what they mean.\n\nFor now, we'll start off on easier molecules to figure out called Alkanes";
    string p4 = "Alkanes are groups of molecules with no functional double or triple bonds, so it's easier to think about how many atoms each atom is next to.\n\n" +
                                "That being said, most drawings of organic chemicals don't include the Hydrogen atoms because they are always dead ends.";
    string p5 = "";
    string p6 = "";
    string p7 = "";
    string p8 = "";
    string[] paragraphs = new string[4]; 
    int paragraph = -1;

    GameObject canvas;
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        Paragraph = ParagraphObject.GetComponent<TextMeshProUGUI>();
        paragraphs[0] = p1;
        paragraphs[1] = p2;
        paragraphs[2] = p3;
        paragraphs[3] = p4;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextParagraph() {
        paragraph++;
        if (paragraph < paragraphs.Length)
        {
            Paragraph.text = paragraphs[paragraph];
        }
    }

    public void SkipIntro()
    {
        canvas.SetActive(false);
    }


}
