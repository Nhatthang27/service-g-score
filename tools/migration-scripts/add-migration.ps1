param(
  [Parameter(Mandatory = $true)]
  [string]$MigrationName
)

# Script is now located at: <root>\tools\migration-scripts\add-migration.ps1
# So root = go up 2 levels from $PSScriptRoot
$rootDir = Resolve-Path (Join-Path $PSScriptRoot "..\..")

$apiProject = Join-Path $rootDir "src\GScore.Presentation\GScore.Presentation.csproj"
$infraProject = Join-Path $rootDir "src\GScore.Infrastructure\GScore.Infrastructure.csproj"
$outputDir = Join-Path $rootDir "src\GScore.Infrastructure\Data\Migrations"

Write-Host "RootDir: $rootDir" -ForegroundColor DarkGray
Write-Host "Adding migration: $MigrationName" -ForegroundColor Green

dotnet ef migrations add $MigrationName `
  --startup-project $apiProject `
  --project $infraProject `
  --output-dir "Data\Migrations"

if ($LASTEXITCODE -eq 0) {
  Write-Host "Migration '$MigrationName' created successfully!" -ForegroundColor Green
}
else {
  Write-Host "Failed to create migration" -ForegroundColor Red
  exit 1
}
