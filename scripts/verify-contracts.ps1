[CmdletBinding()]
param(
    [string] $DotNet = "dotnet"
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$testProject = Join-Path $repositoryRoot "tests\VehicleHandlers.Contracts.Tests\VehicleHandlers.Contracts.Tests.csproj"

& $DotNet run --project $testProject --configuration Release
if ($LASTEXITCODE) { exit $LASTEXITCODE }
