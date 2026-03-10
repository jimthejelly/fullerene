
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.IO;
using System;



/// <summary>
    /// A class for saving and loading presets of molecules. Currently saves and loads from a CML file, but requires more work to rebuild the molecule fully from the CML file. 
    /// Also saves a prefab of the molecule, but this is currently unused and may end up being scrapped depending on how we decide to implement presets in the future.
/// </summary>
public class selectPreset : MonoBehaviour
{
    /// <summary> The name of the preset to save or load. </summary>
    public static string preset;
    /// <summary> The number of the preset </summary>
    public int presetNumber;
    /// <summary> The path to the preset file</summary>
    string path;
    /// <summary>
    /// Start is called before the first frame update and initializes the preset </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// saveMolecule saves the current molecule in the scene to a CML file and a prefab. The CML file is used to rebuild the molecule when loading the preset, 
    /// and the prefab is currently unused but may end up being scrapped depending on how we decide to implement presets in the future.
    /// </summary>
    private void saveMolecule()
    {
        // Saving molecule to CML file
        GameObject molecule = GameObject.Find("moleculeBody");
        if (molecule.transform.childCount == 0)
        {
            Debug.Log("No molecule to save!");
            return;
        }
        XmlTextWriter writer = new XmlTextWriter("./Assets/Resources/preset" + presetNumber + ".cml", null);
        writer.WriteStartDocument();

        writer.Formatting = Formatting.Indented;
        writer.Indentation = 4;

        writer.WriteStartElement("molecule");

        writer.WriteStartElement("atomArray");

        // Saves atomIds and their corresponding elements in a dictionary so we can reference them when saving bonds
        // Saves IDtoAtom and their corresponding atomIds in a dictionary so we can reference then when saving atoms
        Dictionary<Elements, string> atomIDs = new Dictionary<Elements, string>();
        Dictionary<string, Elements> IDToAtom = new Dictionary<string, Elements>();
        int count = 1;

        // Loops through all elements in the molecule and saves their properties to the CML file
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

    /// <summary>
    /// saveNeighbors saves the neighbors of each element in the molecule to a text file.
    /// </summary>
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

/// <summary>
///  savePreset saves the current molecule in the scene to a CML file and a prefab. The CML file is used to rebuild the molecule when loading the preset,
/// </summary>
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
/// <summary>
/// getPreset loads the molecule from the CML file and prefab saved by savePreset.
/// </summary>
    public void getPreset()
    {
        GameObject body = GameObject.Find("moleculeBody");
        if (body == null)
        {
            Debug.LogError("No moleculeBody object in scene.");
            return;
        }

        ///Goes through all children of moleculeBody and destroys them to clear the scene before loading the new molecule.
        foreach (Transform child in body.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        //Load CML
        string cmlPath = "./Assets/Resources/molecule.cml";
        if (!File.Exists(cmlPath))
        {
            Debug.LogError("CML file not found at: " + cmlPath);
            return;
        }

        XmlDocument doc = new XmlDocument();
        try
        {
            doc.Load(cmlPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load CML: " + e);
            return;
        }

        ///Parses the CML file for atoms and bonds, instantiates the corresponding prefabs, and sets their properties based on the CML data. Also builds the neighbor lists for each element based on the bonds.
        XmlNode moleculeNode = doc.SelectSingleNode("/molecule");
        if (moleculeNode == null)
        {
            Debug.LogError("CML has no molecule root.");
            return;
        }

        ///Selects the atomArray and bondArray nodes from the CML file, which contain the information about the atoms and bonds in the molecule. If either of these nodes is missing, it logs an error and returns.
        XmlNode atomArrayNode = moleculeNode.SelectSingleNode("atomArray");
        XmlNode bondArrayNode = moleculeNode.SelectSingleNode("bondArray");

        if (atomArrayNode == null || bondArrayNode == null)
        {
            Debug.LogError("CML missing atomArray or bondArray.");
            return;
        }

        ///Creates a dictionary to map atom IDs from the CML file to their corresponding Elements components in the scene. This is used to set up the bonds between elements after all the atoms have been instantiated.
        var idToElement = new Dictionary<string, Elements>();

        foreach (XmlNode atomNode in atomArrayNode.ChildNodes)
        {
            if (atomNode.Name != "atom") continue;

            ///Parses the attributes of each atom node in the CML file to get the atom's ID, element type, and 3D coordinates. It then instantiates the corresponding element prefab based on the element type, sets its position, and adds it to the scene. It also updates the idToElement dictionary to map the atom ID to the instantiated Elements component for later reference when setting up bonds.
            string id = atomNode.Attributes["id"].Value;
            string symbol = atomNode.Attributes["elementType"].Value;

            float x = float.Parse(atomNode.Attributes["x3"].Value, CultureInfo.InvariantCulture);
            float y = float.Parse(atomNode.Attributes["y3"].Value, CultureInfo.InvariantCulture);
            float z = float.Parse(atomNode.Attributes["z3"].Value, CultureInfo.InvariantCulture);
            Vector3 pos = new Vector3(x, y, z);

            int atomicNumber = (int)(ElementSymbols)Enum.Parse(typeof(ElementSymbols), symbol);

            ///Constructs the path to the element prefab based on the atomic number, loads the prefab, and instantiates it in the scene at the specified position. It also sets the protons property of the Elements component to the atomic number for later reference when setting up bonds.
            string elementPrefabPath = $"Assets/Elements/{atomicNumber}.prefab";
            GameObject elementPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(elementPrefabPath);


            ///Tries to load the element prefab from the specified path. If the prefab cannot be loaded, it logs an error and continues to the next atom node without instantiating anything for this atom.
            if (elementPrefab == null)
            {
                Debug.LogError($"Could not load element prefab for {symbol} at {elementPrefabPath}");
                continue;
            }

            GameObject elementGO = PrefabUtility.InstantiatePrefab(elementPrefab) as GameObject;
            if (elementGO == null)
            {
                Debug.LogError("Failed to instantiate element prefab: " + elementPrefabPath);
                continue;
            }


            ///Selects the instantiated element GameObject, sets its parent to the moleculeBody, and positions it according to the coordinates specified in the CML file. It also retrieves the Elements component from the instantiated GameObject, checks if it exists, and sets its protons property to the atomic number for later reference when setting up bonds. Finally, it updates the idToElement dictionary to map the atom ID from the CML file to the instantiated Elements component for later reference when setting up bonds.
            elementGO.transform.SetParent(body.transform, true);
            elementGO.transform.position = pos;

            Elements elementComp = elementGO.GetComponent<Elements>();
            if (elementComp == null)
            {
                Debug.LogError("Element prefab has no Elements component: " + elementPrefabPath);
                continue;
            }

            elementComp.protons = atomicNumber;

            idToElement[id] = elementComp;
        }

        ///Loops through all the bond nodes in the bondArray from the CML file, parses the atomRefs2 attribute to get the IDs of the two atoms that are bonded, looks up the corresponding Elements components from the idToElement dictionary, and instantiates a bond prefab to represent the bond between them. It also sets the bond order based on the order attribute in the CML file and updates the neighbor lists for each element based on the bonds.
        foreach (XmlNode bondNode in bondArrayNode.ChildNodes)
        {
            if (bondNode.Name != "bond") continue;

            //Parses the atoms involved in the bond from the atomRefs2 attribute, which contains the IDs of the two atoms that are bonded. It splits the atomRefs2 string to get the individual atom IDs and checks if they are valid.
            string atomRefs = bondNode.Attributes["atomRefs2"].Value;
            string[] parts = atomRefs.Split(' ');
            //If the atomRefs2 attribute does not contain exactly two atom IDs, it logs a warning and skips this bond node.
            if (parts.Length != 2)
            {
                Debug.LogWarning("bond atomRefs2 malformed: " + atomRefs);
                continue;
            }

            string id1 = parts[0];
            string id2 = parts[1];

            //If either of the atom IDs from the bond node cannot be found in the idToElement dictionary (which means that one of the atoms involved in the bond was not successfully instantiated), it logs a warning and skips this bond node.
            if (!idToElement.TryGetValue(id1, out Elements e1) ||
                !idToElement.TryGetValue(id2, out Elements e2))
            {
                Debug.LogWarning($"Bond refers to unknown atom ids: {id1}, {id2}");
                continue;
            }

            int order = int.Parse(bondNode.Attributes["order"].Value, CultureInfo.InvariantCulture);

            //Loads the bond prefab from a specified path, instantiates it in the scene, and sets its parent to the moleculeBody.
            string bondPrefabPath = "Assets/Resources/SingleBond.prefab";
            GameObject bondPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(bondPrefabPath);
            if (bondPrefab == null)
            {
                Debug.LogError("Could not load bond prefab at: " + bondPrefabPath);
                continue;
            }

            //Loads the bond prefab from the specified path. If the prefab cannot be loaded, it logs an error and continues to the next bond node without instantiating anything for this bond.
            GameObject bondGO = PrefabUtility.InstantiatePrefab(bondPrefab) as GameObject;
            bondGO.transform.SetParent(body.transform, true);

            Bonds b = bondGO.GetComponent<Bonds>();
            if (b == null)
            {
                Debug.LogError("Bond prefab has no Bonds component.");
                continue;
            }


            //Sets the elements involved in the bond and the bond order based on the information from the CML file. It also updates the bond count, bond orders, and electron counts for each element based on the bond order.
            b.SetElements(e1, e2);
            b.bondOrder = order;

            e1.bondCount++;
            e1.bondOrders += order;
            e1.UpdateElectrons(order);

            e2.bondCount++;
            e2.bondOrders += order;
            e2.UpdateElectrons(order);

            e1.GetNeighbors().Add(new Tuple<GameObject, GameObject>(bondGO, e2.gameObject));
            e2.GetNeighbors().Add(new Tuple<GameObject, GameObject>(bondGO, e1.gameObject));

            int element1 = e1.protons;
            int element2 = e2.protons;
            e1.neighborLoad.Add(new Tuple<int, int>(element1, element2));
            e2.neighborLoad.Add(new Tuple<int, int>(element2, element1));
        }

        foreach (Transform child in body.transform)
        {
            if (!child.CompareTag("Element")) continue;
            Elements el = child.GetComponent<Elements>();
            if (el == null) continue;

            el.ResetChildPositions();
            el.MoveChildren(el == creationUser.head ? 0 : 1);
        }

        Debug.Log("Preset rebuilt from CML.");
    }
}

/// <summary>
/// An enum for the element symbols, which is used to convert between the element type specified in the CML file and the corresponding atomic number for loading the correct element prefab.
/// </summary>
 public enum ElementNames
    {
        Hydrogen = 1,
        Helium = 2,
        Lithium = 3,
        Beryllium = 4,
        Boron = 5,
        Carbon = 6,
        Nitrogen = 7,
        Oxygen = 8,
        Fluorine = 9,
        Neon = 10,
        Sodium = 11,
        Magnesium = 12,
        Aluminum = 13,
        Silicon = 14,
        Phosphorus = 15,
        Sulfur = 16,
        Chlorine = 17,
        Argon = 18,
        Potassium = 19,
        Calcium = 20,
        Scandium = 21,
        Titanium = 22,
        Vanadium = 23,
        Chromium = 24,
        Manganese = 25,
        Iron = 26,
        Cobalt = 27,
        Nickel = 28,
        Copper = 29,
        Zinc = 30,
        Gallium = 31,
        Germanium = 32,
        Arsenic = 33,
        Selenium = 34,
        Bromine = 35,
        Krypton = 36,
        Rubidium = 37,
        Strontium = 38,
        Yttrium = 39,
        Zirconium = 40,
        Niobium = 41,
        Molybdenum = 42,
        Technetium = 43,
        Ruthenium = 44,
        Rhodium = 45,
        Palladium = 46,
        Silver = 47,
        Cadmium = 48,
        Indium = 49,
        Tin = 50,
        Antimony = 51,
        Tellurium = 52,
        Iodine = 53,
        Xenon = 54,
        Cesium = 55,
        Barium = 56,
        Lanthanum = 57, 
        Cerium = 58,
        Praseodymium = 59,
        Neodymium = 60,
        Promethium = 61,
        Samarium = 62,
        Europium = 63,
        Gadolinium = 64,
        Terbium = 65,
        Dysprosium = 66,
        Holmium = 67,
        Erbium = 68,
        Thulium = 69,
        Ytterbium = 70,
        Lutetium = 71, 
        Hafnium = 72,
        Tantalum = 73,
        Tungsten = 74,
        Rhenium = 75,
        Osmium = 76,
        Iridium = 77,
        Platinum = 78,
        Gold = 79,
        Mercury = 80,
        Thallium = 81,
        Lead = 82,
        Bismuth = 83,
        Polonium = 84,
        Astatine = 85,
        Radon = 86,
        Francium = 87,
        Radium = 88,
        Actinium = 89, 
        Thorium = 90,
        Protactinium = 91,
        Uranium = 92,
        Neptunium = 93,
        Plutonium = 94,
        Americium = 95,
        Curium = 96,
        Berkelium = 97,
        Californium = 98,
        Einsteinium = 99,
        Fermium = 100,
        Mendelevium = 101,
        Nobelium = 102,
        Lawrencium = 103, 
        Rutherfordium = 104,
        Dubnium = 105,
        Seaborgium = 106,
        Bohrium = 107,
        Hassium = 108,
        Meitnerium = 109,
        Darmstadtium = 110,
        Roentgenium = 111,
        Copernicium = 112,
        Nihonium = 113,
        Flerovium = 114,
        Moscovium = 115,
        Livermorium = 116,
        Tennessine = 117,
        Oganesson = 118
    }