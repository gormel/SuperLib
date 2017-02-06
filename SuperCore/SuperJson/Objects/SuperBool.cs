namespace SuperJson.Objects
{
    public class SuperBool : SuperToken
    {
        public SuperBool(bool value)
        {
            Value = value;
        }

        public override SuperTokenType TokenType => SuperTokenType.Bool;

        public bool TypedValue => (bool) Value;
    }
}
