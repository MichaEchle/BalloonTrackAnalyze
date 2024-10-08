﻿using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Competition.Validation
{
    public static class ValidationHelper
    {

        private static readonly ILogger Logger = LogConnector.LoggerFactory.CreateLogger(nameof(ValidationHelper));


        /// <summary>
        /// Applies all specified declaration rules for goals of the specified goal number and return the latest valid declaration
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="goalNumber">the target goal number</param>
        /// <param name="declarationValidationRules">the list of rules to be applied</param>
        /// <returns>the latest valid declaration if any exists, otherwise null</returns>
        public static Declaration GetValidDeclaration(Track track, int goalNumber, List<IDeclarationValidationRules> declarationValidationRules)
        {
            List<Declaration> declarations = track.Declarations.Where(x => x.GoalNumber == goalNumber).ToList();
            List<Declaration> validDeclarations = [];
            if (declarations.Count == 0)
            {
                Logger?.LogWarning("No declaration found for goal number '{goalNumber}'", goalNumber);
                return null;
            }
            else
            {
                foreach (Declaration declaration in declarations)
                {
                    bool isValid = true;
                    if (declarationValidationRules != null)
                    {
                        foreach (IDeclarationValidationRules declarationValidationRule in declarationValidationRules)
                        {
                            if (!declarationValidationRule.IsComplaintToRule(declaration))
                            {
                                isValid = false;
                                break;
                            }
                        }
                    }
                    if (isValid)
                        validDeclarations.Add(declaration);
                }
                if (validDeclarations.Count == 0)
                {
                    Logger?.LogWarning("No declaration of goal number '{goalNumber}' is conform to specified rules", goalNumber);
                    return null;
                }
                else if (validDeclarations.Count == 1)
                    return validDeclarations[0];
                else
                    return validDeclarations.OrderByDescending(x => x.PositionAtDeclaration.TimeStamp).ToList()[0];
            }
        }

        ///// <summary>
        ///// Applies the specified rules to a marker drop and returns if its conform to the rules
        ///// </summary>
        ///// <param name="markerDrop">the marker to be checked</param>
        ///// <param name="markerValidationRules">the rules to be applied</param>
        ///// <returns>true: marker is valid; false: marker is invalid</returns>
        //public static bool IsMarkerValid(MarkerDrop markerDrop, List<IMarkerValidationRules> markerValidationRules)
        //{
        //    bool isValid = true;
        //    foreach (IMarkerValidationRules markerValidationRule in markerValidationRules)
        //    {
        //        isValid &= markerValidationRule.CheckConformance(markerDrop);
        //    }
        //    return isValid;
        //}

        /// <summary>
        /// Applies the specified rules to a marker drop and returns if its conform to the rules
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="markerNumber">the target marker number</param>
        /// <param name="markerValidationRules">the rules to be applied</param>
        /// <returns>true: marker is valid; false: marker is invalid or doesn't exists</returns>
        public static bool IsMarkerValid(Track track, int markerNumber, List<IMarkerValidationRules> markerValidationRules)
        {
            bool isValid = true;
            MarkerDrop markerDrop = track.MarkerDrops.FirstOrDefault(x => x.MarkerNumber == markerNumber);
            if (markerDrop == null)
            {
                //Console.WriteLine($"No Marker '{FirstMarkerNumber}' found");
                Logger?.LogWarning("No Marker '{markerNumber}' found", markerNumber);
                isValid = false;
            }
            else
            {
                if (markerValidationRules?.Count > 0)
                {
                    foreach (IMarkerValidationRules markerValidationRule in markerValidationRules)
                    {
                        isValid &= markerValidationRule.IsComplaintToRule(markerDrop);
                    }
                }
                if (!isValid)
                {
                    Logger?.LogWarning("Marker '{markerNumber}' is not conform to specified rules", markerNumber);
                }
            }
            return isValid;
        }
    }
}
