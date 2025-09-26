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

    public void getPreset()
    {
        preset = "Preset " + presetNumber;
        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/Presets/" + preset + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity, GameObject.Find("moleculeBody").transform);
        //Deletes the all of the elements in moleculeBody,
        GameObject body = GameObject.Find("moleculeBody");
        (creationUser.head.GetComponent<Elements>() as Elements).DeleteElement();
        //Need to figure out
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < clone.transform.childCount; i++)
        {
            children.Add(clone.transform.GetChild(i));
        }
        foreach (Transform child in children)
        {
            child.SetParent(body.transform, false); // keep local transform
        }
        //clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);
    }
}