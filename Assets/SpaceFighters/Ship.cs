using Unity.Netcode;
using SpaceFighters.Server;
using UnityEngine;
using System.Collections.Generic;

#nullable enable

namespace SpaceFighters
{
    public class Ship : NetworkBehaviour, IDamagable, IShooter
    {
        public delegate void OnDeathDelegate();

        public float MaxHealth;

        [SerializeField] private List<Collider2D> HurtBoxes = new();

        [HideInInspector] public NetworkVariable<float> CurrentHealth = new();

        public event OnDeathDelegate? OnDeath;

        public override void OnNetworkSpawn()
        {
            OnShipSpawn();
        }

        public void OnShipSpawn()
        {
            CurrentHealth.Value = MaxHealth;
        }

        public void ReceiveDamage(DamageInfo damage)
        {
            float currentHp = CurrentHealth.Value;
            currentHp -= damage.Amount;
            if (currentHp <= 0)
            {
                currentHp = 0;
            }   
            CurrentHealth.Value = currentHp;
            if (currentHp <= 0)
            {
                Death();
            }
        }

        public void Death()
        {
            NetworkObject.Despawn();
            OnDeath?.Invoke();
        }

        public IEnumerable<Collider2D> GetHurtboxes()
        {
            return HurtBoxes;
        }
    }
}
