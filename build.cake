#tool nuget:?package=GitVersion.CommandLine&version=3.6.5

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore();
    //NuGetRestore("./XUnitTestProject1.sln");
});

Task("Version")
    .Does(() =>
{
    var version = GitVersion();
    Information($"SemVer = {version.SemVer}");
    Information($"AssemblySemVer = ${version.AssemblySemVer}");

	if (!BuildSystem.IsLocalBuild) {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true
        });
    }
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Version")
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
    // for .net framework
    // #tool nuget:?package=xunit.runner.console&version=2.3.1
    // XUnit2($"./**/bin/{configuration}/CakeSample.Test.dll", 
    //      new XUnit2Settings {
    //          Parallelism = ParallelismOption.All
    //      });

    // for .net core
    DotNetCoreTest("./CakeSample.Test/CakeSample.Test.csproj", 
        new DotNetCoreTestSettings {
            Configuration = configuration,
            NoBuild = true
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