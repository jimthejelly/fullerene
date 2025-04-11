using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Compound
{
    public string name;
    public string formula;
}

public class GameObjectsManager : MonoBehaviour
{
    [SerializeField] GameObject CNR;            //turret
    [SerializeField] GameObject CNRbarrel;      //turret barrel for direction
    [SerializeField] GameObject AddMoleculePanel;   //canvas panel
    [SerializeField] GameObject AddMoleculeName;    //||
    [SerializeField] GameObject AddMoleculeFormula;//||
    [SerializeField] GameObject AddMoleculeList;    //||
    [SerializeField] GameObject HUD;                //||
    [SerializeField] GameObject PauseMenu;          //||
    [SerializeField] GameObject Spawner;            // invisible object that moves back and forth
    [SerializeField] GameObject CompoudEditor; //Prefab Object
    [SerializeField] GameObject Laser;              //prefab for lasers
    [SerializeField] GameObject CNRLoader;          //miscelaneous block that serves as a way to know where the canvas item with respect to normal game objects
    [SerializeField] GameObject ScoreText;          //textbax to display score

    [SerializeField] TMP_InputField Name;           //canvas item
    [SerializeField] TMP_InputField Formula;        //||
    [SerializeField] TMP_InputField CNRLoaderText;  //||
    [SerializeField] TMP_InputField FileName;
    TextMeshProUGUI MList;                          //text box to display created pairs
    TextMeshProUGUI Score;                          //text box to display score

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
        if (Input.GetKeyDown(KeyCode.LeftAlt) && reload)
        {
            print("Firing");
            CheckShoot(CNRLoaderText.text);
            reload = false;
        }

        if (!reload && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            print("reloaded");
            reload = true;

        }
        if (Playing)
        {
            CNRLoaderText.Select();
            CNRLoaderText.ActivateInputField();
        }
    }

    void ClearText()
    {
        print(CNRLoaderText);
        CNRLoaderText.text = "";
    }

    private void FixedUpdate()
    {

    }

    public void AddMoleculePair()  //After intro text can add molecules, maybe create/copy the save system from earlier to save certain molecule lists
    {
        GameObject compound = CompoudEditor;
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
        if (!Pause)
        {
            print(Word);
            foreach (Compound molecule in ActiveMolecules)
            {


                if (molecule.name == Word || molecule.formula == Word)
                {

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
        float y = Targetposition.y - laser.transform.position.y;
        laser.transform.Rotate(0, 0, 0);
        //laser.transform.rotation = Quaternion.AngleAxis(-Mathf.Atan2(x, y) * Mathf.Rad2Deg, Vector3.forward);
        //laser.transform.Rotate(0, 0, (Mathf.Atan2(y, x) * Mathf.Deg2Rad) + 90);
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
        gameObject.GetComponent<ShootMoleculeLives>().Lives = 3;
        gameObject.GetComponent<ShootMoleculeLives>().Alive = true;
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

            print(CompoudEditor);
            GameObject instance = Instantiate(CompoudEditor);
            instance.transform.position = Spawner.transform.position;
            print(instance);
            activeObjects.Add(instance);
            Compound InstanceMolecule = yetToSpawn[Spawn];
            print(yetToSpawn[Spawn].name);
            print(yetToSpawn);
            print(yetToSpawn.Count);
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
        removeYetToSpawn();
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
        print("Emptying");
        if (ActiveMolecules.Count == 0&&activeObjects.Count == 0)
        {
            return;
        } 
        else 
        {
            print(activeObjects.Count);
            print(ActiveMolecules.Count);
            if (ActiveMolecules.Count > 0)
            {
                ActiveMolecules.Remove(ActiveMolecules[0]);
            }
            if (activeObjects.Count > 0)
            {
                Destroy(activeObjects[0]);
                activeObjects.Remove(activeObjects[0]);
            }
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
            print(CompoudEditor);
        }
    }

    void removeYetToSpawn()
    {
        if (yetToSpawn.Count == 0) { return; }
        else
        {
            yetToSpawn.Remove(yetToSpawn[0]);
            removeYetToSpawn();
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
        print(activeObjects[0]);
        if (activeObjects[0] == null)
        {
            activeObjects.Remove(activeObjects[0]);
        }
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

    public void SaveList()
    {
        List<Compound> newList = new List<Compound>();
        for (int i = 0; i < totalMolecules.Count; i++)
        {
            newList.Add(totalMolecules[i]);
        }

        string json = JsonConvert.SerializeObject(newList);  // Use Newtonsoft.Json for JSON serialization
        string filePath = Application.persistentDataPath + "/" + FileName + ".json";
        File.WriteAllText(filePath, json);
    }

    public void LoadList()
    {
        string filePath = Application.persistentDataPath + "/" + FileName + ".json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            //return JsonConvert.DeserializeObject<List<Compound>>(json);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + FileName);
            //return null;
        }
    }

    public void reloadScene()
    {
        SceneManager.LoadScene("ShootAMolecule", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}
