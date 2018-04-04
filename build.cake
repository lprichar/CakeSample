#tool nuget:?package=GitVersion.CommandLine&version=3.6.5
#tool vswhere
#tool nuget:?package=xunit.runner.visualstudio&version=2.3.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var testDirectory = Directory("./CakeSample.Test/");
var testLocation = testDirectory.Path + $"/**/bin/{configuration}/netcoreapp2.0/*.Test.dll";

Task("Clean")
    .Does(() =>
{
    CleanDirectories($"./**/bin/{configuration}/netcoreapp2.0");
    CleanDirectories($"./**/obj/{configuration}/netcoreapp2.0");
    CleanDirectories($"./TestResults");
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

// the VSTest task has a bug where it doesn't look in the right
// location for vstest.console.exe, see 
// https://github.com/cake-build/cake/issues/1522#issuecomment-341612194
VSTestSettings FixToolPath(VSTestSettings settings)
{
  settings.ToolPath = VSWhereLatest(new VSWhereLatestSettings { 
    Requires = "Microsoft.VisualStudio.PackageGroup.TestTools.Core" 
  }).CombineWithFilePath(File(@"Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"));
  return settings;
}

Task("Generate-Coverage")
    .IsDependentOn("Build")
    .Does(() =>
{
    VSTest(testLocation, FixToolPath(new VSTestSettings
    {
        EnableCodeCoverage = true,
        // use separate process to get accurate coverage data
        InIsolation = true, 
        // VSTS only speaks trx
        Logger = "trx", 
        // run VSTest with the xunit adapter
        TestAdapterPath = 
          "tools/xunit.runner.visualstudio.2.3.1/build/_common" 
    }));
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