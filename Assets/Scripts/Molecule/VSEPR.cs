using System;
using UnityEditor;
using UnityEngine;

namespace Molecule
{
    public class VSEPR
    {
        
        /// <summary>
        /// Resets the position of this atom to be moved with moveChildren() later
        /// </summary>
        public void ResetAtomPosition(VisualAtom atom)
        {
            
            var neighbors = _abstractMolecule.GetBondedNeighbors(atom);

            foreach (var neighbor in neighbors)
            {
                // TODO: make this check ionic vs covalent
                float radius = atom.GetAbstractAtom().GetCovalentRadius() + neighbor.GetCovalentRadius();
                
                if(gameObject == creationUser.head) {
                    child.Item1.transform.localPosition = transform.position;
                    child.Item1.transform.localEulerAngles = new Vector3(atom.transform.localEulerAngles.x,
                        this.transform.localEulerAngles.y, atom.transform.localEulerAngles.z);
                    child.Item1.transform.Translate(0, -1 * (radius / 2), 0);

                    child.Item2.transform.localPosition = transform.position;
                    child.Item2.transform.localEulerAngles = new Vector3(atom.transform.localEulerAngles.x,
                        this.transform.localEulerAngles.y, atom.transform.localEulerAngles.z);
                    child.Item2.transform.Translate(0, -1 * (radius), 0);
                }
                else if(!Equals(child, neighbors[0])) {

                    child.Item1.transform.localPosition = transform.position;
                    child.Item1.transform.localEulerAngles = new Vector3(atom.transform.localEulerAngles.x,
                        this.transform.localEulerAngles.y, atom.transform.localEulerAngles.z + 180);
                    child.Item1.transform.Translate(0, -1 * (radius / 2), 0);

                    child.Item2.transform.localPosition = transform.position;
                    child.Item2.transform.localEulerAngles = new Vector3(atom.transform.localEulerAngles.x,
                        this.transform.localEulerAngles.y, atom.transform.localEulerAngles.z + 180);
                    child.Item2.transform.Translate(0, -1 * (radius), 0);


                }
                else {

                }

            }
        }
        
