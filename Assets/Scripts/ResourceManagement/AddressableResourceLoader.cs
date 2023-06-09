using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

/// <summary>
/// Class for load resources by addressable assets
/// </summary>
public class AddressableResourceLoader : IResourceLoader
{
    /// <summary>
    /// Initialize Addressables assets
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        AsyncOperationHandle<IResourceLocator> initialize = Addressables.InitializeAsync();
        await initialize.Task;
        Debug.Log("ResourceLoader initialized");
    }

    /// <summary>
    /// Preload assets to memories
    /// </summary>
    /// <param name="assetKeys"></param>
    /// <returns></returns>
    public async Task Preload(IEnumerable<string> assetKeys)
    {
        List<Task> tasks = new List<Task>();
        foreach (string key in assetKeys)
        {
            tasks.Add(Load<System.Object>(key));
        }
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Load assets from addressable assets key
    /// </summary>
    /// <param name="key">addressable assets key</param>
    /// <typeparam name="T">resource type</typeparam>
    /// <returns></returns>
    public async Task<T> Load<T>(string key)
    {
        AsyncOperationHandle<T> loadAssetAsync = Addressables.LoadAssetAsync<T>(key);
        loadAssetAsync.Completed += (operation) =>
        {
            if (operation.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to load asset : [{key}]");
            }
        };
        T loadedAsset = await loadAssetAsync.Task;
        return loadedAsset;
    }

    /// <summary>
    /// Load prefab and get type
    /// </summary>
    /// <param name="key">addressable assets key</param>
    /// <typeparam name="T">script type attach to prefab</typeparam>
    /// <returns></returns>
    public async Task<T> LoadPrefab<T>(string key)
    {
        GameObject loadedPrefab = await Load<GameObject>(key);
        T component = loadedPrefab.GetComponent<T>();
        return component;
    }

    public async Task<T> InstantiateAsync<T>(object key, Transform parent = null, bool instantiateInWorldSpace = false, bool trackHandle = true) where T : MonoBehaviour
    {
        AsyncOperationHandle<GameObject> loadAssetAsync = Addressables.InstantiateAsync(key, parent);
        loadAssetAsync.Completed += (operation) =>
        {
            if (operation.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to instantiate asset : [key]");
            }
        };
        GameObject instantiatedObject = await loadAssetAsync.Task;
        T component = instantiatedObject.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"Failed to get component [{typeof(T).Name}] from instantiate object");
        }
        return component;
    }

    public async Task<T> LoadJsonAsModel<T>(string key)
    {
        TextAsset textAsset = await Load<TextAsset>(key);
        T model = JsonUtility.FromJson<T>(textAsset.text);
        return model;
    }

    public async Task SetSpriteAsync(string key, SetSpriteHandler spriteHandler)
    {
        Sprite sprite = await Load<Sprite>(key);
        spriteHandler?.Invoke(sprite);
    }

    /// <summary>
    /// Release assets from reference
    /// </summary>
    /// <param name="key"></param>
    public void Release(System.Object obj)
    {
        Addressables.Release(obj);
    }

    /// <summary>
    /// Release instance which created from InstantiateAsync
    /// </summary>
    /// <param name="key"></param>
    public void ReleaseInstance(GameObject obj)
    {
        Addressables.ReleaseInstance(obj);
    }
}