namespace Shaderlens.Tests.Serialization.Text
{
    [TestClass]
    public class LineTransformerTests
    {
        private class TestTransformContext : ITransformContext
        {
            public List<SourceLine> PrependLines { get; } = new List<SourceLine>();

            public SourceLine SourceLine { get; }
            public string? Line { get; set; }

            public TestTransformContext(SourceLine sourceLine)
            {
                this.SourceLine = sourceLine;
                this.Line = sourceLine.Line;
            }

            public void PrependLine(SourceLine line)
            {
                this.PrependLines.Add(line);
            }
        }

        [TestMethod]
        public void ReplaceLine_Completes_LineIsReplaced()
        {
            // setup
            var transformer = new ReplaceLineTransformer("target");

            // test
            Assert.AreEqual("target", TransformLine(transformer, "source").Line); // line is replaced
        }

        [TestMethod]
        public void ReplaceLine_Completes_IndentationIsPreserved()
        {
            // setup
            var transformer = new ReplaceLineTransformer("target");

            // test
            Assert.AreEqual("  target", TransformLine(transformer, "  source").Line); // indentation is preserved
        }

        [TestMethod]
        public void InsertLine_Completes_LineIsPrepended()
        {
            // setup
            var targetLine = CreateSourceLine("target");
            var transformer = new InsertLineTransformer(targetLine);

            // test
            CollectionAssert.AreEquivalent(new[] { targetLine }, TransformLine(transformer, "source").PrependLines.ToArray()); // line is prepended
        }

        [TestMethod]
        public void InsertLine_Completes_SourceLineIsPreserved()
        {
            // setup
            var transformer = new InsertLineTransformer(CreateSourceLine("target"));

            // test
            Assert.AreEqual("source", TransformLine(transformer, "source").Line); // source line is preserved
        }

        [TestMethod]
        public void RemoveLine_Completes_LineIsRemoved()
        {
            // setup
            var transformer = RemoveLineTransformer.Instance;

            // test
            Assert.AreEqual(null, TransformLine(transformer, "source").Line); // line is removed
        }

        [TestMethod]
        public void RemoveLineRegex_WithMatchingCompleteLine_LineIsRemoved()
        {
            // setup
            var transformer = new RemoveLineRegexTransformer("a+", false);

            // test
            Assert.AreEqual(null, TransformLine(transformer, "aaa").Line); // line is removed
        }

        [TestMethod]
        public void RemoveLineRegex_WithMatchingPartialLine_LineIsNotRemoved()
        {
            // setup
            var transformer = new RemoveLineRegexTransformer("a+", false);

            // test
            Assert.AreEqual("Aaaa", TransformLine(transformer, "Aaaa").Line); // line is not removed
            Assert.AreEqual("aaaB", TransformLine(transformer, "aaaB").Line); // line is not removed
        }

        [TestMethod]
        public void Replace_WithMatchingValue_ValueIsReplaced()
        {
            // setup
            var transformer = new ReplaceTransformer("source", "target", false);

            // test
            Assert.AreEqual("AtargetB", TransformLine(transformer, "AsourceB").Line); // value is replaced
        }

        [TestMethod]
        public void Remove_WithMatchingValue_ValueIsRemoved()
        {
            // setup
            var transformer = new RemoveTransformer("value", false);

            // test
            Assert.AreEqual("AB", TransformLine(transformer, "AvalueB").Line); // value is removed
        }

        [TestMethod]
        public void ReplaceWord_WithMatchingWord_WordIsReplaced()
        {
            // setup
            var transformer = new ReplaceWordTransformer("source", "target", false);

            // test
            Assert.AreEqual("A,target,B", TransformLine(transformer, "A,source,B").Line); // word is replaced
        }

        [TestMethod]
        public void ReplaceWord_WithMatchingWordWithoutBoundaries_WordIsNotReplaced()
        {
            // setup
            var transformer = new ReplaceWordTransformer("source", "target", false);

            // test
            Assert.AreEqual("AsourceB", TransformLine(transformer, "AsourceB").Line); // word is not replaced
        }

        [TestMethod]
        public void ReplaceWord_WithMultipleWordMatches_AllWordsAreReplaced()
        {
            // setup
            var transformer = new ReplaceWordTransformer("source", "target", false);

            // test
            Assert.AreEqual("A,target,B,target,C", TransformLine(transformer, "A,source,B,source,C").Line); // all words are replaced
        }

        [TestMethod]
        public void RemoveWord_WithMatchingWord_WordIsRemoved()
        {
            // setup
            var transformer = new RemoveWordTransformer("word", false);

            // test
            Assert.AreEqual("A,,B", TransformLine(transformer, "A,word,B").Line); // word is removed
        }

