using System;
using UnityEngine;
using System.Collections;
using System.Text;

namespace GPCommon
{
    public class CodeBuilder
    {
        protected StringBuilder sb = new StringBuilder();

        public void AppendFormat(string s, params object[] args)
        {
            sb.AppendLine(string.Format(s, args));
        }

        public void WrapBracket(string title, Action action)
        {
            AppendLine(title);
            AppendLine("{");
            action();
            AppendLine("}");
        }

        public void LineFeed()
        {
            sb.AppendLine("\n");
        }

        public void AppendLine(string s)
        {
            sb.AppendLine(s);
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
