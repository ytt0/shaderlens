namespace Shaderlens.Serialization.Project
{
    public readonly struct RenderSequenceSettings
    {
        public static readonly RenderSequenceSettings Default = new RenderSequenceSettings(null, 0, 0, 25, 100, 0, new RenderSize(800, 600), false, false, false);

        public readonly string? TargetPath;
        public readonly int BufferIndex;
        public readonly int BufferTextureIndex;
        public readonly int FrameRate;
        public readonly int FrameCount;
        public readonly int StartFrame;
        public readonly RenderSize RenderSize;
        public readonly bool Prerender;
        public readonly bool OverridePath;
        public readonly bool RelativeIndex;

        public RenderSequenceSettings(string? targetPath, int bufferIndex, int bufferTextureIndex, int frameRate, int frameCount, int startFrame, RenderSize renderSize, bool prerender, bool overridePath, bool relativeIndex)
        {
            this.TargetPath = targetPath;
            this.BufferIndex = bufferIndex;
            this.BufferTextureIndex = bufferTextureIndex;
            this.FrameRate = frameRate;
            this.FrameCount = frameCount;
            this.StartFrame = startFrame;
            this.RenderSize = renderSize;
            this.Prerender = prerender;
            this.OverridePath = overridePath;
            this.RelativeIndex = relativeIndex;
        }
    }

    public class RenderSequenceSettingsSerializer : IJsonSerializer<RenderSequenceSettings>
    {
        private readonly RenderSequenceSettings defaultValue;
        private readonly ValueJsonSerializer<bool> boolSerializer;
        private readonly ValueJsonSerializer<int> intSerializer;
        private readonly ValueJsonSerializer<string?> stringSerializer;
        private readonly RenderSizeJsonSerializer renderSizeSerializer;

        public RenderSequenceSettingsSerializer(RenderSequenceSettings defaultValue)
        {
            this.defaultValue = defaultValue;
            this.boolSerializer = new ValueJsonSerializer<bool>();
            this.intSerializer = new ValueJsonSerializer<int>();
            this.stringSerializer = new ValueJsonSerializer<string?>();
            this.renderSizeSerializer = new RenderSizeJsonSerializer();
        }

        public RenderSequenceSettings Deserialize(JsonNode? source)
        {
            if (source == null)
            {
                return RenderSequenceSettings.Default;
            }

            var targetPath = this.stringSerializer.Deserialize(source[nameof(RenderSequenceSettings.TargetPath)], this.defaultValue.TargetPath);
            var bufferIndex = this.intSerializer.Deserialize(source[nameof(RenderSequenceSettings.BufferIndex)], this.defaultValue.BufferIndex);
            var bufferTextureIndex = this.intSerializer.Deserialize(source[nameof(RenderSequenceSettings.BufferTextureIndex)], this.defaultValue.BufferTextureIndex);
            var size = this.renderSizeSerializer.Deserialize(source[nameof(RenderSequenceSettings.RenderSize)], this.defaultValue.RenderSize);
            var frameRate = this.intSerializer.Deserialize(source[nameof(RenderSequenceSettings.FrameRate)], this.defaultValue.FrameRate);
            var frameCount = this.intSerializer.Deserialize(source[nameof(RenderSequenceSettings.FrameCount)], this.defaultValue.FrameCount);
            var startFrame = this.intSerializer.Deserialize(source[nameof(RenderSequenceSettings.StartFrame)], this.defaultValue.StartFrame);
            var prerender = this.boolSerializer.Deserialize(source[nameof(RenderSequenceSettings.Prerender)], this.defaultValue.Prerender);
            var relativeIndex = this.boolSerializer.Deserialize(source[nameof(RenderSequenceSettings.RelativeIndex)], this.defaultValue.RelativeIndex);

            return new RenderSequenceSettings(targetPath, bufferIndex, bufferTextureIndex, frameRate, frameCount, startFrame, size, prerender, false, relativeIndex);
        }

        public JsonNode? Serialize(RenderSequenceSettings source)
        {
            if (Equals(source, RenderSequenceSettings.Default))
            {
                return null;
            }

            var target = new JsonObject();

            target[nameof(RenderSequenceSettings.TargetPath)] = this.stringSerializer.Serialize(source.TargetPath);
            target[nameof(RenderSequenceSettings.BufferIndex)] = this.intSerializer.Serialize(source.BufferIndex);
            target[nameof(RenderSequenceSettings.BufferTextureIndex)] = this.intSerializer.Serialize(source.BufferTextureIndex);
            target[nameof(RenderSequenceSettings.RenderSize)] = this.renderSizeSerializer.Serialize(source.RenderSize);
            target[nameof(RenderSequenceSettings.FrameRate)] = this.intSerializer.Serialize(source.FrameRate);
            target[nameof(RenderSequenceSettings.FrameCount)] = this.intSerializer.Serialize(source.FrameCount);
            target[nameof(RenderSequenceSettings.StartFrame)] = this.intSerializer.Serialize(source.StartFrame);
            target[nameof(RenderSequenceSettings.Prerender)] = this.boolSerializer.Serialize(source.Prerender);
            target[nameof(RenderSequenceSettings.RelativeIndex)] = this.boolSerializer.Serialize(source.RelativeIndex);

            return target;
        }
    }
}
