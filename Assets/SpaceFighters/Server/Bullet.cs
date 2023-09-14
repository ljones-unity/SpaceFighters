using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private float Speed;
        [SerializeField] private float Lifetime;
        [SerializeField] private float Damage;

        [SerializeField] private Rigidbody2D? MainRigidbody;
        [SerializeField] private List<Collider2D> HitBoxes = new();

        private IShooter? shooter;

        public void Shoot(IShooter shooter)
        {
            this.shooter = shooter;
            foreach (Collider2D hurtbox in shooter.GetHurtboxes())
            {
                foreach (Collider2D hitbox in HitBoxes)
                {
                    Physics2D.IgnoreCollision(hurtbox, hitbox);
                }
            }

            if (MainRigidbody == null) return;
            MainRigidbody.velocity = transform.up * Speed;
            if (IsServer)
            {
                StartCoroutine(LifetimeCoroutine(Lifetime));
            }
        }

        public void DealDamage(IDamagable damagable)
        {
            if (!IsServer) return;
            if (shooter == null) return;
            DamageInfo damageInfo = new DamageInfo
            {
                Amount = Damage,
                Shooter = shooter,
            };
            damagable.ReceiveDamage(damageInfo);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsServer) return;
            if (!IsSpawned) return;
            Debug.Log("Collision Detected");
            IDamagable? damagable = collision.GetComponentInParent<IDamagable>();
            if (damagable == null) return;
            DealDamage(damagable);
            NetworkObject.Despawn();
        }

        private IEnumerator LifetimeCoroutine(float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            NetworkObject.Despawn();
        }
    }

    public interface IShooter
    {
        public IEnumerable<Collider2D> GetHurtboxes();
    }
}
