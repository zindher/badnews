#!/usr/bin/env pwsh
<#
.SYNOPSIS
BadNews Automated Test Runner
Runs unit and integration tests with coverage reporting

.DESCRIPTION
This script:
- Runs all unit tests
- Runs all integration tests
- Generates coverage reports
- Displays summary statistics
- Exits with appropriate error codes

.PARAMETER TestType
Specify which tests to run: All, Unit, Integration

.PARAMETER GenerateCoverage
Generate code coverage report (requires coverlet)

.PARAMETER Verbose
Show detailed test output

.EXAMPLE
./run-tests.ps1 -TestType All -GenerateCoverage
./run-tests.ps1 -TestType Unit
#>

param(
    [ValidateSet("All", "Unit", "Integration")]
    [string]$TestType = "All",
    
    [switch]$GenerateCoverage,
    [switch]$Verbose
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Define colors
$colors = @{
    Success = "Green"
    Error = "Red"
    Warning = "Yellow"
    Info = "Cyan"
}

function Write-Info {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $colors.Info
}

function Write-Success {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $colors.Success
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $colors.Error
}

function Write-Warning-Custom {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $colors.Warning
}

# Get script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendDir = Join-Path $scriptDir ".."

# Check if we're in the backend directory
if (!(Test-Path (Join-Path $backendDir "BadNews.csproj"))) {
    Write-Error-Custom "Error: BadNews.csproj not found in $backendDir"
    exit 1
}

Write-Info "BadNews Test Runner"
Write-Info "==================="
Write-Info ""
Write-Info "Test Type: $TestType"
Write-Info "Generate Coverage: $GenerateCoverage"
Write-Info "Verbose: $Verbose"
Write-Info ""

# Build the project first
Write-Info "Building project..."
$buildOutput = dotnet build $backendDir -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error-Custom "Build failed!"
    exit 1
}
Write-Success "Build successful!"
Write-Info ""

# Prepare test arguments
$testArgs = @("test")
$testArgs += $backendDir
$testArgs += "-c", "Release"
$testArgs += "--no-build"

if ($Verbose) {
    $testArgs += "-v", "n"
    $testArgs += "--logger", "console;verbosity=detailed"
}

# Handle coverage
if ($GenerateCoverage) {
    $testArgs += "/p:CollectCoverage=true"
    $testArgs += "/p:CoverageFormat=opencover"
    $testArgs += "/p:CoverageDirectory=$backendDir/coverage"
}

# Filter by test type
switch ($TestType) {
    "Unit" {
        $testArgs += "--filter", "ClassName=UnitTests"
        Write-Info "Running UNIT TESTS..."
    }
    "Integration" {
        $testArgs += "--filter", "ClassName=IntegrationTests"
        Write-Info "Running INTEGRATION TESTS..."
    }
    "All" {
        Write-Info "Running ALL TESTS (Unit + Integration)..."
    }
}

Write-Info ""

# Run tests
$startTime = Get-Date
$testOutput = & dotnet @testArgs
$testExitCode = $LASTEXITCODE
$endTime = Get-Date
$duration = ($endTime - $startTime).TotalSeconds

# Display results
Write-Info ""
Write-Info "Test Execution Complete"
Write-Info "======================="

if ($testExitCode -eq 0) {
    Write-Success "✅ All tests PASSED"
} else {
    Write-Error-Custom "❌ Some tests FAILED"
}

Write-Info "Duration: $([Math]::Round($duration, 2)) seconds"
Write-Info ""

# Parse and display summary
if ($testOutput -match "(\d+) test\(s\) passed") {
    $passedTests = $matches[1]
    Write-Success "✓ Passed: $passedTests"
}

if ($testOutput -match "(\d+) test\(s\) failed") {
    $failedTests = $matches[1]
    Write-Error-Custom "✗ Failed: $failedTests"
}

if ($testOutput -match "(\d+) test\(s\) skipped") {
    $skippedTests = $matches[1]
    Write-Warning-Custom "⊘ Skipped: $skippedTests"
}

Write-Info ""

# Display coverage report if generated
if ($GenerateCoverage -and (Test-Path "$backendDir/coverage/coverage.opencover.xml")) {
    Write-Info "Coverage report generated: $backendDir/coverage/coverage.opencover.xml"
    
    # Try to display coverage summary using ReportGenerator if available
    $reportGen = Get-Command reportgenerator -ErrorAction SilentlyContinue
    if ($reportGen) {
        Write-Info "Generating HTML coverage report..."
        & reportgenerator -reports:"$backendDir/coverage/coverage.opencover.xml" `
                         -targetdir:"$backendDir/coverage/report" `
                         -reporttypes:"HtmlSummary"
        Write-Success "Coverage report: $backendDir/coverage/report/index.html"
    }
}

Write-Info ""

# Exit with test exit code
exit $testExitCode
