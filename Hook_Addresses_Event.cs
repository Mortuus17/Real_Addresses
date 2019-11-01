using ColossalFramework.UI;
using ICities;
using System.Linq;

namespace Addresses
{
    public class Hook_Addresses_Event : ILoadingExtension
    {
        private bool CheckInitialConditions
        (ushort building_ID)
        {
            return ((BuildingManager.instance.m_buildings.m_buffer[building_ID].Info.m_placementMode == BuildingInfo.PlacementMode.Roadside)
                    &&
                    (BuildingManager.instance.m_buildings.m_buffer[building_ID].Info.editorCategory == "Default")
                    &&
                    (BuildingManager.instance.m_buildings.m_buffer[building_ID].Info.category == "Default"));
        }
        public virtual void OnBuildingCreated(ushort building_ID)
        {
            SimulationManager.instance.AddAction<bool>(BuildingManager.instance.SetBuildingName(building_ID, Logic_Buildings.NameHouse(building_ID)));
        }
        public virtual void OnBuildingDestroyed(ushort building_ID)
        {
            if (CheckInitialConditions(building_ID))
            {
                Logic_Buildings.RemoveHouse();
            }
            else return;
        }
        // called when level loading begins
        public void OnCreated
        (ILoading loading)
        {
            // Default instances
            Hook_Addresses_Event loading_hook = new Hook_Addresses_Event();

            // Events
            BuildingManager.instance.EventBuildingCreated += loading_hook.OnBuildingCreated;

            BuildingManager.instance.EventBuildingReleased += loading_hook.OnBuildingDestroyed;

            // TODO
            // BuildingManager.instance.EventBuildingRelocated += loading_hook.OnBuildingDestroyed;
            // BuildingManager.instance.EventBuildingRelocated += loading_hook.OnBuildingCreated;
        }
        // called when level is loaded
        public void OnLevelLoaded
        (LoadMode mode)
        {
            // // create dialog panel
            // ExceptionPanel panel = UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel");
            // 
            // // display a message for the user in the panel
            // panel.SetMessage("Real Addresses", Addresses_SaveGame_Hook.CitySave.Roads.Count().ToString(), false);
        }
        // called when unloading begins
        public void OnLevelUnloading
        ()
        {
        }
        // called when unloading finished
        public void OnReleased
        ()
        {
        }
    }
}
