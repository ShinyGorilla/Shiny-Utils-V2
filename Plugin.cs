using BepInEx;
using GorillaFriends;
using GorillaNetworking;
using MonkeNotificationLib;
using Photon.Pun;
using Shiny_Utils_V2.Behaviors;
using UnityEngine;
using UnityEngine.InputSystem;
using Shiny_Utils_V2.UI;
using Cinemachine;
using System.IO;
using Shiny_Utils_V2.Patches.GNotPatching;
using HarmonyLib;

namespace Shiny_Utils_V2
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("com.sinai.unityexplorer")]
    [BepInDependency("crafterbot.notificationlib")]
    public class Plugin : BaseUnityPlugin
    {
        private void Start()
        {
            new Callbacks();
            new ShinyGui();
        }

        void OnEnable() => HarmonyPatches.ApplyHarmonyPatches();
        void OnDisable() => HarmonyPatches.RemoveHarmonyPatches();

        public float GuiTab = 1f;
        public static float moveAmount = 0.82f;
        public static float moveUp = 0.25f;

        public static bool AlreadyDone = false;
        public static bool thirdperson = true;
        public static bool XrEnabled = false;

        public static string TheRoom;
        public static string[] VerifiedIDs =
        {
            "36B456067A5E1453",
            "970B338BBDC11A77", //bird
            "480BF235B17212F",
            "FBE3EE50747CB892", //luna
            "4994748F8B361E31", //evelyn
            "E354E818871BD1D8", //dev
            "CA8FDFF42B7A1836", //stone
            "D322FC7F6A9875DB", //decal
            "F8A85C07F35E787D", //sodah
            "359E716BF71A54A9", //squid
            "637F3CE95093F98E", //striker
            "E249F41EABC2B38E", //Jon the smart boi
        }; //Not used to track btw

        public static Collider handr;

        public static GameObject PCCam;

        void Update() { if (GorillaTagger.Instance != null) { GoodUpdate(); } }
        public static void GoodUpdate()
        {
            ShinyGui.VoiceActive = GorillaComputer.instance.voiceChatOn == "TRUE";

            if (!XrEnabled)
            {
                XrEnabled = ControllerInputPoller.instance.leftControllerPosition.ToString() == "(0.0, 0.0, 0.0)";
            }
            UpdateCam();

            //Click
            if (PCCam != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Ray ray = GorillaTagger.Instance.mainCamera.GetComponent<Camera>().ScreenPointToRay(UnityEngine.InputSystem.Pointer.current.position.value);
                    if (Physics.Raycast(ray, out RaycastHit hit, 5000f))
                    {
                        GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>().enabled = false;
                        GorillaTagger.Instance.rightHandTriggerCollider.transform.position = hit.point;
                        GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>().enabled = true;

                    }
                }
            }

            //Emotes
            if (Mouse.current.middleButton.isPressed)
            {
                switch (ShinyGui.dancestyle)
                {
                    case 1f: //Tpose
                        GorillaLocomotion.Player.Instance.rightControllerTransform.position += GorillaLocomotion.Player.Instance.rightControllerTransform.up;
                        GorillaLocomotion.Player.Instance.leftControllerTransform.position += GorillaLocomotion.Player.Instance.leftControllerTransform.up;
                        break;
                    case 2f: //Up
                        GorillaLocomotion.Player.Instance.rightControllerTransform.position += Camera.main.transform.up;
                        GorillaLocomotion.Player.Instance.leftControllerTransform.position += Camera.main.transform.up;
                        break;
                    case 3f: //Down
                        GorillaLocomotion.Player.Instance.rightControllerTransform.position -= Camera.main.transform.up;
                        GorillaLocomotion.Player.Instance.leftControllerTransform.position -= Camera.main.transform.up;
                        break;
                    case 4f: //Griddy
                        GorillaLocomotion.Player.Instance.leftControllerTransform.position += Camera.main.transform.forward * 0.5f;
                        GorillaLocomotion.Player.Instance.leftControllerTransform.position += Camera.main.transform.up * 0.4f;

                        GorillaLocomotion.Player.Instance.rightControllerTransform.position += Camera.main.transform.forward * 0.5f;
                        GorillaLocomotion.Player.Instance.rightControllerTransform.position += Camera.main.transform.up * 0.4f;
                        break;
                }
            }

            // Hand move
            if (Keyboard.current.eKey.isPressed)
            {
                GorillaLocomotion.Player.Instance.leftControllerTransform.position += Camera.main.transform.forward * moveAmount;
                GorillaLocomotion.Player.Instance.leftControllerTransform.position += Camera.main.transform.up * moveUp;
                GripPatch.forceGripL = true;
            }
            else
                GripPatch.forceGripL = false;

            if (Keyboard.current.rKey.isPressed)
            {
                GorillaLocomotion.Player.Instance.rightControllerTransform.position += Camera.main.transform.forward * moveAmount;
                GorillaLocomotion.Player.Instance.rightControllerTransform.position += Camera.main.transform.up * moveUp;
                GripPatch.forceGripR = true;
            }
            else
                GripPatch.forceGripR = false;

            //Room Joining
            if (PhotonNetwork.InRoom && !AlreadyDone)
            {
                TheRoom = PhotonNetwork.CurrentRoom.Name;
                NotificationController.AppendMessage("ShinyUtils".WrapColor("green"), "Joined room: " + TheRoom.WrapColor("cyan"));
                AlreadyDone = true;
            }

            if (!PhotonNetwork.InRoom && AlreadyDone)
            {
                NotificationController.AppendMessage("ShinyUtils".WrapColor("green"), "Left room: " + TheRoom.WrapColor("cyan"));
                AlreadyDone = false;
            }

            if (PhotonNetwork.InRoom)
                PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gameMode", out ShinyGui.GameMode);

            if (!ShinyGui.GameMode.ToString().Contains("MODDED") && PhotonNetwork.InRoom && !XrEnabled)
                SExtensions.LeaveRoomReason($"Gamemode was not modded -> Left lobby".WrapColor("danger"));
        }
        public static void FixEmoteType() //Called when changing emote type, fixes emote type being past the limit
        {
            if (ShinyGui.dancestyle > 4f)
                ShinyGui.dancestyle = 1f;
        }
        public static void UpdateCam() //Called when switching to third or first person, sets the components enabled value to the right value and gets the PCCam obj again
        {
            PCCam.GetComponent<CinemachineBrain>().enabled = thirdperson;
            PCCam.GetComponent<ShinyCam>().enabled = !thirdperson;
            PCCam = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
        }
        public static void FixedStart() //Called in the start of the game when preloader is finished
        {
            PCCam = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
            PCCam.AddComponent<ShinyCam>();

            handr = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHandTriggerCollider").GetComponent<Collider>();
            Camera.main.useOcclusionCulling = true;
            PhotonNetworkController.Instance.disableAFKKick = true;
            GorillaComputer.instance.currentGameMode.Value = "MODDED_MODDED_CASUALCASUAL";

            Shiny_Utils_V2.Config.Create();

            //Custom cfg reading
            string[] readText = File.ReadAllLines(Shiny_Utils_V2.Config.fullPath);
            Debug.Log("File content: " + readText.ToString());

            if (readText.Length > 0)
            {
                //lerp
                bool cfg1 = readText[0] == "1";
                ShinyGui.camlerp = cfg1;

                //Tfl
                bool cfg2 = readText[1] == "1";
                ShinyGui.TFL = cfg2;

                //gnot
                bool cfg3 = readText[2] == "1";
                ShinyGui.GnotNotif = cfg3;

                //Prop notifs
                bool cfg4 = readText[3] == "1";
                ShinyGui.PropNotifs = cfg4;

                //third
                bool cfg5 = readText[4] == "1";
                thirdperson = cfg5;

                //Dance style
                string cfg6string = readText[5];
                float cfg6 = float.Parse(cfg6string);
                ShinyGui.dancestyle = cfg6;

                //Sound to play
                string cfg7 = readText[6];
                ShinyGui.NoiseString = cfg7;

            }

            NotificationController.AppendMessage("ShinyUtils".WrapColor("green"), "ShinyUtils loaded");
            Debug.LogError("Please help I am dying inside, I have had no sleep - ShinyGorilla");
        }
    }
}
