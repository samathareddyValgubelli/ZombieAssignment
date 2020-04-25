using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AssetBundleDownloadHandler : MonoBehaviour
{
    public Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

    public Dictionary<string, string> unityAssets = new Dictionary<string, string>(); // Dictionary<string, Bundle> bundlesInformation = new Dictionary<string, Bundle> ();

    public const string NoInterenet = "NoInternet";

    public const string ServerError = "ServerError";

    public const string Success = "Success";


    #region Singleton

    private static AssetBundleDownloadHandler instance = null;

    public static AssetBundleDownloadHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AssetBundleDownloadHandler>();
            }
            return instance;
        }
    }
    #endregion

    private void Awake()
    {
        unityAssets.Add("test unity asset12", "https://alpha.socialkinesis.com/data/unityAssets/novellus-test-project/5e6f7bf303c55.unity3d");

        //if (unityAssets != null && unityAssets.Count > 0)
        //{
        //    foreach (KeyValuePair<string, string> bundle in unityAssets)
        //    {
        //        StartCoroutine(LoadBundle(bundle.Key, (string s) =>
        //        {
        //            Debug.Log("..." + s);

        //            string[] scenes = assetBundles["test unity asset12"].GetAllScenePaths();

        //            SceneManager.LoadScene(scenes[0]);

        //        }));
        //    }
        //}
    }

    public int assetsCount;

    public void DownloadAssetBundles(System.Action<string> action)
    {
        if (unityAssets != null && unityAssets.Count > 0)
        {
            assetsCount = 0;

            foreach (KeyValuePair<string, string> bundle in unityAssets)
            {
                //StartCoroutine(LoadBundle(bundle.Key,action));

                StartCoroutine(LoadBundle(bundle.Key, (string s) =>
                {
                    assetsCount++;

                    action?.Invoke(s);
                }));
            }
        }
    }

    public IEnumerator LoadBundle(string bundleName,System.Action<string> action)
    {        
        if(!IsBundleLoaded(bundleName))
        {
            string url = unityAssets[bundleName];

            using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {

                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log("Code : " + request.responseCode);

                    if (request.responseCode >= 400)
                    {
                        action?.Invoke(NoInterenet);
                    }
                    else if (request.responseCode >= 500)
                    {
                        action?.Invoke(ServerError);
                    }
                    else
                    {
                        action?.Invoke(NoInterenet);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(request.error))
                    {
                        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

                        if (bundle != null)
                        {
                            assetBundles.Add(bundleName, bundle);

                            action?.Invoke(Success);
                        }
                        else
                        {
                            Debug.Log("...." + request.error);

                            Debug.Log(request.downloadHandler.text);
                        }
                    }
                }
            }
        }
        
    }
    public void UnloadAllBundles()
    {
        foreach (KeyValuePair<string, AssetBundle> entry in assetBundles)
        {
            if (entry.Value != null)
            {
                entry.Value.Unload(true);
            }
        }
    }

    void OnDisable()
    {
        UnloadAllBundles();
    }

    public bool IsBundleLoaded(string bundleName)
    {
        if (IsBundleCached(bundleName))
        {
            return (assetBundles[bundleName] != null);
        }
        return false;
    }

    public bool IsBundleCached(string bundleName)
    {
        return assetBundles.ContainsKey(bundleName);
    }
}
