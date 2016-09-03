namespace SuperJson.Objects
{
    public class SuperNumber : SuperToken
    {
        public SuperNumber(double value)
        {
            Value = value;
        }
        public override SuperTokenType TokenType => SuperTokenType.Number;
        public double TypedValue => (double) Value;
    }
}
