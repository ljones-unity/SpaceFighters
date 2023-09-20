using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

#nullable enable

namespace SpaceFighters
{
    public static class UnityServicesHelper
    {
        public static async Task<bool> SafeInitializeAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Unity Services failed to initialize: {e.Message}");
                return false;
            }
        }

        public static async Task<bool> EnsureInitialized()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                return true;
            }
            else return await SafeInitializeAsync();
        }
    }
}
