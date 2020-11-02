namespace IngameScript
{
    public struct NamedOption<T>
    {
        public string Name;
        public T Value;

        public NamedOption(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => Name;
    }
}
