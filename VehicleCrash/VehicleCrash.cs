using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Framework.UI.Devkit;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace VehicleCrash
{
    public class VehicleCrash : RocketPlugin<VehicleConfig>
    {
        public static VehicleCrash Instance;

        public List<UnturnedPlayer> FixPlayers = new List<UnturnedPlayer>();

        protected override void Load()
        {
            Instance = this;

            VehicleManager.onDamageVehicleRequested += OnDamageVehicle;
        }

        public override Rocket.API.Collections.TranslationList DefaultTranslations
        {
            get
            {
                return new Rocket.API.Collections.TranslationList
                {
                    {"warning", "(color=yellow)[Vehicle Essentials](/color)(color=red) {0} need a mechanic SMS him(/color)"},
                    {"iconwarning", "https://darknessplugins.com/images/21.png"}
                };
            }
        }

        private void GenerateDamage(UnturnedPlayer player, int wheels, float random)
        {
            if (random < VehicleCrash.Instance.Configuration.Instance.wheelchancedamage && VehicleCrash.Instance.Configuration.Instance.wheelsdamage)
            {
                player.CurrentVehicle.askDamageTire(wheels);

                if (VehicleCrash.Instance.Configuration.Instance.autowarnmechanic)
                    ChatManager.serverSendMessage(VehicleCrash.Instance.Translate("warning", player.DisplayName).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, VehicleCrash.Instance.Translate("iconwarning", player.DisplayName), true);
            }
        }

        private void OnDamageVehicle(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (damageOrigin.ToString() == "Vehicle_Collision_Self_Damage")
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(instigatorSteamID);

                if (player == null)
                    return;

                Instance.GenerateDamage(player, UnityEngine.Random.Range(1, 2), UnityEngine.Random.value);

                foreach (var passenger in vehicle.passengers)
                {
                    UnturnedPlayer jugador = UnturnedPlayer.FromSteamPlayer(passenger.player);

                    if (jugador != null && !jugador.GetComponent<PlayerComponent>().niggagetwork)
                        StartCoroutine(crash(jugador));
                break;
                }
            }
        }

        IEnumerator crash(UnturnedPlayer vehicle)
        {
            EPlayerKill kill;
            PlayerLife life = vehicle.Player.life;
            life.askDamage(VehicleCrash.Instance.Configuration.Instance.healthdamage, Vector3.zero, EDeathCause.KILL, ELimb.SPINE, CSteamID.Nil, out kill);
            vehicle.Player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, VehicleCrash.Instance.Configuration.Instance.nausea);
            yield return new WaitForSeconds(VehicleCrash.Instance.Configuration.Instance.nauseatime);
            vehicle.Player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, false);
        }
        

        protected override void Unload()
        {
            VehicleManager.onDamageVehicleRequested -= OnDamageVehicle;
        }
    }
}
