using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;
using TMPro;
using System.Diagnostics.Tracing;
using System;
using System.IO;



// public static class Globals {
//     public static GameObject element;
// }

public class selectPreset : MonoBehaviour
{
    public static string preset;
    public int presetNumber;
    string path;
    void Start()
    {
        // numberText.text = "Loaded";
    }

    // public void SavePreset() {
    //     bool success;
    //     preset = "Preset " + presetNumber;
    //     path = "Assets/Resources/Presets/" + preset + ".prefab";
    //     if (File.Exists("Assets/Resources/Presets/" + preset + ".prefab")) {
    //         Debug.Log("Here " + path);
    //         AssetDatabase.DeleteAsset(path);
    //     } else {
    //         Debug.Log("Not here" + path);
    //     }
    //     path = AssetDatabase.GenerateUniqueAssetPath(path);
    //     PrefabUtility.SaveAsPrefabAsset(GameObject.Find("moleculeBody"), path, out success);

    //     if (success) {
    //         Debug.Log("yippe");
    //     } else {
    //         Debug.Log("whoops");
    //     }
    //     AssetDatabase.SaveAssets();
    //     AssetDatabase.Refresh();
    // }

    public void savePreset()
    {
        preset = "Preset " + presetNumber;
        path = "Assets/Resources/Presets/" + preset + ".prefab";
        if (File.Exists("Assets/Resources/Presets/" + preset + ".prefab"))
        {
            Debug.Log("Existing Asset Deleted" + path);
            AssetDatabase.DeleteAsset(path);
        }
        GameObject obj = GameObject.Find("moleculeBody");
        if (obj.transform.childCount > 0)
        {
            bool success;
            PrefabUtility.SaveAsPrefabAsset(obj, path, out success);
            if (success)
            {
                Debug.Log("Preset Saved: " + path);
            }
            else
            {
                Debug.Log("Error Saving Preset");
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        } else {
            Debug.Log("No molecule to save");
        }
    }

    // public void getPreset() {
    // //     preset = "Preset " + presetNumber;
    // //    GameObject obj = PrefabUtility.LoadPrefabContents("Assets/Resources/Presets/" + preset + ".prefab");
    // //    GameObject clone = GameObject.Find("moleculeBody");
    // //    Instantiate(obj, transform.position, transform.rotation);
    // //    clone = obj;

    // }
}