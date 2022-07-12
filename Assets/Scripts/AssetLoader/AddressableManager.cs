using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager : MonoBehaviour
{
    [SerializeField]
    private AssetReference RoomPrefabAsset;

    private GameObject roomObject;
    // Start is called before the first frame update
    void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        //load the room asset
        Addressables.InitializeAsync().Completed += AddressableManager_Completed;

    }
    public List<GameObject> InstantiatedObjects = new List<GameObject>();
    private void AddressableManager_Completed(AsyncOperationHandle<IResourceLocator> obj)
    {
        
        AsyncOperationHandle<GameObject> loadOp = RoomPrefabAsset.LoadAssetAsync<GameObject>();
        loadOp.Completed += (go) =>
        {
            roomObject = go.Result;
            for (int i = 0; i < 5; ++i)
            {
                // Using the plain old Instantiate will not be tracked by ResourceManager
                // it may release the asset before proper destruction of the gameObjects,
                // which will cause missing reference errors.
                InstantiatedObjects.Add(Instantiate(roomObject));
            }
        };

        // the other method is to use the InstantiateAsync method.
        // look at the documentation
        // https://docs.unity3d.com/Packages/com.unity.addressables@1.15/manual/InstantiateAsync.html
    }

    private void Update()
    {

    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "destroy"))
        {
            if(roomObject != null)
            {
                // this will remove the roomObject cache.
                // you can see this code's effect on the Addressable assets profiler window.
                RoomPrefabAsset.ReleaseInstance(roomObject);
                Resources.UnloadUnusedAssets();
                roomObject = null;
            }


            GameObject go = InstantiatedObjects[InstantiatedObjects.Count - 1];
            InstantiatedObjects.Remove(go);
            Destroy(go);
        }
    }
    void CustomExceptionHandler(AsyncOperationHandle handle, Exception exception)
    {
        if (exception.GetType() != typeof(InvalidKeyException))
            Addressables.LogException(handle, exception);
    }

    private void OnDestroy()
    {
        //RoomPrefabAsset.ReleaseInstance(roomObject);
        
    }
}
