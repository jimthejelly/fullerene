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

    private List<GameObject> bondedElements;
    private List<GameObject> bonds;

    string Element = "";
    int startingB = 0;
    int numberOfBonds = 0;
    int MaxBonds = 8;
    bool Active = false;
    Vector3 mousepos;

    [SerializeField] GameObject elementPrefab;

    GameObject mainScript;

    private void Awake()
    {
        elementLooks = this.GetComponent<SpriteRenderer>();
        elementLooks.color = ElementColor;
        bondedElements = new List<GameObject>();
        bonds = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {

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
        if (numberOfBonds > 0 && (Element == "Carbon" || Element == "Hydrogen" || Element == "Oxygen"))
        {
            elementLooks.color = invisible;
        }
        else
        {
            elementLooks.color = ElementColor;
        }
    }

    public void setElement(int index)
    {
        Element = possibleElements[index];
        if (index == 0)     //Hydrogen
        {
            MaxBonds = 2;
            elementLooks.color = Color.gray;
            ElementColor = Color.gray;
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
        else if (index < 7)    //Boron, Aluminum
        {
            startingB = 3;
        }
        else if (index == 8)    //Carbon
        {
            startingB = 4;
            Element = "Carbon";
            elementLooks.color = Color.black;
            ElementColor = Color.black;

        }
        else if (index < 11)    //Nitrogen, Phosphorus
        {
            startingB = 5;
        }
        else if (index < 13) // Oxygen, Sulfer
        {

            startingB = 6;
            print(index);
            if (index == 11) {
                Element = "Oxygen";
                elementLooks.color = Color.red;
                ElementColor = Color.red;
            }

        }
        else                //Florine, Chlorine, Bromine, Iodine
        {
            print(index);
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
                Vector3 newElementLocation = new Vector3(x,y, 0);
                newElementLocation = newElementLocation.normalized * 25;
                GameObject newElement = Instantiate(elementPrefab);
                newElement.GetComponent<Transform>().position = newElementLocation + transform.position;
                newElement.GetComponent<ElementBehavoir>().setElement(mainScript.GetComponent<DrawOrgo>().selectedElement);
                newElement.GetComponent<ElementBehavoir>().incrementBond();
                
                bondedElements.Add(newElement);

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

    public void addBond(GameObject bond)
    {
        bonds.Add(bond);
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

    private void OnMouseOver()
    {
        if (mainScript.GetComponent<DrawOrgo>().Function == "Remove" && Input.GetMouseButtonDown(0)) 
        {
            if (numberOfBonds > 0)
            {
                for (int i = 0;i < numberOfBonds; i++)
                {
                    Destroy(bonds[i].gameObject);
                    bondedElements[i].GetComponent<ElementBehavoir>().decrementBond();
                    
                }
            }
            Destroy(gameObject);
        }
    }


    /*
    private void OnMouseExit()
    {
        if (numberOfBonds + startingB <= MaxBonds)
        {
            for (int i = numberOfBonds; i < MaxBonds + 1; i++)
            {
                //Each element has 8 hidden buttons,
                //only display as many buttons as available bonds

                ElementMaterial = GetComponent<Material>();
                for (int j = 0; j < MaxBonds; j++)
                {
                    transform.GetChild(j).gameObject.SetActive(false);
                }


            }
        }
    }

    */

    public void incrementBond()
    {
        numberOfBonds += 1;
    }

    public void decrementBond()
    {
        numberOfBonds -= 1;
    }

}
