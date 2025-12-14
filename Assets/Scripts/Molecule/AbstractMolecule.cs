using System.Collections.Generic;
using System.Linq;

namespace Molecule
{
    
    /// <summary> A simplified molecule,
    /// made of <see cref="AbstractAtom"> atoms </see> and the bonds between them.
    /// Designed as an undirected weighted graph
    /// with the atoms as vertices and edge weights representing bond strength. </summary>
    public class AbstractMolecule
    {
        
        /// <summary> A reference to the corresponding visual molecule. </summary>
        private readonly VisualMolecule _visualMolecule;
        
        /// <returns> The corresponding visual molecule. </returns>
        public VisualMolecule GetVisualMolecule() => _visualMolecule;
        
        
        /// <summary> The internal list of atoms. </summary>
        private readonly List<AbstractAtom> _atoms = new();
        
        /// <returns> (an unmodifiable reference to) The internal list of atoms. </returns>
        public IReadOnlyList<AbstractAtom> GetAtoms() =>
            _atoms.AsReadOnly();
        
        /// <summary> Generates and adds an atom to this molecule. </summary>
        /// <param name="element"> The element of which to add an atom. </param>
        /// <returns> The created atom. </returns>
        public AbstractAtom AddAbstractAtom(Element element)
        {

            AbstractAtom atom = new(this, element);
            
            // add to internal list of atoms
            _atoms.Add(atom);
            
            // add entries for this atom in the bond matrix
            foreach (var otherBonds in _bonds.Values)
                otherBonds[atom] = 0;
            _bonds[atom] = new Dictionary<AbstractAtom, int>();
            foreach (var otherAtom in _atoms) _bonds[atom][otherAtom] = 0; // includes itself :p
            
            // add visual counterpart to visual molecule
            _visualMolecule.AddVisualAtom(atom.GetVisualAtom());

            return atom;

        }

        /// <summary> Removes an atom from this molecule. </summary>
        public void RemoveAbstractAtom(AbstractAtom atom)
        {
            
            // remove from internal list of atoms
            _atoms.Remove(atom);
            
            // remove entries for this atom in the bond matrix
            foreach (var otherBonds in _bonds.Values)
                otherBonds.Remove(atom);
            _bonds.Remove(atom);
            
            _visualMolecule.RemoveVisualAtom(atom.GetVisualAtom());
            
        }


        /// <summary> An adjacency matrix representing the bond strength between each atom.
        /// An entry of zero represents no bond between those atoms. </summary>
        private readonly Dictionary<AbstractAtom, Dictionary<AbstractAtom, int>> _bonds = new();

        /// <returns> The strength of the bond between the given atoms. </returns>
        public int GetBondStrength(AbstractAtom atom1, AbstractAtom atom2) =>
            _bonds[atom1][atom2];
        
        /// <summary> Sets a bond between the two given atoms to the given strength.
        /// Use a bond strength of 0 to remove the bond entirely. </summary>
        public void SetBondStrength(AbstractAtom atom1, AbstractAtom atom2, int bondStrength)
        {
            if (!_bonds.ContainsKey(atom1)) _bonds.Add(atom1, new Dictionary<AbstractAtom, int>());
            if (!_bonds.ContainsKey(atom2)) _bonds.Add(atom2, new Dictionary<AbstractAtom, int>());
            _bonds[atom1][atom2] = bondStrength;
            _bonds[atom2][atom1] = bondStrength;
            _visualMolecule.SetVisualBond(atom1.GetVisualAtom(), atom2.GetVisualAtom(), bondStrength);
        }

        /// <returns> A generated list of all other atoms in the molecule which
        /// are bonded to the given atom. </returns>
        public IReadOnlyList<AbstractAtom> GetBondedNeighbors(AbstractAtom atom) =>
            _atoms.Where(potentialNeighbor =>
                _bonds[atom][potentialNeighbor] != 0).ToList(); 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="atom"></param>
        /// <returns></returns>
        public int GetTotalOutgoingBonds(AbstractAtom atom) =>
            _atoms.Sum(neighbor => _bonds[atom][neighbor]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="molecule"></param>
        /// <returns></returns>
        public bool EqualsMolecule(AbstractMolecule molecule)
        {
            // TODO
            return false;
        }
        
        

        /// <summary>
        /// DFS algorithm for marking atoms for deletion
        /// </summary>
        /// <param name="current">The current atom being checked by the algorithm</param>
        /// <param name="found">The list of all atoms that have been found by the algorithm</param>
        private void DeletionDFS(AbstractAtom current, HashSet<AbstractAtom> found)
        {
            
            // TODO: rewrite this whole thing. the idea should be to keep only the largest connected group of atoms. i think thats the most intuitive approach
            
            if (!found.Add(current))
            { // if this element has been visited already, return to the last one
                return;
            }

            foreach (var other in current.GetMolecule().GetBondedNeighbors(current))
            {
                
                if (!found.Contains(other)) continue; // only if this bond has not been travelled already
                
                DeletionDFS(other, found);
            }
        }
        
    }
    
}