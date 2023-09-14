using UnityEngine;
using UnityEngine.UI;

#nullable enable

namespace SpaceFighters
{
    public class SignInUI : MonoBehaviour
    {
        [SerializeField] private Button? SignInButton;
        [SerializeField] private LoadingIndicatorUI? LoadingIndicator;
        [SerializeField] private GameObject? FindGameUI;

        private void OnEnable()
        {
            if (SignInButton == null) return;
            SignInButton.onClick.AddListener(OnSignInButtonPressed);
        }

        private void OnDisable()
        {
            if (SignInButton == null) return;
            SignInButton.onClick.RemoveListener(OnSignInButtonPressed);
        }

        private async void OnSignInButtonPressed()
        {
            if (PregameManager.Instance == null) return;
            SignInButton?.gameObject.SetActive(false);
            LoadingIndicator?.SetLoadingText("Signing in...");
            LoadingIndicator?.gameObject.SetActive(true);
            bool result = await PregameManager.Instance.SignIn();
            LoadingIndicator?.gameObject.SetActive(false);
            if (result)
            {
                FindGameUI?.SetActive(true);
                SignInButton?.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Sign in failed");
                SignInButton?.gameObject.SetActive(true);
            }
        }
    }
}
