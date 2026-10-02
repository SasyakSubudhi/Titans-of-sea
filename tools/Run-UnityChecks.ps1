[CmdletBinding()]
param(
    [ValidateSet('Initialize','Sandbox','EditMode','PlayMode')]
    [string]$Mode = 'Sandbox'
)
$ErrorActionPreference = 'Stop'
$kitRoot = Split-Path $PSScriptRoot -Parent
$projectRoot = $kitRoot
$logRoot = Join-Path $kitRoot '.cache\unity'
[IO.Directory]::CreateDirectory($logRoot) | Out-Null
$editor = 'C:\Program Files\Unity\Hub\Editor\6000.3.25f1\Editor\Unity.exe'
if (-not (Test-Path -LiteralPath $editor)) { throw 'Unity 6000.3.25f1 is not installed at its default location.' }
$unityArgs = @('-batchmode','-nographics','-projectPath',('"' + $projectRoot + '"'),'-logFile',('"' + (Join-Path $logRoot ($Mode + '.log')) + '"'))
switch ($Mode) {
    'Initialize' { $unityArgs += '-quit' }
    'Sandbox' { $unityArgs += @('-quit','-executeMethod','TitansOfTheSea.Core.Editor.BSandboxBuilder.CreateBatch') }
    'EditMode' { $unityArgs += @('-runTests','-testPlatform','EditMode','-assemblyNames','TitansOfTheSea.Tests.EditMode','-testResults',('"' + (Join-Path $logRoot 'EditMode.xml') + '"')) }
    'PlayMode' { $unityArgs += @('-runTests','-testPlatform','PlayMode','-assemblyNames','TitansOfTheSea.Tests.PlayMode','-testResults',('"' + (Join-Path $logRoot 'PlayMode.xml') + '"')) }
}
$taskProcess = Start-Process -FilePath $editor -ArgumentList $unityArgs -WindowStyle Hidden -PassThru
Write-Output "Started ${Mode}: Unity PID $($taskProcess.Id). Logs: $logRoot"

