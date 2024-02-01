using System.Collections.Generic;
using System.Linq;
using Imaginarium.Ontology;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace GameSetup
{
    public class SetupMaster : MonoBehaviour
    {
        [SerializeField] private int minFamilySize;
        [SerializeField] private int maxFamilySize;
        
        // character generation
        private static Ontology _ontology;
   
        // graph generation
        private int _familyOneSize;
        private int _familyTwoSize;
        private string _familyOneSurname;
        private string _familyTwoSurname;
        private Family _familyOne;
        private Family _familyTwo;
        private Family _combinedFamily;
    
        /// <summary>
        /// Initializes the families.
        /// </summary>
        private void Awake()
        {
            FamilyPreprocessing();
            
            _familyOne = new Family(_familyOneSize, _familyOneSurname);
            _familyTwo = new Family(_familyTwoSize, _familyTwoSurname);
            _combinedFamily = new Family(_familyOneSize + _familyTwoSize, _familyOne, _familyTwo);

            // _ontology = new Ontology("Characters", $"{Application.persistentDataPath}");

            // FileCopier.CheckForCopies();

            // MakeCharacters();
        }

        /// <summary>
        /// Selects a random size and a random surname for each family.
        /// </summary>
        private void FamilyPreprocessing()
        {
            // set family sizes
            _familyOneSize = Random.Range(minFamilySize, maxFamilySize);
            _familyTwoSize = Random.Range(minFamilySize, maxFamilySize);
            
            // select family surnames
            List<string> surnames = 
                Resources.Load<TextAsset>("Imaginarium/surnames").text.Split('\n').ToList();
            int familyOneSurnameIndex = Random.Range(0, surnames.Count);
            _familyOneSurname = surnames[familyOneSurnameIndex];
            surnames.RemoveAt(familyOneSurnameIndex);
            _familyTwoSurname = surnames[Random.Range(0, surnames.Count)];
        }

        // todo: change void to ?
        /// <summary>
        /// 
        /// </summary>
        public void MakeCharacters()
        {
            var character = _ontology.CommonNoun("character");
            var invention = character.MakeGenerator(_familyOneSize + _familyTwoSize).Generate();

            var print = "";
            
            foreach (var c in invention.PossibleIndividuals)
            {
                print += c.Name + "\n";
            }
            
            Debug.Log(print);
        }

        /// <summary>
        /// Shows the graph corresponding to the selected family and prints the edges.
        /// </summary>
        /// <param name="index">1 if family one, 2 if family two, 3 if combined family.</param>
        public void ShowGraph(int index)
        {
            switch (index)
            {
                case 1:
                    _familyOne.ShowGraph();
                    _familyOne.PrintEdges();
                    break;
                case 2:
                    _familyTwo.ShowGraph();
                    _familyTwo.PrintEdges();
                    break;
                case 3:
                    _combinedFamily.ShowGraph();
                    _combinedFamily.PrintEdges();
                    break;
            }
        }
    }
}