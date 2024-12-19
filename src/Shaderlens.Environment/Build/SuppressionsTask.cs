using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace Shaderlens
{
    public class SuppressionsTask : Microsoft.Build.Utilities.Task
    {
        private static readonly Regex NamespaceRegex = new Regex("\\bnamespace\\s+(?<name>[\\w.]+)");
        private static readonly Regex ClassRegex = new Regex("\\bclass\\s+(?<name>[\\w]+)");
        private static readonly Regex MainMethodRegex = new Regex("\\bvoid\\s+main\\b");
        private static readonly Regex MainImageMethodRegex = new Regex("\\bvoid\\s+mainImage\\b");
        private static readonly Regex UniformLineAnnotationRegex = new Regex("^\\s*//@\\s*uniform(\\s|,|$)");
        private static readonly Regex FieldDeclarationRegex = new Regex("^\\s*((public|private|internal|static|readonly|const)\\s+)*(?<type>\\w+)\\s+(?<name>\\w+)");

        [Required]
        public ITaskItem[] Compile { get; set; }

        [Required]
        public string TargetPath { get; set; }

        public override bool Execute()
        {
            var suppressionContent = new StringBuilder();
            suppressionContent.AppendLine("using System.Diagnostics.CodeAnalysis;");

            var isUniform = false;

            foreach (var item in this.Compile)
            {
                var lines = File.ReadAllLines(item.GetMetadata("FullPath"));
                var containingNamespace = String.Empty;
                var containingClass = String.Empty;

                foreach (var line in lines)
                {
                    var match = NamespaceRegex.Match(line);
                    if (match.Success)
                    {
                        containingNamespace = match.Groups["name"].Value;
                    }

                    match = ClassRegex.Match(line);
                    if (match.Success)
                    {
                        containingClass = match.Groups["name"].Value;
                    }

                    if (MainMethodRegex.IsMatch(line))
                    {
                        suppressionContent.AppendLine($"[assembly: SuppressMessage(\"CodeQuality\", \"IDE0051:Remove unused private members\", Scope = \"member\", Target = \"~M:{containingNamespace}.{containingClass}.main()\")]");
                        suppressionContent.AppendLine($"[assembly: SuppressMessage(\"Style\", \"IDE0060:Remove unused parameter\", Scope = \"member\", Target = \"~M:{containingNamespace}.{containingClass}.main()\")]");
                    }

                    if (MainImageMethodRegex.IsMatch(line))
                    {
                        suppressionContent.AppendLine($"[assembly: SuppressMessage(\"CodeQuality\", \"IDE0051:Remove unused private members\", Scope = \"member\", Target = \"~M:{containingNamespace}.{containingClass}.mainImage(Shaderlens.vec4@,Shaderlens.vec2@)\")]");
                        suppressionContent.AppendLine($"[assembly: SuppressMessage(\"Style\", \"IDE0060:Remove unused parameter\", Scope = \"member\", Target = \"~M:{containingNamespace}.{containingClass}.mainImage(Shaderlens.vec4@,Shaderlens.vec2@)\")]");
                    }

                    if (isUniform)
                    {
                        match = FieldDeclarationRegex.Match(line);
                        if (match.Success)
                        {
                            suppressionContent.AppendLine($"[assembly: SuppressMessage(\"Style\", \"IDE0044:Add readonly modifier\", Scope = \"member\", Target = \"~F:{containingNamespace}.{containingClass}.{match.Groups["name"]}\")]");
                        }
                    }

                    if (!String.IsNullOrEmpty(line))
                    {
                        isUniform = UniformLineAnnotationRegex.IsMatch(line);
                    }
                }
            }

            File.WriteAllText(this.TargetPath, suppressionContent.ToString());
            return true;
        }
    }
}
