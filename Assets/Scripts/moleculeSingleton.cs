using UnityEngine.SceneManagement;
using UnityEngine;

public class moleculeSingleton : MonoBehaviour {
    private static GameObject _Instance;
    private Scene sceneManager;

    public static GameObject Instance {
        get {
                if (_Instance == null) {
                    _Instance = GameObject.Find("moleculeBody");
                    //DontDestroyOnLoad(_Instance.gameObject);
                }
                return _Instance;
        }
    }
}