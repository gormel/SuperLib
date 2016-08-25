using System;
using System.Linq;
using System.Reflection;
using SuperCore.Core;

namespace SuperCore.NetData
{
    public class CallInfo : Call
    {
        public Guid CallID = Guid.NewGuid();
        public Guid ClassID;

        public string TypeName;
        public string MethodName;
        public object[] Args;

        private Type mType;
        public MethodInfo GetMethodInfo()
        {
            if (mType == null)
            {
                mType = Type.GetType(TypeName) ?? 
                    AppDomain.CurrentDomain.GetAssemblies ()
                    .SelectMany (a => a.GetTypes())
                    .First (t => t.AssemblyQualifiedName == TypeName);
                //mType = Type.GetType(TypeName, true);
            }
            if (mType == null)
                return null;

            var methods = Super.CollectMethods(mType);
            return methods.FirstOrDefault(i => i.Name == MethodName);
        }
    }
}
