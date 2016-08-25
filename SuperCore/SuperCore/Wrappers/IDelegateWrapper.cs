namespace SuperCore.Wrappers
{
    public interface IDelegateActionWrapperBase
    {
    }

    public interface IDelegateActionWrapper : IDelegateActionWrapperBase
    {
        void Invoke();
    }

    public interface IDelegateActionWrapper<T1> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1);
    }


    public interface IDelegateActionWrapper<T1, T2> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1, T2 arg2);
    }


    public interface IDelegateActionWrapper<T1, T2, T3> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5, T6> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7, T8> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IDelegateActionWrapperBase
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    }

    public interface IDelegateFuncWrapperBase
    {
    }

    public interface IDelegateFuncWrapper<TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke();
    }

    public interface IDelegateFuncWrapper<T1, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1);
    }
    
    public interface IDelegateFuncWrapper<T1, T2, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1, T2 arg2);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, T6, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, T8, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> : IDelegateFuncWrapperBase
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    }



}
