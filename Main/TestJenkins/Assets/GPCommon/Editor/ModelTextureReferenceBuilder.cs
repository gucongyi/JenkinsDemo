using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GPCommon
{
    public class ModelTextureReferenceBuilder
    {
        internal class ModelTextureBuilderTask : ResItemCodeMaker.IResItemMakerTask
        {
            private const string ItemCodePath = "Assets/Scripts/ResourcesManager/ModelTextureReferences.cs";
            private const string ItemFolderPath = "Assets/StaticAssets/CJ/Texture/";
            internal const string ResFolderPath = "Resources/ModelTexture/";

            public string FolderFilePath
            {
                get { return CommonUtils.GetFullPath(ItemFolderPath); }
            }

            public string CodeFilePath
            {
                get { return CommonUtils.GetFullPath(ItemCodePath); }
            }

            public string GetResPath(string itemFilePath)
            {
                var t = itemFilePath.Replace(ItemFolderPath, ResFolderPath);
                var resPath = CommonUtils.GetResPath(t);
                return resPath;
            }

            public void OnEachItemCreated(string itemName, string itemFilePath, int currentIndex, int maxIndex)
            {

                // Prepare path
                var assetPath = CommonUtils.GetUnityAssetsPath(itemFilePath);
                var prefabAssetPath = string.Format("Assets/Resources/{0}.prefab", GetResPath(itemFilePath));

                // Prepare object by path
                var texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
                var prefab =  AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);


                // process prefab stuff
                if (prefab != null)
                {
                    // Repalce sprite
                    if (prefab.GetComponent<TextureReference>().Texture != texture)
                    {
                        prefab.GetComponent<TextureReference>().Texture = texture;
                        AssetDatabase.SaveAssets();

                        Debug.Log(prefabAssetPath + " Replaced");
                    }
                }
                else
                {
                    var go = new GameObject(itemName);
                    go.AddComponent<TextureReference>().Texture = texture;

                    EditorHelper.PreparePathDirectory(prefabAssetPath);

                    PrefabUtility.CreatePrefab(prefabAssetPath, go);

                    Debug.Log(prefabAssetPath + " Created");

                    Object.DestroyImmediate(go);
                }

                // Display progress infomation
                if (currentIndex == maxIndex)
                {
                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    EditorUtility.DisplayProgressBar("UpdateModelTextureReference",
                        string.Format("{0} {1}/{2}", itemName, currentIndex, maxIndex),
                        currentIndex / (float) maxIndex);
                }
            }
        }

        [MenuItem("Assets/GPCommon-AtlasSprite and ModelTexture/UpdateModelTextureReference-Full")]
        public static void BuildAtlasSpriteReferenceFull()
        {
            Debug.Log("GPCommon-UpdateModelTextureReference-Full start");

            // Delete previous prefab
            FileUtil.DeleteFileOrDirectory("Assets/" + ModelTextureBuilderTask.ResFolderPath);

            UpdateModelTextureReference();

            Debug.Log("GPCommon-UpdateModelTextureReference-Full complete");
        }

        [MenuItem("Assets/GPCommon-AtlasSprite and ModelTexture/UpdateModelTextureReference-Quick")]
        public static void BuildAtlasSpriteReferenceQuick()
        {
            Debug.Log("GPCommon-UpdateModelTextureReference-Quick start");

            UpdateModelTextureReference();

            Debug.Log("GPCommon-UpdateModelTextureReference-Quick complete");
        }

        private static void UpdateModelTextureReference()
        {
            // Create new prefab when item code building
            var task = new ModelTextureBuilderTask();
            var codeBuilder = new ResItemCodeMaker(task);

            // Write file
            if (!File.Exists(task.CodeFilePath))
            {
                File.Create(task.CodeFilePath);

                Save();
            }

            File.WriteAllText(task.CodeFilePath, codeBuilder.ToString(), Encoding.UTF8);

            Save();
        }

        private static void Save()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}