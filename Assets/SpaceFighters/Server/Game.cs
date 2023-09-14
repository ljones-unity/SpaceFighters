using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    public class Game
    {
        public Dictionary<ulong, Player> PlayerGameInfoTable = new();

        public void AddPlayer(ulong clientId, Player player)
        {
            if (player.ship == null) return;
            PlayerGameInfoTable.Add(clientId, player);
            player.clientId = clientId;
            player.ship.OnDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            CheckForWinCondition();
        }

        private void CheckForWinCondition()
        {
            List<Player> alive = new();
            foreach (Player player in PlayerGameInfoTable.Values)
            {
                if (player.ship == null) continue;
                if (player.ship.CurrentHealth.Value > 0)
                {
                    alive.Add(player);
                }
            }

            if (alive.Count == 1)
            {
                GameEndVictory(alive[0]);
            }
        }

        private void GameEndVictory(Player player)
        {
            Debug.Log($"Game End Victory for player {player.clientId}");
        }
    }
}
