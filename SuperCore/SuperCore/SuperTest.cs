namespace SuperCore
{
    public class SuperTest : Super
    {
        public override CallResult SendCall(CallInfo info)
        {
            return Call(info);
        }
    }
}
