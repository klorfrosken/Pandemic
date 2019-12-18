using System;
using System.Collections.Generic;
using Pandemic.Managers;
using Pandemic.Exceptions;
using Pandemic.Game_Elements.Roles;

namespace Pandemic.Game
{
    public class City
    {
        public string Name;
        public Colors Color;
        public Boolean ResearchStation = false;
        public Boolean MultipleDiseases = false;
        public Dictionary<Colors, int> DiseaseCubes = new Dictionary<Colors, int>
        {
            {Colors.Yellow, 0 },
            {Colors.Red, 0 },
            {Colors.Blue, 0 },
            {Colors.Black, 0 }
        };
        public List<City> ConnectedCities = new List<City>();

        public City(string Name, Colors Color)
        {
            this.Name = Name;
            this.Color = Color;
        }

        public Boolean IsConnectedTo(City otherCity)
        {
            return ConnectedCities.Exists(City => City == otherCity);
        }

        public Boolean BuildResearchStation()
        {
            if (ResearchStation)
            {
                Console.WriteLine($"There is already a researchstation in {Name}");
                return false;
            } else
            {
                ResearchStation = true;
                return true;
            }            
        }

        public void TreatDisease(Colors Color, StateManager State)
        {
            if (!DiseaseCubes.ContainsKey(Color))
            {
                throw new UnexpectedBehaviourException($"An invalid argument was used in the Treat Disease method in City. {Color} is not a valid color");
            } else if (DiseaseCubes[Color] == 0 )
            {
                throw new IllegalMoveException($"There are no {Color} cubes in {Name}");
            } else
            {
                DiseaseCubes[Color]--;
                State.CubePools[Color]++;
            }
        }

        public Boolean DiseaseIsEradicated(Colors Color, StateManager State)
        {
            bool CubePoolIsFull = State.CubePools[Color] == State.MaxCubesInCubePool;
            bool CureIsFound = State.Cures[Color];
            return (CubePoolIsFull && CureIsFound);
        }

        public Boolean InfectionPreventedByQuarantineSpecialist(StateManager State)
        {
            if (!State.QuarantineSpecialistInGame)
            {
                return false;
            } else
            {
                Role QuarantineSpecialist = State.Roles.Find(Player => Player is QuarantineSpecialist);
                Boolean QSisHere = (QuarantineSpecialist.CurrentCity == this);
                Boolean QSisInConnectedCity = ConnectedCities.Exists(City => QuarantineSpecialist.CurrentCity == City);

                return (QSisHere || QSisInConnectedCity);
            }
        }

        public Boolean InfectionPreventedByMedic(Colors Color, StateManager State)
        {
            if (!State.MedicInGame)
            {
                return false;
            } else
            {
                Role Medic = State.Roles.Find(Player => Player is Medic);
                Boolean CureFound = State.Cures[Color];

                return (Medic.CurrentCity == this && CureFound);
            }
        }

        public void InfectCity(Colors Color, StateManager State)
        {
            if (!DiseaseIsEradicated(Color, State))
            {
                if (!InfectionPreventedByQuarantineSpecialist(State))
                {
                    if (!InfectionPreventedByMedic(Color, State))
                    {
                        Boolean OutbreakThisChain = State.OutbreakThisChain.Exists(City => City == this);
                        if (DiseaseCubes[Color] == 3 && !OutbreakThisChain)
                        {
                            Outbreak(Color, State);
                            //blir dette helt riktig? kan den bryte ut på nytt hvis en annen by får den til å gjøre det?
                        }
                        else
                        {
                            if (Color != this.Color)
                            {
                                MultipleDiseases = true;
                            }

                            if (State.CubePools[Color] == 0)
                            {
                                throw new TheWorldIsDeadException($"There are no more {Color} cubes left. The disease has spread too much.");
                            }
                            else
                            {
                                DiseaseCubes[Color]++;
                                State.CubePools[Color]--;
                                TextManager.PrintInfection(this, Color);
                            }
                        }
                    }
                }
            }
        }

        public void Outbreak(Colors Color, StateManager State)
        {
            State.Outbreaks++;

            if (State.Outbreaks == State.MaxOutbreaks)
            {
                throw new TheWorldIsDeadException("There was 8 outbreaks. There is a worldwide panic and it's your fault.");
            } else
            {
                State.OutbreakThisChain.Add(this);

                foreach (City City in ConnectedCities)
                {
                    City.InfectCity(Color, State);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if(obj is City)
            {
                City otherCity = obj as City;
                return (otherCity.Name == this.Name && otherCity.Color == this.Color);

            } else
            {
                return false;
            }
        }

        //kopiert fra https://github.com/loganfranken/overriding-equals-in-c-sharp/blob/master/OverridingEquals/PhoneNumber.cs
        public override int GetHashCode()
        {
            unchecked
            {
                // Choose large primes to avoid hashing collisions
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Name) ? Name.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!Object.ReferenceEquals(null, Color) ? Color.GetHashCode() : 0);

                return hash;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
