using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Configuration;

namespace CRMAzureBusIntegration
{
    public class Broker
    {
        public static void Listener(RemoteExecutionContext context)
        {
            try
            {
                IEnumerable<Type> classList = GetClasses();
                if (classList != null)
                {
                    foreach (Type cls in classList.Where(x => x.Namespace == ConfigurationManager.AppSettings["Namespace"]))
                    {
                        if (cls.Name == context.PrimaryEntityName)
                        {
                            Object obj = GetInstance(cls);
                            if (obj != null)
                            {
                                MethodInfo methodInfo = cls.GetMethod("Execute");
                                if (methodInfo != null)
                                {
                                    object[] parametersArray = new object[] { context };
                                    methodInfo.Invoke(obj, parametersArray);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inside Listener: Error[" + ex.Message + "]");
            }
        }
        static IEnumerable<Type> GetClasses()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            return asm != null ? asm.GetTypes() : null;
        }
        static object GetInstance(Type typeList)
        {
            return typeList != null ? Activator.CreateInstance(typeList, false) : null;
        }
    }
}
