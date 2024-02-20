using HarmonyLib;
using MonkeNotificationLib;
using UnityEngine;
using Shiny_Utils_V2.UI;

namespace Shiny_Utils_V2.Patches.GNotPatching
{
    [HarmonyPatch(typeof(GorillaNot), "SendReport")]
    internal class GNotReportPatch
    {
        static void Postfix(string susReason, string susId, string susNick)
        {
            if (ShinyGui.GnotNotif)
            {
                NotificationController.AppendMessage("Gorilla Not".WrapColor("danger"), $"Anticheat reported {susNick.ToUpper().WrapColor("cyan")} for \"{susReason.WrapColor("danger")}\" - NOT ALL REPORTS ARE ACCURATE!");
            }
            Debug.LogError($"GNot reported {susNick} for {susReason} (ID: {susId})");
        }
    }
}
