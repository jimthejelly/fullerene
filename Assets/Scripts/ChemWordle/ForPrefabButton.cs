using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForPrefabButton : MonoBehaviour
{

    private int cid;
    public void SetCID(int cid)
    {
        this.cid = cid;
    }

    public TMPro.TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PLayAgian()
    {
        SceneManager.LoadScene("ChemicalWordle", LoadSceneMode.Single);
    }

    public void QuitToMain()
    {
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
    public void DoSomethingCool()
    {
        GameObject thing = GameObject.Find("WordleManager");
        WordleManager wordleManager = thing.GetComponent<WordleManager>();
        PubChemAPIManager pubChemAPIManager = wordleManager.pubChemAPIManager;
        wordleManager.set(pubChemAPIManager.generalDataController.GetChemicalWithProperty("CID", cid.ToString()), true);
    }

}
