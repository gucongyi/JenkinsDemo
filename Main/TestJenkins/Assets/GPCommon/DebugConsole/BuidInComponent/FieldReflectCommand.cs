using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GPCommon
{
    public interface IGameGlobalObjectFinder
    {
        object Find(string name);
    }

    /// <summary>
    /// ref [name] [field]
    /// Example: ref hero hp, print 'hp' field attribute of object named 'hero' in finder
    /// White list in Assets/link
    /// </summary>
    public class FieldReflectCommand : ConsoleConmand
    {
#if DEBUG_CONSOLE
        public static string ID = "ref";
#endif

        private IGameGlobalObjectFinder finder;

        public FieldReflectCommand()
        {
#if DEBUG_CONSOLE
            Id = ID;
#endif
            callback = Callback;
        }

        public void SetFinder(IGameGlobalObjectFinder p_finder)
        {
            finder = p_finder;
        }

        private void Callback(List<string> pars)
        {
            if (finder == null)
            {
                Watchdog.LogError(GetType().ToString(), "Finder has not been assigned!");
            }
            else
            {
                if (pars == null || pars.Count != 2)
                {
                    Watchdog.LogError(GetType().ToString(), "Parameter count incorrect");
                }
                else
                {

                    Queue<string> fieldQueue = new Queue<string>(pars[0].Split('.').ToList());

                    // First object supported by finder
                    string firstField = fieldQueue.Dequeue();
                    object curObj = finder.Find(firstField);

                    if (curObj == null)
                    {
                        Watchdog.LogError(GetType().ToString(), string.Format("{0} doesn't implemented in finder", firstField));
                    }
                    else
                    {
                        // Support dot hierarchy
                        FieldInfo fieldInfo = null;


                        // Parse sub-obj iteratively
                        while (fieldQueue.Count > 0)
                        {
                            fieldInfo = GetFieldInfo(curObj, fieldQueue.Dequeue());
                            if (fieldInfo == null) return;

                            curObj = fieldInfo.GetValue(curObj);
                        }

                        // Display the last object
                        fieldInfo = GetFieldInfo(curObj, pars[1]);
                        if (fieldInfo == null) return;

                        // Display the field name, value, and attributes.
                        Watchdog.Log(GetType().ToString(), string.Format("{0} = \"{1}\"; attributes: {2}",
                            fieldInfo.Name, fieldInfo.GetValue(curObj), fieldInfo.Attributes));
                    }
                }
            }
        }

        private FieldInfo GetFieldInfo(object obj, string field)
        {

            // Get field info from type
            Type type = obj.GetType();

            // Find field info iteratively
            FieldInfo fieldInfo = null;
            foreach (int one in Enum.GetValues(typeof(BindingFlags)))
            {
                foreach (int two in Enum.GetValues(typeof(BindingFlags)))
                {
                    fieldInfo = type.GetField(field, (BindingFlags)one | (BindingFlags)two);
                    if (fieldInfo != null) goto next;
                }
            }

        next:

            if (fieldInfo == null)
            {
                Watchdog.Log(GetType().ToString(), string.Format("Field '{0}' not found in {1}", field, obj));
                return null;
            }
            else
            {
                return fieldInfo;
            }
        }
    }
}