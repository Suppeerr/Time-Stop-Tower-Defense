using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public static class AddressableLoader
{
    // Allows any addressable asset to be loaded into any script 
    public static async Task<T> GetAsset<T>(string address) where T : UnityEngine.Object
    {
        T asset = await Addressables.LoadAssetAsync<T>(address).Task;

        if (asset == null)
        {
            Debug.LogError("Failed to load asset at address: " + address);
        }

        return asset;
    }
}