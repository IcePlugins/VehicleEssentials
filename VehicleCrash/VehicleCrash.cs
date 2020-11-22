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
using SDG.Framework.Utilities;
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

        protected override void Load()
        {
            Instance = this;

            VehicleManager.onDamageVehicleRequested += OnDamageVehicle;
            VehicleManager.onEnterVehicleRequested += onEnterVehicleRequested;
        }

        public override Rocket.API.Collections.TranslationList DefaultTranslations
        {
            get
            {
                return new Rocket.API.Collections.TranslationList
                {
                    {"warning", "(color=yellow)[Vehicle Essentials](/color)(color=red) {0} need a mechanic SMS him(/color)"},
                    {"iconwarning", "https://plugins.darknesscommunity.club/images/21.png"}
                };
            }
        }
        private void onEnterVehicleRequested(Player player, InteractableVehicle vehicle, ref bool shouldAllow)
        {
            if (vehicle.health < VehicleCrash.Instance.Configuration.Instance.ifvehiclehasXhealthStopWork)
            {
                shouldAllow = false;
            }
            else
            {
                shouldAllow = true;
            }
        }
        private void tire_Damage(UnturnedPlayer player, int wheels, float random)
        {
            if (random < VehicleCrash.Instance.Configuration.Instance.wheelchancedamage && VehicleCrash.Instance.Configuration.Instance.wheelsdamage)
            {
                player.CurrentVehicle.askDamageTire(wheels);

                if (VehicleCrash.Instance.Configuration.Instance.autowarnmechanic)
                    ChatManager.serverSendMessage(VehicleCrash.Instance.Translate("warning", player.DisplayName).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, VehicleCrash.Instance.Translate("iconwarning", player.DisplayName), true);
            }
        }
        private void burn_Fuel(InteractableVehicle veh)
        {
            if (VehicleCrash.Instance.Configuration.Instance.burnfuelifvehiclestopworking)
            {
                VehicleManager.instance.channel.send("tellVehicleFuel", ESteamCall.ALL, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, veh.instanceID, VehicleCrash.Instance.Configuration.Instance.burnfueldamageifvehiclestopworking);
            }
        }

        private void OnDamageVehicle(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(instigatorSteamID);

            // uff zombie detector and player -_-
            if (pendingTotalDamage <= 2)
                return;

            if (player == null)
                return;

            // need to do some changes here, with health update, because when u hit a player still working this function, nelson nigger don't want to fix this...
            if (damageOrigin.ToString() == "Vehicle_Collision_Self_Damage")
            {
                if (player.CurrentVehicle.asset.engine == EEngine.CAR)
                {
                    if (player.CurrentVehicle.health < VehicleCrash.Instance.Configuration.Instance.ifvehiclehasXhealthStopWork)
                    {
                        burn_Fuel(player.CurrentVehicle);

                        vehicle.forceRemoveAllPlayers();

                        if (VehicleCrash.Instance.Configuration.Instance.autowarnmechanic)
                            ChatManager.serverSendMessage(VehicleCrash.Instance.Translate("warning", player.DisplayName).Replace('(', '<').Replace(')', '>'), Color.white, null, null, EChatMode.SAY, VehicleCrash.Instance.Translate("iconwarning", player.DisplayName), true);
                    }
                    else
                    {
                        tire_Damage(player, UnityEngine.Random.Range(1, 2), UnityEngine.Random.value);

                        foreach (var passenger in vehicle.passengers)
                        {
                            UnturnedPlayer jugador = UnturnedPlayer.FromSteamPlayer(passenger.player);

                            if (jugador != null && !jugador.GetComponent<PlayerComponent>().niggagetwork)
                                StartCoroutine(crash(jugador));
                            // break; my cucumber idk, for prevent bugs
                            break;
                        }
                    }
                }
            }
        }

        IEnumerator crash(UnturnedPlayer vehicle)
        {
            EPlayerKill kill;
            vehicle.Player.life.askDamage(VehicleCrash.Instance.Configuration.Instance.healthdamage, Vector3.zero, EDeathCause.KILL, ELimb.SPINE, CSteamID.Nil, out kill);
            vehicle.Player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, VehicleCrash.Instance.Configuration.Instance.nausea);
            vehicle.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, VehicleCrash.Instance.Configuration.Instance.nausea);
            yield return new WaitForSeconds(VehicleCrash.Instance.Configuration.Instance.nauseatime);
            vehicle.Player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, false);
            vehicle.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }


        protected override void Unload()
        {
            VehicleManager.onDamageVehicleRequested -= OnDamageVehicle;
            VehicleManager.onEnterVehicleRequested -= onEnterVehicleRequested;
        }
    }
}
