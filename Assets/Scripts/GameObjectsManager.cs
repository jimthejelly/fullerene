using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Compound
{
    public string name;
    public string formula;
}

public class GameObjectsManager : MonoBehaviour
{
    [SerializeField] GameObject CNR;
    [SerializeField] GameObject CNRbarrel;
    [SerializeField] GameObject AddMoleculePanel;
    [SerializeField] GameObject AddMoleculeName;
    [SerializeField] GameObject AddMoleculeFormula;
    [SerializeField] GameObject AddMoleculeList;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject Spawner;
    [SerializeField] GameObject CompoudEditor; //Prefab Object
    [SerializeField] GameObject CNRLoader;

    TextMeshProUGUI Name;
    TextMeshProUGUI Formula;
    TextMeshProUGUI MList;
    TextMeshProUGUI CNRLoaderText;

    public List<GameObject> totalObjects = new List<GameObject>(); //to track all the compounds that exist
    public List<Compound> totalMolecules = new List<Compound>(); //to track all the compounds that exist for the sake of the label
    public List<GameObject> activeObjects = new List<GameObject>();//to track all active compounds that can get shot
    public List<Compound> ActiveMolecules = new List<Compound>();//to track all active compounds names for the sake of the label

    bool Playing;
    bool Pause;

    float CNRSpeed = 7.50f;


    // Start is called before the first frame update
    void Start()
    {
        Pause = false;
        CNR.SetActive(false);
        PauseMenu.SetActive(false);
        Name = AddMoleculeName.GetComponentInChildren<TextMeshProUGUI>();
        Formula = AddMoleculeFormula.GetComponentInChildren<TextMeshProUGUI>();
        MList = AddMoleculeList.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Playing = !Playing;
            Paused();
        }
        if (Playing)
        {
            Play();
        }
        CNRbarrel.transform.Rotate(new Vector3(0.0f,0.0f,1.0f), CNRSpeed * Time.deltaTime);
        if (CNRbarrel.transform.rotation.z > 90 || CNRbarrel.transform.rotation.z < -90)
        {
            CNRSpeed = -CNRSpeed;
        } 

    }

    public void AddMoleculePair()  //After intro text can add molecules, maybe create/copy the save system from earlier to save certain molecule lists
    {
        GameObject compound = new GameObject();
        compound = CompoudEditor;
        Compound molecule = new Compound();
        molecule.name = Name.GetComponent<TextMeshProUGUI>().text;
        Name.GetComponent<TextMeshProUGUI>().text = "";
        molecule.formula = Formula.GetComponent<TextMeshProUGUI>().text;
        Formula.GetComponent<TextMeshProUGUI>().text = "";
        totalObjects.Add(compound);
        totalMolecules.Add(molecule);
        UpdateList();
    }

    void UpdateList()
    {
        print("UpdatingList");
        MList.GetComponent<TextMeshProUGUI>().text = "";
        int x = 0;
        foreach (Compound molecule in totalMolecules) {
            MList.GetComponent<TextMeshProUGUI>().text += "Compund " + (x + 1) + ": " + molecule.name + " = " + molecule.formula+ "\r\n";
            x++;
        }
    }

    public void confirmMolecules()
    {
        CNR.SetActive(true);
        AddMoleculePanel.SetActive(false);
        Playing = true;
    }

    public void Paused()
    {
        Pause = !Pause;
        Playing = !Playing;
        if (Pause)
        {
            PauseMenu.SetActive(true);

            foreach (GameObject compound in totalObjects)
            {
                compound.transform.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            }
        } else
        {
            PauseMenu.SetActive(false);
        }
    }

    void Play()
    {
        Playing = false;
        InvokeRepeating("LaunchMolecule", 2.0f, 2.5f);
    }

    void LaunchMolecule()
    {
        GameObject instance = Instantiate(CompoudEditor);
        instance.transform.position = Spawner.transform.position;

        activeObjects.Add(instance);   

        Compound InstanceMolecule = totalMolecules[Random.Range(0, totalMolecules.Count)];

        ActiveMolecules.Add(InstanceMolecule);
        if (0 == Random.Range(0, 2)) {
            instance.GetComponentInChildren<TextMeshPro>().text = InstanceMolecule.name;
        } else
        {
            instance.GetComponentInChildren<TextMeshPro>().text = InstanceMolecule.formula;
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
