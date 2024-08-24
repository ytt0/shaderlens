using Microsoft.Win32;

namespace Shaderlens.Presentation.Extensions
{
    using Path = System.IO.Path;

    public static class FileDialogExtensions
    {
        public static bool? ShowDialog(this OpenFileDialog openFileDialog, Window owner, string path)
        {
            openFileDialog.InitialDirectory = Path.GetDirectoryName(path);
            openFileDialog.FileName = Path.GetFileName(path);

            return openFileDialog.ShowDialog(owner);
        }

        public static bool? ShowDialog(this OpenFolderDialog openFolderDialog, Window owner, string path)
        {
            openFolderDialog.InitialDirectory = path;
            openFolderDialog.FolderName = String.Empty;

            return openFolderDialog.ShowDialog(owner);
        }

        public static bool? ShowDialog(this SaveFileDialog saveFileDialog, Window owner, string path, bool overwritePrompt)
        {
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(path);
            saveFileDialog.FileName = Path.GetFileName(path);
            saveFileDialog.OverwritePrompt = overwritePrompt;

            return saveFileDialog.ShowDialog(owner);
        }
    }
}
