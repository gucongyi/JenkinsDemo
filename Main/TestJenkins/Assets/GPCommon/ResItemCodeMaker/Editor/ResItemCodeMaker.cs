using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GPCommon
{
    public class ResItemCodeMaker : CodeBuilder
    {
        public interface IResItemMakerTask
        {
            string FolderFilePath { get; }
            string CodeFilePath { get; }
            string GetResPath(string itemFilePath);
            void OnEachItemCreated(string itemName, string itemFilePath, int currentIndex, int maxIndex);
        }

        public ResItemCodeMaker(IResItemMakerTask task)
        {
            // Get item name list
            List<string>
                pathList = Directory.GetFiles(task.FolderFilePath, "*", SearchOption.AllDirectories)
                    .ToList(); // All path

            List<string>
                itemFilePathList = pathList.Where(p => p.Split('.').Last() != "meta").ToList(); // Ignore meta data
            List<string> nameList = itemFilePathList.Select(Path.GetFileNameWithoutExtension).ToList();
            string fileName = Path.GetFileNameWithoutExtension(task.CodeFilePath);

            // Check if name legal and duplicated
            ValidataNameList(nameList);

            // Generic opening code
            AppendFormat("// Auto-generated code, do not edit. {0}", DateTimeUtils.StandardTimeStr);
            AppendFormat("// From: {0}", CommonUtils.GetUnityAssetsPath(task.FolderFilePath));

            AppendLine("using GPCommon;");

            LineFeed();

            WrapBracket(string.Format("public class {0} : {1}", fileName, typeof(IResCode).Name), () =>
            {
                // Item name
                foreach (string t in nameList)
                {
                    AppendFormat("\tpublic const string {0} = \"{0}\";", t);
                }

                LineFeed();
                LineFeed();

                WrapBracket("public string GetResPath(string name)", () =>
                {
                    WrapBracket("switch (name)", () =>
                    {
                        for (var i = 0; i < itemFilePathList.Count; i++)
                        {
                            string itemFilePath = itemFilePathList[i];

                            string itemName = Path.GetFileNameWithoutExtension(itemFilePath);

                            string resPath = task.GetResPath(itemFilePath);

                            AppendFormat("case {0}: return \"{1}\";",
                                itemName, resPath);

                            task.OnEachItemCreated(itemName, itemFilePath, i, itemFilePathList.Count - 1);
                        }
                    });

                    AppendLine("return null;");
                });

                WrapBracket("public string ResRoot",
                    () => { AppendFormat("get{{ return \"{0}\"; }}", task.GetResPath(task.FolderFilePath)); });
            });
        }

        private void ValidataNameList(List<string> nameList)
        {
            foreach (string name in nameList)
            {
                if (!EditorHelper.IsLegalVariableName(name))
                {
                    throw new System.Exception(name + " is not a legal variable name");
                }
            }

            string d;
            if (CommonUtils.CheckDuplicated<string>(nameList, out d))
            {
                throw new System.Exception(d + " is duplicated");
            }
        }
    }
}