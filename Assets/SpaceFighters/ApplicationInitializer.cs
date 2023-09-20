using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

namespace SpaceFighters
{
    public class ApplicationInitializer : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_SERVER
            SceneManager.LoadScene("Battleground", LoadSceneMode.Single);
#else
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
#endif
        }
    }
}
