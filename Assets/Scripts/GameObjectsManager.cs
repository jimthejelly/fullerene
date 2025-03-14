using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Compound
{
    public string name;
    public string formula;
}

public class GameObjectsManager : MonoBehaviour
{
    [SerializeField] GameObject CNR;
    [SerializeField] GameObject AddMoleculePanel;
    [SerializeField] GameObject AddMoleculeName;
    [SerializeField] GameObject AddMoleculeFormula;
    [SerializeField] GameObject AddMoleculeList;
    GameObject CompoudEditor; //to target induvidual things
    public List<Compound> total; //to track all the compounds that exist
    public List<Compound> active;//to track all active compounds that can get shot

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMoleculePair()  //After intro text can add molecules, maybe create/copy the save system from earlier to save certain molecule lists
    {
        Compound compound = new Compound();
        compound.name = AddMoleculeName.GetComponent<TextMeshProUGUI>().text; 
        compound.formula = AddMoleculeFormula.GetComponent<TextMeshProUGUI>().text;
        total.Append(compound);
        UpdateList();
    }

    void UpdateList()
    {
        AddMoleculeList.GetComponent<TextMeshProUGUI>().text = "";
        int x = 0;
        foreach (Compound compound in total) {
            AddMoleculeList.GetComponent<TextMeshProUGUI>().text += "Compund " + (x + 1) + ": " + compound.name + " = " + compound.formula;
        }
    }
}
