using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Game;
using Pandemic.Exceptions;

namespace Pandemic.Cards.EventCards
{
    public class Airlift : EventCard
    {
        readonly static string eventName = "Airlift";
        readonly static string eventDescription = $"Move any player to any city";

        public Airlift() : base (eventName, eventDescription) { }

        public override void Play(Role playerWithCard, StateManager state)
        {
            if (!playerWithCard.CardInHand(eventName)){
                throw new IllegalMoveException($"The {playerWithCard.RoleName} does not have {eventName} in their hand to play.");
            } 
            else
            {
                int playerChoice = TextManager.ChooseItemFromList(state.Roles, "move");
                Role playerToMove = state.Roles[playerChoice];
                if(playerToMove == null)
                {
                    throw new UnexpectedBehaviourException("The player you chose could not be found. That's very unexpected and it means the program crashes... Sorry!");
                } else
                {
                    List<City> EligibleCities = new List<City>(state.Cities);
                    EligibleCities.Remove(playerToMove.CurrentCity);
                    int cityChoice = TextManager.ChooseItemFromList(EligibleCities, $"move the {playerToMove} to");
                    City nextCity = state.Cities[cityChoice];

                    if(nextCity == null)
                    {
                        throw new UnexpectedBehaviourException("The city you chose could not be found. That's very unexpected and it means the program crashes... Sorry!");
                    } else
                    {
                        playerToMove.ChangeCity(nextCity, state);
                    }
                }
            }
        }
    }
}