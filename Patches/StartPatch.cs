using System.Collections;
using Cinemachine;
using GorillaLocomotion;
using HarmonyLib;

namespace Shiny_Utils_V2.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    internal class StartPatch //Patches the start
    {
        private static void Postfix(Player __instance)
        {
            __instance.StartCoroutine(Delay());
        }
        private static IEnumerator Delay()
        {
            yield return 0;
            Plugin.FixedStart();
            yield break;
        }
    }
}
