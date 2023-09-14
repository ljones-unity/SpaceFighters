using Unity.Netcode;

#nullable enable

namespace SpaceFighters.Client
{
    public class DisableIfNotServer : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            gameObject.SetActive(IsServer);
        }
    }
}
