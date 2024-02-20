using HarmonyLib;
using MonkeNotificationLib;
using UnityEngine;
using Photon.Pun;
using Shiny_Utils_V2.UI;

namespace Shiny_Utils_V2.Patches.GNotPatching
{
    [HarmonyPatch(typeof(GorillaNot), "IncrementRPCCallLocal")]
    internal class GNotRPCPatch
    {
        static void Postfix(PhotonMessageInfo info, string rpcFunction)
        {
            if (ShinyGui.GnotNotif && !rpcFunction.Contains("PlayerHandTap"))
            {
                NotificationController.AppendMessage("RPC".WrapColor("warning"), $"[{rpcFunction.ToUpper().WrapColor("cyan")}] Called by: {info.Sender.NickName.ToUpper().WrapColor("cyan")}");
            }
            Debug.Log($"{rpcFunction} called by {info.Sender.NickName} (ID: {info.Sender.UserId})");
            Plugin.RpcsCalled += 1f;
        }
    }
}
