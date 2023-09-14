using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    public class LevelBounds : NetworkBehaviour
    {
        [SerializeField] private Bounds Boundary;
        [SerializeField] private float ForcePerDistance;

        public override void OnNetworkSpawn()
        {
            if (!IsServer) enabled = false;
        }

        private void FixedUpdate()
        {
            if (!IsServer) return;
            if (ServerManager.Instance == null || ServerManager.Instance.ActiveGame == null) return;
            foreach (Player player in ServerManager.Instance.ActiveGame.PlayerGameInfoTable.Values)
            {
                if (player == null) continue;
                ShipController? controller = player.controller;
                if (controller == null) continue;
                ApplyForce(controller);
            }
        }

        private void ApplyForce(ShipController controller)
        {
            Vector2 pos = controller.Position;
            Vector2 force = Vector2.zero;
            float distanceLeft = Boundary.min.x - pos.x;
            if (distanceLeft > 0)
            {
                force.x += ForcePerDistance * distanceLeft;
            }
            float distanceRight = pos.x - Boundary.max.x;
            if (distanceRight > 0)
            {
                force.x -= ForcePerDistance * distanceRight;
            }
            float distanceDown = Boundary.min.y - pos.y;
            if (distanceDown > 0)
            {
                force.y += ForcePerDistance * distanceDown;
            }
            float distanceUp = pos.y - Boundary.max.y;
            if (distanceUp > 0)
            {
                force.y -= ForcePerDistance * distanceUp;
            }
            controller.Push(force);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + Boundary.center, Boundary.size);
        }
    }
}
