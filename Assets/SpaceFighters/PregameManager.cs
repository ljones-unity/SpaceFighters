using PlasticGui;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable

namespace SpaceFighters
{
    public class PregameManager : MonoBehaviour
    {
        public static PregameManager? Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public async Task<bool> SignIn()
        {
            // TODO: Implement actual signing in
            await Task.Delay(3000);
            return true;
        }

        public async Task<bool> FindMatch()
        {
            await Task.Delay(3000);
            return true;
        }

        public void StartGame()
        {
            SceneManager.LoadScene("Battleground", LoadSceneMode.Single);
        }
    }
}
