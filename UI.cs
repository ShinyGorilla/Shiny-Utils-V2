using UnityEngine;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using MonkeNotificationLib;
using BepInEx;
using HarmonyLib;
using Shiny_Utils_V2.Behaviors;

namespace Shiny_Utils_V2.UI
{
    [BepInPlugin("ShinyGorilla.ShinyGUI", "ShinyGUI", "1.0.0")]
    internal class ShinyGui : BaseUnityPlugin
    {
        int toolbarInt = 0;
        string[] toolbarStrings = { "Codes", "Board", "Cam", "Silly", "Notifs", "Misc"};
        string wantCode = "";
        public static string NoiseString = "1";
        public string LeaderBoard;
        public static object GameMode = "";
        public static float Fov = 80f;
        public static float dancestyle = 1f;
        public static bool camlerp;
        public static bool TFL;
        public static bool GnotNotif;
        public static bool PropNotifs;
        public static bool VoiceActive;
        void OnGUI()
        {
            toolbarInt = GUI.Toolbar(new Rect(20, 25, 350, 30), toolbarInt, toolbarStrings);
            switch (toolbarInt)
            {
                case 0: //Code joining
                    wantCode = GUI.TextField(new Rect(25f, 65f, 250f, 25f), wantCode.ToUpper());
                    if (GUI.Button(new Rect(25f, 95f, 250f, 25f), "Join Code"))
                    {
                        if (wantCode.Length > 0f)
                        {
                            if (!wantCode.Contains(" "))
                            {
                                if (wantCode.Length <= 10)
                                    wantCode = wantCode.ToUpper();
                                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(wantCode);
                                PhotonNetworkController.Instance.attemptingToConnect = true;
                                PlayerPrefs.Save();
                            }
                            else
                            {
                                NotificationController.AppendMessage("ShinyGUI".WrapColor("green"), $"[{"Protector".WrapColor("cyan")}]" + " Cannot join a room with spaces".WrapColor("danger"));
                            }
                        }
                        else
                        {
                            NotificationController.AppendMessage("ShinyGUI".WrapColor("green"), $"[{"Protector".WrapColor("cyan")}]" + " Cannot join blank room".WrapColor("danger"));
                        }
                        
                    }

                    if (GUI.Button(new Rect(25f, 125f, 250f, 25f), "Leave Code"))
                        PhotonNetworkController.Instance.AttemptDisconnect();
                    if (GUI.Button(new Rect(25f, 155f, 250f, 25f), "Join random (Modded Casual)"))
                        SExtensions.JoinRandomModded("casual");
                    if (GUI.Button(new Rect(25f, 185f, 250f, 25f), "Join random (Modded Infection)"))
                        SExtensions.JoinRandomModded("infection");
                    break;



                case 1: //Leaderboard
                    if (PhotonNetwork.InRoom)
                    {
                        LeaderBoard += $"-Room info-\r\n<color=#00ffff>Room: {PhotonNetwork.CurrentRoom.Name}\r\nPublic: {PhotonNetwork.CurrentRoom.IsVisible.ToString()}\r\nPlayers: {PhotonNetwork.CurrentRoom.PlayerCount}/10\r\nGamemode: {GameMode} </color>\r\n\r\n-Leaderboard-\r\n";
                        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                        {
                            Player player = PhotonNetwork.PlayerList[i];
                            if (player.IsMasterClient && !PhotonNetwork.IsMasterClient)
                            {
                                if (i == 0)
                                    LeaderBoard += $"<color=#f0ad4e>{player.NickName} (LEGIT MASTER)</color>\r\n";
                                else
                                    LeaderBoard += $"<color=#d9534f>{player.NickName} (CHEATED MASTER)</color>\r\n";
                            }
                            else if (player.UserId == PhotonNetwork.LocalPlayer.UserId && PhotonNetwork.IsMasterClient)
                            {
                                if (i != 0)
                                    Application.Quit();

                                LeaderBoard += $"<color=#f0ad4e>{player.NickName} (MASTER CLIENT)</color> <color=#00ffff>(YOU)</color>\r\n";
                            }
                            else if (player.UserId == PhotonNetwork.LocalPlayer.UserId)
                                LeaderBoard += $"<color=#00ffff>{player.NickName} (YOU)</color>\r\n";
                            else
                                LeaderBoard += $"{player.NickName.WrapColor("green")}\r\n";
                        }

                        GUI.Label(new Rect(20f, 65f, 500f, 25000f), LeaderBoard);
                        LeaderBoard = "";
                    }
                    else
                        GUI.Label(new Rect(20f, 65f, 500f, 75f), "<color=red>Please join a lobby to access this feature</color>");

                    break;



                case 2: //Camera
                    if (GUI.Button(new Rect(20f, 65f, 250f, 25f), $"Toggle Lerp Rotation ({camlerp.ToString()})"))
                    {
                        camlerp = !camlerp;
                        NotificationController.AppendMessage("ShinyGUI".WrapColor("green"), $"[{"Camera".WrapColor("camera")}] Lerp rotation ({camlerp.ToString().ToUpper().WrapColor("cyan")})");

                        if (camlerp) //Rewrite save
                            Shiny_Utils_V2.Config.Save(0, "1");
                        else
                            Shiny_Utils_V2.Config.Save(0, "0");
                    }

                    if (GUI.Button(new Rect(20f, 95f, 250f, 25f), $"Toggle third person ({Plugin.thirdperson.ToString()})"))
                    {
                        Plugin.thirdperson = !Plugin.thirdperson;
                        Plugin.UpdateCam();
                        NotificationController.AppendMessage("ShinyGUI".WrapColor("green"), $"[{"Camera".WrapColor("camera")}] Third person ({Plugin.thirdperson.ToString().ToUpper().WrapColor("cyan")})");

                        if (Plugin.thirdperson)
                            Shiny_Utils_V2.Config.Save(4, "1");
                        else
                            Shiny_Utils_V2.Config.Save(4, "0");
                    }

                    if (GUI.Button(new Rect(20f, 125f, 250f, 25f), $"<color=green>Fov +</color>"))
                        Fov += 1f;

                    if (GUI.Button(new Rect(20f, 155f, 250f, 25f), $"<color=red>Fov -</color>"))
                        Fov -= 1f;

                    GUI.Label(new Rect(20f, 185f, 500, 50), "Current FOV: \r\n" + Fov.ToString());

                    break;



                case 3: //Silly
                    if (GUI.Button(new Rect(20f, 65f, 250f, 25f), $"Toggle friend leave ({TFL.ToString().ToUpper()})"))
                    {
                        TFL = !TFL;
                        NotificationController.AppendMessage("ShinyGUI".WrapColor("green"), $"[{"Silly".WrapColor("purple")}] Leave on friend disconnect set to {TFL.ToString().ToUpper().WrapColor("cyan")}");

                        if (TFL) //Rewrite save
                            Shiny_Utils_V2.Config.Save(1, "1");
                        else
                            Shiny_Utils_V2.Config.Save(1, "0");
                    }
                        
                    if (GUI.Button(new Rect(20f, 95f, 250f, 25f), $"Change emote type (GOOFY) ({dancestyle.ToString().ToUpper()})"))
                    {
                        dancestyle += 1f;
                        Plugin.FixEmoteType();
                        Shiny_Utils_V2.Config.Save(5, dancestyle.ToString());
                        NotificationController.AppendMessage("ShinyGUI".WrapColor("green"), $"[{"Silly".WrapColor("purple")}] Emote style changed to {dancestyle.ToString().WrapColor("cyan")}");
                    }
                    break;



                case 4: //Notifs
                    if (GUI.Button(new Rect(20f, 65f, 250f, 25f), $"Toggle Gorilla Not notifs ({GnotNotif.ToString().ToUpper()})"))
                    {
                        GnotNotif = !GnotNotif;
                        NotificationController.AppendMessage("ShinyGUI".WrapColor("green"), $"[{"Notifs".WrapColor("misc")}] Gorilla Not notifications {GnotNotif.ToString().ToUpper().WrapColor("cyan")}");

                        if (GnotNotif) //Rewrite save
                            Shiny_Utils_V2.Config.Save(2, "1");
                        else
                            Shiny_Utils_V2.Config.Save(2, "0");
                    }
                    if (GUI.Button(new Rect(20f, 95f, 250f, 25f), $"Toggle Property Update notifs ({PropNotifs.ToString().ToUpper()})"))
                    {
                        PropNotifs = !PropNotifs;
                        NotificationController.AppendMessage("ShinyGUI".WrapColor("green"), $"[{"Notifs".WrapColor("misc")}] Room/Player Prop notifications {PropNotifs.ToString().ToUpper().WrapColor("cyan")}");

                        if (PropNotifs) //Rewrite save
                            Shiny_Utils_V2.Config.Save(3, "1");
                        else
                            Shiny_Utils_V2.Config.Save(3, "0");
                    }
                    if (GUI.Button(new Rect(20f, 125f, 250f, 25f), $"Test controller device"))
                    {
                        Debug.Log(ControllerInputPoller.instance.leftControllerPosition);
                    }
                    break;


                case 5:
                    NoiseString = GUI.TextField(new Rect(25f, 65f, 250f, 25f), NoiseString);
                    Shiny_Utils_V2.Config.Save(6, NoiseString);
                    GUI.Label(new Rect(20f, 95f, 250f, 25f), $"XR enabled: {Plugin.XrEnabled.ToString()}");
                    if (GUI.Button(new Rect(20f, 125f, 250f, 25f), $"Toggle Voice ({VoiceActive.ToString().ToUpper()})"))
                    {
                        GorillaComputer.instance.voiceChatOn = GorillaComputer.instance.voiceChatOn == "TRUE" ? "FALSE" : "TRUE";
                        PlayerPrefs.SetString("voiceChatOn", GorillaComputer.instance.voiceChatOn);

                        AccessTools.Method(typeof(GorillaTagger).Assembly.GetType("RigContainer"), "RefreshAllRigVoices").Invoke(null, null);
                    }
                    if (GUI.Button(new Rect(20f, 155f, 250f, 25f), "Random Noise"))
                    {
                        int noiseInt = int.Parse(NoiseString);
                        
                        GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(noiseInt, true, 1f);
                        //Just to clear some things up this is completely client sided so it is not a cheat
                    }
                    break;
            }
        }
    }
}