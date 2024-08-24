namespace Shaderlens.Serialization.Extensions
{
    public static class FileSystemExtensions
    {
        public static IFileResource<string> ReadText(this IFileSystem fileSystem, string path, JsonNode sourceNode)
        {
            try
            {
                return fileSystem.ReadText(path);
            }
            catch (StorageException e)
            {
                throw new JsonSourceException(e.Message, sourceNode, e.InnerException);
            }
            catch (Exception e)
            {
                throw new JsonSourceException(e.ToString(), sourceNode, e);
            }
        }

        public static IFileResource<byte[]> ReadBytes(this IFileSystem fileSystem, string path, JsonNode sourceNode)
        {
            try
            {
                return fileSystem.ReadBytes(path);
            }
            catch (StorageException e)
            {
                throw new JsonSourceException(e.Message, sourceNode, e.InnerException);
            }
            catch (Exception e)
            {
                throw new JsonSourceException(e.ToString(), sourceNode, e);
            }
        }
    }
}