        [TestMethod]
        public void RemoveWord_WithMatchingWordPrecededByWhiteSpace_WhiteSpaceIsRemove()
        {
            // setup
            var transformer = new RemoveWordTransformer("word", false);

            // test
            Assert.AreEqual("A,,B", TransformLine(transformer, "A, word,B").Line); // preceding white space is removed
        }

        [TestMethod]
        public void RemoveWord_WithMatchingWordFollowedByWhiteSpace_WhiteSpaceIsRemove()
        {
            // setup
            var transformer = new RemoveWordTransformer("word", false);

            // test
            Assert.AreEqual("A,,B", TransformLine(transformer, "A,word  ,B").Line); // trailing white space is removed
        }

        [TestMethod]
        public void RemoveWord_WithMatchingWordSurroundedWithWhiteSpace_FollowedWhiteSpaceIsRemove()
        {
            // setup
            var transformer = new RemoveWordTransformer("word", false);

            // test
            Assert.AreEqual("A, ,B", TransformLine(transformer, "A, word  ,B").Line); // only trailing white space is removed
        }

        [TestMethod]
        public void RemoveWord_WithMatchingWordWithoutBoundaries_WordIsNotRemoved()
        {
            // setup
            var transformer = new RemoveWordTransformer("word", false);

            // test
            Assert.AreEqual("AwordB", TransformLine(transformer, "AwordB").Line); // word is not removed
        }

        [TestMethod]
        public void RemoveWord_WithMultipleWordMatches_AllWordsAreRemoved()
        {
            // setup
            var transformer = new RemoveWordTransformer("word", false);

            // test
            Assert.AreEqual("A,,B,,C", TransformLine(transformer, "A,word,B,word,C").Line); // all words are removed
        }

        [TestMethod]
        public void ReplaceRegex_WithMatchingValue_ValueIsReplaced()
        {
            // setup
            var transformer = new ReplaceRegexTransformer("a+", "b", false);

            // test
            Assert.AreEqual("AbB", TransformLine(transformer, "AaaB").Line); // value is replaced
        }

        [TestMethod]
        public void ReplaceRegex_WithMultipleMatches_AllValuesReplaced()
        {
            // setup
            var transformer = new ReplaceRegexTransformer("a+", "b", false);

            // test
            Assert.AreEqual("AbBbC", TransformLine(transformer, "AaaBaaaC").Line); // all values are replaced
        }

        [TestMethod]
        public void ReplaceRegex_WithReplaceGroupNumbers_GroupsArePlacedCorrectly()
        {
            // setup
            var transformer = new ReplaceRegexTransformer("(a+)(b+)", "$2$1", false);

            // test
            Assert.AreEqual("AbbbaaB", TransformLine(transformer, "AaabbbB").Line); // groups are placed in the correct order
        }

        [TestMethod]
        public void ReplaceRegex_WithReplaceDelimitedGroupNumbers_GroupsArePlacedCorrectly()
        {
            // setup
            var transformer = new ReplaceRegexTransformer("(a+)(b+)", "$(2)$(1)", false);

            // test
            Assert.AreEqual("AbbbaaB", TransformLine(transformer, "AaabbbB").Line); // groups are placed in the correct order
        }

        [TestMethod]
        public void ReplaceRegex_WithReplaceGroupNames_GroupsArePlacedCorrectly()
        {
            // setup
            var transformer = new ReplaceRegexTransformer("(?<group1>a+)(?<group2>b+)", "$group2$group1", false);

            // test
            Assert.AreEqual("AbbbaaB", TransformLine(transformer, "AaabbbB").Line); // groups are placed in the correct order
        }

        [TestMethod]
        public void ReplaceRegex_WithReplaceDelimitedGroupNames_GroupsArePlacedCorrectly()
        {
            // setup
            var transformer = new ReplaceRegexTransformer("(?<group1>a+)(?<group2>b+)", "$(group2)$(group1)", false);

            // test
            Assert.AreEqual("AbbbaaB", TransformLine(transformer, "AaabbbB").Line); // groups are placed in the correct order
        }

        [TestMethod]
        public void Uniform_Completes_AnnotationLineIsPrepended()
        {
            // setup
            var annotationLine = CreateSourceLine("//@uniform, key1: value1");
            var transformer = new UniformTransformer(CreateAnnotation(annotationLine));

            // test
            CollectionAssert.AreEquivalent(new[] { annotationLine }, TransformLine(transformer, "int value;").PrependLines.ToArray()); // annotation is prepended
        }

        [TestMethod]
        public void Uniform_WithVariableDeclaration_UniformKeywordIsPrepended()
        {
            // setup
            var transformer = new UniformTransformer(CreateAnnotation("//@uniform"));

            // test
            Assert.AreEqual("uniform int value;", TransformLine(transformer, "int value;").Line); // uniform keyword is prepended
        }

