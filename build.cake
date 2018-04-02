//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory($"./ClassLibrary1/bin/{configuration}/netcoreapp2.0");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Version")
    .Does(() =>
{
    var version = GitVersion(new GitVersionSettings{
        UpdateAssemblyInfo = true
    });
    Information($"SemVer = {version.SemVer}");
    Information($"AssemblySemVer = ${version.AssemblySemVer}");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./XUnitTestProject1.sln");
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild("./XUnitTestProject1.sln", new MSBuildSettings() {
        Configuration = configuration
    });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
        NoResults = true
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);