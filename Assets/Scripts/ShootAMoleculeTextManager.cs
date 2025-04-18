using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootAMoleculeTextManager : MonoBehaviour
{

    [SerializeField] GameObject GunEditor;
    [SerializeField] GameObject IntroParagraphCanvas;
    [SerializeField] GameObject AddMoleculeCanvas;
    [SerializeField] GameObject ConstantsCanvas;
    ArrayList molecules;
    TextMeshProUGUI IntroParagraph;

    int Paragraph = 0;
    string Introparagraph0 = "Welcome to Shoot the Molecule!\r\n\r\nHere you can practice your memory and quick thinking by shooting MoleculeAsteroids before they hit Earth and break the Ozone Layer Even more!";
    string Introparagraph1 = "When the molecules start falling from the sky, type their name or their makeup into the Compound Neutralizing Ray (CNR).\r\nWhen the CNR is loaded with the proper, press the Left 'Alt' to fire the neutralizing ray!";
    string Introparagraph2 = "If you think the CNR has been loaded incorrectly, don't panic, you can quickly clear the loaded ray by pressing 'Tab'.";

    // Start is called before the first frame update
    void Start()
    {
        IntroParagraph = IntroParagraphCanvas.GetComponentInChildren<TextMeshProUGUI>();

        AddMoleculeCanvas.SetActive(false);
        ConstantsCanvas.SetActive(false);
        IntroParagraph.GetComponent<TextMeshProUGUI>().text = Introparagraph0;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IntroText()    //parsing intro text at the beginning of the minigame @ startup
    { 
        Paragraph++;


        if (Paragraph == 3) {
            AddMoleculePanel();
        }
        else {
            if (Paragraph == 1)
            {
                IntroParagraph.GetComponent<TextMeshProUGUI>().text = Introparagraph1;
            }
            else 
            {
                IntroParagraph.GetComponent<TextMeshProUGUI>().text = Introparagraph2;
            }
        }


    }

    public void AddMoleculePanel()
    {
        AddMoleculeCanvas.SetActive(true);
        IntroParagraphCanvas.SetActive(false);
    }





}