        [TestMethod]
        public void Uniform_WithVariableDeclaration_IndentationIsPreserved()
        {
            // setup
            var transformer = new UniformTransformer(CreateAnnotation("//@uniform"));

            // test
            Assert.AreEqual("  uniform int value;", TransformLine(transformer, "  int value;").Line); // indentation is preserved
        }

        [TestMethod]
        public void Uniform_WithVariableDeclarationAndAssignment_AssignmentIsPreserved()
        {
            // setup
            var transformer = new UniformTransformer(CreateAnnotation("//@uniform"));

            // test
            Assert.AreEqual("uniform int value = 1;", TransformLine(transformer, "int value = 1;").Line); // assignment is preserved
        }

        [TestMethod]
        public void Uniform_WithVariableDeclarationAndModifiers_ModifiersAreRemoved()
        {
            // setup
            var transformer = new UniformTransformer(CreateAnnotation("//@uniform"));

            // test
            Assert.AreEqual("uniform int value = 1;", TransformLine(transformer, "private const int value = 1;").Line); // modifiers are removed
        }

        [TestMethod]
        public void Define_WithValueAssignment_DefineValueIsFormattedCorrectly()
        {
            // setup
            var transformer = DefineTransformer.Instance;

            // test
            Assert.AreEqual("#define value 1", TransformLine(transformer, "int value = 1;").Line); // define is formatted correctly
        }

        [TestMethod]
        public void Define_WithValueAssignmentAndModifiers_ModifiersAreRemoved()
        {
            // setup
            var transformer = DefineTransformer.Instance;

            // test
            Assert.AreEqual("#define value 1", TransformLine(transformer, "private const int value = 1;").Line); // modifiers are removed
        }

        [TestMethod]
        public void Define_WithLambdaMethod_MacroIsFormattedCorrectly()
        {
            // setup
            var transformer = DefineTransformer.Instance;

            // test
            Assert.AreEqual("#define func(value) body", TransformLine(transformer, "int func(int value) => body;").Line); // macro is formatted correctly
        }

        [TestMethod]
        public void Define_WithLambdaMethodAndModifiers_ModifiersAreRemoved()
        {
            // setup
            var transformer = DefineTransformer.Instance;

            // test
            Assert.AreEqual("#define func(value) body", TransformLine(transformer, "public static int func(int value) => body;").Line); // modifiers are removed
        }

        [TestMethod]
        public void AddPrefix_Completes_PrefixIsAdded()
        {
            // setup
            var transformer = new AddPrefixTransformer("target");

            // test
            Assert.AreEqual("target source", TransformLine(transformer, "source").Line); // prefix is added
        }

        [TestMethod]
        public void AddPrefix_Completes_IndentationIsPreserved()
        {
            // setup
            var transformer = new AddPrefixTransformer("target");

            // test
            Assert.AreEqual("  target source", TransformLine(transformer, "  source").Line); // prefix is added
        }

        [TestMethod]
        public void Scope_WithScopeDeclaration_ScopeDeclarationIsRemoved()
        {
            // setup
            var transformer = new ScopeTransformer("class");

            // test
            Assert.AreEqual(null, TransformLine(transformer, "public class A : B").Line); // scope declaration is removed
        }

        [TestMethod]
        public void Scope_WithScopeStart_ScopeStartIsRemoved()
        {
            // setup
            var transformer = new ScopeTransformer("namespace");

            // test
            TransformLine(transformer, "namespace A");
            Assert.AreEqual(null, TransformLine(transformer, "{").Line); // scope start is removed
        }

        [TestMethod]
        public void Scope_WithScopeEnd_ScopeEndIsRemoved()
        {
            // setup
            var transformer = new ScopeTransformer("namespace");

            // test
            TransformLine(transformer, "namespace A");
            TransformLine(transformer, "{");
            Assert.AreEqual(null, TransformLine(transformer, "}").Line); // scope end is removed
        }

        [TestMethod]
        public void Scope_WithScopeStarted_FollowingLinesIndentationIsReplaced()
        {
            // setup
            var transformer = new ScopeTransformer("namespace");

            // test
            TransformLine(transformer, "  namespace A");
            TransformLine(transformer, "  {");
            Assert.AreEqual("  line", TransformLine(transformer, "    line").Line); // line indentation is replaced
        }

        [TestMethod]
        public void Scope_WithScopeEnded_FollowingLinesIndentationIsNotReplaced()
        {
            // setup
            var transformer = new ScopeTransformer("namespace");

            // test
            TransformLine(transformer, "  namespace A");
            TransformLine(transformer, "  {");
            TransformLine(transformer, "  }"); // scope end
            Assert.AreEqual("    line", TransformLine(transformer, "    line").Line); // line indentation outside scope is not replaced
        }

