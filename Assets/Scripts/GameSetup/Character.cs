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
        public bool InFamilyOne { get; private set; }

        /// <summary>
        /// Creates a character based off of a given individual and surname. Extracts the character's properties from
        /// the solution and assigns them to the character.
        /// </summary>
        /// <param name="character">The character generated.</param>
        /// <param name="surname">The surname of the character.</param>
        /// <param name="solution">The solution of the generated output.</param>
        /// <param name="inFamilyOne"></param>
        public Character(PossibleIndividual character, string surname, Solution solution, bool inFamilyOne)
        {
            FirstName = character.Name;

            Surname = surname;

            Dictionary<Property, Variable> properties = character.Individual.Properties;

            Age = (int)Math.Round((float)solution[properties[SetupMaster.Ontology.Property("age")]]);

            List<string> adjectives =
                character.AdjectivesDescribing().Select(adjective => adjective.ToString()).ToList();
            adjectives.RemoveAt(adjectives.Count - 1); // type of name, not relevant

            Alignment = adjectives[0];

            PersonalityTraits = adjectives.GetRange(1, 2).ToArray();

            Occupation = solution[properties[SetupMaster.Ontology.Property("occupation")]].ToString();
            
            InFamilyOne = inFamilyOne;

            Debug.Log(
                $"{FirstName} {Surname}\nage: {Age}\nalignment: {Alignment}\npersonality traits: {string.Join(", ", PersonalityTraits)}\noccupation: {Occupation}");

            // Debug.Log(string.Join(", ", properties.Select(property => $"{property.Key} : {property.Value}")));
        }
        
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
// someone invites you to join their startup
// you see a young robber run from the convenience store with merchandise they stole. do you shoot to wound?
// have you heard of our lord and savior?
// baubau shows up at your door

// todo: play mask of the rose