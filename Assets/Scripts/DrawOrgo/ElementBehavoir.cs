using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBehavoir : MonoBehaviour
{
    [SerializeField] GameObject bondLine;
    [SerializeField] Color ElementColor;
    private SpriteRenderer elementLooks;
    private Color invisible = new Color(0, 0, 0, 0);
    [SerializeField] Material ElementMaterial = null;
    static string[] possibleElements = { "Hydrogen", "Lithium", "Sodium", "Potassium", "Magnesium", "Boron", "Aluminum", "Carbon", "Nitrogen","Phosphorus", "Oxygen", "Sulfur", "Flourine", "Clorine", "Bromine", "Iodine"};

    string Element = "";
    int startingB = 0;
    int numberOfBonds = 0;
    int MaxBonds = 8;
    bool Active = false;
    Vector3 mousepos;

    [SerializeField] GameObject elementPrefab;

    GameObject mainScript;
    // Start is called before the first frame update
    void Start()
    {
        elementLooks = this.GetComponent<SpriteRenderer>();
        elementLooks.color = ElementColor;
        mainScript = GameObject.Find("script");
        ElementMaterial = this.GetComponent<Material>();
        for (int i = 0; i < 8; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        setElement(mainScript.GetComponent<DrawOrgo>().selectedElement);

    }

    // Update is called once per frame
    void Update()
    {
        mousepos = Input.mousePosition;
        if (numberOfBonds > 0 && (Element == "Carbon" || Element == "Hydrogen"))
        {
            elementLooks.color = invisible;
        }
    }

    public void setElement(int index)
    {
        Element = possibleElements[index];
        if (index == 0)     //Hydrogen
        {
            MaxBonds = 2;
            elementLooks.color = Color.gray;
            Element = "Hydrogen";
        }
        if (index < 4)  //Lithium, Sodium, Potassium
        {
            startingB = 1;
        }
        else if (index == 4)    //Magnesium
        {
            startingB = 2;
            Element = "Magnesium";
        }
        else if (index <  7)    //Boron, Aluminum
        {
            startingB = 3;
        }
        else if (index == 8)    //Carbon
        {
            startingB = 4;
            Element = "Carbon";
            elementLooks.color = Color.black;

        }
        else if (index < 11)    //Nitrogen, Phosphorus
        {
            startingB = 5;
        }
        else if (index < 13) // Oxygen, Sulfer
        {
            startingB = 6;
        }
        else                //Florine, Chlorine, Bromine, Iodine
        {
            startingB = 7;
        }

        Active = true;

    }

    public Color GetColor()
    {
        return ElementColor;
    }

    public void forgeBond(Vector3 bondLocation)
    {
        if (numberOfBonds + startingB <= MaxBonds)
        {
            if (mainScript.GetComponent<DrawOrgo>().selectedElement > -1)
            {

                float x, y;
                x = bondLocation.x;
                y = bondLocation.y;
                print(bondLocation);
                Vector3 newElementLocation = new Vector3(x,y, 0);
                newElementLocation = newElementLocation.normalized * 15;
                print(newElementLocation);
                GameObject newElement = Instantiate(elementPrefab);
                newElement.GetComponent<Transform>().position = newElementLocation + transform.position;
                newElement.GetComponent<ElementBehavoir>().incrementBond();

                mainScript.GetComponent<DrawOrgo>().drawBond(gameObject, newElement);

                numberOfBonds++;

                updateBonds();
                
            }
        }
        else
        {
            print("this element already has too many bonds");
        }
        
    }

    public void updateBonds()
    {
        for (int i = 0; i < 8; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }


    private void OnMouseEnter()
    {
        if (numberOfBonds + startingB <= MaxBonds)
        {
            for (int i = numberOfBonds; i < MaxBonds+1; i++)
            {
                //Each element has 8 hidden buttons,
                //only display as many buttons as available bonds

                ElementMaterial = GetComponent<Material>();
                for (int j = numberOfBonds + startingB; j < MaxBonds; j++)
                {
                    transform.GetChild(j).gameObject.SetActive(true);
                }


            }
        }
    }


    public void incrementBond()
    {
        numberOfBonds += 1;
    }

}