        [TestMethod]
        public void Scope_WithFileScopeDeclaration_FileScopeDeclarationIsRemoved()
        {
            // setup
            var transformer = new ScopeTransformer("namespace");

            // test
            Assert.AreEqual(null, TransformLine(transformer, "namespace A;").Line); // file scope declaration is removed
        }

        [TestMethod]
        public void Scope_WithFileScopeDeclaration_FollowingLinesIndentationIsNotReplaced()
        {
            // setup
            var transformer = new ScopeTransformer("namespace");

            // test
            TransformLine(transformer, "  namespace A;"); // file scope with indentation
            Assert.AreEqual("    line", TransformLine(transformer, "    line").Line); // line indentation is not replaced
        }

        [TestMethod]
        public void Array_WithSingleLineArray_ArrayLineIsConverted()
        {
            // setup
            var transformer = new ArrayTransformer();

            // test
            Assert.AreEqual("int[] ( 1, 2, 3 )", TransformLine(transformer, "new int[] { 1, 2, 3 }").Line); // array is converted
        }

        [TestMethod]
        public void Array_WithMultiLineArray_ArrayLinesAreConverted()
        {
            // setup
            var transformer = new ArrayTransformer();

            // test
            Assert.AreEqual("int[]", TransformLine(transformer, "new int[]").Line); // array lines are converted
            Assert.AreEqual("(", TransformLine(transformer, "{").Line);
            Assert.AreEqual("1, 2, 3", TransformLine(transformer, "1, 2, 3").Line);
            Assert.AreEqual(")", TransformLine(transformer, "}").Line);
        }

        [TestMethod]
        public void Array_WithMultiLineArrayAndCommentedArrayEnd_CommentIsSkipped()
        {
            // setup
            var transformer = new ArrayTransformer();

            // test
            Assert.AreEqual("int[] (", TransformLine(transformer, "new int[] {").Line); // array lines are converted
            Assert.AreEqual("1, //}", TransformLine(transformer, "1, //}").Line); // commented array end is skipped
            Assert.AreEqual("2 )", TransformLine(transformer, "2 }").Line);
        }

        [TestMethod]
        public void Array_WithValueFollowsSingleLineArray_ValueIsNotConverted()
        {
            // setup
            var transformer = new ArrayTransformer();

            // test
            Assert.AreEqual("int[] ( 1, 2, 3 ) { value }", TransformLine(transformer, "new int[] { 1, 2, 3 } { value }").Line); // following value is not converted
        }

        [TestMethod]
        public void Array_WithLinesFollowSingleLineArray_LinesAreNotConverted()
        {
            // setup
            var transformer = new ArrayTransformer();

            // test
            TransformLine(transformer, "new int[] { 1, 2, 3 }");
            Assert.AreEqual("{ value }", TransformLine(transformer, "{ value }").Line); // following value is not converted
        }

        [TestMethod]
        public void Array_WithValueFollowsMultiLineArray_LinesAreNotConverted()
        {
            // setup
            var transformer = new ArrayTransformer();

            // test
            TransformLine(transformer, "new int[]"); // multi line array
            TransformLine(transformer, "{");
            TransformLine(transformer, "1, 2, 3");
            Assert.AreEqual(") { value }", TransformLine(transformer, "} { value }").Line); // following value is not converted
        }

        [TestMethod]
        public void Array_WithLinesFollowMultiLineArray_LinesAreNotConverted()
        {
            // setup
            var transformer = new ArrayTransformer();

            // test
            TransformLine(transformer, "new int[]"); // multi line array
            TransformLine(transformer, "{");
            TransformLine(transformer, "1, 2, 3");
            TransformLine(transformer, "}");
            Assert.AreEqual("{ value }", TransformLine(transformer, "{ value }").Line); // following value is not converted
        }

        private static TestTransformContext TransformLine(ILineTransformer transformer, string line)
        {
            var context = new TestTransformContext(CreateSourceLine(line));
            transformer.Transform(context);
            return context;
        }

        private static SourceLine CreateSourceLine(string line)
        {
            var singleLineResource = new FileResource<string>(new FileResourceKey("Test Content"), line);
            return new SourceLine(singleLineResource, line, 0);
        }

        private static ISourceLineAnnotation CreateAnnotation(string line)
        {
            return CreateAnnotation(CreateSourceLine(line));
        }

        private static ISourceLineAnnotation CreateAnnotation(SourceLine sourceLine)
        {
            var annotationParser = new SourceLineAnnotationParser(false);
            return annotationParser.Parse(sourceLine);
        }
    }
}
