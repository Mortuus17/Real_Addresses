using System.IO;
using System.Collections.Generic;
using ICities;
using ColossalFramework.IO;
using UnityEngine;

namespace Addresses
{
    public class Hook_Addresses_SaveGame : SerializableDataExtensionBase, IDataContainer
    {
        // Singleton instance
        private static Hook_Addresses_SaveGame citySave;

        // Fields
        private Hook_Addresses_Options cityOptions = Hook_Addresses_Options.Global;
        private List<string> roads = new List<string>();
        private List<List<ushort>> modArray = new List<List<ushort>>();
        private const string dataID = "Addresses_SaveGame";
        private const int dataVersion = 1;

        // Properties
        public static Hook_Addresses_SaveGame CitySave
        {
            get { return citySave; } 
            set {citySave = value; }
        }
        public Hook_Addresses_Options Options
        {
            get { return cityOptions; }
            set { cityOptions = value; }
        }
        public List<string> Roads
        {
            get { return roads; }
            set { roads = value; }
        }
        public List<List<ushort>> ModArray
        {
            get { return modArray; }
            set { modArray = value; }
        }
        /// <summary>
        /// Savegame hook
        /// </summary>

        // Singleton setter
        public override void OnCreated
        (ISerializableData serializedData)
        {
            base.OnCreated(serializedData);
            CitySave = this;
        }
        public override void OnReleased
        ()
        {
            CitySave = null;
        }
        public void Serialize
        (DataSerializer s )
        {
            s.WriteObject<Hook_Addresses_SaveGame>(CitySave);
        }
        public void Deserialize
        (DataSerializer s)
        {
            CitySave = s.ReadObject<Hook_Addresses_SaveGame>();
        }
        public void AfterDeserialize
        (DataSerializer s)
        {
        }
        public override void OnLoadData
        ()
        {
            byte[] bytes = serializableDataManager.LoadData(Hook_Addresses_SaveGame.dataID);
            if (bytes != null)
            {
                using (var stream = new MemoryStream(bytes))
                {
                    CitySave = DataSerializer.Deserialize<Hook_Addresses_SaveGame>(stream, DataSerializer.Mode.Memory);
                }
                Debug.LogFormat("Data loaded (Size in bytes: {0})", bytes.Length);
            }
            else
            {
                CitySave = new Hook_Addresses_SaveGame();
                Debug.Log("Data created");
            }
        }
        public override void OnSaveData
        ()
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                DataSerializer.Serialize(stream, DataSerializer.Mode.Memory, dataVersion, CitySave);
                bytes = stream.ToArray();
            }
            serializableDataManager.SaveData(dataID, bytes);
            Debug.LogFormat("Data saved (Size in bytes: {0})", bytes.Length);
        }
    }
}