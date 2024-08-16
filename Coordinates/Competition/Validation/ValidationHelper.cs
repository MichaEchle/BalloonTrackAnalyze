using Coordinates;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Competition.Validation
{
    public enum ValidationStrictnessType
    {
        First,
        FirstValid,
        LatestValid
    }


    public static class ValidationHelper
    {

        private static readonly ILogger Logger = LogConnector.LoggerFactory.CreateLogger(nameof(ValidationHelper));


        /// <summary>
        /// Applies all specified declaration rules for goals of the specified goal number and return the latest valid declaration
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="goalNumber">the target goal number</param>
        /// <param name="declarationValidationRule">the rule to check if a declaration is valid. Use null to omit or use <see cref="DeclarationAndRule"/> or <see cref="DeclarationOrRule"/> to chain rules</param>
        /// <param name="validationStrictness">the strictness of the validation</param>
        /// <returns>a valid declaration if one exists, null otherwise</returns>
        public static Declaration GetValidDeclaration(Track track, int goalNumber, IDeclarationValidationRule declarationValidationRule, ValidationStrictnessType validationStrictness)
        {
            List<Declaration> declarations = track.Declarations.Where(x => x.GoalNumber == goalNumber).OrderBy(x => x.PositionAtDeclaration.TimeStamp).ToList();
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

                    if (declarationValidationRule?.IsComplaintToRule(declaration) ?? true)
                    {
                        validDeclarations.Add(declaration);
                    }
                }
            }
            if (validDeclarations.Count == 0)
            {
                Logger?.LogWarning("No declaration of goal number '{goalNumber}' is conform to specified rules", goalNumber);
                return null;
            }
            else
            {
                switch (validationStrictness)
                {
                    case ValidationStrictnessType.First:
                        {
                            if (validDeclarations.Contains(declarations[0]))
                            {
                                return validDeclarations[0];
                            }
                            else
                            {
                                Logger?.LogWarning("The first valid declaration for goal number '{goalNumber}' is not the first declaration", goalNumber);
                                return null;
                            }
                        }
                    case ValidationStrictnessType.FirstValid:
                        Logger?.LogInformation("Detected '{numberOfValidDeclarations}' valid declarations for goal number '{goalNumber}'. The first valid declaration will be used", validDeclarations.Count, goalNumber);
                        return validDeclarations[0];
                    case ValidationStrictnessType.LatestValid:
                        Logger?.LogInformation("Detected '{numberOfValidDeclarations}' valid declarations for goal number '{goalNumber}'. The latest valid declaration will be used", validDeclarations.Count, goalNumber);
                        return validDeclarations.MaxBy(x => x.PositionAtDeclaration.TimeStamp);
                    default:
                        {
                            Logger?.LogError("Unknown validation strictness type '{validationStrictness}'", validationStrictness);
                            return null;
                        }
                }
            }
        }


        /// <summary>
        /// Applies the specified rules to a marker drop and returns if its conform to the rules
        /// </summary>
        /// <param name="track">the track to be used</param>
        /// <param name="markerNumber">the target marker number</param>
        /// <param name="markerValidationRule">the rule to check if a declaration is valid. Use null to omit or use <see cref="MarkerAndRule"/> or <see cref="MarkerOrRule"/> to chain rules</param>
        /// <param name="validationStrictness">the strictness of the validation</param>
        /// <returns>a valid marker if one exists, null otherwise</returns>
        public static MarkerDrop GetValidMarker(Track track, int markerNumber, IMarkerValidationRule markerValidationRule, ValidationStrictnessType validationStrictness)
        {
            List<MarkerDrop> markers = track.MarkerDrops.Where(x => x.MarkerNumber == markerNumber).ToList();
            List<MarkerDrop> validMarkers = [];
            if (markers.Count == 0)
            {
                Logger?.LogWarning("No marker found for marker number '{markerNumber}'", markerNumber);
                return null;
            }
            else
            {
                foreach (MarkerDrop marker in markers)
                {
                    if (markerValidationRule?.IsComplaintToRule(marker) ?? true)
                    {
                        validMarkers.Add(marker);
                    }
                }
            }
            if (validMarkers.Count == 0)
            {
                Logger?.LogWarning("No marker of marker number '{markerNumber}' is conform to specified rules", markerNumber);
                return null;
            }
            else
            {
                switch (validationStrictness)
                {
                    case ValidationStrictnessType.First:
                        {
                            if (markers.Contains(validMarkers[0]))
                            {
                                return validMarkers[0];
                            }
                            else
                            {
                                Logger?.LogWarning("The first valid marker for marker number '{markerNumber}' is not the first marker", markerNumber);
                                return null;
                            }
                        }
                    case ValidationStrictnessType.FirstValid:
                        Logger?.LogInformation("Detected '{numberOfValidMarkers}' valid markers for marker number '{markerNumber}'. The first valid marker will be used", validMarkers.Count, markerNumber);
                        return validMarkers[0];
                    case ValidationStrictnessType.LatestValid:
                        Logger?.LogInformation("Detected '{numberOfValidMarkers}' valid markers for marker number '{markerNumber}'. The latest valid marker will be used", validMarkers.Count, markerNumber);
                        return validMarkers.MaxBy(x => x.MarkerLocation.TimeStamp);
                    default:
                        {
                            Logger?.LogError("Unknown validation strictness type '{validationStrictness}'", validationStrictness);
                            return null;
                        }
                }
            }
        }
    }
}
