@echo off
cls
if not exist ".\tools\FAKE\tools\Fake.exe" (
    ".\tools\nuget\nuget.exe" "install" "FAKE" "-OutputDirectory" "tools" "-ExcludeVersion"
)
if not exist ".\tools\xUnit.runner.console\tools\xunit.console.exe" (
   ".\tools\nuget\nuget.exe" "install" "xunit.runner.console" "-OutputDirectory" "tools" "-ExcludeVersion"
)
"tools\FAKE\tools\Fake.exe" build.fsx %*