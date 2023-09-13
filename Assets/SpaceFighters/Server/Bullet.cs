using System.Collections;
using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private float Speed;
        [SerializeField] private float Lifetime;

        [SerializeField] private Rigidbody2D? MainRigidbody;

        public override void OnNetworkSpawn()
        {
            if (MainRigidbody == null) return;
            MainRigidbody.velocity = transform.up * Speed;
            if (IsServer)
            {
                StartCoroutine(LifetimeCoroutine(Lifetime));
            }
        }

        private IEnumerator LifetimeCoroutine(float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            NetworkObject.Despawn();
        }

    }
}
