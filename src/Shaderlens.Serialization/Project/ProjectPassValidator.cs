namespace Shaderlens.Serialization.Project
{
    public interface IChannelBindingValidator
    {
        void Validate(string key, int textureIndex, JsonNode sourceNode);
    }

    public class ChannelBindingValidator : IChannelBindingValidator
    {
        private readonly Dictionary<string, int> definitions;

        public ChannelBindingValidator()
        {
            this.definitions = new Dictionary<string, int>();
        }

        public void AddFramebufferDefinition(string key, int outputs)
        {
            this.definitions.Add(key, outputs);
        }

        public void Validate(string key, int textureIndex, JsonNode sourceNode)
        {
            if (!this.definitions.TryGetValue(key, out var outputs))
            {
                throw new JsonSourceException($"{key} pass definition is missing", sourceNode);
            }

            if (textureIndex >= outputs)
            {
                throw new JsonSourceException($"Index {textureIndex} is invalid, {key} pass has {outputs} {(outputs == 1 ? "output" : "outputs")} (expected index is between 0 and {outputs - 1})", sourceNode);
            }
        }
    }
}
