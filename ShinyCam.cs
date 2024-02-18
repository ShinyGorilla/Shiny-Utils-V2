using UnityEngine;
using GorillaLocomotion;
using System;
using Shiny_Utils_V2;
using Shiny_Utils_V2.UI;

namespace Shiny_Utils_V2
{
    internal class ShinyCam : MonoBehaviour
    {
        public void Update()
        {
            base.transform.position = Player.Instance.headCollider.transform.position;
            if (ShinyGui.camlerp)
            {
                base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Player.Instance.headCollider.transform.rotation, 5f * Time.deltaTime);
            }
            else
            {
                base.transform.rotation = Player.Instance.headCollider.transform.rotation;
            }
            Plugin.PCCam.GetComponent<Camera>().fieldOfView = ShinyGui.Fov;
            Plugin.PCCam.GetComponent<Camera>().shutterSpeed = 9.005f;
        }
    }
}
