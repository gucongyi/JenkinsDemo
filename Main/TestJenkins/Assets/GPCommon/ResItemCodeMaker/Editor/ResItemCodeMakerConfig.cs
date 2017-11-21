using System.Collections.Generic;
using UnityEngine;

namespace GPCommon
{
    public class ResItemCodeMakerConfig : ScriptableObject
    {
        [System.Serializable]
        public class CreatorTask : ResItemCodeMaker.IResItemMakerTask
        {
            [SerializeField]
            public string mFolderPath;

            [SerializeField]
            public string mCodePath;

            public string FolderFilePath
            {
                get { return CommonUtils.GetFullPath(mFolderPath); }
                set { mFolderPath = CommonUtils.GetUnityAssetsPath(value); }
            }

            public string CodeFilePath
            {
                get { return CommonUtils.GetFullPath(mCodePath); }
                set { mCodePath = CommonUtils.GetUnityAssetsPath(value); }
            }

            public string GetResPath(string itemFilePath)
            {
                return CommonUtils.GetResPath(itemFilePath);
            }

            public void OnEachItemCreated(string itemName, string itemFilePath, int currentIndex, int maxIndex)
            {
            }
        }

        public List<CreatorTask> taskList = new List<CreatorTask>();
    }
}