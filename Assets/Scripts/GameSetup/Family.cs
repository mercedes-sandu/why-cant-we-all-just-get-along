﻿using System.Collections.Generic;
using System.Linq;
using CatSAT;
using CatSAT.SAT;
using GraphVisualizing;
using Imaginarium.Generator;
using Imaginarium.Ontology;
using UnityEngine;

namespace GameSetup
{
    public class Family
    {
        private readonly int _size;
        private readonly string _surname;

        private Problem _problem;
        private Graph _graph;
        public Dictionary<ushort, EdgeProposition> Edges;
        private Solution _solution;
        private GraphViz<Character> _graphViz;
        public Dictionary<int, Character> Characters;

        private bool _isFamilyOne;

        /// <summary>
        /// Initializes a family based off of a given number of members and surname. Creates a graph that is passed to
        /// CatSAT to generate a solution. Creates a GraphViz object to visualize the graph.
        /// </summary>
        /// <param name="size">The number of members in the family (the number of nodes in the graph).</param>
        /// <param name="surname">The surname of the family.</param>
        /// <param name="minDensity"></param>
        /// <param name="maxDensity"></param>
        /// <param name="isFamilyOne"></param>
        public Family(int size, string surname, float minDensity, float maxDensity, bool isFamilyOne)
        {
            _size = size;
            _surname = surname;

            _problem = new Problem();
            _graph = new Graph(_problem, _size);
            _graph.Connected();
            _graph.Density(minDensity, maxDensity);
            Edges = _graph.SATVariableToEdge;
            _solution = _problem.Solve();
            Characters = new Dictionary<int, Character>();
            
            _isFamilyOne = isFamilyOne;

            _graphViz = new GraphViz<Character>();
        }

        /// <summary>
        /// Initializes a family based off of two other families. Used to create the "combined" family. Incorporates an
        /// offset to the nodes as well as the indices of the edges to avoid overlap.
        /// </summary>
        /// <param name="size">The size of the combined family (the number of nodes in the first family's graph added
        /// to the number of nodes in the second family's graph).</param>
        /// <param name="familyOne">The first family.</param>
        /// <param name="familyTwo">The second family.</param>
        public Family(int size, Family familyOne, Family familyTwo)
        {
            _size = size;
            _surname = $"{familyOne._surname} and {familyTwo._surname}";
            Characters = new Dictionary<int, Character>();

            _graphViz = new GraphViz<Character>(); // change to pair of int and family number

            // todo: pick some number of edges to add between nodes in family one and nodes in family two
            // should i do this with catsat ?? ^^

            // separate set of edge predicates linking the two graphs
            // separate cardinality constraint on those edge predicates

            // to address relationship types, use generation numbers

            Edges = new Dictionary<ushort, EdgeProposition>();
            foreach (var (index, edge) in familyOne.Edges)
            {
                Edges.TryAdd(index, edge);
            }

            ushort indexOffset = (ushort)familyOne.Edges.Count;
            foreach (var (index, edge) in familyTwo.Edges)
            {
                Edges.TryAdd((ushort)(index + indexOffset), edge);
            }
        }

        /// <summary>
        /// Sets the characters in the family. Assigns them to the nodes in the graph.
        /// </summary>
        /// <param name="generatedCharacters">A pair (model, list of generated characters).</param>
        public void SetCharacters((Solution, List<PossibleIndividual>) generatedCharacters)
        {
            foreach (PossibleIndividual character in generatedCharacters.Item2)
            {
                int index = Characters.Count;
                Characters.Add(index, new Character(character, _surname, generatedCharacters.Item1, _isFamilyOne));
            }

            // todo: is there a way to specify which node styles to use for which nodes?
            foreach (var (index, edge) in Edges.Where(edge => _solution[edge.Value]))
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(Characters[edge.SourceVertex],
                    Characters[edge.DestinationVertex]));
            }
        }

        /// <summary>
        /// Sets the characters in the combined family graph. Assigns them to the nodes in the graph.
        /// </summary>
        /// <param name="familyOne">The first family which was generated.</param>
        /// <param name="familyTwo">The second family which was generated.</param>
        public void SetCharacters(Family familyOne, Family familyTwo)
        {
            foreach (var (index, character) in familyOne.Characters)
            {
                Characters.Add(index, character);
            }

            foreach (var (index, character) in familyTwo.Characters)
            {
                Characters.Add(index + familyOne._size, character);
            }
            
            foreach (var edge in familyOne.Edges.Values.Where(edge => familyOne._solution[edge]))
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(Characters[edge.SourceVertex],
                    Characters[edge.DestinationVertex]));
            }
            
            foreach (var edge in familyTwo.Edges.Values.Where(edge => familyTwo._solution[edge]))
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(Characters[edge.SourceVertex + familyOne._size],
                    Characters[edge.DestinationVertex + familyOne._size]));
            }
        }

        // todo: do i need this?
        /// <summary>
        /// Returns the dictionary of edges in the graph representing the family.
        /// NOTE: If this is of a single family, then all edges (whether actually present in the solution or not) are
        /// included in the dictionary. If this is of a combined family, then only the edges present in the solution are
        /// included in the dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ushort, EdgeProposition> GetEdges() => Edges;

        /// <summary>
        /// Shows the graph in the GraphVisualizer canvas.
        /// </summary>
        public void ShowGraph()
        {
            GraphVisualizer.ShowGraph(_graphViz);
        }

        /// <summary>
        /// Prints the edges present in the family's graph.
        /// </summary>
        public void PrintEdges()
        {
            if (_solution == null)
            {
                Debug.Log(_surname + " Families: \n" + Edges.Aggregate("", (current, edge) => current + edge.Key +
                    ": " +
                    edge.Value.SourceVertex + "--" + edge.Value.DestinationVertex + "\n"));
            }
            else
            {
                Debug.Log(_surname + " Family: \n" + Edges.Where(edge => _solution[edge.Value]).Aggregate("",
                    (current, edge) => current + edge.Key + ": " + edge.Value.SourceVertex + "--" +
                                       edge.Value.DestinationVertex + "\n"));
            }
        }
    }
}