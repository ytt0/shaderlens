using System;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace Shaderlens.BuildTasks
{
    public class VersionTask : Microsoft.Build.Utilities.Task
    {
        private const string VersionPattern = "v?(?<major>[0-9]+)\\.(?<minor>[0-9]+)(\\.(?<patch>[0-9]+))?(-(?<label>.*))?";

        private static readonly Regex DescribeRegex = new Regex("^((?<tag>.*)-g)?(?<sha>[0-9a-f]{40})(?<dirty>-dirty)?$");
        private static readonly Regex DescribeTagRegex = new Regex($"{VersionPattern}-(?<commits>[0-9]+)");
        private static readonly Regex ReleaseBranchVersionRegex = new Regex($"^release/{VersionPattern}$");

        // Output of: git describe --tags --always --long --dirty --abbrev=40
        [Required]
        public string GitDescribeOutput { get; set; }

        // Output of: git branch --show-current
        [Required]
        public string GitBranchOutput { get; set; }

        // Version first number
        [Output]
        public int Major { get; set; }

        // Version second number
        [Output]
        public int Minor { get; set; }

        // Version third number
        [Output]
        public int Patch { get; set; }

        // Optional version suffix (separated by "-")
        [Output]
        public string Label { get; set; }

        // Number of commits since the base tagged commit
        [Output]
        public int Commits { get; set; }

        // Current commit hash
        [Output]
        public string Sha { get; set; }

        // Any uncommited changes
        [Output]
        public bool Dirty { get; set; }

        // Formatted value: {Major}.{Minor}.{Patch}
        [Output]
        public string MajorMinorPatch { get; set; }

        // Formatted value: {MajorMinorPatch}.0
        [Output]
        public string AssemblyVersion { get; set; }

        // Formatted value: {MajorMinorPatch}[-{Label}]
        [Output]
        public string SemVer { get; set; }

        // Formatted value: {SemVer}[+{Commits}]
        [Output]
        public string FullSemVer { get; set; }

        // Formatted value: {FullSemVer}.Branch.{Branch}.Sha.{Sha}[-modified]
        [Output]
        public string InformationalVersion { get; set; }

        public override bool Execute()
        {
            ParseDescribeValue(this.GitDescribeOutput ?? String.Empty, out var major, out var minor, out var patch, out var label, out var commits, out var sha, out var dirty, out var tagMatched);

            if (commits > 0 || dirty || !tagMatched)
            {
                if (TryParseReleaseBranchVersion(this.GitBranchOutput ?? String.Empty, out var branchMajor, out var branchMinor, out var branchPatch, out var branchLabel, out var branchPatchMatched))
                {
                    if (major == branchMajor && minor == branchMinor && !branchPatchMatched)
                    {
                        patch++;
                    }
                    else
                    {
                        major = branchMajor;
                        minor = branchMinor;
                        patch = branchPatch;
                    }

                    label = branchLabel;

                    if (String.IsNullOrEmpty(label))
                    {
                        label = "preview";
                    }
                }
                else
                {
                    minor++;
                    patch = 0;
                    label = "dev";
                }
            }

            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            this.Label = label;
            this.Commits = commits;
            this.Sha = sha;
            this.Dirty = dirty;

            this.MajorMinorPatch = $"{major}.{minor}.{patch}";
            this.AssemblyVersion = this.MajorMinorPatch + ".0";
            this.SemVer = this.MajorMinorPatch + AppendSegment($"-{label}", !String.IsNullOrEmpty(label));
            this.FullSemVer = this.SemVer + AppendSegment($"+{commits}", commits > 0);
            this.InformationalVersion = $"{this.FullSemVer}." + AppendSegment($"Branch.{this.GitBranchOutput?.Replace('/', '-')}.", !String.IsNullOrEmpty(this.GitBranchOutput) && this.GitBranchOutput != "HEAD") + $"Sha.{sha}" + AppendSegment("-modified", dirty);

            return true;
        }

        private static bool TryParseReleaseBranchVersion(string value, out int major, out int minor, out int patch, out string label, out bool patchMatched)
        {
            major = 0;
            minor = 0;
            patch = 0;
            label = null;
            patchMatched = false;

            var match = ReleaseBranchVersionRegex.Match(value);
            if (match.Success)
            {
                GetVersionMatchValues(match, out major, out minor, out patch, out label, out patchMatched);
                return true;
            }

            return false;
        }

        private static void ParseDescribeValue(string value, out int major, out int minor, out int patch, out string label, out int commits, out string sha, out bool dirty, out bool tagMatched)
        {
            major = 0;
            minor = 0;
            patch = 0;
            label = null;
            commits = 0;
            sha = null;
            dirty = false;
            tagMatched = false;

            var match = DescribeRegex.Match(value);
            if (match.Success)
            {
                var tag = match.Groups["tag"].Value;
                sha = match.Groups["sha"].Value;
                dirty = !String.IsNullOrEmpty(match.Groups["dirty"].Value);

                match = DescribeTagRegex.Match(tag);
                if (match.Success)
                {
                    GetVersionMatchValues(match, out major, out minor, out patch, out label, out _);
                    commits = Int32.Parse(match.Groups["commits"].Value);
                    tagMatched = true;
                }
            }
        }

        private static void GetVersionMatchValues(Match match, out int major, out int minor, out int patch, out string label, out bool patchMatched)
        {
            major = Int32.Parse(match.Groups["major"].Value);
            minor = Int32.Parse(match.Groups["minor"].Value);
            patchMatched = match.Groups["patch"].Length > 0;
            patch = patchMatched ? Int32.Parse(match.Groups["patch"].Value) : 0;
            label = match.Groups["label"].Value;
        }

        private static string AppendSegment(string value, bool condition)
        {
            return condition ? value : String.Empty;
        }
    }
}