        /// <summary> Moves the "children" of this Element to their proper VSEPR
        /// geometrical positions (Does not currently account for lone pairs)
        /// <br/> NOTE: This does not currently work with cyclic molecules </summary>
        /// <param name="bondCount"> The number of bonds the current Element has
        /// (does nothing with bonds less than 2 or bonds greater than 6) </param>
        /// <param name="start"> An offset variable that ensures
        /// moveChildren() will never move the "parent" Element </param>
        public void MoveChildren(VisualAtom atom)
        {

            var neighbors = _abstractMolecule.GetBondedNeighbors(atom);

            var lonePairs = atom.GetLonePairs();
            
            var bondCount = neighbors.Count;
            
            if (bondCount < 2)
            {
                return;
            }
            // TODO: item1 vs. item2
            int bonds = bondCount + atom.GetLonePairs();
            Debug.Log("bonds: " + bonds);
            if (bonds == 2)
            {
                // if (gameObject == creationUser.head)
                // {
                neighbors[neighbors.Count - 1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                neighbors[neighbors.Count - 1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                // }

                Debug.Log("index: " + (1) + "   name: " + neighbors[1].Item2.name);
            }
            else if (bonds == 3)
            {
                if (gameObject == creationUser.head)
                {
                    for (int i = 1; i < 3 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 120 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 120 * i);
                    }
                }
                else
                {
                    for (int i = 2; i < 4 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 120 * (i - 1));
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 120 * (i - 1));
                    }
                }

            }
            else if (bonds == 4)
            {
                if (gameObject == creationUser.head)
                {
                    for (int i = 1; i < 4 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 120);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 120);

                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 120 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 120 * i);

                    }
                }
                else
                {
                    for (int i = 1; i < 4 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 120);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 120);

                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 120 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 120 * i);

                    }
                }
            }
            else if (bonds == 5)
            {
                if (gameObject == creationUser.head)
                {
                    neighbors[1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                    neighbors[1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                    for (int i = 2; i < 5 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 90);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 90);

                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 120 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 120 * i);

                    }
                }
                else
                {
                    neighbors[1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                    neighbors[1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                    for (int i = 2; i < 5 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 90);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 90);

                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 120 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 120 * i);

                    }
                }
            }
            else if (bonds == 6)
            {
                if (gameObject == creationUser.head)
                {
                    neighbors[1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                    neighbors[1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                    for (int i = 2; i < 6 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 90);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 90);

                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 90 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 90 * i);
                    }
                    if (lonePairs == 2)
                    { // fixing square planar geometry
                        neighbors[2].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 90);
                        neighbors[2].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 90);
                    }
                }
                else
                {
                    neighbors[1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                    neighbors[1].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 180);
                    for (int i = 2; i < 6 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 90);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 90);

                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 90 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 90 * i);
                    }
                    if (lonePairs == 2)
                    { // fixing square planar geometry
                        neighbors[3].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 90);
                        neighbors[3].GetVisualAtom().transform.RotateAround(transform.position, transform.up, 90);
                    }
                }
            }
            else if (bonds == 7)
            {
                if (gameObject == creationUser.head)
                {
                    for (int i = 0; i < 6 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 72 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 72 * i);
                    }
                    if (lonePairs < 2)
                    {
                        neighbors[5].GetVisualAtom().transform.RotateAround(transform.position, transform.right, 90);
                        neighbors[5].GetVisualAtom().transform.RotateAround(transform.position, transform.right, 90);
                        if (lonePairs < 1)
                        {
                            neighbors[6].GetVisualAtom().transform.RotateAround(transform.position, transform.right, -90);
                            neighbors[6].GetVisualAtom().transform.RotateAround(transform.position, transform.right, -90);
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < 6 - lonePairs; i++)
                    {
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 72 * i);
                        neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 72 * i);
                    }
                    if (lonePairs < 2)
                    {
                        neighbors[5].GetVisualAtom().transform.RotateAround(transform.position, transform.right, 90);
                        neighbors[5].GetVisualAtom().transform.RotateAround(transform.position, transform.right, 90);
                        if (lonePairs < 1)
                        {
                            neighbors[6].GetVisualAtom().transform.RotateAround(transform.position, transform.right, -90);
                            neighbors[6].GetVisualAtom().transform.RotateAround(transform.position, transform.right, -90);
                        }
                    }
                }
            }
            else if (bonds == 8)
            {
                // TODO: should probably fix this, it's eyeballed without proper mathed-out angles (and also is only square antiprismal)
                // getting an axis for rotating things at a 60 degree offset from transform.right
                Vector3 newAxis = Vector3.RotateTowards(transform.right, transform.up, Mathf.PI / 6, 0);
                for (int i = 4; i < 8; i++)
                {
                    neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 60);
                    neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, transform.forward, 60);

                    neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, newAxis, 45);
                    neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, newAxis, 45);
                }
                for (int i = 0; i < 8; i++)
                {
                    neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, newAxis, 90 * i);
                    neighbors[i].GetVisualAtom().transform.RotateAround(transform.position, newAxis, 90 * i);
                }
            }
            else
            {
                // TODO: implement coordination numbers up to 16.
                // This is way unimportant right now given coordination numbers above 8 aren't possible with just valence electrons/lone pairs
            }
            // recursively move all grandchildren
            foreach (var neighbor in neighbors)
            {
                if (gameObject != creationUser.head && Equals(neighbor, neighbors[0]))
                {
                    continue;
                }
                ResetChildPositions(neighbor);
                MoveChildren(neighbor, 1);
            }
        }
        
        /// <summary>
        /// Calculates the positions of the lone pairs of this atom and displays them
        /// <br></br>
        /// TODO: Change lone pair position calculation from distance-based to charge-based (using coulomb's law and likely the individual charges of atoms)
        /// </summary>
        public void ShowLonePairs()
        {
            if (lonePairs == 0)
            {
                return;
            }

            // finding the position on the atom furthest from any bonds
            int count = 0;
            Vector3 lonePairCenter = Vector3.zero;
            Vector3 long1 = Vector3.zero;
            Vector3 long2 = Vector3.zero;
            Vector3 short1 = Vector3.positiveInfinity;
            Vector3 short2 = Vector3.negativeInfinity;
            foreach (Tuple<GameObject, GameObject> item in neighbors)
            {
                lonePairCenter += item.Item2.transform.position;
                count++;
                foreach (Tuple<GameObject, GameObject> other in neighbors)
                {
                    if (!Equals(item, other))
                    {
                        if (Vector3.Distance(item.Item2.transform.position, other.Item2.transform.position) >
                            Vector3.Distance(long1, long2))
                        {
                            long1 = item.Item2.transform.position;
                            long2 = other.Item2.transform.position;
                        }

                        if (Vector3.Distance(item.Item2.transform.position, other.Item2.transform.position) <
                            Vector3.Distance(short1, short2))
                        {
                            short1 = item.Item2.transform.position;
                            short2 = other.Item2.transform.position;
                        }
                    }
                }
            }

            if (count > 0)
            {
                lonePairCenter /= count;
            }

            lonePairCenter = (transform.position - (lonePairCenter - transform.position));
            lonePairCenter = Vector3.Lerp(transform.position, lonePairCenter,
                (transform.localScale.x / 2 + 0.1f) / Vector3.Distance(transform.position, lonePairCenter));

            // finding which axis has the greatest distance between atoms
            Vector3 rotationAxis = long1 - long2;
            float lonePairAngle = Vector3.Angle(short1 - transform.position, short2 - transform.position);

            // if atom is solo, set an arbitrary axis for the lone pairs to rotate around
            if (neighbors.Count == 0)
            {
                rotationAxis = Vector3.up;
            }

            // if atom only has 1 bond, set the rotation axis to be in line with the bond
            if (neighbors.Count == 1)
            {
                rotationAxis = transform.position - lonePairCenter;
            }

            bool
                fullRadial =
                    false; // determines if the lone pairs should spread evenly around the axis or if they will be constrained by the other atoms
            // setting the center point if lonePairCenter = center of atom
            // this currently only works if the molecular geometry is linear or planar, I don't know enough chemistry to know if that's okay
            if (Mathf.Abs(lonePairCenter.x) < 0.01f && Mathf.Abs(lonePairCenter.y) < 0.01f &&
                Mathf.Abs(lonePairCenter.z) < 0.01f)
            {
                fullRadial = true;
                // setting an arbitrarily different value from the center if neighbors = 0
                Vector3 otherPosition = transform.position;
                otherPosition.x += 1;
                // setting an arbitrarily different value from a neighbor if neighbors = 2
                if (neighbors.Count == 2)
                {
                    otherPosition = neighbors[0].Item2.transform.position;
                    if (Mathf.Abs(otherPosition.x) < 0.01f)
                    {
                        otherPosition.x += 1;
                    }
                    else if (Mathf.Abs(otherPosition.y) < 0.01f)
                    {
                        otherPosition.y += 1;
                    }
                    else
                    {
                        otherPosition.z += 1;
                    }
                }
                else if (neighbors.Count > 2)
                {
                    // if there are more than 2 neighbors there's guaranteed to be an atom not on the rotation axis
                    // finding an atom not on the rotation axis
                    foreach (Tuple<GameObject, GameObject> item in neighbors)
                    {
                        if (item.Item2.transform.position != long1 && item.Item2.transform.position != long2)
                        {
                            otherPosition = item.Item2.transform.position;
                        }
                    }
                }

                otherPosition = Vector3.Cross(rotationAxis, transform.position - otherPosition);
                lonePairCenter = Vector3.Lerp(transform.position, otherPosition,
                    (transform.localScale.x / 2 + 0.1f) / Vector3.Distance(transform.position, otherPosition));
            }

            if (neighbors.Count == 1)
            {
                // currently this is hard-coded, I'm not aware of a way to do this mathematically
                // also it only works for 3 or less lone pairs, I don't think atoms can have more and still bond but I might be wrong there
                if (lonePairs == 1)
                {
                    // spawning a lone pair
                    GameObject obj =
                        AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as
                            GameObject;
                    GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                    clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                    // making lone pair face the atom
                    clone.transform.LookAt(transform);
                    clone.transform.Rotate(0, 90, 0);
                }
                else if (lonePairs == 2)
                {
                    // getting the actual axis of rotation
                    Vector3 otherPosition = neighbors[0].Item2.transform.position;
                    if (Mathf.Abs(otherPosition.x) < 0.01f)
                    {
                        otherPosition.x += 1;
                    }
                    else if (Mathf.Abs(otherPosition.y) < 0.01f)
                    {
                        otherPosition.y += 1;
                    }
                    else
                    {
                        otherPosition.z += 1;
                    }

                    rotationAxis = Vector3.Cross(rotationAxis, transform.position - otherPosition);

                    // spawning a lone pair
                    GameObject obj =
                        AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as
                            GameObject;
                    GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                    clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                    // making lone pair face the atom
                    clone.transform.LookAt(transform);
                    clone.transform.Rotate(0, 90, 0);

                    // moving the lone pair
                    clone.transform.RotateAround(transform.position, rotationAxis, 60);

                    // spawning the second lone pair
                    obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as
                        GameObject;
                    clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                    clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                    // making lone pair face the atom
                    clone.transform.LookAt(transform);
                    clone.transform.Rotate(0, 90, 0);

                    // moving the lone pair
                    clone.transform.RotateAround(transform.position, rotationAxis, -60);
                }
                else if (lonePairs == 3)
                {
                    // getting the secondary axis of rotation
                    Vector3 otherAxis = neighbors[0].Item2.transform.position;
                    if (Mathf.Abs(otherAxis.x) < 0.01f)
                    {
                        otherAxis.x += 1;
                    }
                    else if (Mathf.Abs(otherAxis.y) < 0.01f)
                    {
                        otherAxis.y += 1;
                    }
                    else
                    {
                        otherAxis.z += 1;
                    }

                    otherAxis = Vector3.Cross(rotationAxis, transform.position - otherAxis);

                    for (int i = 0; i < 3; i++)
                    {
                        // spawning a lone pair
                        GameObject obj =
                            AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as
                                GameObject;
                        GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                        clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                        // making lone pair face the atom
                        clone.transform.LookAt(transform);
                        clone.transform.Rotate(0, 90, 0);

                        // moving the lone pair
                        clone.transform.RotateAround(transform.position, otherAxis, 71);
                        clone.transform.RotateAround(transform.position, rotationAxis, 120 * i);
                    }
                }
                else
                {
                    Debug.Log("More than 3 lone pairs!");
                }
            }
            else if (fullRadial)
            {
                for (int i = 0; i < lonePairs; i++)
                {
                    // spawning a lone pair
                    GameObject obj =
                        AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as
                            GameObject;
                    GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                    clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                    // making lone pair face the atom
                    clone.transform.LookAt(transform);
                    clone.transform.Rotate(0, 90, 0);

                    // moving the lone pair into place
                    clone.transform.RotateAround(transform.position, rotationAxis, i * (360 / lonePairs));
                }
            }
            else
            {
                if (lonePairs % 2 == 0)
                {
                    // this wouldn't work for even lone pair counts other than 2, but I don't believe those cases exist
                    // spawning a lone pair
                    GameObject obj =
                        AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as
                            GameObject;
                    GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                    clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                    // making lone pair face the atom
                    clone.transform.LookAt(transform);
                    clone.transform.Rotate(0, 90, 0);

                    // moving the lone pair
                    clone.transform.RotateAround(transform.position, rotationAxis, lonePairAngle / 2);

                    // spawning the second lone pair
                    obj = AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as
                        GameObject;
                    clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                    clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                    // making lone pair face the atom
                    clone.transform.LookAt(transform);
                    clone.transform.Rotate(0, 90, 0);

                    // moving the lone pair
                    clone.transform.RotateAround(transform.position, rotationAxis, lonePairAngle / -2);
                }
                else
                {
                    for (int i = 0; i < lonePairs; i++)
                    {
                        // spawning a lone pair
                        GameObject obj =
                            AssetDatabase.LoadAssetAtPath("Assets/Resources/LonePair.prefab", typeof(GameObject)) as
                                GameObject;
                        GameObject clone = Instantiate(obj, lonePairCenter, Quaternion.identity);
                        clone.transform.SetParent(GameObject.Find("moleculeBody").transform, true);

                        // making lone pair face the atom
                        clone.transform.LookAt(transform);
                        clone.transform.Rotate(0, 90, 0);

                        // moving the lone pair
                        if (i > lonePairs / 2)
                        {
                            clone.transform.RotateAround(transform.position, rotationAxis, (i / 2) * lonePairAngle);
                        }
                        else
                        {
                            clone.transform.RotateAround(transform.position, rotationAxis, i * lonePairAngle);
                        }
                    }
                }
            }

        }
        
    }
}