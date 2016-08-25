using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperCore.Utilses
{
    internal static class Utils
    {
        private static readonly Dictionary<Tuple<Type, int>, Type> mTypesCashe = new Dictionary<Tuple<Type, int>, Type>(); 

        public static Type GetActionWrapper<T>(Type[] argTypes)
        {
            var key = Tuple.Create(typeof (T), argTypes.Length);
            if (!mTypesCashe.ContainsKey(key))
            {
                var genericTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.BaseType == typeof (T) || t.GetInterface(typeof (T).Name) != null);

                var foundType = genericTypes.First(t => t.GetGenericArguments().Length == argTypes.Length);
                mTypesCashe.Add(key, foundType);
            }
            var interestingType = mTypesCashe[key];
            if (argTypes.Length <= 0)
                return interestingType;

            return interestingType.MakeGenericType(argTypes);
        }

        public static Type GetFuncWrapper<T>(Type resultType, Type[] argTypes)
        {
            return GetActionWrapper<T>(argTypes.Concat(new[] {resultType}).ToArray());
        }
    }
}
