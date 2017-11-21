using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GPCommon
{
    public class AssemblyUtility
    {
        public static object GetNewInstanceOfInterface(System.Type interfaceType, params System.Type[] typesToExlude)
        {
            Assembly currentAssembly = typeof(AssemblyUtility).Assembly;
            System.Type[] allTypesInAssembly = currentAssembly.GetTypes();

            System.Type interfaceInheritor = null;
            for (var i = 0; i < allTypesInAssembly.Length; i++)
            {
                System.Type currentType = allTypesInAssembly[i];
                var isInheritor = interfaceType.IsAssignableFrom(currentType);
                var isNotTheSameType = !interfaceType.Equals(currentType);
                var isNotToExlude = !typesToExlude.Contains(currentType);

                if (isInheritor && isNotTheSameType && isNotToExlude)
                {
                    interfaceInheritor = currentType;
                    break;
                }
            }

            if (interfaceInheritor == null)
            {
                return null;
            }

            return System.Activator.CreateInstance(interfaceInheritor);
        }

        public static List<T> GetChildTypeObjectList<T>()
        {
            var typeResult = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(T)))
                .ToList();

            var objResult = typeResult.Select(Activator.CreateInstance).Cast<T>().ToList();

            return objResult;
        }
    }
}