using ExitGames.Client.Photon;
using MonkeNotificationLib;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Shiny_Utils_V2.UI;
using UnityEngine;

namespace Shiny_Utils_V2.Behaviors
{
    internal class Callbacks : IInRoomCallbacks, IOnEventCallback
    {
        internal Callbacks() =>
            Photon.Pun.PhotonNetwork.AddCallbackTarget(this);

        #region IInRoomCallbacks

        void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer) //Player join room
        {
            
            Debug.Log($"Player {newPlayer.UserId} joined with nickname {newPlayer.NickName}");

            if (Plugin.VerifiedIDs.Contains(newPlayer.UserId) && PhotonNetwork.LocalPlayer.UserId == "7FBC02D1E523A769")
            {
                NotificationController.AppendMessage("Player Event".WrapColor("warning"), $"Friend {newPlayer.NickName.ToUpper().WrapColor("purple")} has entered the room");
            }
            else
            {
                NotificationController.AppendMessage("Player Event".WrapColor("warning"), $"{newPlayer.NickName.ToUpper().WrapColor("cyan")} has entered the room");
            }
        }

        void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer) //Player leave room
        {
            NotificationController.AppendMessage("Player Event".WrapColor("warning"), $"{otherPlayer.NickName.ToUpper().WrapColor("cyan")} has left the room");
            if (Plugin.VerifiedIDs.Contains(otherPlayer.UserId) && ShinyGui.TFL && PhotonNetwork.LocalPlayer.UserId == "7FBC02D1E523A769")
                SExtensions.LeaveRoomReason("Disconnect because someone bitch");
        }

        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) //Player prop update
        {
            if (ShinyGui.PropNotifs)
            {
                string StringchangedProps = JsonConvert.SerializeObject(changedProps);

                if (!StringchangedProps.Contains("CATLICKER")) //Fuck you catlicker
                {
                    if (StringchangedProps.Length > 50f) //Check if big long prop
                    {
                        if (StringchangedProps.Contains("Bark"))
                        {
                            NotificationController.AppendMessage("Player Props".WrapColor("warning"), $"[{"Props".WrapColor("cyan")}] Player {targetPlayer.NickName.ToUpper().WrapColor("cyan")} changed a long player property (Bark modules)");
                        }
                        else
                        {
                            NotificationController.AppendMessage("Player Props".WrapColor("warning"), $"[{"Props".WrapColor("cyan")}] Player {targetPlayer.NickName.ToUpper().WrapColor("cyan")} changed a long player property");
                        }
                    }
                    else if (!StringchangedProps.Contains("Tuto"))
                    {
                        NotificationController.AppendMessage("Player Props".WrapColor("warning"), $"[{"Props".WrapColor("cyan")}] Player {targetPlayer.NickName.ToUpper().WrapColor("cyan")} changed personal props {StringchangedProps.WrapColor("warning")}");
                    }
                }
                
            }
        }
        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient) //Master switch
        {
            NotificationController.AppendMessage("Room Event".WrapColor("warning"), $"[{"MASTER".WrapColor("cyan")}] Player {newMasterClient.NickName.ToUpper().WrapColor("cyan")} is now master!");
        }
        void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) //Room prop update
        {
            string StringPropschanged = JsonConvert.SerializeObject(propertiesThatChanged);

            if (ShinyGui.PropNotifs)
            {
                NotificationController.AppendMessage("Room Props".WrapColor("warning"), $"[{"Properties".WrapColor("cyan")}] Room prop {StringPropschanged} changed");
            }
        }
        #endregion

        #region IOnEventCallback
        void IOnEventCallback.OnEvent(EventData photonEvent) //Tagging
        {
            if (photonEvent.Code == 1 || photonEvent.Code == 2)
            {
                if (photonEvent.CustomData is object[] eventData)
                {
                    var playerList = Photon.Pun.PhotonNetwork.PlayerList;

                    Player tagging = playerList.First(x => x.UserId == (string)eventData[0]);
                    Player tagged = playerList.First(x => x.UserId == (string)eventData[1]);
                    NotificationController.AppendMessage("Gamemode".WrapColor("warning"), $"[{"Infection".WrapColor("cyan")}] {tagging.NickName.ToUpper().WrapColor("cyan")} tagged {tagged.NickName.ToUpper().WrapColor("cyan")}");
                }
            }
            switch (photonEvent.Code)
            {
                case 207:
                    if (photonEvent.CustomData is object[] eventData)
                    {
                        var playerList = PhotonNetwork.PlayerList;

                        Player player = playerList.First(x => x.UserId == (string)eventData[0]);
                        NotificationController.AppendMessage("Photon Events".WrapColor("warning"), $"{player.NickName} used destroy all!".WrapColor("danger"));
                    }
                    break;
            }
        }
        #endregion
    }
}
