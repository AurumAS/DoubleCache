// include Fake lib
#r @"tools/FAKE/tools/FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open Fake.Git
open Fake.Testing.XUnit2

let buildDir = "./build/"
let testDir = "./test/"
let nugetDir = "./nuget"

let references  = !! "source/DoubleCache/*.csproj"
let testReferences = !! "source/DoubleCacheTests/*.csproj"    

let version = "1.1.1"
let commitHash = Information.getCurrentSHA1(".")

let projectName = "DoubleCache"
let projectDescription = "Layered distributed cache-aside implementation"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; testDir; nugetDir]
)

Target "RestoreNugetPackages" (fun _ ->
    !! "source/DoubleCache*/packages.config"
    |> Seq.iter(RestorePackage (fun p -> { p with ToolPath = "./tools/nuget/nuget.exe"
                                                  OutputPath = "source/packages"})))
Target "Build" (fun _ ->
    CreateCSharpAssemblyInfo "./source/DoubleCache/Properties/AssemblyInfo.cs"
        [Attribute.Title projectName
         Attribute.Description projectDescription
         Attribute.Guid "505f87a8-3062-4070-af1f-cd7358ccd06a"
         Attribute.Product projectName
         Attribute.Version version
         Attribute.InformationalVersion version
         Attribute.FileVersion version
         Attribute.Metadata("githash", commitHash)]
    MSBuild buildDir "Build" [ "Configuration", "Release" ] references
        |> Log "AppBuild-Output: ")

Target "BuildTest" (fun _ ->
    MSBuild testDir "Build" [ "Configuration", "Release" ] testReferences
        |> Log "TestBuild-Output: ")

Target "xUnitTest" (fun _ ->
    !! (testDir @@ "*Tests.dll")
    |> xUnit2 (fun p -> 
        { p with 
            HtmlOutputPath = Some ("xunit.html");
            WorkingDir = Some(testDir) })
)

Target "Default" DoNothing

Target "CreateNuget" (fun _ ->
    // Copy all the package files into a package folder
    NuGet (fun p -> 
        {p with
            Authors = ["Harald Schult Ulriksen / Aurum AS"]
            Project = projectName
            Description = projectDescription                               
            OutputPath = nugetDir
            WorkingDir = buildDir
            Version = version
            Publish = false
            Dependencies = [
                "StackExchange.Redis", "1.0"
                "MsgPack.Cli", "0.6.5"
            ]
            Files = [
                (@"DoubleCache.*", Some @"lib\net46", None)
            ]
        }) 
        "DoubleCache.nuspec"
)

// Dependencies
"Clean" 
 ==> "RestoreNugetPackages"
 ==> "Build"
 ==> "BuildTest"
 ==> "xUnitTest"
 ==> "Default"
 ==> "CreateNuget"
 
// start build
RunTargetOrDefault "Default"
