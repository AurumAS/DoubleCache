// include Fake lib
#r @"tools/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing.XUnit2

let buildDir = "./build/"
let testDir = "./test/"

let references  = !! "source/DoubleCache/*.csproj"
let testReferences = !! "source/DoubleCacheTests/*.csproj"    
               
// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Build" (fun _ ->
    MSBuild buildDir "Build" [ "Configuration", "Debug" ] references
        |> Log "AppBuild-Output: ")

Target "BuildTest" (fun _ ->
    MSBuild testDir "Build" [ "Configuration", "Debug" ] testReferences
        |> Log "TestBuild-Output: ")

Target "xUnitTest" (fun _ ->
    !! (testDir @@ "*Tests.dll")
    |> xUnit2 (fun p -> { p with 
        HtmlOutputPath = Some ("xunit.html");
        WorkingDir = Some(testDir) })
)

// Dependencies
"Clean" 
 ==> "Build"
 ==> "BuildTest"
 ==> "xUnitTest"
 
// start build
RunTargetOrDefault "xUnitTest"