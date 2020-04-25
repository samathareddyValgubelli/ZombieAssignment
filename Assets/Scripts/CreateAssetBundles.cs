#if UNITY_EDITOR

using UnityEditor;
using System.IO;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundlePath = Application.streamingAssetsPath + "/AssetBundles";

        //if (!Directory.Exists(assetBundlePath))
        //{
        //    Directory.CreateDirectory(assetBundlePath);
        //}
        BuildPipeline.BuildAssetBundles(assetBundlePath,
                                        BuildAssetBundleOptions.None,
                                        BuildTarget.StandaloneWindows);
    }
}
#endif
