using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleCrash
{
    public class PlayerComponent : UnturnedPlayerComponent
    {
        public bool niggagetwork = false, check = false;
        
        public void FixedUpdate()
        {
            /*if (Player.CurrentVehicle.asset.engine == EEngine.CAR)
            {*/
                if (Player.IsInVehicle && Player.CurrentVehicle.asset.engine == EEngine.CAR)
                {
                    if (Player.Player.input.keys[5])
                    {
                        if (!check)
                            niggagetwork = !niggagetwork;
                        check = true;
                    }
                    else
                        check = false;

                    if (niggagetwork)
                    {
                        EffectManager.askEffectClearByID(14033, Player.CSteamID);

                        EffectManager.sendUIEffect(14044, 14035, Player.CSteamID, true);
                    }
                    else
                    {
                        EffectManager.askEffectClearByID(14044, Player.CSteamID);
                        EffectManager.sendUIEffect(14033, 14034, Player.CSteamID, true);
                    }
                }
                else
                {
                    EffectManager.askEffectClearByID(14033, Player.CSteamID);
                    EffectManager.askEffectClearByID(14044, Player.CSteamID);
                    niggagetwork = false;
                }
                // }
            }
    }
}
