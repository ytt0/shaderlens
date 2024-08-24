namespace Shaderlens.Tests.BuildTasks
{
    [TestClass]
    public class VersionTaskTests
    {
        private const string DefaultSha = "0123456789abcdef0123456789abcdef01234567";
        private const string DefaultBranch = "test";

        [TestMethod]
        public void Execute_WithoutInputs_InitialVersionIsReturned()
        {
            var task = new VersionTask(); // no inputs
            task.Execute();

            AssertResult(task, 0, 1, 0, "dev", 0, null, false);
        }

        [TestMethod]
        public void Execute_WithoutTag_InitialVersionWithShaIsReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput(null, 0, DefaultSha, false), // without tag
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 0, 1, 0, "dev", 0, DefaultSha, false);
        }

        [TestMethod]
        public void Execute_WithoutTagAndDirtyState_InitialVersionWithShaIsReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput(null, 0, DefaultSha, true), // dirty state
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 0, 1, 0, "dev", 0, DefaultSha, true);
        }

        [TestMethod]
        public void Execute_WithInvalidTag_InitialVersionWithShaIsReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("tag-name", 0, DefaultSha, false), // "tag-name" cannot be parsed as a version and a label
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 0, 1, 0, "dev", 0, DefaultSha, false);
        }

        [TestMethod]
        public void Execute_WithTag_TagVersionIsReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 0, DefaultSha, false), // tag contains a valid a version without a label
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 1, 2, 3, "", 0, DefaultSha, false); // version is matching
        }

        [TestMethod]
        public void Execute_WithTagVersionPrefix_TagVersionIsReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("v1.2.3", 0, DefaultSha, false), // tag version starts with "v"
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 1, 2, 3, "", 0, DefaultSha, false); // version is matching
        }

        [TestMethod]
        public void Execute_WithTagWithoutPatchNumber_VersionWithDefaultPatchNumberIsReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2", 0, DefaultSha, false), // tag version only contains major and minor
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 1, 2, 0, "", 0, DefaultSha, false); // patch number is 0
        }

        [TestMethod]
        public void Execute_WithTagAndLabel_TagVersionAndLabelAreReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-label-123", 0, DefaultSha, false), // tag contains a valid version with a label
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 1, 2, 3, "label-123", 0, DefaultSha, false); // version and label are matching
        }

        [TestMethod]
        public void Execute_WithCommits_MinorVersionIsIncrementedAndDefaultLabelIsSet()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-label", 4, DefaultSha, false), // 4 commits after tag
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 1, 3, 0, "dev", 4, DefaultSha, false); // minor version was incremented and default label was set
        }

        [TestMethod]
        public void Execute_WithDirtyState_MinorVersionIsIncrementedAndDefaultLabelIsSet()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-label", 0, DefaultSha, true), // dirty state
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            AssertResult(task, 1, 3, 0, "dev", 0, DefaultSha, true); // minor version was incremented and default label was set
        }

        [TestMethod]
        public void Execute_WithInvalidReleaseBranchVersion_TagVersionIsUsed()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 4, DefaultSha, false), // 4 commits after tag
                GitBranchOutput = "release/invalid-version", // release branch with invalid version
            };

            task.Execute();

            AssertResult(task, 1, 3, 0, "dev", 4, DefaultSha, false); // minor version was incremented and default label was set
        }

        [TestMethod]
        public void Execute_WithReleaseBranchAndCleanState_TagVersionIsUsed()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-label", 0, DefaultSha, false), // no commits and clean state
                GitBranchOutput = "release/2.0.0", // release branch
            };

            task.Execute();

            AssertResult(task, 1, 2, 3, "label", 0, DefaultSha, false); // release branch version was not used
        }

        [TestMethod]
        public void Execute_WithReleaseBranchLabelAndCommits_ReleaseBranchVersionAndLabelAreSet()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 4, DefaultSha, false), // 4 commits after tag
                GitBranchOutput = "release/2.0.0-release-label", // release branch with label
            };

            task.Execute();

            AssertResult(task, 2, 0, 0, "release-label", 4, DefaultSha, false); // release branch version and label were set
        }

        [TestMethod]
        public void Execute_WithReleaseBranchLabelAndDirtyState_ReleaseBranchVersionAndLabelAreSet()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 0, DefaultSha, true), // dirty state
                GitBranchOutput = "release/2.0.0-release-label", // release branch with label
            };

            task.Execute();

            AssertResult(task, 2, 0, 0, "release-label", 0, DefaultSha, true); // release branch version and label were set
        }

        [TestMethod]
        public void Execute_WithReleaseBranchAndNoLabel_DefaultReleaseLabelIsSet()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 4, DefaultSha, false), // 4 commits after tag
                GitBranchOutput = "release/2.0.0", // release branch without label
            };

            task.Execute();

            AssertResult(task, 2, 0, 0, "preview", 4, DefaultSha, false); // default release branch label was set
        }

        [TestMethod]
        public void Execute_WithReleaseBranchVersionPrefix_ReleaseBranchVersionIsSet()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 4, DefaultSha, false), // 4 commits after tag
                GitBranchOutput = "release/v2.0.0", // release branch version starts with "v"
            };

            task.Execute();

            AssertResult(task, 2, 0, 0, "preview", 4, DefaultSha, false); // branch version is matching
        }

        [TestMethod]
        public void Execute_WithReleaseBranchWithOnlyMajorMinorVersion_DefaultPatchNumberIsReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 4, DefaultSha, false), // 4 commits after tag
                GitBranchOutput = "release/2.3", // release branch version only contains major and minor
            };

            task.Execute();

            AssertResult(task, 2, 3, 0, "preview", 4, DefaultSha, false); // default patch number 0 is used
        }

        [TestMethod]
        public void Execute_WithReleaseBranchWithOnlyMajorMinorVersionAndMatchingTag_TagPatchNumberIsIncremented()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 5, DefaultSha, false), // tag with patch number 3
                GitBranchOutput = "release/1.2", // release branch version only contains major and minor
            };

            task.Execute();

            AssertResult(task, 1, 2, 4, "preview", 5, DefaultSha, false); // tag patch number is used, incremented to 4
        }

        [TestMethod]
        public void Execute_WithReleaseBranchVersionAndMatchingTag_BranchPatchNumberIsReturned()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 5, DefaultSha, false), // tag with patch number 3
                GitBranchOutput = "release/1.2.0", // release branch version contains patch number
            };

            task.Execute();

            AssertResult(task, 1, 2, 0, "preview", 5, DefaultSha, false); // release branch patch number is used
        }

        [TestMethod]
        public void Execute_WithVersion_MajorMinorPatchContainsVersion()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 0, DefaultSha, false),
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            Assert.AreEqual("1.2.3", task.MajorMinorPatch); // MajorMinorPatch was formatted correctly
        }

        [TestMethod]
        public void Execute_WithVersion_AssemblyVersionContainsVersion()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 0, DefaultSha, false),
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            Assert.AreEqual("1.2.3.0", task.AssemblyVersion); // assembly version was formatted correctly
        }

        [TestMethod]
        public void Execute_WithVersion_SemVerContainsVersion()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 0, DefaultSha, false),
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            Assert.AreEqual("1.2.3", task.SemVer); // semver was formatted correctly
        }

        [TestMethod]
        public void Execute_WithVersionAndLabel_SemVerContainsVersionAndLabel()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3-tag-label", 0, DefaultSha, false),
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            Assert.AreEqual("1.2.3-tag-label", task.SemVer); // semver contains label
        }

        [TestMethod]
        public void Execute_WithCommits_FullSemVerContainsCommits()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 4, DefaultSha, false), // 4 commits
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            Assert.AreEqual("1.3.0-dev+4", task.FullSemVer); // full semver contains 4 commits
        }

        [TestMethod]
        public void Execute_WithoutCommits_FullSemVerDoesNotContainCommits()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 0, DefaultSha, false), // clean state
                GitBranchOutput = DefaultBranch,
            };

            task.Execute();

            Assert.AreEqual("1.2.3", task.FullSemVer); // full semver does not contain commits
        }

        [TestMethod]
        public void Execute_WithDetachedHead_InformationalVersionContainsEscapedBranch()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 4, DefaultSha, false),
                GitBranchOutput = "HEAD", // detached head
            };

            task.Execute();

            Assert.AreEqual($"1.3.0-dev+4.Sha.{DefaultSha}", task.InformationalVersion); // informational version does not contain .Branch segment
        }

        [TestMethod]
        public void Execute_WithoutEmptyBranchOutput_InformationalVersionContainsEscapedBranch()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 4, DefaultSha, false),
                GitBranchOutput = null, // no branch output
            };

            task.Execute();

            Assert.AreEqual($"1.3.0-dev+4.Sha.{DefaultSha}", task.InformationalVersion); // informational version does not contain .Branch segment
        }

        [TestMethod]
        public void Execute_WithBranch_InformationalVersionContainsEscapedBranch()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 4, DefaultSha, false),
                GitBranchOutput = "feature/branch/1.2.3", // branch
            };

            task.Execute();

            Assert.AreEqual($"1.3.0-dev+4.Branch.feature-branch-1.2.3.Sha.{DefaultSha}", task.InformationalVersion); // informational version contains escaped branch
        }

        [TestMethod]
        public void Execute_WithDirtyState_InformationalVersionContainsModifiedSuffix()
        {
            var task = new VersionTask
            {
                GitDescribeOutput = FormatDescribeOutput("1.2.3", 4, DefaultSha, true), // dirty state
                GitBranchOutput = "feature/branch/1.2.3",
            };

            task.Execute();

            Assert.AreEqual($"1.3.0-dev+4.Branch.feature-branch-1.2.3.Sha.{DefaultSha}-modified", task.InformationalVersion); // informational version contains "-modified" suffix
        }

        private static void AssertResult(VersionTask task, int expectedMajor, int expectedMinor, int expectedPatch, string? expectedLabel, int expectedCommits, string? expectedSha, bool expectedDirty)
        {
            Assert.AreEqual(expectedMajor, task.Major);
            Assert.AreEqual(expectedMinor, task.Minor);
            Assert.AreEqual(expectedPatch, task.Patch);
            Assert.AreEqual(expectedLabel, task.Label);
            Assert.AreEqual(expectedCommits, task.Commits);
            Assert.AreEqual(expectedSha, task.Sha);
            Assert.AreEqual(expectedDirty, task.Dirty);
        }

        private static string FormatDescribeOutput(string? tag, int commits, string sha, bool dirty)
        {
            return (tag != null ? $"{tag}-{commits}-g{sha}" : sha) + (dirty ? "-dirty" : "");
        }
    }
}