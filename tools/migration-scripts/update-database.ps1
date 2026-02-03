# Script location: <root>\tools\migration-scripts\update-database.ps1
# Root = go up 2 levels from $PSScriptRoot
$rootDir = Resolve-Path (Join-Path $PSScriptRoot "..\..")

$apiProject = Join-Path $rootDir "src\GScore.Presentation\GScore.Presentation.csproj"
$infraProject = Join-Path $rootDir "src\GScore.Infrastructure\GScore.Infrastructure.csproj"

Write-Host "RootDir: $rootDir" -ForegroundColor DarkGray
Write-Host "Updating database..." -ForegroundColor Green

dotnet ef database update `
  --startup-project $apiProject `
  --project $infraProject

if ($LASTEXITCODE -eq 0) {
  Write-Host "Database updated successfully!" -ForegroundColor Green
}
else {
  Write-Host "Failed to update database" -ForegroundColor Red
  exit 1
}
