using System;
using System.Linq;
using System.Reflection;

namespace SuperCore
{
    public class CallInfo
    {
        public Guid CallID = Guid.NewGuid();

        public string TypeName;
        public string MethodName;
        public object[] Args;

        private Type mType;
        public MethodInfo GetMethodInfo()
        {
            if (mType == null)
            {
                mType =
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.FullName == TypeName);
            }
            return mType?.GetMethod(MethodName);
        }
    }
}
