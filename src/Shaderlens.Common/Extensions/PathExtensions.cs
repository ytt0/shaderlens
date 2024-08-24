namespace Shaderlens.Extensions
{
    public static class PathExtensions
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        public static bool IsPathValid(string path)
        {
            var root = Path.GetPathRoot(path);
            if (root == null)
            {
                return false;
            }

            var segments = path.Substring(root.Length).Split(Path.DirectorySeparatorChar);
            return segments.All(IsFileNameValid);
        }

        public static bool IsFileNameValid(string fileName)
        {
            return !fileName.EndsWith(".") && !InvalidFileNameChars.Intersect(fileName).Any();
        }
    }
}
