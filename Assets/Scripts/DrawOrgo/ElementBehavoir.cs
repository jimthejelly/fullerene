using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBehavoir : MonoBehaviour
{

    [SerializeField] Color ElementColor = Color.white;
    [SerializeField] Material ElementMaterial = null;
    static string[] possibleElements = { "Hydrogen", "Lithium", "Sodium", "Potassium", "Magnesium", "Boron", "Aluminum", "Carbon", "Nitrogen","Phosphorus", "Oxygen", "Sulfur", "Flourine", "Clorine", "Bromine", "Iodine"};
    string Element = "";
    int numberOfBonds = 0;
    int MaxBonds = 8;
    bool Active = false;

    [SerializeField] GameObject elementPrefab;

    GameObject mainScript;
    // Start is called before the first frame update
    void Start()
    {
        mainScript = GameObject.Find("script");
        ElementMaterial = GetComponent<Material>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setElement(int index)
    {
        Element = possibleElements[index];
        if (index == 0)     //Hydrogen
        {
            MaxBonds = 2;
        }
        if (index < 4)  //Lithium, Sodium, Potassium
        {
            numberOfBonds = 1;
        }
        else if (index == 4)    //Magnesium
        {
            numberOfBonds = 2;
        }
        else if (index <  7)    //Boron, Aluminum
        {
            numberOfBonds = 3;
        }
        else if (index == 8)    //Carbon
        {
            numberOfBonds = 4;
        }
        else if (index < 11)    //Nitrogen, Phosphorus
        {
            numberOfBonds = 5;
        }
        else if (index < 13) // Oxygen, Sulfer
        {
            numberOfBonds = 6;
        }
        else                //Florine, Chlorine, Bromine, Iodine
        {
            numberOfBonds = 7;
        }

        Active = true;


    }

    public void forgeBond(int bondType)
    {
        if (Active && numberOfBonds + bondType <= MaxBonds)
        {
            if (mainScript.GetComponent<DrawOrgo>().selectedElement > -1)
            {
                int eigths = 45;
                int spawnDegree = eigths * (numberOfBonds);
                float x, y;
                if (spawnDegree % 2 != 0)
                {
                    x = Mathf.Sqrt(2);
                    y = Mathf.Sqrt(2);
                }
                else if (spawnDegree == 0 || spawnDegree == 180)
                {
                    x = 1; y = 0;
                }
                else
                {
                    y = 1; x = 0;
                }

                if (spawnDegree > 180)
                {
                    y *= -1;
                }

                if (spawnDegree > 90 && spawnDegree < 270)
                {
                    x *= -1;
                }
                Vector3 newElementLocation = new Vector3(x * 10, y * 10, 0) + GetComponent<Transform>().position;

                GameObject newElement = Instantiate(elementPrefab);
                newElement.GetComponent<Transform>().position = newElementLocation;
                newElement.GetComponent<ElementBehavoir>().setElement(mainScript.GetComponent<DrawOrgo>().selectedElement);
            }
        }
        else
        {
            print("this element already has too many bonds");
        }
        
    }


    private void OnMouseEnter()
    {
        if (Active && numberOfBonds <= MaxBonds)
        {
            for (int i = numberOfBonds; i < MaxBonds+1; i++)
            {
                                        //create semitransparent spots for new elements
            }
        }
    }



}
