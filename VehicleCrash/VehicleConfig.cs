using Rocket.API;

namespace VehicleCrash
{
    public class VehicleConfig : IRocketPluginConfiguration
    {

        public byte healthdamage;

        public bool nausea;

        public float nauseatime;

        public bool wheelsdamage;

        public bool autowarnmechanic;

        public float wheelchancedamage;

        public void LoadDefaults()
        {
            nausea = true;

            nauseatime = 10;

            healthdamage = 11;

            wheelsdamage = true;

            wheelchancedamage = .45f;

            autowarnmechanic = true;
        }
    }
}