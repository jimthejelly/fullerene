using UnityEngine.SceneManagement;
using UnityEngine;

public class keepMolecule : MonoBehaviour
{
    public Canvas canvas;
    public GameObject moleculeBody;

    private Scene sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(moleculeBody);
    }

    // Update is called once per frame
    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        if(sceneManager != scene) {
             Debug.Log(moleculeBody.transform.childCount);
            for(int i = 0; i < moleculeBody.transform.childCount; i++) {
                Destroy(moleculeBody.transform.GetChild(i).gameObject);
            }

        } 
        sceneManager = scene;
    }
}
