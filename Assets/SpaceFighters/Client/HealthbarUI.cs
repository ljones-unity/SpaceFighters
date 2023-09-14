using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace SpaceFighters.Client
{
    public class HealthbarUI : NetworkBehaviour
    {
        [SerializeField] private Ship? ship;

        [SerializeField] private Transform? healthbar;
        [SerializeField] private Image? healthbarImage;
        [SerializeField] private Color AllyColor;
        [SerializeField] private Color EnemyColor;

        public override void OnNetworkSpawn()
        {
            if (ship == null) return;
            ship.CurrentHealth.OnValueChanged += OnHealthChanged;
            if (healthbarImage == null) return;
            if (IsOwner) healthbarImage.color = AllyColor;
            else healthbarImage.color = EnemyColor;
        }

        private void OnHealthChanged(float previousValue, float newValue)
        {
            if (healthbar == null) return;
            if (ship == null) return;
            healthbar.localScale = new Vector3(newValue / ship.MaxHealth, 1, 1);
        }
    }
}
