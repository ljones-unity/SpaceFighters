using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace SpaceFighters
{
    public class DestroyDuplicateNetworkManager : MonoBehaviour
    {
        [SerializeField] private NetworkManager? NetworkManager;

        private void Awake()
        {
            if (NetworkManager.Singleton != null)
            {
                Destroy(gameObject);
            }
        }

        private void Reset()
        {
            NetworkManager = GetComponent<NetworkManager>();
        }
    }
}
