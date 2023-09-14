using System;
using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    public class ServerManager : NetworkBehaviour
    {
        public static ServerManager? Instance { get; private set; }

        [SerializeField] private NetworkManager? Network;

        public Game? ActiveGame { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (Network == null) Network = NetworkManager.Singleton;

            Debug.Log("Server Manager Spawned");
            // Only exist on server
            if (IsServer && Instance == null)
            {
                Instance = this;
                Initialize();
            }
            else
            {
                enabled = false;
            }
        }

        public override void OnNetworkDespawn()
        {
            Deinitialize();
        }

        private void Initialize()
        {
            Debug.Log("Initializing Server Manager");
            if (Network == null)
            {
                Debug.LogError($"Failed to Initialize {nameof(ServerManager)}: {nameof(Network)} is null");
                return;
            }

            ActiveGame = new Game();
            Debug.Log("Initialized New Game");
            
            Network.OnClientConnectedCallback += OnClientConnected;
            InitializeExistingClients(Network, ActiveGame);
            Debug.Log("Finished Initializing Server Manager");
        }

        private void Deinitialize()
        {
            if (Network == null) return;

            Network.OnClientConnectedCallback -= OnClientConnected;
        }

        private void InitializeExistingClients(NetworkManager network, Game game)
        {
            foreach (NetworkClient client in network.ConnectedClientsList)
            {
                InitializeConnectedClient(client, game);
            }
        }

        private void OnClientConnected(ulong clientId)
        {
            Debug.Log($"Client {clientId} Connected");
            if (Network == null) return;
            if (ActiveGame == null) return;
            NetworkClient client = Network.ConnectedClients[clientId];
            if (client == null) return;
            InitializeConnectedClient(client, ActiveGame);
        }

        private void InitializeConnectedClient(NetworkClient client, Game game)
        {
            Player player = client.PlayerObject.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError($"Failed to initialize client {client.ClientId}: Player object does not have {nameof(Player)} component");
                return;
            }

            if (game.PlayerGameInfoTable.ContainsKey(client.ClientId))
            {
                return;
            }

            game.AddPlayer(client.ClientId, player);
            Debug.Log($"Initalized client {client.ClientId}");
        }
    }
}
