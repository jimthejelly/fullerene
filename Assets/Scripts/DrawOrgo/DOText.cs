using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class DOText : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject ParagraphObject;
    [SerializeField] GameObject showElement;
    [SerializeField] GameObject showFunction;
    TextMeshProUGUI Paragraph;
    GameObject mainScript;
    DrawOrgo script;

    static string[] possibleElements = { "Hydrogen", "Lithium", "Sodium", "Potassium", "Magnesium", "Boron", "Aluminum", "Carbon", "Nitrogen", "Phosphorus", "Oxygen", "Sulfur", "Flourine", "Clorine", "Bromine", "Iodine" };

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
    string p9 = "Lets start going over the controls of the game. At the bototm of the screen, you will see 3 buttons";
    string p10 = "The first button says \"Place Element\". This button allows you to choose any element in Organic Chemistry, and primes the game to let you right click anywhere on the screen.\n\n" +
                           "This will create a small circle that represents an element.";
    string p11 = "Each circle will have a set of smaller circles that surround it. These represent available or open bonds\n\nYou can click on them to create a new element and bond it to the original.";
    string p12 = "If you want to bond different elements, place the first element and then choose another one by pressing on the \"Place Elements\" button.\n\n" +
                            "This will allow you to change your selected element and now when you add a bond to the original, the new element will be added..";
    string p13 = "As you create new bonds, the circles will dissapear and will be replaced by lines. These simply represent the bond, as bonds imply elements. \n\nThe smaller circles will still appear.";
    string p14 = "If you ever feel like you made a mistake or want to remove clutter, you can always press on the \"Delete Elements\" button. This will prime the game to remove any element you click on.";
    string p15 = "Lastly, if you feel as if the bonds are confusing or are overlaping, you can click on the \"Organize Elements\".\n\n" +
                                    "This will cause the element to stick to your mouse curser, and you can \'let go\' by right clicking anywhere.";
    string p16 = "That's it! \n\nYou are now ready to play the game! When you click next, the game will begin and you can try to make any molecule you want! \n\nHave fun!.";
    string[] paragraphs = new string[16]; 
    int paragraph = -1;

    GameObject canvas;
    GameObject pt1;         //prefixTables
    GameObject pt2;

    bool PrefixTable = false;
    void Start()
    {
        mainScript = GameObject.Find("script");
        script = mainScript.GetComponent<DrawOrgo>();
        canvas = GameObject.Find("Canvas");
        canvas.transform.GetChild(1).gameObject.SetActive(false);
        canvas.transform.GetChild(2).gameObject.SetActive(false);
        pt1 = GameObject.Find("PrefixTable1");
        pt2 = GameObject.Find("PrefixTable2");
        pt1.SetActive(false);
        pt2.SetActive(false);
        showElement.SetActive(false);
        showFunction.SetActive(false);
        Paragraph = ParagraphObject.GetComponent<TextMeshProUGUI>();
        paragraphs[0] = p1;
        paragraphs[1] = p2;
        paragraphs[2] = p3;
        paragraphs[3] = p4;
        paragraphs[4] = p5;
        paragraphs[5] = p6;
        paragraphs[6] = p7;
        paragraphs[7] = p8;
        paragraphs[8] = p9;
        paragraphs[9] = p10;
        paragraphs[10] = p11;
        paragraphs[11] = p12;
        paragraphs[12] = p13;
        paragraphs[13] = p14;
        paragraphs[14] = p15;
        paragraphs[15] = p16;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayPrefixTable();
        }

        
    }

    public void UpdateTextUI()
    {
        showElement.GetComponent<TextMeshProUGUI>().text = "Selected Element: " + possibleElements[script.selectedElement];
        showFunction.GetComponent<TextMeshProUGUI>().text = "Function: " + script.Function;
    }


    public void NextParagraph() {
        paragraph++;
        if (paragraph < paragraphs.Length)
        {
            Paragraph.text = paragraphs[paragraph];
        }else
        {
            canvas.transform.GetChild(0).gameObject.SetActive(false);
            canvas.transform.GetChild(1).gameObject.SetActive(true);
            showFunction.SetActive(true);
            showElement.SetActive(true);
            UpdateTextUI();
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
        canvas.transform.GetChild(0).gameObject.SetActive(false);
        canvas.transform.GetChild(1).gameObject.SetActive(true);
        paragraph = -1;
        showFunction.SetActive(true);
        showElement.SetActive(true);
        UpdateTextUI();

    }


}
