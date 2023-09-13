using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    // TODO: Update input at server tick rate, and receive input into a buffer at a variable tick rate
    public class PlayerInputHandler : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        public void HandleMoveInputServerRpc(Vector2 input, ServerRpcParams rpc)
        {
            if (ServerManager.Instance == null) return;
            if (ServerManager.Instance.ActiveGame == null) return;
            ulong sender = rpc.Receive.SenderClientId;
            if (ServerManager.Instance.ActiveGame.PlayerGameInfoTable.TryGetValue(sender, out Player player))
            {
                player.controller?.Move(input, Time.deltaTime);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void HandleShootInputServerRpc(ServerRpcParams rpc)
        {
            if (ServerManager.Instance == null) return;
            if (ServerManager.Instance.ActiveGame == null) return;
            ulong sender = rpc.Receive.SenderClientId;
            if (ServerManager.Instance.ActiveGame.PlayerGameInfoTable.TryGetValue(sender, out Player player))
            {
                player.controller?.Shoot();
            }
        }
    }
}
