using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using SpaceFighters.Server;

#nullable enable

namespace SpaceFighters.Client
{
    public class PlayerInputReceiver : NetworkBehaviour
    {
        [SerializeField] private PlayerInputHandler? InputHandler;

        private PlayerInput? input;

        public override void OnNetworkSpawn()
        {
            if (!IsClient) enabled = false;
            input = new();
            input.Enable();
        }

        private void Update()
        {
            if (InputHandler != null && input != null)
            {
                float accel = input.Combat.Accelerate.ReadValue<float>();
                float rot = input.Combat.Rotate.ReadValue<float>();
                InputHandler.HandleMoveInputServerRpc(new Vector2(rot, accel), new ServerRpcParams());

                if (input.Combat.Shoot.WasPerformedThisFrame())
                {
                    InputHandler.HandleShootInputServerRpc(new ServerRpcParams());
                }
            }
        }
    }
}
