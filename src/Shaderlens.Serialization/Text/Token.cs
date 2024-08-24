namespace Shaderlens.Serialization.Text
{
    public readonly struct Token<T>
    {
        public readonly T Type;
        public readonly string Value;
        public readonly int Start;

        public Token(T type, string value, int start)
        {
            this.Type = type;
            this.Value = value;
            this.Start = start;
        }

        public override string ToString()
        {
            return String.Format("{0} \"{1}\" ({2})", this.Type, this.Value, this.Start);
        }
    }
}
