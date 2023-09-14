using UnityEngine;

#nullable enable

namespace SpaceFighters.Server
{
    public class Player : MonoBehaviour
    {
        public ShipController? controller;
        public Ship? ship;

        [HideInInspector] public ulong clientId;
    }
}
