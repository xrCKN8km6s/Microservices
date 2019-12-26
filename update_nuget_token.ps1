Param (
	[Parameter(Position=0)][string] $current = $(throw "-current is required"),
	[Parameter(Position=1)][string] $new = $(throw "-new is required")
)

(Get-ChildItem -Filter NuGet.Config -Recurse -Depth 2) | % {$f=$_; (Get-Content $_.PSPath) | % {$_ -replace $current, $new} | Set-Content $f.PSPath }
