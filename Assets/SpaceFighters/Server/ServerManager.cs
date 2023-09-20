using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    public class ServerManager : NetworkBehaviour
    {
        public static ServerManager? Instance { get; private set; }

        [SerializeField] private NetworkManager? Network;

        public Game? ActiveGame { get; private set; }

        private IServerQueryHandler? serverQueryHandler;

        private void Start()
        {
            if (Network == null) Network = NetworkManager.Singleton;
#if UNITY_SERVER
            InitializeServerAsync();
#else
            Network.StartClient();
#endif
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer && Instance == null)
            {
                Instance = this;
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

        private async void InitializeServerAsync()
        {
            if (Network == null)
            {
                Debug.LogError($"Failed to Initialize {nameof(ServerManager)}: {nameof(Network)} is null");
                return;
            }

            if (!await UnityServicesHelper.EnsureInitialized()) return;
            
            // NOTE: This is hardcoded for now as it is good practice but useless for the scope of this project
            serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(2, "Matchmaking Server", "1v1 Casual", "Latest", "Battleground");

            MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
            multiplayEventCallbacks.Allocate += OnMultiplayAllocate;
            multiplayEventCallbacks.Deallocate += OnMultiplayDeallocate;
            multiplayEventCallbacks.Error += OnMultiplayError;
            multiplayEventCallbacks.SubscriptionStateChanged += OnMultiplaySubscriptionStateChanged;

            await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

            // If our server has already been allocated at this point in time,
            // then we can proceed with initialization now.
            ServerConfig config = MultiplayService.Instance.ServerConfig;
            if (!string.IsNullOrEmpty(config.AllocationId))
            {
                InitializeAfterAllocationAsync(Network, config);
            }
        }

        private async void InitializeAfterAllocationAsync(NetworkManager network, ServerConfig config)
        {
            UnityTransport transport = network.GetComponent<UnityTransport>();
            if (transport == null)
            {
                Debug.LogError("Failed to initialize server: NetworkManager does not have UnityTransport component");
                return;
            }
            transport.SetConnectionData(config.IpAddress, config.Port, "0.0.0.0");
            ActiveGame = new Game();
            network.OnClientConnectedCallback += OnClientConnected;
            network.OnClientDisconnectCallback += OnClientDisconnected;
            network.StartServer();
            InitializeExistingClients(network, ActiveGame);
            await MultiplayService.Instance.ReadyServerForPlayersAsync();
        }

        private void Deinitialize()
        {
            if (Network == null) return;

            Network.OnClientConnectedCallback -= OnClientConnected;
        }

        private void Update()
        {
            if (serverQueryHandler != null)
            {
                serverQueryHandler.UpdateServerCheck();
            }
        }

        private void OnMultiplaySubscriptionStateChanged(MultiplayServerSubscriptionState state)
        {
        }

        private void OnMultiplayError(MultiplayError error)
        {
        }

        private void OnMultiplayDeallocate(MultiplayDeallocation deallocation)
        {
        }

        private void OnMultiplayAllocate(MultiplayAllocation allocation)
        {
            ServerConfig config = MultiplayService.Instance.ServerConfig;
            if (string.IsNullOrEmpty(config.AllocationId))
            {
                Debug.LogError("Failed to allocate server: Allocation ID is null or empty");
                return;
            }
            
            if (Network == null)
            {
                Debug.LogError($"Failed to Initialize {nameof(ServerManager)}: {nameof(Network)} is null");
                return;
            }

            InitializeAfterAllocationAsync(Network, config);
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

        private void OnClientDisconnected(ulong clientId)
        {
            if (Network == null)
            {
                ShutDownServer();
                return;
            }
            if (Network.ConnectedClientsList.Count == 0)
            {
                ShutDownServer();
            }
        }

        // TODO: Not working on server builds???
        private void ShutDownServer()
        {
            Application.Quit(0);
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
