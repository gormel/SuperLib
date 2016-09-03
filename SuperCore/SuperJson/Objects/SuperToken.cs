namespace SuperJson.Objects
{
    public abstract class SuperToken
    {
        public abstract SuperTokenType TokenType { get; }

        public object Value { get; set; }
    }
}
