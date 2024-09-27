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
    void Start() {
        // numberText.text = "Loaded";
    }

    public void SavePreset() {
        bool success;
        preset = "Preset " + presetNumber;
        path = "Assets/Resources/Presets/" + preset + ".prefab";
        if (File.Exists("Assets/Resources/Presets/" + preset + ".prefab")) {
            Debug.Log("Here " + path);
            AssetDatabase.DeleteAsset(path);
        } else {
            Debug.Log("Not here" + path);
        }
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        PrefabUtility.SaveAsPrefabAsset(GameObject.Find("moleculeBody"), path, out success);
        if (success) {
            Debug.Log("yippe");
        } else {
            Debug.Log("whoops");
        }
    }

    public void getPreset() {
        preset = "Preset " + presetNumber;
       GameObject obj = PrefabUtility.LoadPrefabContents("Assets/Resources/Presets/" + preset + ".prefab");
    //    GameObject clone = GameObject.Find("moleculeBody");
       Instantiate(obj, transform.position, transform.rotation);
    //    clone = obj;
    
    }
}