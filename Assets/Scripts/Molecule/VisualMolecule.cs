using System.Collections.Generic;
using UnityEngine;

namespace Molecule
{
    
    /// <summary> Handles the in-engine rendering aspects of molecules. </summary>
    public class VisualMolecule : MonoBehaviour
    {

        /// <summary> A reference to the corresponding abstract molecule. </summary>
        private readonly AbstractMolecule _abstractMolecule;
        public AbstractMolecule GetAbstractMolecule() => _abstractMolecule;
        
        /// <summary> A list of atoms in this molecule. </summary>
        private readonly List<VisualAtom> _atoms = new();
        public IReadOnlyList<VisualAtom> GetAtoms() => _atoms.AsReadOnly();

        /// <summary> A list of bonds within this molecule. </summary>
        private readonly List<VisualBond> _bonds = new();

        
        /// <summary> Adds an atom to this molecule. </summary>
        /// <param name="atom"> The atom to add. </param>
        public void AddVisualAtom(VisualAtom atom)
        {
            _atoms.Add(atom);
        }

        /// <summary> Removes an atom from this molecule. </summary>
        /// <param name="atom"> The atom to remove. </param>
        public void RemoveVisualAtom(VisualAtom atom)
        {
            _atoms.Remove(atom);
        }

        public void PositionVisualAtomAround(VisualAtom parent, VisualAtom atom)
        {
            
            var radius =
                parent.GetAbstractAtom().GetCovalentRadius() +
                atom.GetAbstractAtom().GetCovalentRadius();
            
            ResetAtomPosition(atom);
            
            
            //atom.transform.localEulerAngles = visualBond.transform.localEulerAngles; //+ this.transform.localEulerAngles;

            //atom.transform.localPosition = visualBond.transform.localPosition;
            //atom.transform.Translate(0, -radius / 2, 0);

            //MoveChildren(atom);

            
        }

        /// <summary>
        /// Manages the visual bond between the two specified atoms.
        /// May create or destroy a visual bond.
        /// </summary>
        /// <param name="atom1"></param>
        /// <param name="atom2"></param>
        /// <param name="bondOrder"></param>
        public void SetVisualBond(VisualAtom atom1, VisualAtom atom2, int bondOrder)
        {
            
            // delete whatever visual bond was already between the two atoms
            var previousVisualBond = _bonds.Find(b => b.Matches(atom1, atom2));
            if (previousVisualBond is not null) RemoveVisualBond(previousVisualBond);
            
            // if the bond order is zero, we're not creating anything.
            if (bondOrder == 0) return;
            
            // create a visual bond object
            var visualBond = VisualBond.CreateVisualBond(atom1, atom2, bondOrder);
            _bonds.Add(visualBond);

        }


        private void RemoveVisualBond(VisualBond visualBond)
        {
            
            _bonds.Remove(visualBond);
            
            Destroy(visualBond.gameObject);
            
        }
        
    }
}