using Coordinates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JansScoring.flights;

public interface ISingleDeclarationTask
{

    protected int DeclarationNumber();

    public void HandleDecleration(Task task, Track track, out Declaration declaration, out String noResultMessage)
    {
        List<int> allMarkerNumbers = track.GetAllGoalNumbers();
        if (allMarkerNumbers == null || !allMarkerNumbers.Any())
        {
            noResultMessage = "No declarations found. | ";
            declaration = null;
            return;
        }

        declaration = track.Declarations.FindLast(drop => drop.GoalNumber == DeclarationNumber());

        if (declaration == null)
        {
            noResultMessage = $"No Declarations at slot {DeclarationNumber()}. | ";
            declaration = null;
            return;
        }

        if (declaration.DeclaredGoal == null || declaration.PositionAtDeclaration == null)
        {
            noResultMessage = $"No valid Declaration in {DeclarationNumber()} | ";
            return;
        }

        noResultMessage = null;
    }
}