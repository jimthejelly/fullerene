using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEditor;
using System.Xml;
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
        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Element")) {
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

        foreach(Transform item in molecule.transform) {
            if(item.tag.Equals("Bond")) {
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

    public void savePreset()
    {
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
        saveMolecule();
        if (body.transform.childCount > 0)
        {
            (creationUser.head.GetComponent<Elements>() as Elements).DeleteElement();
        }

        preset = "Preset " + presetNumber;
        GameObject obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/Presets/" + preset + ".prefab", typeof(GameObject)) as GameObject;
        GameObject clone = Instantiate(obj, Vector3.zero, Quaternion.identity, GameObject.Find("moleculeBody").transform);

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