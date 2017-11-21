using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GPCommon
{
    public class AtlasSpriteReferenceBuilder
    {
        internal class AtlasSpriteCodeBuilderTask : ResItemCodeMaker.IResItemMakerTask
        {
            private const string ItemCodePath = "Assets/Scripts/ResourcesManager/SpriteAtlasReferences.cs";
            private const string ItemFolderPath = "Assets/StaticAssets/AtlasSources/";
            internal const string ResFolderPath = "Resources/AtlasSpriteReference/";

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
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                var importer = (TextureImporter) AssetImporter.GetAtPath(assetPath);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);

                var tag = CommonUtils.GetParentFolder(itemFilePath);

                // Process sprite importer stuff
                if (importer != null && (importer.spritePackingTag != tag || importer.mipmapEnabled))
                {
                    importer.spritePackingTag = tag;
                    importer.mipmapEnabled = false;
                    importer.SaveAndReimport();
                }

                // process prefab stuff
                if (prefab != null)
                {
                    // Repalce sprite
                    if (prefab.GetComponent<SpriteRenderer>().sprite != sprite)
                    {
                        prefab.GetComponent<SpriteRenderer>().sprite = sprite;
                        AssetDatabase.SaveAssets();

                        Debug.Log(prefabAssetPath + " Replaced");
                    }
                }
                else
                {
                    var go = new GameObject(itemName);
                    go.AddComponent<SpriteRenderer>().sprite = sprite;

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
                    EditorUtility.DisplayProgressBar("UpdateAtlasSpriteReference",
                        string.Format("{0} {1}/{2}", itemName, currentIndex, maxIndex),
                        currentIndex / (float) maxIndex);
                }
            }
        }

        [MenuItem("Assets/GPCommon-AtlasSprite and ModelTexture/UpdateAtlasSpriteReference-Full")]
        public static void BuildAtlasSpriteReferenceFull()
        {
            Debug.Log("GPCommon-UpdateAtlasSpriteReference-Full start");

            // Delete previous prefab
            FileUtil.DeleteFileOrDirectory("Assets/" + AtlasSpriteCodeBuilderTask.ResFolderPath);

            UpdateSpriteReference();

            Debug.Log("GPCommon-UpdateAtlasSpriteReference-Full complete");
        }

        [MenuItem("Assets/GPCommon-AtlasSprite and ModelTexture/UpdateAtlasSpriteReference-Quick")]
        public static void BuildAtlasSpriteReferenceQuick()
        {
            Debug.Log("GPCommon-UpdateAtlasSpriteReference-Quick start");

            UpdateSpriteReference();

            Debug.Log("GPCommon-UpdateAtlasSpriteReference-Quick complete");
        }

        private static void UpdateSpriteReference()
        {
            // Create new prefab when item code building
            var task = new AtlasSpriteCodeBuilderTask();
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