using System;

namespace SuperCore.Wrappers
{
    internal class DelegateActionWrapperBase { }
	internal class DelegateActionWrapper : DelegateActionWrapperBase
    {
	    private readonly Delegate mSubject;

	    public DelegateActionWrapper(Delegate subject)
	    {
	        mSubject = subject;
	    }

	    public void Invoke()
	    {
	        mSubject.DynamicInvoke();
	    }
    }

    internal class DelegateActionWrapper<T1> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1)
        {
            mSubject.DynamicInvoke(arg1);
        }
    }

    internal class DelegateActionWrapper<T1, T2> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            mSubject.DynamicInvoke(arg1, arg2);
        }
    }

    internal class DelegateActionWrapper<T1, T2, T3> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            mSubject.DynamicInvoke(arg1, arg2, arg3);
        }
    }

    internal class DelegateActionWrapper<T1, T2, T3, T4> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            mSubject.DynamicInvoke(arg1, arg2, arg3, arg4);
        }
    }

    internal class DelegateActionWrapper<T1, T2, T3, T4, T5> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5);
        }
    }

    internal class DelegateActionWrapper<T1, T2, T3, T4, T5, T6> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }

    internal class DelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }

    internal class DelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7, T8> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
    }

    internal class DelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7, T8, T9> : DelegateActionWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateActionWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
    }

    public class DelegateFuncWrapperBase { }

    public class DelegateFuncWrapper<TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke()
        {
            return (TResult)mSubject.DynamicInvoke();
        }
    }

    public class DelegateFuncWrapper<T1, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1)
        {
            return (TResult)mSubject.DynamicInvoke(arg1);
        }
    }
    
    public class DelegateFuncWrapper<T1, T2, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1, T2 arg2)
        {
            return (TResult)mSubject.DynamicInvoke(arg1, arg2);
        }
    }

    public class DelegateFuncWrapper<T1, T2, T3, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            return (TResult)mSubject.DynamicInvoke(arg1, arg2, arg3);
        }
    }

    public class DelegateFuncWrapper<T1, T2, T3, T4, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return (TResult)mSubject.DynamicInvoke(arg1, arg2, arg3, arg4);
        }
    }

    public class DelegateFuncWrapper<T1, T2, T3, T4, T5, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return (TResult)mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5);
        }
    }

    public class DelegateFuncWrapper<T1, T2, T3, T4, T5, T6, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return (TResult)mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }

    public class DelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return (TResult)mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }
    }

    public class DelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return (TResult)mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }
    }

    public class DelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> : DelegateFuncWrapperBase
    {
        private readonly Delegate mSubject;

        public DelegateFuncWrapper(Delegate subject)
        {
            mSubject = subject;
        }

        public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return (TResult)mSubject.DynamicInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }
    }

}

