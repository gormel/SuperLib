using System;
using System.Linq;
using System.Reflection;

namespace SuperCore.NetData
{
    public class CallInfo
    {
        public Guid CallID = Guid.NewGuid();
        public Guid ClassID = Guid.Empty;

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
