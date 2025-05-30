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
        moleculeBody = moleculeSingleton.Instance;
    }

}
