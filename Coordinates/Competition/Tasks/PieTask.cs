using Coordinates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Competition.Tasks
{
    public class PieTask : CompetitionTask
    {
        public class PieTier
        {
            public const double NOT_APPLICABLE = double.NaN;

            public int GoalNumber
            {
                get; set;
            }

            public bool IsReentranceAllowed
            {
                get; set;
            }

            public double Radius
            {
                get; set;
            }

            public double LowerBoundary
            {
                get; set;
            }

            public double UpperBoundary
            {
                get; set;
            }

            public double Multiplier
            {
                get; set;
            }

            public List<IDeclarationValidationRules> DeclarationValidationRules
            {
                get;set;
            }

            public PieTier()
            {

            }

            public bool CalculateTierResult(Track track, bool useGPSAltitude, out double result)
            {
                result = 0.0;
                List<(int, Coordinate)> trackPointsInDonut = new List<(int trackPointNumber, Coordinate coordinate)>();
                
                DeclaredGoal targetGoal = GetValidGoal(track,GoalNumber,DeclarationValidationRules);
                if (targetGoal == null)
                {
                    Debug.WriteLine("No valid goal found");
                    return false;
                }
                List<Coordinate> coordinates = track.TrackPoints;
                if (LowerBoundary != NOT_APPLICABLE)
                {
                    if (useGPSAltitude)
                        coordinates = coordinates.Where(x => x.AltitudeGPS >= LowerBoundary).ToList();//take all point above lower boundary
                    else
                        coordinates = coordinates.Where(x => x.AltitudeBarometric >= LowerBoundary).ToList();//take all point above lower boundary
                }
                if (UpperBoundary != NOT_APPLICABLE)
                {
                    if (useGPSAltitude)
                        coordinates = coordinates.Where(x => x.AltitudeGPS <= UpperBoundary).ToList();//take all points below upper boundary
                    else
                        coordinates = coordinates.Where(x => x.AltitudeBarometric <= UpperBoundary).ToList();//take all points below upper boundary
                }

                for (int index = 0; index < coordinates.Count; index++)
                {
                    double distanceToGoal = CoordinateHelpers.CalculateDistance2D(track.TrackPoints[index], targetGoal.GoalDeclared);//calculate distance to goal
                    if (distanceToGoal <= Radius)//save all trackpoints within the radius
                        trackPointsInDonut.Add((index, track.TrackPoints[index]));

                }
                List<List<Coordinate>> chunksInDonut = new List<List<Coordinate>>();
                int addIndex = 0;
                chunksInDonut.Add(new List<Coordinate>());
                for (int index = 0; index < trackPointsInDonut.Count - 1; index++)
                {
                    if (trackPointsInDonut[index + 1].Item1 - trackPointsInDonut[index].Item1 == 0)//trackpoints are successive
                    {
                        if (chunksInDonut[addIndex].Count == 0)
                        {
                            chunksInDonut[addIndex].Add(trackPointsInDonut[index].Item2);
                            chunksInDonut[addIndex].Add(trackPointsInDonut[index + 1].Item2);
                        }
                        else
                        {
                            chunksInDonut[addIndex].Add(trackPointsInDonut[index + 1].Item2);
                        }
                    }
                    else//trackpoints are not successive -> create new chunk
                    {
                        chunksInDonut.Add(new List<Coordinate>());
                        addIndex++;
                    }
                }

                if (!IsReentranceAllowed)//evaluate first chunk only
                {
                    if (chunksInDonut[0].Count >= 2)
                    {
                        for (int index = 0; index < chunksInDonut[0].Count - 1; index++)
                        {
                            double tempResult = CoordinateHelpers.CalculateDistance2D(chunksInDonut[0][index], chunksInDonut[0][index + 1]);
                            result += tempResult;
                        }
                    }
                }
                else//evaluate all chunks
                {
                    for (int chuckIndex = 0; chuckIndex < chunksInDonut.Count; chuckIndex++)
                    {
                        if (chunksInDonut[chuckIndex].Count >= 2)
                        {
                            for (int index = 0; index < chunksInDonut[chuckIndex].Count - 1; index++)
                            {
                                double tempResult = CoordinateHelpers.CalculateDistance2D(chunksInDonut[chuckIndex][index], chunksInDonut[chuckIndex][index + 1]);
                                result += tempResult;
                            }
                        }
                    }
                }
                result *= Multiplier;
                return true;
            }

            public DeclaredGoal GetValidGoal(Track track, int goalNumber, List<IDeclarationValidationRules> declarationValidationRules)
            {
                List<DeclaredGoal> declarations = track.DeclaredGoals.Where(x => x.GoalNumber == goalNumber).ToList();
                List<DeclaredGoal> validDeclarations = new List<DeclaredGoal>();
                foreach (DeclaredGoal declaredGoal in declarations)
                {
                    bool isValid = true;
                    foreach (IDeclarationValidationRules declarationValidationRule in declarationValidationRules)
                    {
                        if (!declarationValidationRule.CheckConformance(declaredGoal))
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (isValid)
                        validDeclarations.Add(declaredGoal);
                }
                if (validDeclarations.Count == 0)
                    return null;
                else if (validDeclarations.Count == 1)
                    return validDeclarations[0];
                else
                    return validDeclarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).ToList()[0];
            }
        }


        public List<PieTier> Tiers
        {
            get; set;
        }

        public PieTask()
        {

        }

        public override bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            result = 0.0;
            foreach (PieTier tier in Tiers)
            {
                double tempResult;
                if (!tier.CalculateTierResult(track, useGPSAltitude,out tempResult))
                    return false;
                result += tempResult;
            }
            return true;
        }
    }


}
