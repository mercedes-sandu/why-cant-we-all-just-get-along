﻿using System.Collections.Generic;
using System.Linq;
using CCSS;
using GameSetup;
using UnityEngine;

namespace Utility
{
    public class GameManager : MonoBehaviour
    {
        public static InGameGraph InGameGraph;

        public static Card CurrentCard { get; private set; }

        public static int CurrentCompatibility { get; private set; }

        [SerializeField] private int minStartCompatibility = 5;
        [SerializeField] private int maxStartCompatibility = 20;

        private static int _currentCardIndex = 0; // todo: remove after preconditions implemented
        private static int _weekNumber = 0;
        private static Character[] _allCharacters;
        private static List<Card> _allPossibleCards;
        private static List<Card> _cardsShown; // todo: do i need this?

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            CurrentCompatibility = Random.Range(minStartCompatibility, maxStartCompatibility);
        }
        
        /// <summary>
        /// Selects the first cardTemplate to display to the player.
        /// Note: At this point, all cards have been loaded by CardTemplateLoader.
        /// </summary>
        private void Start()
        {
            InGameGraph = new InGameGraph(SetupMaster.CombinedFamily);
            _cardsShown = new List<Card>();
            _allPossibleCards = new List<Card>();
            _allCharacters = InGameGraph.AllCharacters.ToArray();
                
            foreach (CardTemplate cardTemplate in CardTemplateLoader.AllCardTemplates)
            {
                Character[] currentCombination = new Character[cardTemplate.NumRoles];
                GenerateAllPossibleCardsForTemplate(cardTemplate, currentCombination, 0, _allCharacters.Length - 1, 0);
            }

            Debug.Log($"generated {_allPossibleCards.Count} cards");

            SelectNewCard(Choice.NullChoice());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardTemplate"></param>
        /// <param name="currentCombination"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="index"></param>
        private void GenerateAllPossibleCardsForTemplate(CardTemplate cardTemplate, Character[] currentCombination,
            int start, int end, int index)
        {
            if (index == cardTemplate.NumRoles)
            {
                Dictionary<string, Character> roleToCharacter = new Dictionary<string, Character>();
                for (int i = 0; i < cardTemplate.NumRoles; i++)
                {
                    roleToCharacter[cardTemplate.Roles[i]] = currentCombination[i];
                }

                Card newCard = new Card(cardTemplate, roleToCharacter);
                _allPossibleCards.Add(newCard);
                return;
            }

            for (int i = start; i <= end && end - i + 1 >= cardTemplate.NumRoles - index; i++)
            {
                currentCombination[index] = _allCharacters[i];
                GenerateAllPossibleCardsForTemplate(cardTemplate, currentCombination, i + 1, end, index + 1);
            }
        }

        /// <summary>
        /// Chooses a new cardTemplate to display to the player. Calls the corresponding game event once the
        /// cardTemplate has been chosen.
        /// </summary>
        /// <param name="choice"></param>
        public static void SelectNewCard(Choice choice)
        {
            if (choice.HasFollowup())
            {
                Dictionary<string, Character> roleToCharacter = CurrentCard.RoleToCharacter;
                CurrentCard = new Card(CardTemplateLoader.GetCardTemplate(choice.FollowupCard), roleToCharacter);
            }
            else
            {
                // todo: eventually incorporate picking cards by preconditions that are satisfied
                CurrentCard = _allPossibleCards[_currentCardIndex];
                _currentCardIndex++;
            }

            _cardsShown.Add(CurrentCard);
            _weekNumber++;
            GameEvent.CardSelected(CurrentCard, _weekNumber);
        }
    }
}