namespace SuperCore.Wrappers
{
    public interface IDelegateActionWrapper
    {
        void Invoke();
    }

    public interface IDelegateActionWrapper<T1>
    {
        void Invoke(T1 arg1);
    }


    public interface IDelegateActionWrapper<T1, T2>
    {
        void Invoke(T1 arg1, T2 arg2);
    }


    public interface IDelegateActionWrapper<T1, T2, T3>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5, T6>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }


    public interface IDelegateActionWrapper<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    }

    public interface IDelegateFuncWrapper<TResult>
    {
        TResult Invoke();
    }

    public interface IDelegateFuncWrapper<T1, TResult>
    {
        TResult Invoke(T1 arg1);
    }
    
    public interface IDelegateFuncWrapper<T1, T2, TResult>
    {
        TResult Invoke(T1 arg1, T2 arg2);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, TResult>
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, TResult>
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, TResult>
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, T6, TResult>
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, TResult>
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    }


    public interface IDelegateFuncWrapper<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
    {
        TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    }



}
