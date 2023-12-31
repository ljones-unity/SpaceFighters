﻿using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace SpaceFighters.Client
{
    public class FindMatchUI : MonoBehaviour
    {
        [SerializeField] private Button? FindMatchButton;
        [SerializeField] private LoadingIndicatorUI? LoadingIndicator;

        private void OnEnable()
        {
            if (FindMatchButton == null) return;
            FindMatchButton.onClick.AddListener(OnFindMatchButtonPressed);
        }

        private void OnDisable()
        {
            if (FindMatchButton == null) return;
            FindMatchButton.onClick.RemoveListener(OnFindMatchButtonPressed);
        }

        private async void OnFindMatchButtonPressed()
        {
            if (PregameManager.Instance == null) return;
            FindMatchButton?.gameObject.SetActive(false);
            LoadingIndicator?.SetLoadingText("Finding match...");
            LoadingIndicator?.gameObject.SetActive(true);
            bool result = await PregameManager.Instance.FindMatchAsync();
            LoadingIndicator?.gameObject.SetActive(false);
            if (result)
            {
                gameObject.SetActive(false);
                PregameManager.Instance.StartGame();
            }
            else
            {
                FindMatchButton?.gameObject.SetActive(true);
            }
        }
    }
}
