using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace SpaceFighters
{
    public class DebugUI : MonoBehaviour
    {
        private void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 100), "Host"))
            {
                NetworkManager.Singleton.StartHost();
                enabled = false;
            }

            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 0, 300, 100), "Client"))
            {
                NetworkManager.Singleton.StartClient();
                enabled = false;
            }

            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 150, 300, 100), "Server"))
            {
                NetworkManager.Singleton.StartServer();
                enabled = false;
            }
        }
    }
}
