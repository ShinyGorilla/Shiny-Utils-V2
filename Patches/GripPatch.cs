using HarmonyLib;
using MonkeNotificationLib;
using UnityEngine;
using Photon.Pun;
using Shiny_Utils_V2.UI;

namespace Shiny_Utils_V2.Patches.GNotPatching
{
    [HarmonyPatch(typeof(ControllerInputPoller), "Update")]
    static class GripPatch
    {
        public static bool forceGripR;
        public static bool forceGripL;
        static void Postfix(ControllerInputPoller __instance)
        {
            if (forceGripR)
            {
                __instance.rightControllerGripFloat = 1;
            }
            if (forceGripL)
            {
                __instance.leftControllerGripFloat = 1;
            }
        }
    }
}