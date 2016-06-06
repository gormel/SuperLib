namespace SuperCore
{
    public class SuperTest : Super
    {
        internal override CallResult SendCall(CallInfo info)
        {
            return Call(info);
        }
    }
}
