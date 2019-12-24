param (
	[string] $current = "TOKEN",
	[string] $new = $(throw "-new is required")
)

(Get-ChildItem -Filter NuGet.Config -Recurse -Depth 2) | % {$f=$_; (Get-Content $_.PSPath) | % {$_ -replace $current, $new} | Set-Content $f.PSPath }
