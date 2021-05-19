function Load-MicrosoftApplicationInsights {
	param(
        [Parameter(Mandatory = $true)]
        [string] $MicrosoftApplicationInsightsPath
    )

	$bytes = [System.IO.File]::ReadAllBytes($MicrosoftApplicationInsightsPath)
	[System.Reflection.Assembly]::Load($bytes) | Out-Null
}

function Get-TelemetryClient {
	param(
        [Parameter(Mandatory = $true)]
        [string] $InstrumentationKey
    )

	$configuration = New-Object Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration -ArgumentList $InstrumentationKey
	$client = New-Object Microsoft.ApplicationInsights.TelemetryClient -ArgumentList $configuration

	return $client
}

function Initialize-Telemetry {
	param(
		[Parameter(Mandatory = $true)]
        [string] $MicrosoftApplicationInsightsPath,

        [Parameter(Mandatory = $true)]
        [string] $InstrumentationKey
    )

	$null = Load-MicrosoftApplicationInsights -MicrosoftApplicationInsightsPath $MicrosoftApplicationInsightsPath

	$script:client = Get-TelemetryClient -InstrumentationKey $InstrumentationKey

    $script:client.Context.User.Id = [System.GUID]::NewGuid().ToString()
    $script:client.Context.Session.Id = [System.GUID]::NewGuid().ToString()
    $script:client.Context.Device.OperatingSystem = [System.Environment]::OSVersion.ToString()
}

function Track-TelemetryEvent {
    param(
		[Parameter(Mandatory = $true)]
        [string] $EventName,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.IDictionary[string,string]] $Properties
    )

    $script:client.TrackEvent($EventName, $Properties)
    $script:client.Flush()
}

function Track-Exception {
    param(
		[Parameter(Mandatory = $true)]
        [System.Exception] $Exception,

        [Parameter(Mandatory = $true)]
        [System.Collections.Generic.IDictionary[string,string]] $Properties
    )

    $script:client.TrackException($Exception, $Properties)
    $script:client.Flush()
}
