using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

#nullable enable

namespace SpaceFighters.Client
{
    public class PregameManager : MonoBehaviour
    {
        public const string CasualMatchmakingQueueName = "Casual";

        public static PregameManager? Instance { get; private set; }

        public Player? AuthenticatedMatchmakerPlayer { get; private set; }

        private string? activeTicketId = null;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public async Task<bool> SignInAsync()
        {
            if (!await UnityServicesHelper.EnsureInitialized()) return false;

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                // Process ID is a hack to allow multiple instances of the game to matchmake on the same machine
                AuthenticatedMatchmakerPlayer = new Player(AuthenticationService.Instance.PlayerId + Process.GetCurrentProcess().Id);
                return true;
            }
            catch (RequestFailedException e)
            {
                Debug.LogError($"Failed to sign in: {e.Message}");
                return false;
            }
        }

        public async Task<bool> FindMatchAsync()
        {
            if (!await UnityServicesHelper.EnsureInitialized()) return false;
            if (!AuthenticationService.Instance.IsSignedIn || AuthenticatedMatchmakerPlayer == null)
            {
                Debug.LogError("Cannot find match without signing in first");
                return false;
            }
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("Cannot find match without NetworkManager");
                return false;
            }


            List<Player> players = new List<Player>
            {
                AuthenticatedMatchmakerPlayer
            };

            CreateTicketOptions options = new CreateTicketOptions(CasualMatchmakingQueueName);

            CreateTicketResponse ticketCreation = await MatchmakerService.Instance.CreateTicketAsync(players, options);
            activeTicketId = ticketCreation.Id;

            while (true)
            {
                TicketStatusResponse ticketStatus = await MatchmakerService.Instance.GetTicketAsync(ticketCreation.Id);
                MultiplayAssignment? assignment = ticketStatus.Value as MultiplayAssignment;
                if (assignment != null)
                {
                    switch (assignment.Status)
                    {
                        case MultiplayAssignment.StatusOptions.Found:
                            string ip = assignment.Ip;
                            ushort port = 0;
                            if (assignment.Port.HasValue) port = (ushort)assignment.Port;
                            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                            transport.SetConnectionData(ip, port);
                            activeTicketId = null;
                            return true;
                        case MultiplayAssignment.StatusOptions.InProgress:
                            break;
                        case MultiplayAssignment.StatusOptions.Failed:
                            Debug.LogError("Matchmaking failed: " + assignment.Message);
                            activeTicketId = null;
                            return false;
                        case MultiplayAssignment.StatusOptions.Timeout:
                            Debug.LogError("Matchmaking timed out");
                            activeTicketId = null;
                            return false;
                    }
                }
                await Task.Delay(2100);
            }
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Battleground", LoadSceneMode.Single);
        }

        private void OnDestroy()
        {
            if (activeTicketId != null) MatchmakerService.Instance.DeleteTicketAsync(activeTicketId);
        }
    }
}
