Param([Parameter(Mandatory=$true)][string]$TargetUrl, 
    [Bool]$TrustAllCertsPolicy = $False) 
if ($TrustAllCertsPolicy) {
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
}

Write-Output "Requesting $TargetUrl"

$Response=Invoke-RestMethod -TimeoutSec 1800 -Uri $TargetUrl -Method get -UseBasicParsing
$lastStatusCode=$Response.status
Write-Output "Received http status code $($lastStatusCode) from $TargetUrl"
    
if($lastStatusCode -ne "Healthy")
{
    Write-Error "Operation Failed, App is not healthy=$($lastStatusCode)" -ErrorAction Stop
}

