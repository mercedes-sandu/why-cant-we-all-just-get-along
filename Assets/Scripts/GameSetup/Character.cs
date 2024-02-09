﻿using System;
using System.Collections.Generic;
using System.Linq;
using CatSAT;
using Imaginarium.Generator;
using Imaginarium.Ontology;
using UnityEngine;

namespace GameSetup
{
    public class Character
    {
        public string FirstName { get; private set; }
        public string Surname { get; private set; }
        public int Age { get; private set; }
        public string Alignment { get; private set; }
        public string[] PersonalityTraits { get; private set; }
        public string Occupation { get; private set; }
        public string[] Likes { get; private set; }
        public string[] Dislikes { get; private set; }

        /// <summary>
        /// Creates a character based off of a given individual and surname. Extracts the character's properties from
        /// the solution and assigns them to the character.
        /// </summary>
        /// <param name="character">The character generated.</param>
        /// <param name="surname">The surname of the character.</param>
        /// <param name="solution">The solution of the generated output.</param>
        public Character(PossibleIndividual character, string surname, Solution solution)
        {
            FirstName = character.Name;
            
            Surname = surname;
            
            // todo: fix the other properties to read like this
            Dictionary<Property, Variable> properties = character.Individual.Properties;
            var ageProperty = properties[SetupMaster.Ontology.Property("age")];
            Age = (int) Math.Round((float)solution[ageProperty]);
            
            // var thing = character[]
            
            List<string> adjectives =
                character.AdjectivesDescribing().Select(adjective => adjective.ToString()).ToList();
            adjectives.RemoveAt(adjectives.Count - 1); // type of name, not relevant
            
            Alignment = adjectives[0];
            
            PersonalityTraits = adjectives.GetRange(1, 2).ToArray();
            
            Occupation = properties[SetupMaster.Ontology.Property("occupation")].ValueString(solution)
                .Split("=")[1];
            
            Debug.Log(
                $"{FirstName} {Surname}\nage: {Age}\nalignment: {Alignment}\npersonality traits: {string.Join(", ", PersonalityTraits)}\noccupation: {Occupation}");
            
            // Debug.Log(string.Join(", ", properties.Select(property => $"{property.Key} : {property.Value}")));
        }

        /// <summary>
        /// Returns a string representation of the character.
        /// </summary>
        /// <returns>The character.</returns>
        public override string ToString()
        {
            // todo: add likes and dislikes
            return
                $"<b>{FirstName} {Surname}</b>\nAge: {Age}\nAlignment: {Alignment}\nOccupation: {Occupation}";
        }
    }
}

// silly scenario ideas from the group
// baby in a basket
// the other family ritually sacrificed your pet parrot
// your mom joke