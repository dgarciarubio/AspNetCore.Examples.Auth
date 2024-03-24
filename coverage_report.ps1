Get-ChildItem -Path . -Directory -Filter TestResults -Recurse | 
  Remove-Item -Force -Recurse -ErrorAction SilentlyContinue

dotnet test --collect:"XPlat Code Coverage"

dotnet tool update dotnet-reportgenerator-globaltool -g
reportgenerator `
  -reports:".\**\TestResults\**\coverage.cobertura.xml" `
  -targetdir:".\TestResults\Report\" `
  -reporttypes:Html