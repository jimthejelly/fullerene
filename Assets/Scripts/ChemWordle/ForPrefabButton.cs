using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void DoSomethingCool()
    {
        GameObject thing = GameObject.Find("WordleManager");
        WordleManager wordleManager = thing.GetComponent<WordleManager>();
        PubChemAPIManager pubChemAPIManager = wordleManager.pubChemAPIManager;
        wordleManager.set(pubChemAPIManager.generalDataController.GetChemicalWithProperty("CID", cid.ToString()), true);
    }

}
