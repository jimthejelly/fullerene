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

    public List<GameObject> bondedElements;
    public List<GameObject> bonds;
    bool sticky = false;

    string Element = "";
    int startingB = 0;
    public int numberOfBonds = 0;
    int MaxBonds = 8;
    bool Active = false;
    Vector3 mousepos;

    [SerializeField] GameObject elementPrefab;

    GameObject mainScript;
    DrawOrgo mScript;

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
        mScript = mainScript.GetComponent<DrawOrgo>();
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
        numberOfBonds = bonds.Count;
        checkBonds();
        if (sticky)
        {
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.5f);
        }
        if (sticky && Input.GetMouseButtonDown(1))
        {
            sticky = false;
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
        else if (index == 7)    //Carbon
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
            if (index == 10) {
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
                

                mainScript.GetComponent<DrawOrgo>().drawBond(gameObject, newElement);



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
        if (gameObject == bond.GetComponent<BondLineBehavoir>().getElement1()) {
            bondedElements.Add(bond.GetComponent<BondLineBehavoir>().getElement2());
        }
        else
        {
            bondedElements.Add(bond.GetComponent<BondLineBehavoir>().getElement1());
        }
        numberOfBonds++;

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

    public void checkBonds()
    {
        bool removed = false;
        for (int i = 0; i < bondedElements.Count; i++)
        {
            if (bondedElements[i] == null)
            {
                bondedElements.Remove(bondedElements[i]);
                removed = true;
                break;
            }
        }
        for (int i = 0; i < bonds.Count; i++)
        {
            if (bonds[i] == null)
            {
                bonds.Remove(bonds[i]);
                removed = true;
                break;
            }
        }
        if (removed)
        {
            checkBonds();
        }
        numberOfBonds = bonds.Count;
    }

    private void OnMouseOver()
    {
        if (mainScript.GetComponent<DrawOrgo>().Function == "Remove" && Input.GetMouseButtonDown(0)) 
        {
            if (numberOfBonds > 0)
            {
                
                while(bondedElements.Count > 0)
                {
                    print(bondedElements.Count);
                    
                    if (bondedElements[0] ==  null)
                    {
                        bondedElements.Remove(bondedElements[0]);
                    }
                    else
                    {
                        bondedElements[0].GetComponent<ElementBehavoir>().decrementBond();
                        bondedElements[0].GetComponent<ElementBehavoir>().checkBonds();
                        bondedElements.Remove(bondedElements[0]);
                    }
                    Destroy(bonds[0].gameObject);
                    checkBonds();
                    
                }
            }
            Destroy(gameObject);
        }
        if (mainScript.GetComponent<DrawOrgo>().Function == "Location" && (Input.GetMouseButtonDown(0)))
        {
            sticky = true;
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

    public string getMainFunction()
    {
        return mScript.Function;
    }

}
