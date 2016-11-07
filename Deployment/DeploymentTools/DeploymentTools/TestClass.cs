using System;
using System.Collections.Generic;
using System.Text;
using EM.Util;
using System.Reflection;
using EM.Collections;

namespace DeploymentTools
{
    public class TestClass
    {
        public static void test()
        {
            Dependencies d = new Dependencies(Assembly.GetExecutingAssembly());
            //d.entryAssembly = Assembly.LoadWithPartialName("DeploymentTools");
                                    
            foreach (Assembly dep in d.assemblies)
            {
                Console.WriteLine(dep.FullName);    
                //Console.WriteLine(dep.Location);  //path
            }
        }

    }
}
