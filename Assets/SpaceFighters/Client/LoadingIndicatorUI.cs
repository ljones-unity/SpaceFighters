using System.Collections;
using TMPro;
using UnityEngine;

#nullable enable

namespace SpaceFighters.Client
{
    public class LoadingIndicatorUI : MonoBehaviour
    {
        [SerializeField] private GameObject? LoadingIndicator;
        [SerializeField] private TextMeshProUGUI? LoadingText;
        [SerializeField] private float RotationSpeed;

        private void OnEnable()
        {
            StartCoroutine(RotateIndicator(RotationSpeed));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            if (LoadingIndicator == null) return;
            LoadingIndicator.transform.rotation = Quaternion.identity;
        }

        public void SetLoadingText(string text)
        {
            if (LoadingText == null) return;
            LoadingText.text = text;
        }

        private IEnumerator RotateIndicator(float speed)
        {
            if (LoadingIndicator == null) yield break;
            float currentRotation = 0;
            while (true)
            {
                currentRotation += speed * Time.deltaTime;
                LoadingIndicator.transform.rotation = Quaternion.Euler(0, 0, -currentRotation);
                yield return null;
            }
        }
    }
}
