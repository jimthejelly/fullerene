using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System;
using System.Linq;



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

    private void saveMolecule()
    {
        GameObject molecule = GameObject.Find("moleculeBody");
        if (molecule.transform.childCount == 0)
        {
            Debug.Log("No molecule to save!");
            return;
        }
        XmlTextWriter writer = new XmlTextWriter("./Assets/Resources/molecule.cml", null);
        writer.WriteStartDocument();

        writer.Formatting = Formatting.Indented;
        writer.Indentation = 4;

        writer.WriteStartElement("molecule");

        writer.WriteStartElement("atomArray");

        Dictionary<Elements, string> atomIDs = new Dictionary<Elements, string>();
        Dictionary<string, Elements> IDToAtom = new Dictionary<string, Elements>();
        int count = 1;
        foreach (Transform item in molecule.transform)
        {
            if (item.tag.Equals("Element"))
            {
                atomIDs.Add(item.gameObject.GetComponent<Elements>() as Elements, "a" + count);
                IDToAtom.Add("a" + count++, item.gameObject.GetComponent<Elements>() as Elements);
                writer.WriteStartElement("atom");
                writer.WriteAttributeString("id", atomIDs[item.gameObject.GetComponent<Elements>() as Elements]);
                writer.WriteAttributeString("elementType", ((ElementSymbols)((item.gameObject.GetComponent<Elements>() as Elements).protons)).ToString("F"));
                writer.WriteAttributeString("x3", item.position.x.ToString());
                writer.WriteAttributeString("y3", item.position.y.ToString());
                writer.WriteAttributeString("z3", item.position.z.ToString());
                writer.WriteEndElement(); // end of atom
            }
        }

        writer.WriteEndElement(); // end of atomArray

        writer.WriteStartElement("bondArray");

        foreach (Transform item in molecule.transform)
        {
            if (item.tag.Equals("Bond"))
            {
                writer.WriteStartElement("bond");
                writer.WriteAttributeString("atomRefs2", atomIDs[(item.gameObject.GetComponent<Bonds>() as Bonds).parent] + " " + atomIDs[(item.gameObject.GetComponent<Bonds>() as Bonds).child]);
                writer.WriteAttributeString("order", ((item.gameObject.GetComponent<Bonds>() as Bonds).bondOrder).ToString());
                writer.WriteEndElement(); // end of bond
            }
        }

        writer.WriteEndElement(); // end of bondArray

        writer.WriteEndElement(); // end of molecule

        writer.WriteEndDocument();
        writer.Close();
    }

    private void saveNeighbors()
    {
        GameObject molecule = GameObject.Find("moleculeBody");
        if (molecule.transform.childCount == 0)
        {
            Debug.Log("No molecule to save!");
            return;
        }
        string path = "Assets/Resources/Presets/template.txt";
        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
        }
        else
        {
            File.WriteAllText(path, string.Empty);
        }

        foreach (Transform item in molecule.transform)
        {
            if (item.tag.Equals("Element"))
            {
                string line = "";
                List<Tuple<int, int>> neighbors = (item.gameObject.GetComponent<Elements>() as Elements).GetStringNeighbors();
                line += neighbors[0].Item1 + ": ";
                foreach (Tuple<int, int> neighbor in neighbors)
                {
                    line += neighbor.Item2 + " ";
                }
                line += "\n";
                File.AppendAllText(path, line);
            }
        }
    }

    public void savePreset()
    {
        saveNeighbors();
        saveMolecule();
        GameObject body = GameObject.Find("moleculeBody");
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
        }
        else
        {
            Debug.Log("No molecule to save");
        }
    }

    public void getPreset()
    {
        GameObject body = GameObject.Find("moleculeBody");
        if (body.transform.childCount > 0)
        {
            (creationUser.head.GetComponent<Elements>() as Elements).DeleteElement();
        }

        preset = "Preset " + presetNumber;
        string load_elements_path = "Assets/Resources/Presets/template.txt";

        try
        {
            Debug.Log("Loading preset: " + preset);
            Dictionary<int, List<int>> elementPairs = new Dictionary<int, List<int>>();
            string[] lines = File.ReadAllLines(load_elements_path);
            
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                string[] token = line.Split(": ");
                string[] elm = token[1].Split(" ");
                Array.Sort(elm);

                if (body.transform.childCount == 0)
                {
                    int elementType = int.Parse(token[0]);
                    GameObject obj1 = AssetDatabase.LoadAssetAtPath("Assets/Elements/" + elementType + ".prefab", typeof(GameObject)) as GameObject;
                    
                    if (obj1 == null)
                    {
                        Debug.LogError("Element prefab not found: Assets/Elements/" + elementType + ".prefab");
                        continue;
                    }
                    
                    GameObject clone = Instantiate(obj1, Vector3.zero, Quaternion.identity, body.transform);
                    creationUser.head = clone;
                    clone.transform.Rotate(180, 0, 0);
                    
                    if (creationUser.lonePairsVisible)
                    {
                        (clone.GetComponent<Elements>() as Elements).ShowLonePairs();
                    }
                    // DO NOT destroy clone here - let it stay in the scene
                }
                
                foreach (string e1 in elm)
                {
                    int element1 = int.Parse(token[0]);
                    int element2 = int.Parse(e1);
                    
                    if (!elementPairs.ContainsKey(element1))
                    {
                        elementPairs[element1] = new List<int>();
                    }
                    elementPairs[element1].Add(element2);
                    Debug.Log("Element 1: " + element1 + "\tElement 2: " + element2);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error loading preset: " + e);
        }
    }
}