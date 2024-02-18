using GorillaNetworking;
using MonkeNotificationLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Shiny_Utils_V2.Behaviors
{
    public class SExtensions
    {
        public static void JoinRandomModded(string gamemode)
        {
            switch (gamemode)
            {
                
                case "casual":
                    GorillaComputer.instance.currentGameMode.Value = $"MODDED_MODDED_CASUALCASUAL";
                    
                    break;
                case "infection":
                    GorillaComputer.instance.currentGameMode.Value = $"MODDED_MODDED_DEFAULTINFECTION";
                    break;
            }

            PhotonNetworkController.Instance.currentJoinTrigger = GameObject.Find("JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit").GetComponent<GorillaNetworkJoinTrigger>();
            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(PhotonNetworkController.Instance.currentJoinTrigger);
        }
        public static void LeaveRoomReason(string Reason)
        {
            PhotonNetworkController.Instance.AttemptDisconnect();
            NotificationController.AppendMessage("ShinyUtils", $"[{"Protector".WrapColor("cyan")}]" + $" {Reason}");
        }
    }
    
}
