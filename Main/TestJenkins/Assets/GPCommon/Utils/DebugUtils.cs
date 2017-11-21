using System;
using System.Collections;
using System.Collections.Generic;

namespace GPCommon
{
    public static class DebugUtils
    {
        public static string ToDebuggingStr(this IEnumerable enumerable)
        {
            var strList = new List<string>();
            foreach (var item in enumerable)
            {
                strList.Add(item.ToString());
            }

            return string.Format("[{0}]", String.Join(", ", strList.ToArray()));
        }
    }
}
