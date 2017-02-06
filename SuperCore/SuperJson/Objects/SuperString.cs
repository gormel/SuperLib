namespace SuperJson.Objects
{
    public class SuperString : SuperToken
    {
        public SuperString(string value)
        {
            Value = value;
        }
        public string TypedValue => (string) Value;
        public override SuperTokenType TokenType => SuperTokenType.String;
    }
}
