using Unity.Netcode;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float Acceleration;
        [SerializeField] private float Speed;
        [SerializeField] private float AngularAcceleration;
        [SerializeField] private float AngularSpeed;

        [SerializeField] private float ShootCooldown;

        [SerializeField] Rigidbody2D? MainRigidbody;
        [SerializeField] Bullet? BulletPrefab;

        private float lastShotTime;

        public void Move(Vector2 input, float dt)
        {
            if (!IsServer) return;
            if (MainRigidbody == null) return;
            Vector2 velocity = MainRigidbody.velocity;
            float angularVelocity = MainRigidbody.angularVelocity;
            velocity += (Vector2)MainRigidbody.transform.up * Acceleration * input.y * dt;
            velocity = Vector2.ClampMagnitude(velocity, Speed);
            angularVelocity += AngularAcceleration * input.x * dt;
            angularVelocity = Mathf.Clamp(angularVelocity, -AngularSpeed, AngularSpeed);
            MainRigidbody.velocity = velocity;
            MainRigidbody.angularVelocity = angularVelocity;
        }

        public void Shoot()
        {
            if (!IsServer) return;
            if (lastShotTime + ShootCooldown > Time.time) return;
            Bullet? bullet = Instantiate(BulletPrefab, transform.position, transform.rotation);
            if (bullet == null) return;
            bullet.NetworkObject.Spawn();
            lastShotTime = Time.time;
        }
    }
}
