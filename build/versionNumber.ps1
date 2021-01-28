   $v0=$env:BUILD_BUILDNUMBER
   $v="1.0.0.0"
   if($env:BUILD_SOURCEBRANCHNAME -eq "master")
   {
      $v="0.0.0$v0"
   }
   elseif($env:BUILD_SOURCEBRANCHNAME -eq "Develop")
   {
      $v="0.2.1$v0"
   }
   elseif($env:BUILD_SOURCEBRANCHNAME -eq "Release")
   {
      $v="0.1.0$v0"
   }
   elseif($env:BUILD_SOURCEBRANCHNAME -eq "merge")
   {
      $v="0.1.0$v0"
   }
   Write-Host $env:BUILD_SOURCEBRANCHNAME
   Write-Host "##vso[task.setvariable variable=packageVersion;]$v"
   Write-Host "##vso[build.updatebuildnumber]$v"