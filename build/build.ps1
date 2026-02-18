param(
  [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "Restoring packages..."
dotnet restore

Write-Host "Building Teacher and Student..."
dotnet build .\src\Alfarid.Teacher\Alfarid.Teacher.csproj -c $Configuration
dotnet build .\src\Alfarid.Student\Alfarid.Student.csproj -c $Configuration

Write-Host "Running unit tests..."
dotnet test .\tests\Alfarid.Tests.Unit\Alfarid.Tests.Unit.csproj -c $Configuration
