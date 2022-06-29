using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Competition.Penalties
{
   public static  class PenaltyCalculation
    {

        public static void CheckForBluePZAndCalculatePenaltyPoints()
        {
            //TODO check if flight above specified altitude
            //penalties are calculated for each point in PZ: infringement in ft * trackpoint interval /100
            //create sum over all trackpoint penalties and round to next tens digit
        }

        public static void CheckFor2DDistanceInfringementAndCalculatePenaltyPoints()
        { 
            //TODO check if minimum or maximum limits have been violated using 2D distance
            //if so calculate infringement in %
            //minimum: (1 - distance/limit) * 100
            //maximum: (distance/limit - 1) * 100
            //penalty: round infringement[%] to one digit * 20 (use integer)
        }

        public static void CheckFor3DDistanceInfringementAndCalculatePenaltyPoints()
        {
            //TODO check if minimum or maximum limits have been violated using 3D distance
            //if so calculate infringement in %
            //minimum: (1 - distance/limit) * 100
            //maximum: (distance/limit - 1) * 100
            //penalty: round infringement[%] to one digit * 20
        }

        public static void CheckForVerticalDistanceInfringementAndCalculatePenaltyPoints()
        {
            //TODO check if minimum or maximum limits have been violated using altitude only
            //if so calculate infringement in %
            //minimum: (1 - distance/limit) * 100
            //maximum: (distance/limit - 1) * 100
            //penalty: round infringement[%] to one digit * 20
        }

        public static void CheckForDangerousFlyingAndCalculatePenaltyPoints()
        {
            //TODO check if vertical velocity is +/- 8m/s  for 5 consecutive seconds
            //penalty: max abs vertical velocity - max abs allowed vertical velocity *250 (use integer)
        }

        public static void CheckForCloseProximityAndCalculatePenaltyPoints()
        {
            //TODO use only trackpoints x seconds after launch and y seconds before landing
            //for each trackpoint: find trackpoint in other track where timestamp is nearest
            //check 3D distance between the two trackpoints
            //Limit 1: more than 3m / s at less than 25m
            //Limit 2: more than 5m / s at less than 50m
            //Limit 3: more than 8m / s at less than 75m
            //Limit 4: more than 8m / s vertical ascend speed
 
        }
    }
}
