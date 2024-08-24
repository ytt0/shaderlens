namespace Shaderlens.Serialization.Project
{
    public class ViewerPassSelection : IEquatable<ViewerPassSelection>
    {
        public static readonly ViewerPassSelection None = new ViewerPassSelection("@None");
        public static readonly ViewerPassSelection ValuesOverlay = new ViewerPassSelection("@ValuesOverlay");

        public string Key { get; }

        public ViewerPassSelection(string key)
        {
            this.Key = key;
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ViewerPassSelection);
        }

        public bool Equals(ViewerPassSelection? other)
        {
            return this.Key == other?.Key;
        }
    }

    public class ViewerPassSelectionSerializer : IJsonSerializer<ViewerPassSelection?>
    {
        public JsonNode? Serialize(ViewerPassSelection? value)
        {
            return value?.Key != null ? JsonValue.Create<string>(value.Key) : null;
        }

        public ViewerPassSelection? Deserialize(JsonNode? source)
        {
            var key = source?.GetStringValue();

            return key == null ? null :
                key == ViewerPassSelection.None.Key ? ViewerPassSelection.None :
                key == ViewerPassSelection.ValuesOverlay.Key ? ViewerPassSelection.ValuesOverlay :
                new ViewerPassSelection(key);
        }
    }
}
