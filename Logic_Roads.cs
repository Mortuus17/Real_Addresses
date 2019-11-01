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
    public static class Logic_Roads
    {
        // Upon road with new name built
        public static void AddRoad
        (string roadName)
        {
            if (Hook_Addresses_SaveGame.CitySave.Roads.Contains(roadName))
            {
                return;
            }
            else
            {
                // add road to roads List
                Hook_Addresses_SaveGame.CitySave.Roads.Add(roadName);

                // add 2 Lists (available numbers/ used numbers), add first number (usually "1") to the first (= available) List after RNG check
                List<ushort> available = new List<ushort>();
                List<ushort> used = new List<ushort>();
                ushort initialHouseNumber_1 = 1;
                ushort initialHouseNumber_2 = 2;

                // RNG options check
                if (Hook_Addresses_SaveGame.CitySave.Options.RandomNumbers == true)
                {
                    System.Random rnd = new System.Random();
                    initialHouseNumber_1 = (ushort)rnd.Next((Hook_Addresses_SaveGame.CitySave.Options.Offset + 1), Hook_Addresses_SaveGame.CitySave.Options.RandomNumbersRange);
                    initialHouseNumber_2 = Logic_Tools.AddAnotherNumber(initialHouseNumber_1, roadName);
                }
                else
                {
                    initialHouseNumber_1 = (ushort)(Hook_Addresses_SaveGame.CitySave.Options.Offset + 1);
                    initialHouseNumber_2 = (ushort)(Hook_Addresses_SaveGame.CitySave.Options.Offset + 2);
                }
                available.Add(initialHouseNumber_1);
                available.Add(initialHouseNumber_2);
                Hook_Addresses_SaveGame.CitySave.ModArray.Add(available);
                Hook_Addresses_SaveGame.CitySave.ModArray.Add(used);
            }
        }
        // Upon destruction of the last road of a particular name
        public static void RemoveRoad
        (string roadName)
        {
            // search for position of the road in all Lists
            ushort roadPosition = (ushort)Hook_Addresses_SaveGame.CitySave.Roads.IndexOf(roadName);

            // remove all road data from all Lists
            Hook_Addresses_SaveGame.CitySave.Roads.Remove(roadName);
            Hook_Addresses_SaveGame.CitySave.ModArray.RemoveAt(2 * roadPosition);
            Hook_Addresses_SaveGame.CitySave.ModArray.RemoveAt(2 * roadPosition);
        }
        public static List<ushort> GetCompleteRoad
        (ushort nearestSegment_ID)
        {
            // List of all road segments ordered from roadSegments[0] ^= most left segment to roadSegments[roadSegments.Count() - 1] ^= most right segment;
            List<ushort> roadSegments = new List<ushort>();
            roadSegments.Add(nearestSegment_ID);

            // Search for all segments to the LEFT of the nearest segment and add them to the List
            ushort identifier_1 = nearestSegment_ID;
            while (NetManager.instance.m_segments.m_buffer[identifier_1].m_endLeftSegment != 0)        // often infinite
            {
                // Looking through all possible segments in road crossings
                ushort identifier_2 = identifier_1;
                while (nearestSegment_ID != NetManager.instance.m_segments.m_buffer[identifier_2].m_endLeftSegment)
                {
                    if (NetManager.instance.m_segments.m_buffer[nearestSegment_ID].m_nameSeed
                        == NetManager.instance.m_segments.m_buffer[NetManager.instance.m_segments.m_buffer[identifier_2].m_endLeftSegment].m_nameSeed)
                    {
                        // Insert at [0]    -   Left
                        roadSegments.Insert(0, NetManager.instance.m_segments.m_buffer[identifier_2].m_endLeftSegment);
                        identifier_2 = NetManager.instance.m_segments.m_buffer[identifier_2].m_endLeftSegment;
                        break;
                    }
                    else continue;
                }
                // Fix for infinite    if the value didn't change in the inner while loop or if the new value is already in the list a.k.a. it's a circular road, STOP
                if ((identifier_2 == identifier_1)
                    ||
                    roadSegments.Contains(identifier_2))
                {
                    break;
                }
                else
                {
                    identifier_1 = identifier_2;
                }
            }
            // Search for all segments to the RIGHT of the nearest segment and add them to the List
            ushort identifier_3 = nearestSegment_ID;
            while (NetManager.instance.m_segments.m_buffer[identifier_1].m_startLeftSegment != 0)
            {
                // Looking through all possible segments in road crossings
                ushort identifier_4 = identifier_3;
                while (nearestSegment_ID != NetManager.instance.m_segments.m_buffer[identifier_4].m_startLeftSegment)
                {
                    if (NetManager.instance.m_segments.m_buffer[nearestSegment_ID].m_nameSeed
                        == NetManager.instance.m_segments.m_buffer[NetManager.instance.m_segments.m_buffer[identifier_4].m_startLeftSegment].m_nameSeed)
                    {
                        // Add()    -   Right
                        roadSegments.Add(NetManager.instance.m_segments.m_buffer[identifier_4].m_startLeftSegment);
                        identifier_4 = NetManager.instance.m_segments.m_buffer[identifier_4].m_startLeftSegment;
                        break;
                    }
                    else continue;
                }
                // Fix for infinite    if the value didn't change in the inner while loop or if the new value is already in the list a.k.a. it's a circular road, STOP
                if ((identifier_4 == identifier_3)
                    ||
                    roadSegments.Contains(identifier_4))
                {
                    break;
                }
                else
                {
                    identifier_3 = identifier_4;
                }
            }
            return roadSegments;
        }
    }
}