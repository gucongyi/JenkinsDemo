using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GPCommon
{
    public static class CommonUtils
    {
        public static bool CheckClassExist(string className, string nameSpace)
        {
            var type = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from t in assembly.GetTypes()
                where t.Name == className && t.Namespace == nameSpace
                select t).FirstOrDefault();

            return type != null;
        }

        public static bool CheckDuplicated<T>(List<T> source, out T item)
        {
            foreach (T t in source)
            {
                if (source.FindAll(s => s.Equals(t)).Count > 1)
                {
                    item = t;
                    return true;
                }
            }

            item = default(T);

            return false;
        }

        public static float TimeMeasure(Action doSomething)
        {
            var start = DateTime.Now;
            doSomething();
            var end = DateTime.Now;
            var span = end - start;
            var milliSeconds = span.TotalMilliseconds;

            return (float) milliSeconds;
        }

        public static Dictionary<Y, T> SwapKeyValue<T, Y>(Dictionary<T, Y> source)
        {
            var result = new Dictionary<Y, T>();
            source.Keys.ToList<T>().ForEach(k => result.Add(source[k], k));
            return result;
        }

        /// <summary>
        /// Create a list which contains from start to end integers
        /// </summary>
        /// <param name="start">inclusive</param>
        /// <param name="end">exclusive</param>
        /// <returns></returns>
        public static List<int> Range(int start, int end)
        {
            var list = new List<int>();

            for (var i = start; i < end; i++)
                list.Add(i);

            return list;
        }

        #region Path & File

        /// <summary>
        ///  计算指定文件的MD5值
        /// </summary>
        /// <param name="fileName">指定文件的完全限定名称</param>
        /// <returns>返回值的字符串形式</returns>
        public static String ComputeMD5(String fileName)
        {
            String hashMD5 = String.Empty;
            //检查文件是否存在，如果文件存在则进行计算，否则返回空值
            if (System.IO.File.Exists(fileName))
            {
                using (System.IO.FileStream fs =
                    new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    //计算文件的MD5值
                    MD5 calculator = MD5.Create();
                    Byte[] buffer = calculator.ComputeHash(fs);
                    calculator.Clear();
                    //将字节数组转换成十六进制的字符串形式
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        stringBuilder.Append(buffer[i].ToString("x2"));
                    }
                    hashMD5 = stringBuilder.ToString();
                } //关闭文件流
            } //结束计算
            return hashMD5;
        }

        /// <summary>
        /// 计算bytes的MD5值
        /// <returns>返回值的字符串形式</returns>
        /// <summary>
        public static String ComputeMD5(byte[] b)
        {
            StringBuilder sb = new StringBuilder();

            byte[] source = MD5.Create().ComputeHash(b);
            for (int i = 0; i < source.Length; i++)
            {
                sb.Append(source[i].ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 返回一个能被Resources.Load处理的路径
        /// </summary>s
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetResPath(string filePath)
        {
            string keyword = "Resources";
            int relativePathStartIndex = filePath.IndexOf(keyword, StringComparison.Ordinal);

            // Note that it doesn't include 'Resources' keyword
            string relativePath = filePath.Substring(relativePathStartIndex + keyword.Length + 1);

            int i = relativePath.IndexOf('.');
            string withoutExtension = i != -1 ? relativePath.Substring(0, i) : relativePath;

            string result = withoutExtension.Replace('\\', '/');
            return result;
        }

        /// <summary>
        /// 返回父文件夹名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetParentFolder(string path)
        {
            var pathFragment = path.Split('/', '\\');
            return pathFragment.Length >= 2 ? pathFragment[pathFragment.Length - 2] : null;
        }

        /// <summary>
        /// 返回一个Assets下的路径，能直接被编辑器脚本使用
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetUnityAssetsPath(string filePath)
        {
            int relativePathStartIndex = filePath.IndexOf("Assets", StringComparison.Ordinal);
            string relativePath = filePath.Substring(relativePathStartIndex);

            string result = relativePath.Replace('\\', '/');

            return result;
        }

        /// <summary>
        /// 返回绝对路径，能直接被System.IO的API处理
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetFullPath(string assetPath)
        {
            string currentDirectory = Environment.CurrentDirectory;
            currentDirectory = currentDirectory.Replace('\\', '/');
            string fullPath = currentDirectory + "/" + assetPath;
            return fullPath;
        }

        public static string GetFileName(string path)
        {
            return path.Split('/').ToList().Last();
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            var fileName = GetFileName(path);

            return fileName.Split('.').ToList().First();
        }

        #endregion

        #region Random

        private static System.Random random = new System.Random((int) DateTime.Now.Ticks);

        public static bool RandomHit(float percent)
        {
            return UnityEngine.Random.Range(0f, 1f) < percent;
        }

        public static string RandomString(int size)
        {
            StringBuilder builder = new System.Text.StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static int RandomInteger(int minValue = 1, int maxValue = 10000)
        {
            return random.Next(minValue, maxValue);
        }

        #endregion

        #region String Format

        private static readonly string[] SizeSuffixes =
            {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }

            int i = 0;
            decimal dValue = (decimal) value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }

        #endregion
    }
}