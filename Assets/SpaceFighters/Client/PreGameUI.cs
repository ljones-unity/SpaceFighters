using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Client
{
    public class PreGameUI : MonoBehaviour
    {
        [SerializeField] private SignInUI? SignInUI;
        [SerializeField] private LoadingIndicatorUI? LoadingIndicator;

        private void OnEnable()
        {
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            if (LoadingIndicator == null) return;
            if (SignInUI == null) return;

            LoadingIndicator.gameObject.SetActive(true);
            LoadingIndicator.SetLoadingText("Initializing Services...");
            await UnityServices.InitializeAsync();
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.LogError("Failed to initialize Unity Services");
                return;
            }
            LoadingIndicator.gameObject.SetActive(false);
            SignInUI.gameObject.SetActive(true);
        }
    }
}
