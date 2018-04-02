#tool nuget:?package=xunit.runner.console&version=2.3.1
#tool nuget:?package=xunit.runner.visualstudio&version=2.3.1

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory($"./ClassLibrary1/bin/");
    CleanDirectory("./CakeSample.Test/bin/");
    CleanDirectory("./ConsoleApp1/bin/");
    CleanDirectory($"./ClassLibrary1/obj/");
    CleanDirectory("./CakeSample.Test/obj/");
    CleanDirectory("./ConsoleApp1/obj/");
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
    DotNetCoreRestore();
    //NuGetRestore("./XUnitTestProject1.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
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
    // XUnit2("./**/bin/" + configuration + "/netcoreapp2.0/CakeSample.Test.dll", new XUnit2Settings {
    //     Parallelism = ParallelismOption.All
    // });

    // for .net core
    DotNetCoreTest("./CakeSample.Test/CakeSample.Test.csproj", new DotNetCoreTestSettings {
        NoBuild = false
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