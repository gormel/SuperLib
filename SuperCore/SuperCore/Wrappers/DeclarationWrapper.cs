using System;

namespace SuperCore.Wrappers
{
    internal class DeclarationWrapper
    {
        public DeclarationWrapper(object instance, Type declaredType)
        {
            Instance = instance;
            TypeName = declaredType.AssemblyQualifiedName;
        }

        public string TypeName { get; }
        public object Instance { get; }
    }
}
