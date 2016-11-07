using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using System.Reflection;
using System.IO;

namespace EM.Util
{
    public class AssemblyLoader
    {

        /// <summary>
        /// the folder where the program was deployed
        /// </summary>
        public static string currentDeploymentFolder
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }


        /// <summary>
        ///  - return instance of classFullName from the assembly file
        /// </summary>
        public static object instantiateClass(string filePath, string classFullName) 
        {
            Assembly assembly = Assembly.LoadFile(filePath);
            return assembly.CreateInstance(classFullName);            
        }

        /// <summary>
        ///  - return a list of instances of T found in the assembly file 
        ///  - classes that have base class as T or implement interface T
        /// </summary>
        public static EList<T> instantiate<T>(string filePath) where T : class
        {
            Assembly assembly = Assembly.LoadFile(filePath);
            return AssemblyLoader.instantiate<T>(assembly);
        }

        /// <summary>
        ///  - return a list of instances of T found in the assembly 
        ///  - classes that have base class as T or implement interface T
        /// </summary>
        public static EList<T> instantiate<T>(Assembly assembly) where T : class
        {
            Type seekType = typeof(T);

            EList<T> result = new EList<T>();
            Type[] allTypes = assembly.GetTypes();
            foreach (Type type in allTypes)
            {
                if (type.BaseType == seekType)
                {
                    result.Add((T)Activator.CreateInstance(type));
                }
                else
                {
                    Type[] interfaces = type.GetInterfaces();
                    foreach (Type interfaceType in interfaces)
                    {
                        if (interfaceType == seekType)
                        {
                            result.Add((T)Activator.CreateInstance(type));
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}
