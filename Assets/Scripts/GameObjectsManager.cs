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
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject Spawner;
    [SerializeField] GameObject CompoudEditor; //Prefab Object
    [SerializeField] GameObject Laser;
    [SerializeField] GameObject CNRLoader;
    [SerializeField] GameObject ScoreText;

    [SerializeField] TMP_InputField Name;
    [SerializeField] TMP_InputField Formula;
    TextMeshProUGUI MList;
    [SerializeField] TMP_InputField CNRLoaderText;
    TextMeshProUGUI Score;

    public List<GameObject> totalObjects = new List<GameObject>(); //to track all the compounds that exist
    public List<Compound> totalMolecules = new List<Compound>(); //to track all the compounds that exist for the sake of the label
    public List<Compound> yetToSpawn = new List<Compound>();    //so the randomizing agent goes through all the compounds at least once every round
    public List<GameObject> activeObjects = new List<GameObject>();//to track all active compounds that can get shot
    public List<Compound> ActiveMolecules = new List<Compound>();//to track all active compounds names for the sake of the label

    bool Playing;
    public bool Pause;
    bool reload = true;
    float CNRSpeed = 7.50f;

    int score = 10;


    // Start is called before the first frame update
    void Start()
    {
        Pause = false;
        CNR.SetActive(false);
        PauseMenu.SetActive(false);
        HUD.SetActive(false);
        MList = AddMoleculeList.GetComponentInChildren<TextMeshProUGUI>();
        Score = ScoreText.GetComponent<TextMeshProUGUI>();
        
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
        if ((CNRbarrel.transform.rotation.z*360) > 90 || (CNRbarrel.transform.rotation.z * 360) < -90)
        {
            CNRSpeed = -CNRSpeed;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            print("trying");
            ClearText();
        }

    }

    void ClearText()
    {
        print(CNRLoaderText);
        CNRLoaderText.text = "";
    }

    private void FixedUpdate()
    {
        if (Input.GetButtonDown("Submit") && reload)
        {
            print("Firing");
            CheckShoot(CNRLoaderText.text);
            reload = false;
        }
        if (Input.GetButtonUp("Submit"))
        {
            reload = true;
            CNRLoaderText.Select();
            CNRLoaderText.ActivateInputField();
        }
    }

    public void AddMoleculePair()  //After intro text can add molecules, maybe create/copy the save system from earlier to save certain molecule lists
    {
        GameObject compound = new GameObject();
        compound = CompoudEditor;
        Compound molecule = new Compound();
        molecule.name = Name.text;
        Name.text = "";
        molecule.formula = Formula.text;
        Formula.text = "";
        molecule.name.Trim();
        molecule.formula.Trim();
        totalObjects.Add(compound);
        totalMolecules.Add(molecule);
        UpdateList();
    }

    void CheckShoot(string Word)
    {
        print(Word);
        foreach (Compound molecule in ActiveMolecules)
        {
            print(molecule.name);
            print(molecule.formula);
            print(Word);
            print(molecule.name == Word);

            if (molecule.name == Word || molecule.formula == Word)
            {
                print(CNRLoaderText.text);
                print(molecule);
                foreach (GameObject rock in activeObjects)
                {
                    if (rock == null)
                    {
                        activeObjects.Remove(rock);
                        CheckShoot(Word);
                        return;
                    }
                    if (rock.name == molecule.name)
                    {
                        Shoot(rock);
                        return;
                    }
                }
            }
        }
    }

    void Shoot(GameObject Target)
    {   
        Vector3 Targetposition = Target.transform.position;
        print("Taking the shot");
        Vector3 LaserDirection = CNR.transform.position - Targetposition; 
        print(LaserDirection);
        CNRbarrel.transform.rotation = Quaternion.LookRotation(LaserDirection);
        GameObject laser = Instantiate(Laser);
        laser.GetComponent<LaserCollision>().target = Target;
        Vector3 spawnPos = CNRbarrel.transform.position;
        laser.transform.position = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z + 2.0f);

        float x = Targetposition.x - laser.transform.position.x;
        float y = Targetposition.y - laser.transform.position.y + 8.6f;
        laser.transform.Rotate(0, 0, (Mathf.Atan2(y, x) * 57.2958f) + 90);
        CNRLoaderText.text = "";

        print(Targetposition);
        print(laser.transform.position);
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
        HUD.SetActive(true);
        Playing = true;
    }

    public void Paused()
    {
        Playing = !Playing;
        Pause = !Pause;
        if (Pause)
        {
            PauseMenu.SetActive(true);
        } else
        {
            PauseMenu.SetActive(false);
        }
    }

    void Play()
    {
        for (int i = 0; i < totalMolecules.Count; i++)
        {
            yetToSpawn.Add(totalMolecules[i]);
        }
        Playing = false;
        gameObject.GetComponent<ShootMoleculeLives>().SpawnHearts();
        Pause = false;
        score = 10;
        Score.text = "Score: " + score;
        InvokeRepeating("LaunchMolecule", 2.0f, 2.5f);
    }

    void LaunchMolecule()
    {
        if (!Pause)
        {

            int Spawn;
            if (yetToSpawn.Count > 1)
            {
                print("normal Spawning");
                Spawn = Random.Range(0, yetToSpawn.Count);

            } else
            {
                if (yetToSpawn.Count == 0)
                {

                    print("reset List");
                    
                    for (int i = 0; i < totalMolecules.Count; i++)
                    {
                        yetToSpawn.Add(totalMolecules[i]);
                    }

                    Spawn = Random.Range(0, yetToSpawn.Count);
                    }
                else
                {
                    print("last item");
                    Spawn = 0;
                }
            }


            GameObject instance = Instantiate(CompoudEditor);
            instance.transform.position = Spawner.transform.position;

            activeObjects.Add(instance);

            print(yetToSpawn);
            print(yetToSpawn.Count);
            print(Spawn);
            Compound InstanceMolecule = yetToSpawn[Spawn];
            yetToSpawn.Remove(yetToSpawn[Spawn]);
            instance.name = InstanceMolecule.name;
            ActiveMolecules.Add(InstanceMolecule);
            if (0 == Random.Range(0, 2))
            {
                instance.GetComponentInChildren<TextMeshPro>().text = InstanceMolecule.name;
            }
            else
            {
                instance.GetComponentInChildren<TextMeshPro>().text = InstanceMolecule.formula;
            }
        }
    }

    public void RemoveObject(GameObject thing)
    {
        for (int i = 0; i < activeObjects.Count; i++) { 
            if (activeObjects[i] == thing)
            {
                ActiveMolecules.Remove(ActiveMolecules[i]);
                break;
            }
        }
        activeObjects.Remove(thing);
    }

    public void ResetList()
    {
        EmptyActives();
        EmptyTotals();
        CancelInvoke("LaunchMolecule");
        CNR.SetActive(false);
        AddMoleculePanel.SetActive(true);
        HUD.SetActive(false);
        PauseMenu.SetActive(false);
        CNRLoaderText.text = "";
        UpdateList();
    }

    void EmptyActives()
    {
        if (ActiveMolecules.Count == 0&&activeObjects.Count == 0)
        {
            return;
        } 
        else 
        {
            GameObject temp = activeObjects[0];
            ActiveMolecules.Remove(ActiveMolecules[0]);
            activeObjects.Remove(activeObjects[0]);
            Destroy(temp);
            EmptyActives();
        }
    }

    void EmptyTotals()
    {
        if (totalMolecules.Count == 0)
        {
            return;
        }
        else
        {
            totalMolecules.Remove(totalMolecules[0]);
            totalObjects.Remove(totalObjects[0]);
            EmptyTotals();
        }
    }

    public void IncreaseScore()
    {
        score += 25;
        Score.text = "Score: " + score;
    }

    public void DecreaseScore()
    {
        print(score);
        if (score <= 0)
        {
            score = 1;
        }
        score -= (int)(score / 4);
        Score.text = "Score: " + score;
    }

    public void targetHit(GameObject target)
    {
        for (int i = 0; i < activeObjects.Count; i++)
        {
            if (target == activeObjects[i]) 
            {
                activeObjects.Remove(target);
                ActiveMolecules.Remove(ActiveMolecules[i]);
                Destroy(target);
            }
        }

    }

    public void ExitGame()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
