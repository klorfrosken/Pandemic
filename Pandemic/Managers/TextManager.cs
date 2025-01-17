﻿using System;
using System.Collections.Generic;
using System.Text;
using Pandemic.Game;
using Pandemic.Cards;
using Pandemic.Cards.EventCards;

namespace Pandemic.Managers
{
    public class TextManager
    {
        public static readonly int AvailableStandardActions = 8;
        
        public static int GetDifficulty()
        {
            Console.WriteLine("What difficulty would you like for the game?");
            Console.WriteLine("1: Introductory (4 epidemic cards)");
            Console.WriteLine("2: Standard (5 epidemic cards)");
            Console.WriteLine("3: Heroic (6 epidemic cards)");

            int UserInput = GetValidInteger(1, 3);

            if (UserInput == 1)
            {
                return 4;
            } else if (UserInput == 2)
            {
                return 5;
            } else
            {
                return 6;
            }
        }

        public static void BeginGame(StateManager State, List<User> Users)
        {
            Console.WriteLine("|---------------------------------------------------------------------------------------|");  
            Console.WriteLine("|     Welcome to Pandemic! The cooperative game where the world (most likely) dies.     |");
            Console.WriteLine("|---------------------------------------------------------------------------------------|");

            Console.WriteLine("\nYou are the following players, with their respective roles:");

            foreach (User CurrentUser in Users)
            {
                Console.WriteLine($"{CurrentUser.UserName} - {CurrentUser.CurrentRole}");
            }

            PrintStatus(State);
        }

        public static void PrintStatus(StateManager State)
        {
            Console.WriteLine($"\nThere have been {State.Outbreaks} outbreaks up till now.");
            Console.WriteLine($"The current infection rate is {State.InfectionRates[State.InfectionIndex]}.");
            Console.WriteLine("\nThe following cities are infected with diseases:");

            //Cities with infection
            foreach(City CurrentCity in State.Cities)
            {
                if (!CurrentCity.MultipleDiseases)
                {
                    if(CurrentCity.DiseaseCubes[CurrentCity.Color] != 0)
                    {
                        Console.WriteLine($"{CurrentCity.Name} has {CurrentCity.DiseaseCubes[CurrentCity.Color]} {CurrentCity.Color} disease cubes");
                    }
                } 
                else
                {
                    List<string> cubeNumbers = new List<string>();
                    if (CurrentCity.DiseaseCubes[Colors.Yellow] != 0)
                    {
                        cubeNumbers.Add($"{CurrentCity.DiseaseCubes[Colors.Yellow]} yellow disease cubes ");
                    }

                    if (CurrentCity.DiseaseCubes[Colors.Red] != 0)
                    {
                        cubeNumbers.Add($"{CurrentCity.DiseaseCubes[Colors.Red]} red disease cubes ");
                    }

                    if (CurrentCity.DiseaseCubes[Colors.Blue] != 0)
                    {
                        cubeNumbers.Add($"{CurrentCity.DiseaseCubes[Colors.Blue]} blue disease cubes ");
                    }

                    if (CurrentCity.DiseaseCubes[Colors.Black] != 0)
                    {
                        cubeNumbers.Add($"{CurrentCity.DiseaseCubes[Colors.Black]} black disease cubes ");
                    }

                    string printLine = $"{CurrentCity.Name} has ";
                    for(int i=0; i<cubeNumbers.Count; i++)
                    {
                        printLine += cubeNumbers[i];
                        if(i+1 == cubeNumbers.Count)
                        {
                            printLine += "and ";
                        }
                    }

                    Console.WriteLine(printLine);
                }
            }
        }

        public static void PrintHand(Role currentRole)
        {
            Console.WriteLine("\nYou have the following cards in your hand:");
            foreach(Card currentCard in currentRole.Hand)
            {
                Console.WriteLine($"{currentCard}");
            }
        }

        public static void PrintEventDescription(EventCard card)
        {
            Console.WriteLine($"{card.Name} allows you to {card.description}");
        }

        public static void AvailableActions(Role currentRole)
        {
            Console.WriteLine("|---------------------------------------------------------------------------------------|");

            Console.WriteLine($"\nIt is the {currentRole.RoleName}'s turn.");
            Console.WriteLine("You have the following available actions: \n");

            Console.WriteLine("MOVEMENT ACTIONS:");
            Console.WriteLine("1: DRIVE/FERRY - Move to a city connected by a white line");
            Console.WriteLine("2: DIRECT FLIGHT - Discard a city card to move to the city named on that card");
            Console.WriteLine("3: CHARTER FLIGHT - Discard the city card that matches the city you are in to move to _any_ city");
            Console.WriteLine("4: SHUTTLE FLIGHT - Move from a city with a research station to any other city that has a research station\n");

            Console.WriteLine("SPECIAL ACTIONS:");
            Console.WriteLine("5: BUILD A RESEARCH STATION - Discard the city card that _matches_ the city that you are in to place a research station there");
            Console.WriteLine("6: TREAT DISEASE - Remove one disease cube from the city you are in. If this color is cured remove all cubes _of that color_ from the city");
            Console.WriteLine("7: DISCOVER A CURE - At _any_ research station, discard 5 city cards of the same disease color to cure that disease");
            Console.WriteLine("8: SHARE KNOWLEDGE - Either: _give_ the card that _matches_ the city that you are in to another player, or _take_ that card from another player. The other player must also be in the city with you\n");

            currentRole.PrintSpecialAbilities();

            Console.WriteLine("--------------------------------------------------------");
            PrintHand(currentRole);
            Console.WriteLine("\nWhich one of the above would you like to do?");
        }

        public static int DiscardOrPlay(List<Card> Hand)
        {
            Boolean EventInHand = Hand.Exists(Card => Card is EventCard);

            string screenText;
            if (EventInHand)
            {
                Console.WriteLine("You have more than 7 cards in your hand and have to either play an event or discard a card.");
                screenText = "discard or play";
            } else
            {
                Console.WriteLine("You have more than 7 cards in your hand and have to discard a card.");
                screenText = "discard";
            }

            return ChooseItemFromList(Hand, screenText);
        }

        public static int ShareKnowledgeWithScientist()
        {
            Console.WriteLine("Which one of you will be receiving a card?");
            Console.WriteLine("1: You will be receiving a card");
            Console.WriteLine("2: The Scientist will be receiving a card");

            return GetValidInteger(1, 2);
        }

        public static int ChooseItemFromList<T>(List<T> ItemList, string PickOneOfTheFollowingTo)
        {
            Console.WriteLine($"Pick one of the following to {PickOneOfTheFollowingTo}:");
            int counter = 0;
            foreach (var item in ItemList)
            {
                counter++;
                Console.WriteLine($"{counter}: {item.ToString()}");
            }

            return (GetValidInteger(1, ItemList.Count+1)-1);
        }

        public static int GetValidInteger(int LowerRange, int UpperRange)
        {
            Boolean InputNotValid = true;
            int UserInput = -1;
            while (InputNotValid)
            {
                UserInput = Int32.Parse(Console.ReadLine());

                if (UserInput < LowerRange || UserInput > UpperRange)
                {
                    Console.WriteLine($"{UserInput} is not valid input. Please enter a number between {LowerRange} and {UpperRange}");
                }
                else
                {
                    InputNotValid = false;
                }
            }
            return UserInput;
        }
    }
}