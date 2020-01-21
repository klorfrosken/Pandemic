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
        public Boolean HasResearchStation = false;
        public Boolean MultipleDiseases = false;
        public Dictionary<Colors, int> DiseaseCubes = new Dictionary<Colors, int>
        {
            {Colors.Yellow, 0 },
            {Colors.Red, 0 },
            {Colors.Blue, 0 },
            {Colors.Black, 0 }
        };
        public List<City> ConnectedCities = new List<City>();
        StateManager _state;
        ITextManager _textManager;

        public City(string Name, Colors Color, StateManager state = null, ITextManager textManager = null)
        {
            this.Name = Name;
            this.Color = Color;
            _state = state;
            _textManager = textManager;
        }

        public Boolean IsConnectedTo(City otherCity)
        {
            if(otherCity == null)
            {
                throw new UnexpectedBehaviourException("The city you are trying to match is null. What kind of city could that be?!");
            } else
            {
                return ConnectedCities.Exists(City => City == otherCity);
            }
        }

        public void BuildResearchStation()
        {
            if (HasResearchStation)
            {
                throw new IllegalMoveException("There is already a research station in this city. Please try another action. ");
            } else
            {
                HasResearchStation = true;
                _state.BuildResearchStation();
            }            
        }

        public void TreatDisease(Colors Color)
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
                _state.CubePools[Color]++;
            }
        }

        Boolean DiseaseIsEradicated(Colors Color)
        {
            bool CubePoolIsFull = (_state.CubePools[Color] == _state.MaxCubesInCubePool);
            bool CureIsFound = _state.Cures[Color];
            return (CubePoolIsFull && CureIsFound);
        }

        Boolean InfectionPreventedByQuarantineSpecialist()
        {
            if (!_state.QuarantineSpecialistInGame)
            {
                return false;
            } else
            {
                Role QuarantineSpecialist = _state.Roles.Find(Role => Role is QuarantineSpecialist);
                Boolean QSisHere = (QuarantineSpecialist.CurrentCity == this);
                Boolean QSisInConnectedCity = ConnectedCities.Exists(City => QuarantineSpecialist.CurrentCity == City);

                return (QSisHere || QSisInConnectedCity);
            }
        }

        Boolean InfectionPreventedByMedic(Colors Color)
        {
            if (!_state.MedicInGame)
            {
                return false;
            } else
            {
                Role Medic = _state.Roles.Find(Player => Player is Medic);
                Boolean CureFound = _state.Cures[Color];

                return (Medic.CurrentCity == this && CureFound);
            }
        }

        public void InfectCity(Colors Color)
        {
            if (!DiseaseIsEradicated(Color))
            {
                if (!InfectionPreventedByQuarantineSpecialist())
                {
                    if (!InfectionPreventedByMedic(Color))
                    {
                        if (DiseaseCubes[Color] == 3)
                        {
                            Boolean OutbreakThisChain = _state.OutbreakThisChain.Exists(City => City == this);
                            if (!OutbreakThisChain)
                            {
                                Outbreak(Color);
                            }
                        }
                        else
                        {
                            if (_state.CubePools[Color] == 0)
                            {
                                throw new TheWorldIsDeadException($"There are no more {Color} cubes left. The disease has spread too much.");
                            }
                            else
                            {
                                if (Color != this.Color)
                                {
                                    MultipleDiseases = true;
                                }

                                DiseaseCubes[Color]++;
                                _state.CubePools[Color]--;
                                _textManager.PrintInfection(this, Color);
                            }
                        }
                    }
                }
            }
        }

        void Outbreak(Colors Color)
        {
            _state.Outbreaks++;

            if (_state.Outbreaks == _state.MaxOutbreaks)
            {
                throw new TheWorldIsDeadException("There was 8 outbreaks. There is a worldwide panic and it's your fault.");
            } else
            {
                _state.OutbreakThisChain.Add(this);

                foreach (City City in ConnectedCities)
                {
                    City.InfectCity(Color);
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

        //copied from https://github.com/loganfranken/overriding-equals-in-c-sharp/blob/master/OverridingEquals/PhoneNumber.cs
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


        //Unit Test methods
        public Boolean TestDiseaseIsEradicated(Colors color)
        {
            return DiseaseIsEradicated(color);
        }
        public Boolean TestInfectionPreventedByQuarantineSpecialist()
        {
            return InfectionPreventedByQuarantineSpecialist();
        }
        public Boolean TestInfectionPreventedByMedic(Colors color)
        {
            return InfectionPreventedByMedic(color);
        }
        public void TestOutbreak(Colors color)
        {
            Outbreak(color);
        }
    }
}
