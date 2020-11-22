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

        public ushort ifvehiclehasXhealthStopWork;

        public ushort burnfueldamageifvehiclestopworking;

        public bool burnfuelifvehiclestopworking;

        public void LoadDefaults()
        {
            nausea = true;

            ifvehiclehasXhealthStopWork = 248; // this means 50%

            nauseatime = 10;

            healthdamage = 11;

            wheelsdamage = true;

            burnfuelifvehiclestopworking = true;

            burnfueldamageifvehiclestopworking = 50;

            wheelchancedamage = .45f; // this means 45% nigga

            autowarnmechanic = true;
        }
    }
}
