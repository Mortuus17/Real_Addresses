using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICities;
using Unity;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.IO;
using UnityEngine;

namespace Addresses
{
    public static class Logic_Buildings
    {
        /// <summary>
        /// UPON HOUSE BUILT
        /// </summary>
        public static string NameHouse
        (ushort building_ID)
        {
            string roadName = NetManager.instance.GetSegmentName(Logic_Tools.GetNearestSegment_toBuilding(BuildingManager.instance.m_buildings.m_buffer[building_ID].m_position));
            Logic_Roads.AddRoad(roadName);
            ushort alpha = (ushort)(2 * (Hook_Addresses_SaveGame.CitySave.Roads.IndexOf(roadName)));

            if (Hook_Addresses_SaveGame.CitySave.Options.ConsiderRoadSide == true)
            {
                string buildingName = RenameEvenOrOdd(Logic_Tools.DetermineSideOfRoad(BuildingManager.instance.m_buildings.m_buffer[building_ID].m_position), roadName);
                Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Add(Logic_Tools.AddAnotherNumber(Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Min(), roadName));
                return buildingName;
            }
            else
            {
                string buildingName = NamingPatternCheck(roadName, Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Min());
                Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Add(Logic_Tools.AddAnotherNumber(Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Min(), roadName));
                return buildingName;
            }
        }
        // Used only after having considered road side options check to be set to true
        private static string RenameEvenOrOdd
        (bool even,
        string roadName)
        {
            ushort alpha = (ushort)(2 * (Hook_Addresses_SaveGame.CitySave.Roads.IndexOf(roadName)));
            ushort beta = (ushort)(Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Count() + Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Count());

            if (even == false)
            {
                for (ushort movingNumber = 1;       movingNumber <= beta;        movingNumber++)
                {
                    // Check, if loop index is in available List
                    if (Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Contains(movingNumber))
                    {
                        // Rename if smallest number found is odd
                        if ((float)(movingNumber) % (float)(2) != 0)
                        {
                            string buildingName = NamingPatternCheck(roadName, movingNumber);
                            return buildingName;
                        }
                    }
                    // if both conditions fail, try the next number
                    else continue;
                }
            }
            else
            {
                for (ushort movingNumber = 1;       movingNumber <= beta;        movingNumber++)
                {
                    if (Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Contains(movingNumber))
                    {
                        if ((float)(movingNumber) % (float)(2) == 0)
                        {
                            string buildingName = NamingPatternCheck(roadName, movingNumber);
                            return buildingName;
                        }
                    }
                    else continue;
                }
            }         
            return "never";
        }
        // Check naming pattern options, name building appropriately, move number from available List to used List
        private static string NamingPatternCheck
        (string roadName,
         ushort movingNumber)
        {
            ushort alpha = (ushort)(2 * (Hook_Addresses_SaveGame.CitySave.Roads.IndexOf(roadName)));

            if (Hook_Addresses_SaveGame.CitySave.Options.NumbersAsPrefix == false)
            {
                string buildingName = roadName + " " + Convert.ToString(movingNumber);
                Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Add(movingNumber);
                Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Remove(movingNumber);
                return buildingName;
            }
            else
            {
                string buildingName = Convert.ToString(movingNumber) + " " + roadName;
                Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Add(movingNumber);
                Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Remove(movingNumber);
                return buildingName;
            }
        }
        /// <summary>
        /// UPON HOUSE DESTROYED
        /// </summary>
        public static void RemoveHouse
        ()
        {
            // Get link of buildings in game to modArray - List of List of strings with key : 1 to 1 to Roads array
            ushort alpha = (ushort)(Hook_Addresses_SaveGame.CitySave.Roads.Count());
            List<string> roadinstance = new List<string>();
            List<List<string>> buildingNames = new List<List<string>>();
            for (ushort i = 0;      i < alpha;        i++)
            {
                buildingNames.Add(roadinstance);
            }
            for (ushort i = 0;      i < BuildingManager.MAX_BUILDING_COUNT;      i++)
            {
                string beta = BuildingManager.instance.GetBuildingName(i, WorldInfoPanel.GetCurrentInstanceID());
                if (beta != null)
                {
                    for (ushort j = 0; j < alpha; j++)
                    {
                        if ((beta.Contains(Hook_Addresses_SaveGame.CitySave.Roads[j])))
                        {
                            buildingNames[j].Add(beta);
                        }
                    }
                }
                else continue;
            }
            // For each List in the new List...
            for (ushort i = 0;      i < alpha;       i++)
            {
                // ...check if number of simulation instance buildings using numbers is equal to number of "used" numbers in mod Array
                // in order to find the ONE List where that's not true
                if (buildingNames[i].Count() == (Hook_Addresses_SaveGame.CitySave.ModArray[1 + (2 * i)].Count() + Hook_Addresses_SaveGame.CitySave.ModArray[2 * i].Count()))
                {
                    continue;
                }
                else
                {
                    // Check which number is used by a building instance
                    List<ushort> numbers_Used = new List<ushort>();
                    ushort delta = (ushort)(buildingNames[i].Count());
                    for (ushort j = 0;      j < delta;        j++)
                    {
                        ushort usedNumber = Logic_Tools.ConvertToNumber(buildingNames[i][j]);
                        numbers_Used.Add(usedNumber);
                    }

                    // For each number in the modArray...
                    ushort epsilon = (ushort)(Hook_Addresses_SaveGame.CitySave.ModArray[1 + (2 * i)].Count());
                    for (ushort j = 0;      j < epsilon;        j++)
                    {
                        // ...skip numbers that are found in building instances...
                        if (numbers_Used.Contains(Hook_Addresses_SaveGame.CitySave.ModArray[1 + (2 * i)][j]))
                        {
                            continue;
                        }
                        // ...until the one number is found
                        else
                        {
                            // delete number from mod array
                            Hook_Addresses_SaveGame.CitySave.ModArray[1 + (2 * i)].Remove(Hook_Addresses_SaveGame.CitySave.ModArray[1 + (2 * i)][j]);

                            // and if this was the last building on a road, delete that road from all mod arrays.
                            // When the road WASN'T deleted, it get's added back in when the first new house is built on it 
                            // => Logic_Roads.AddRoad() called in Logic_Buildings.NameHouse called after the EvenOnHouseBuilt
                            if (Hook_Addresses_SaveGame.CitySave.ModArray[1 + (2 * i)].Count() == 0)
                            {
                                Logic_Roads.RemoveRoad(Hook_Addresses_SaveGame.CitySave.Roads[i]);
                            }
                            return;
                        }
                    }
                }
            }
        }
    }
}