using Coordinates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Competition
{
    public class DonutTask : CompetitionTask
    {
        public const double NOT_APPLICABLE = double.NaN;//-9999 will be entered in the GUI

        public int GoalNumber
        {
            get; set;
        }

        public int NumberOfDeclartions
        {
            get; set;
        }

        public double InnerRadius
        {
            get; set;
        }

        public double OuterRadius
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

        public bool IsReentranceAllowed
        {
            get; set;
        }

        public List<IDeclarationValidationRules> DeclarationValidationRules
        {
            get;set;
        }


        public override bool CalculateResults(Track track, bool useGPSAltitude, out double result)
        {
            result = 0.0;
            List<(int, Coordinate)> trackPointsInDonut = new List<(int trackPointNumber, Coordinate coordinate)>();
            
            DeclaredGoal targetGoal = GetValidGoal(track,GoalNumber,DeclarationValidationRules);
            if(targetGoal==null)
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
                if (distanceToGoal <= OuterRadius && distanceToGoal >= InnerRadius)//save all trackpoints between outer and inner radius
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
            return true;
        }
    }
}
