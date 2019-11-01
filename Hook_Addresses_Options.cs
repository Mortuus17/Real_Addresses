using System;
using ICities;
using ColossalFramework.UI;

namespace Addresses
{
    // XML_Save hook
    [ConfigurationPath("Real_Addresses.xml")]
    public class Hook_Addresses_Options : IUserMod
    {
        // Constructor
        public Hook_Addresses_Options
        ()
        {
            // Order matters
            this.NumbersAsPrefix = numbersAsPrefix;
            this.ConsiderRoadSide = considerRoadSide;
            this.RandomNumbers = randomNumbers;
            this.RandomNumbersRange = randomNumbersRange;
            this.Offset = offset;
        }

        // Instance which is active before a game is loaded
        public static Hook_Addresses_Options Global = new Hook_Addresses_Options();

        // Fields
        private ushort offset = 0;
        private ushort randomNumbersRange = 65500;
        private bool randomNumbers = false;
        private bool numbersAsPrefix = false;
        private bool considerRoadSide = true;

        // Properties
        public ushort Offset
        {
            get { return this.offset; }

            set
            {
                if ((value + 15500) >= this.RandomNumbersRange)
                {
                    this.offset = (ushort)(this.RandomNumbersRange - 15500);
                }
                else if (value > 50000)
                {
                    this.offset = 50000;
                }
                else
                {
                    this.offset = value;
                }
            }
        }
        public ushort RandomNumbersRange
        {
            get { return this.randomNumbersRange; }

            set
            {
                if ((value - 15500) <= this.Offset)
                {
                    this.randomNumbersRange = (ushort)(this.Offset + 15500);
                }
                if ((value == 0)
                    ||
                    (value < 0)
                    ||
                    (value > 65500))
                {
                    this.randomNumbersRange = 65500;
                }
                else if (value < 655)
                {
                    this.randomNumbersRange = 655;
                }
                else
                {
                    this.randomNumbersRange = value;
                }
            }
        }
        public bool RandomNumbers
        {
            get { return this.randomNumbers; }

            set { this.randomNumbers = value; }

        }
        public bool NumbersAsPrefix
        {
            get { return this.numbersAsPrefix; }

            set { this.numbersAsPrefix = value; }
        }
        public bool ConsiderRoadSide
        {
            get { return this.considerRoadSide; }

            set { this.considerRoadSide = value; }
        }
        /// <summary>
        /// Options Menu Hook
        /// </summary>
        private UISlider UIoffset;
        private UISlider UIrandomNumbersRange;
        public string Name
        {
            get { return "Real Addresses 0.9.0"; }
        }
        public string Description
        {
            get { return "\nDynamically replaces those repetitive building names with the name of the road and an appropriate number."; }
        }
        public void OnSettingsUI
        (UIHelperBase helper)
        {
            Hook_Addresses_Options.Global = XML_Save<Hook_Addresses_Options>.Load();

            UIHelperBase uiHelperBase1 = helper.AddGroup("Random Numbers");

                ((UIComponent)uiHelperBase1.AddCheckbox("Randomly generated Numbers", Hook_Addresses_Options.Global.randomNumbers, new OnCheckChanged(this.RandomNumbers_Event))).tooltip = "Enables randomly generated numbers";

                this.UIrandomNumbersRange = (UISlider)uiHelperBase1.AddSlider("Range of randomly generated Numbers", 0.0f, 65500f, 655f, Hook_Addresses_Options.Global.RandomNumbersRange, new OnValueChanged(this.RandomNumbersSlider_Event));
                this.UIrandomNumbersRange.tooltip = ("Defines the largest number that can be generated randomly \n Current Value:" + Convert.ToString(Hook_Addresses_Options.Global.RandomNumbersRange));

            UIHelperBase uiHelperBase2 = helper.AddGroup("Naming Pattern");

                this.UIoffset = (UISlider)uiHelperBase2.AddSlider("Offset", 0.0f, 50000f, 500f, Hook_Addresses_Options.Global.Offset, new OnValueChanged(this.OffsetSlider_Event));
                this.UIoffset.tooltip = ("Defines the smallest number that can be generated (randomly) \n Current Value:" + Convert.ToString(Hook_Addresses_Options.Global.Offset));

                ((UIComponent)uiHelperBase2.AddCheckbox("Numbers as Prefixes", Hook_Addresses_Options.Global.NumbersAsPrefix, new OnCheckChanged(this.NumbersAsPrefix_Event))).tooltip = "When enabled, houses will be named like this: \"1592 Park Avenue\" \n instead of: \"Park Avenue 1592\"";

                ((UIComponent)uiHelperBase2.AddCheckbox("Consider Roadside", Hook_Addresses_Options.Global.ConsiderRoadSide, new OnCheckChanged(this.ConsiderRoadSide_Event))).tooltip = "When enabled, houses can only recieve even or odd numbers, respectively, \n according to which side of the road they're built on";
        }

        private void RandomNumbers_Event
        (bool disabled)
        {
            Hook_Addresses_Options.Global.RandomNumbers = disabled;
            XML_Save<Hook_Addresses_Options>.Save();
        }
        private void RandomNumbersSlider_Event
        (float slider)
        {
            Hook_Addresses_Options.Global.RandomNumbersRange = (ushort)slider;
            XML_Save<Hook_Addresses_Options>.Save();
            this.UIrandomNumbersRange.tooltip = ("Defines the largest number that can be generated randomly \nCurrent Value: " + Convert.ToString(Hook_Addresses_Options.Global.randomNumbersRange));
        }
        private void OffsetSlider_Event
        (float slider)
        {   
            Hook_Addresses_Options.Global.Offset = (ushort)slider;
            XML_Save<Hook_Addresses_Options>.Save();
            this.UIoffset.tooltip = ("Defines the smallest number that can be generated (randomly) \nCurrent Value: " + Convert.ToString(Hook_Addresses_Options.Global.Offset));
        }
        public void NumbersAsPrefix_Event
        (bool disabled)
        {
            Hook_Addresses_Options.Global.NumbersAsPrefix = disabled;
            XML_Save<Hook_Addresses_Options>.Save();
        }
        public void ConsiderRoadSide_Event
        (bool enabled)
        {
            Hook_Addresses_Options.Global.ConsiderRoadSide = enabled;
            XML_Save<Hook_Addresses_Options>.Save();
        }
    }
}