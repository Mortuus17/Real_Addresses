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
    public static class Logic_Tools
    {
        public static ushort AddAnotherNumber
        (ushort newHouseNumber_1,
        string roadName)
        {
            ushort alpha = (ushort)(2 * (Hook_Addresses_SaveGame.CitySave.Roads.IndexOf(roadName)));

            // For even numbers
            if ((float)newHouseNumber_1 % (float)2 == 0)
            {
                // RNG
                if (Hook_Addresses_SaveGame.CitySave.Options.RandomNumbers == true)
                {
                    // Initialize random number and assign a value
                    System.Random randomNumber_2 = new System.Random();
                    ushort newHouseNumber_2 = (ushort)randomNumber_2.Next(Hook_Addresses_SaveGame.CitySave.Options.Offset + 1, Hook_Addresses_SaveGame.CitySave.Options.RandomNumbersRange);

                    // Check, if the random number is already in use. If so, assign a new random value
                    while ((((Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Contains(newHouseNumber_2))
                              &&
                              (float)newHouseNumber_2 % (float)2 != 0)
                                                                     ||
                                                                     ((Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Contains(newHouseNumber_2))
                                                                     &&
                                                                     (float)newHouseNumber_2 % (float)2 != 0)))
                    {
                        newHouseNumber_2 = (ushort)randomNumber_2.Next(Hook_Addresses_SaveGame.CitySave.Options.Offset + 1, Hook_Addresses_SaveGame.CitySave.Options.RandomNumbersRange);
                    }
                    return newHouseNumber_2;
                }
                else
                {
                    List<ushort> searchFor_largestNumber = new List<ushort>();

                    // Before each Method call, there's a number being added to the available List
                    searchFor_largestNumber.Add(Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Max());
                    if (Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Count() == 0)
                    {
                        ;
                    }
                    else
                    {
                        searchFor_largestNumber.Add(Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Max());
                    }

                    ushort largestNumber = searchFor_largestNumber.Max();

                    // Loop through all even numbers between 2 and largestNumber PLUS TWO // (largestNumber + 3) / 2 in the loop because of rounding down by default
                    for (ushort i = (ushort)(Hook_Addresses_SaveGame.CitySave.Options.Offset + 1); i <= ((largestNumber + 3) / 2); i++)
                    {
                        // If any of these numbers are in the available List, stop attempting to add a second number
                        if (Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Contains((ushort)((2 * i) - 1)))
                        {
                            continue;
                        }
                        // if any of these numbers are in the used List, try with the next largest even number
                        else if (Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Contains((ushort)((2 * i) - 1)))
                        {
                            break;
                        }
                        // Add the first number that's not found in either List to the available List. Last iteration of the loop is a completely new number
                        else
                        {
                            return (ushort)((2 * i) - 1);
                        }
                    }
                }
            }
            // For odd numbers
            else
            {
                // RNG
                if (Hook_Addresses_SaveGame.CitySave.Options.RandomNumbers == true)
                {
                    // Initialize random number and assign a value
                    System.Random randomNumber_2 = new System.Random();
                    ushort newHouseNumber_2 = (ushort)randomNumber_2.Next(Hook_Addresses_SaveGame.CitySave.Options.Offset + 1, Hook_Addresses_SaveGame.CitySave.Options.RandomNumbersRange);

                    // Check, if the random number is already in use. If so, assign a new random value
                    while ((((Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Contains(newHouseNumber_2))
                             &&
                             (float)newHouseNumber_2 % (float)2 == 0)
                                                                    ||
                                                                    ((Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Contains(newHouseNumber_2))
                                                                    &&
                                                                    (float)newHouseNumber_2 % (float)2 == 0)))
                    {
                        newHouseNumber_2 = (ushort)randomNumber_2.Next(Hook_Addresses_SaveGame.CitySave.Options.Offset + 1, Hook_Addresses_SaveGame.CitySave.Options.RandomNumbersRange);
                    }
                    return newHouseNumber_2;
                }
                else
                {
                    List<ushort> searchFor_largestNumber = new List<ushort>();

                    // Before each Method call, there's a number being added to the available List
                    searchFor_largestNumber.Add(Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Max());
                    if (Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Count() == 0)
                    {
                        ;
                    }
                    else
                    {
                        searchFor_largestNumber.Add(Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Max());
                    }

                    ushort largestNumber = searchFor_largestNumber.Max();

                    // Loop through all even numbers between 2 and largestNumber PLUS TWO // (largestNumber + 3) / 2 in the loop because of rounding down by default
                    for (ushort i = (ushort)(Hook_Addresses_SaveGame.CitySave.Options.Offset + 1); i <= ((largestNumber + 3) / 2); i++)
                    {
                        // If any of these numbers are in the available List, stop attempting to add a second number
                        if (Hook_Addresses_SaveGame.CitySave.ModArray[1 + alpha].Contains((ushort)(2 * i)))
                        {
                            continue;
                        }
                        // if any of these numbers are in the used List, try with the next largest even number
                        else if (Hook_Addresses_SaveGame.CitySave.ModArray[alpha].Contains((ushort)(2 * i)))
                        {
                            break;
                        }
                        // Add the first number that's not found in either List to the available List. Last iteration of the loop is a completely new number
                        else
                        {
                            return (ushort)(2 * i);
                        }
                    }
                }
            }
            ushort never = 0;
            return never;
        }
        public static bool DetermineSideOfRoad      // TODO
                                                    // PROBLEM: Additional Segments with curves are added later on PROBLEM: Change direction in the middle of Segment 
                                                    // SOLUTION: "If its ON the segment of a curve do this: [...]"
        (Vector3 buildingPosition)
        {
            if (buildingPosition.magnitude < NetManager.instance.m_segments.m_buffer[GetNearestSegment_toBuilding(buildingPosition)].m_middlePosition.magnitude)
            {
                return true;
            }
            else return false;
            //ushort nearestSegment_ID = GetNearestSegment_toBuilding(buildingPosition);
            //
            //Vector3 nearestSegment_Position = NetManager.instance.m_segments.m_buffer[nearestSegment_ID].m_middlePosition;            // problem: most curved roads
            //
            //List<ushort> roadSegments = Logic_Roads.GetCompleteRoad(nearestSegment_ID);
            //ushort roadSegments_Count = (ushort)(roadSegments.Count());
            //
            //List<ushort> numberOfCurves = new List<ushort>();
            //
            //// There is a curve in the road for each segment (except the first and last segment of a road; PROBLEM: CIRCLE), if for that segment, both neigbours x or y values are both either larger or smaller than that segments x or y values respectively
            //for (ushort i = 1; i < (roadSegments_Count - 1); i++)
            //{
            //    if (((NetManager.instance.m_segments.m_buffer[roadSegments[i - 1]].m_middlePosition.x < NetManager.instance.m_segments.m_buffer[roadSegments[i]].m_middlePosition.x)
            //        &&
            //        (NetManager.instance.m_segments.m_buffer[roadSegments[i + 1]].m_middlePosition.x < NetManager.instance.m_segments.m_buffer[roadSegments[i]].m_middlePosition.x))
            //                                                ||
            //                                                ((NetManager.instance.m_segments.m_buffer[roadSegments[i - 1]].m_middlePosition.x > NetManager.instance.m_segments.m_buffer[roadSegments[i]].m_middlePosition.x)
            //                                                &&
            //                                                (NetManager.instance.m_segments.m_buffer[roadSegments[i + 1]].m_middlePosition.x > NetManager.instance.m_segments.m_buffer[roadSegments[i]].m_middlePosition.x))
            //                                                                                        ||
            //                                                                                        ((NetManager.instance.m_segments.m_buffer[roadSegments[i - 1]].m_middlePosition.y < NetManager.instance.m_segments.m_buffer[roadSegments[i]].m_middlePosition.y)
            //                                                                                        &&
            //                                                                                        (NetManager.instance.m_segments.m_buffer[roadSegments[i + 1]].m_middlePosition.y < NetManager.instance.m_segments.m_buffer[roadSegments[i]].m_middlePosition.y))
            //                                                                                                                                ||
            //                                                                                                                                ((NetManager.instance.m_segments.m_buffer[roadSegments[i - 1]].m_middlePosition.y > NetManager.instance.m_segments.m_buffer[roadSegments[i]].m_middlePosition.y)
            //                                                                                                                                &&
            //                                                                                                                                (NetManager.instance.m_segments.m_buffer[roadSegments[i + 1]].m_middlePosition.y > NetManager.instance.m_segments.m_buffer[roadSegments[i]].m_middlePosition.y)))
            //    {
            //        numberOfCurves.Add(roadSegments[i]);
            //    }
            //}
            //// See, how often "isBuiltOnLeft" flips
            //bool numberOfFlips = false;
            //for (ushort i = 0; i < (numberOfCurves.Count() + 1); i++)
            //{
            //    ushort alpha = (ushort)(roadSegments.IndexOf(numberOfCurves[0]));
            //
            //    List<ushort> iteration = new List<ushort>();
            //    ushort iteration_count = (ushort)(iteration.Count());
            //    for (ushort j = 0; j <= iteration_count; j++)
            //    {
            //        iteration.RemoveAt(0);
            //    }
            //    for (ushort j = 0; j <= alpha; j++)
            //    {
            //        iteration.Add(roadSegments[j]);
            //    }
            //    if (iteration.Contains(nearestSegment_ID))
            //    {
            //        if (buildingPosition.magnitude < nearestSegment_Position.magnitude)
            //        {
            //            return numberOfFlips;
            //        }
            //        else return numberOfFlips;
            //    }
            //    else
            //    {
            //        numberOfFlips = !numberOfFlips;
            //        continue;
            //    }
            //}
            //bool never = false;
            //return never;
        }
        public static ushort GetNearestSegment_toBuilding       // problem: corners; building exactly between 2 roads with different names
        (Vector3 buildingPosition)
        {
            // Store the distances between all segments and the building in a List
            List<float> deltaVectorMagnitudes = new List<float>();
            for (ushort i = 0; i < NetManager.MAX_SEGMENT_COUNT; i++)
            {
                deltaVectorMagnitudes.Add((NetManager.instance.m_segments.m_buffer[i].m_middlePosition - buildingPosition).magnitude);
            }

            // New List with sorted distances
            List<float> deltaVectorMagnitudes_Sorted = new List<float>();
            for (ushort i = 0; i < NetManager.MAX_SEGMENT_COUNT; i++)
            {
                deltaVectorMagnitudes_Sorted.Add(deltaVectorMagnitudes[i]);
            }
            deltaVectorMagnitudes_Sorted.Sort();

            // Iterate through the sorted List, assign the smallest numbers' index in the unsorted list to nearerstSegmentID
            ushort arrayIndex = 0;
            ushort nearestSegment_ID = (ushort)(deltaVectorMagnitudes.IndexOf(deltaVectorMagnitudes_Sorted[arrayIndex]));

            // Check, if the currently smallest delta vector magnitude doesn't point to a road                                    compatability with Network Extensions 2 
            while (NetManager.instance.m_segments.m_buffer[nearestSegment_ID].Info.category != "RoadsTiny"
                   &&
                   NetManager.instance.m_segments.m_buffer[nearestSegment_ID].Info.category != "RoadsSmall"
                   &&
                   NetManager.instance.m_segments.m_buffer[nearestSegment_ID].Info.category != "RoadsSmallHV"
                   &&
                   NetManager.instance.m_segments.m_buffer[nearestSegment_ID].Info.category != "RoadsMedium"
                   &&
                   NetManager.instance.m_segments.m_buffer[nearestSegment_ID].Info.category != "RoadsLarge")
            {
                // If it does not point to a road, try with the next largest delta vector magnitude
                arrayIndex++;
                nearestSegment_ID = (ushort)(deltaVectorMagnitudes.IndexOf(deltaVectorMagnitudes_Sorted[arrayIndex]));
            }
            return nearestSegment_ID;
        }
        public static ushort ConvertToNumber
        (string houseName)
        {
            List<ushort> max_Number = new List<ushort>();
            ushort roadPosition = (ushort)(Hook_Addresses_SaveGame.CitySave.Roads.IndexOf(GetRoadName(houseName)));
            if (Hook_Addresses_SaveGame.CitySave.ModArray[2 * roadPosition].Count() == 0)
            {
                ;
            }
            else
            {
                max_Number.Add(Hook_Addresses_SaveGame.CitySave.ModArray[2 * roadPosition].Max());
            }
            if (Hook_Addresses_SaveGame.CitySave.ModArray[1 + (2 * roadPosition)].Count() == 0)
            {
                ;
            }
            else
            {
                max_Number.Add(Hook_Addresses_SaveGame.CitySave.ModArray[1 + (2 * roadPosition)].Max());
            }
            ushort alpha = max_Number.Max();
            for (ushort i = 0;      i < alpha;        i++)
            {
                if (houseName.Contains((alpha - i).ToString()))
                {
                    return (ushort)(alpha - i);
                }
                else continue;
            }
            ushort never = 0;
            return never;
        }
        public static string GetRoadName
        (string houseName)
        {
            ushort alpha = (ushort)(Hook_Addresses_SaveGame.CitySave.Roads.Count());
            for (ushort i = 0;      i < alpha;      i++)
            {
                if (houseName.Contains(Hook_Addresses_SaveGame.CitySave.Roads[i]))
                {
                    return (Hook_Addresses_SaveGame.CitySave.Roads[i]);
                }
                else continue;
            }
            return "never";
        }
    }
}