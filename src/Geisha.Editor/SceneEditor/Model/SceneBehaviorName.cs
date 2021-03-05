namespace Geisha.Editor.SceneEditor.Model
{
    public readonly struct SceneBehaviorName
    {
        public SceneBehaviorName(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() => $"{nameof(Value)}: {Value}";
    }
}