using HarmonyLib;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Morpheus
{
    [HarmonyPatch(typeof(ModSleeping), "AreAllPlayersSleeping")]
    class Patch
    {
        public static ICoreServerAPI sapi;
        public static float percent = 50;

        static int lastSleepersCount = 0;

        public static bool Prefix(ref bool __result, ModSleeping __instance)
        {
            int quantitySleeping = 0;
            int quantityAwake = 0;

            foreach (IPlayer player in sapi.World.AllOnlinePlayers)
            {
                IServerPlayer splr = player as IServerPlayer;
                if (splr.ConnectionState != EnumClientState.Playing || splr.WorldData.CurrentGameMode == EnumGameMode.Spectator) continue;

                IMountable mount = player.Entity?.MountedOn;
                if (mount != null && mount is BlockEntityBed)
                {
                    quantitySleeping++;
                }
                else
                {
                    quantityAwake++;
                }
            }

            if (quantityAwake == 0)
            {
                if (quantitySleeping == 0)
                {
                    __result = false;
                } else
                {
                    __result = true;
                }
            } else if (quantitySleeping == 0)
            {
                __result = false;
            }
            else
            {
                int total = quantitySleeping + quantityAwake;
                float ratio = quantitySleeping / total * 100f;
                if (lastSleepersCount != quantitySleeping)
                {
                    sapi.BroadcastMessageToAllGroups("Night skipping vote: " + quantitySleeping + " / " + total + "(" + ratio + "%)", EnumChatType.Notification);
                    lastSleepersCount = quantitySleeping;
                }
                __result = (ratio >= percent);
            }

            return true;
        }
    }
}
