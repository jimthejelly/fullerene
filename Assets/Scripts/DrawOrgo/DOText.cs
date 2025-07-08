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
    string p3 = "That just means that each molecule has 3 main parts and that we'll teach you how to discern what they mean.";
    string p4 = "Alkanes are groups of molecules with no functional double or triple bonds, so it's easier to think about how many atoms each atom is next to.\n\n" +
                                "That being said, most drawings of organic chemicals don't include the Hydrogen atoms because they are always dead ends.";
    string p5 = "The first aspect of naming organic molecules is the \'prefix\'.\n\nPrefixes in organic chemestry tell us the number of Carbon atoms within the molecular structure.\nThey range from meth-1 to dec-10.";
    string p6 = "This is a table you can refer to for the whole range.";
    string p7 = "If you ever forget, you can pull up the prefix table again by pressing \'p\'.\n\nNow lets talk about the rest of body of the molecule.";
    string p8 = "The Suffix is just as important in describing the molecule.\n\nThe suffix contains both the highest level of bonds, and the kind of group a molecule belongs to.";
    string[] paragraphs = new string[8]; 
    int paragraph = -1;

    GameObject canvas;
    GameObject pt1;         //prefixTables
    GameObject pt2;

    bool PrefixTable = false;
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        pt1 = GameObject.Find("PrefixTable1");
        pt2 = GameObject.Find("PrefixTable2");
        pt1.SetActive(false);
        pt2.SetActive(false);
        Paragraph = ParagraphObject.GetComponent<TextMeshProUGUI>();
        paragraphs[0] = p1;
        paragraphs[1] = p2;
        paragraphs[2] = p3;
        paragraphs[3] = p4;
        paragraphs[4] = p5;
        paragraphs[5] = p6;
        paragraphs[6] = p7;
        paragraphs[7] = p8;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPrefixTable();
        }
        
    }

    public void NextParagraph() {
        paragraph++;
        if (paragraph < paragraphs.Length)
        {
            Paragraph.text = paragraphs[paragraph];
        }else
        {
            canvas.SetActive(false);
        }
        if (paragraph == 5)
        {
            pt1.SetActive(true);
            pt2.SetActive(true);
        } else
        {
            pt1.SetActive (false);
            pt2.SetActive (false);
        }
    }

    public void DisplayPrefixTable()
    {
        if (PrefixTable)        //table is showing and we need to hide it
        {
            pt1.SetActive(false);
            pt2.SetActive(false);
            PrefixTable = false;
        } else                  //table is hidden and we need to show it
        {
            PrefixTable = true;
            pt1.SetActive(true);
            pt2.SetActive(true);
        }
    }

    public void SkipIntro()
    {
        canvas.SetActive(false);
        paragraph = -1;
        
    }


}
