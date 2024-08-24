namespace Shaderlens.Serialization.Text
{
    public interface ICSharpTransformer
    {
        SourceLines Transform(SourceLines sourceLines);
    }

    public class CSharpTransformer : ICSharpTransformer
    {
        public static readonly ICSharpTransformer Instance = new CSharpTransformer();

        private readonly SourceLineAnnotationParser annotationParser;

        private CSharpTransformer()
        {
            this.annotationParser = new SourceLineAnnotationParser(true);
        }

        public SourceLines Transform(SourceLines sourceLines)
        {
            var target = new List<SourceLine>();

            var defaultLineTransformers = new List<ILineTransformer>
            {
                new RemoveLineRegexTransformer("^\\s*namespace\\s+.*;\\s*$", true),
                new RemoveLineRegexTransformer("^\\s*using\\s+.*;\\s*$", true),
                new ScopeTransformer("namespace"),
                new ScopeTransformer("class"),
                new ArrayTransformer(),
                new RemoveLineRegexTransformer("^\\s*#region\\b.*$", true),
                new RemoveLineRegexTransformer("^\\s*#endregion\\b.*$", true),
                new RemoveWordTransformer("public", true),
                new RemoveWordTransformer("private", true),
                new RemoveWordTransformer("internal", true),
                new RemoveWordTransformer("protected", true),
                new RemoveWordTransformer("virtual", true),
                new RemoveWordTransformer("override", true),
                new RemoveWordTransformer("static", true),
                new RemoveWordTransformer("new", true),
                new ReplaceTransformer("(float)", "", true),
                new ReplaceTransformer("(double)", "", true),
                new ReplaceWordTransformer("readonly", "const", true),
                new ReplaceWordTransformer("_bool", "bool", true),
                new ReplaceWordTransformer("_int", "int", true),
                new ReplaceWordTransformer("_uint", "uint", true),
                new ReplaceWordTransformer("_float", "float", true),
                new ReplaceWordTransformer("bool_", "bool", true),
                new ReplaceWordTransformer("int_", "int", true),
                new ReplaceWordTransformer("uint_", "uint", true),
                new ReplaceWordTransformer("float_", "float", true),
                new ReplaceWordTransformer("Float", "float", true),
                // 1.23f -> 1.23
                new ReplaceRegexTransformer("(?<number>[0-9]*.[0-9]+)[fF]", "$number", true),
                // getValue(ref value1, out value2) -> getValue(value1, value2)
                new ReplaceRegexTransformer("\\b(out|ref)\\s+(?<parameter>(\\w+\\s*[,\\)]))", "$parameter", true),

                // #if !identifier -> #ifndef identifier
                new ReplaceRegexTransformer("^\\s*#if\\s+!\\s*(?<identifier>\\w+)\\s*(//.*)?$", "#ifndef $identifier", true),
                // #if identifier -> #ifdef identifier
                new ReplaceRegexTransformer("^\\s*#if\\s+(?<identifier>[^!]\\w*)\\s*(//.*)?$", "#ifdef $identifier", true),

                new ReplaceWordTransformer("ref", "inout", true),
            };

            var globalLineTransformers = new List<ILineTransformer>();
            var nextLineTransformers = new List<ILineTransformer>();

            var i = 0;
            while (i < sourceLines.Count)
            {
                var sourceLine = sourceLines[i];
                try
                {
                    if (String.IsNullOrWhiteSpace(sourceLine.Line))
                    {
                        target.Add(sourceLine);
                    }
                    else if (sourceLine.Line.Trim().StartsWith("//@"))
                    {
                        var annotation = this.annotationParser.Parse(sourceLine);
                        var lineTransformer = CreateLineTransformer(annotation);

                        if (lineTransformer.IsGlobal)
                        {
                            globalLineTransformers.Add(lineTransformer);
                        }
                        else
                        {
                            nextLineTransformers.Add(lineTransformer);
                        }
                    }
                    else
                    {
                        var context = new TransformContext(sourceLine);
                        foreach (var lineTransformer in nextLineTransformers.Concat(globalLineTransformers).Concat(defaultLineTransformers))
                        {
                            lineTransformer.Transform(context);
                        }
                        context.Commit(target);

                        nextLineTransformers.Clear();
                    }
                }
                catch (ParseException e)
                {
                    throw new SourceLineException(e.Message, sourceLine, e);
                }

                i++;
            }

            return new SourceLines(target);
        }

        private static ILineTransformer CreateLineTransformer(ISourceLineAnnotation annotation)
        {
            if (annotation.Type == "insert-line")
            {
                return CreateInsertLineTransformer(annotation);
            }

            if (annotation.Type == "remove-line")
            {
                ValidateEmpty(annotation);
                return RemoveLineTransformer.Instance;
            }

            if (annotation.Type == "replace-line")
            {
                return CreateReplaceLineTransformer(annotation);
            }

            if (annotation.Type == "replace")
            {
                return CreateReplaceTransformer(annotation, false);
            }

            if (annotation.Type == "replace-all")
            {
                return CreateReplaceTransformer(annotation, true);
            }

            if (annotation.Type == "replace-word")
            {
                return CreateReplaceWordTransformer(annotation, false);
            }

            if (annotation.Type == "replace-word-all")
            {
                return CreateReplaceWordTransformer(annotation, true);
            }

            if (annotation.Type == "replace-regex")
            {
                return CreateReplaceRegexTransformer(annotation, false);
            }

            if (annotation.Type == "replace-regex-all")
            {
                return CreateReplaceRegexTransformer(annotation, true);
            }

            if (annotation.Type == "remove")
            {
                return CreateRemoveTransformer(annotation, false);
            }

            if (annotation.Type == "remove-all")
            {
                return CreateRemoveTransformer(annotation, true);
            }

            if (annotation.Type == "remove-word")
            {
                return CreateRemoveWordTransformer(annotation, false);
            }

            if (annotation.Type == "remove-word-all")
            {
                return CreateRemoveWordTransformer(annotation, true);
            }

            if (annotation.Type == "const")
            {
                ValidateEmpty(annotation);
                return new AddPrefixTransformer("const");
            }

            if (annotation.Type == "define")
            {
                ValidateEmpty(annotation);
                return DefineTransformer.Instance;
            }

            if (annotation.Type == "uniform")
            {
                return new UniformTransformer(annotation);
            }

            if (annotation.Type == "uniform-group")
            {
                return new InsertLineTransformer(annotation.SourceLine);
            }

            if (annotation.Type == null)
            {
                throw new SourceLineException($"Invalid transformer definition", annotation.SourceLine);
            }

            throw new SourceLineException($"Invalid transformer type \"{annotation.Type}\"", annotation.SourceLine);
        }

        private static RemoveTransformer CreateRemoveTransformer(ISourceLineAnnotation annotation, bool isGlobal)
        {
            var annotationReader = new SourceLineAnnotationReader(annotation);
            if (!annotationReader.TryGetValue(out var value) || String.IsNullOrWhiteSpace(value))
            {
                throw new SourceLineException($"Remove value is expected", annotation.SourceLine);
            }

            annotationReader.ValidateEmpty();
            return new RemoveTransformer(value, isGlobal);
        }

        private static RemoveWordTransformer CreateRemoveWordTransformer(ISourceLineAnnotation annotation, bool isGlobal)
        {
            var annotationReader = new SourceLineAnnotationReader(annotation);
            if (!annotationReader.TryGetValue(out var value) || String.IsNullOrWhiteSpace(value))
            {
                throw new SourceLineException($"Remove value is expected", annotation.SourceLine);
            }

            annotationReader.ValidateEmpty();
            return new RemoveWordTransformer(value, isGlobal);
        }

        private static InsertLineTransformer CreateInsertLineTransformer(ISourceLineAnnotation annotation)
        {
            var annotationReader = new SourceLineAnnotationReader(annotation);

            if (!annotationReader.TryGetValue(out var value) || String.IsNullOrWhiteSpace(value))
            {
                throw new SourceLineException($"Insert value is expected", annotation.SourceLine);
            }

            annotationReader.ValidateEmpty();
            return new InsertLineTransformer(new SourceLine(annotation.SourceLine.Source, value, annotation.SourceLine.Index));
        }

        private static ReplaceTransformer CreateReplaceTransformer(ISourceLineAnnotation annotation, bool isGlobal)
        {
            var annotationReader = new SourceLineAnnotationReader(annotation);

            if (!annotationReader.TryGetValue("source", out var source))
            {
                throw new SourceLineException($"Replace source value is expected", annotation.SourceLine);
            }

            if (!annotationReader.TryGetValue("target", out var target))
            {
                throw new SourceLineException($"Replace target value is expected", annotation.SourceLine);
            }

            annotationReader.ValidateEmpty();
            return new ReplaceTransformer(source, target, isGlobal);
        }

        private static ReplaceWordTransformer CreateReplaceWordTransformer(ISourceLineAnnotation annotation, bool isGlobal)
        {
            var annotationReader = new SourceLineAnnotationReader(annotation);

            if (!annotationReader.TryGetValue("source", out var source))
            {
                throw new SourceLineException($"Replace source value is expected", annotation.SourceLine);
            }

            if (!annotationReader.TryGetValue("target", out var target))
            {
                throw new SourceLineException($"Replace target value is expected", annotation.SourceLine);
            }

            annotationReader.ValidateEmpty();
            return new ReplaceWordTransformer(source, target, isGlobal);
        }

        private static ReplaceLineTransformer CreateReplaceLineTransformer(ISourceLineAnnotation annotation)
        {
            var annotationReader = new SourceLineAnnotationReader(annotation);

            if (!annotationReader.TryGetValue(out var value) || String.IsNullOrWhiteSpace(value))
            {
                throw new SourceLineException($"Replace value is expected", annotation.SourceLine);
            }

            annotationReader.ValidateEmpty();
            return new ReplaceLineTransformer(value);
        }

        private static ReplaceRegexTransformer CreateReplaceRegexTransformer(ISourceLineAnnotation annotation, bool isGlobal)
        {
            var annotationReader = new SourceLineAnnotationReader(annotation);

            if (!annotationReader.TryGetValue("source", out var source))
            {
                throw new SourceLineException($"Replace source value is expected", annotation.SourceLine);
            }

            if (!annotationReader.TryGetValue("target", out var target))
            {
                throw new SourceLineException($"Replace target value is expected", annotation.SourceLine);
            }

            annotationReader.ValidateEmpty();
            return new ReplaceRegexTransformer(source, target, isGlobal);
        }

        private static void ValidateEmpty(ISourceLineAnnotation annotation)
        {
            var annotationReader = new SourceLineAnnotationReader(annotation);
            annotationReader.ValidateEmpty();
        }
    }
}
