[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $MelonLoaderRoot,

    [Parameter(Mandatory = $true)]
    [string] $AssemblyPath
)

$ErrorActionPreference = "Stop"
$script:PassCount = 0
$cecilPath = Join-Path $MelonLoaderRoot "net6\Mono.Cecil.dll"

if (-not (Test-Path -LiteralPath $AssemblyPath -PathType Leaf)) {
    throw "Missing Vehicle Handlers assembly: $AssemblyPath"
}

if (-not (Test-Path -LiteralPath $cecilPath -PathType Leaf)) {
    throw "Missing Mono.Cecil reference: $cecilPath"
}

[void] [System.Reflection.Assembly]::LoadFrom($cecilPath)
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($AssemblyPath)

try {
    function Get-RequiredType([string] $FullName) {
        $type = $assembly.MainModule.GetTypes() |
            Where-Object { $_.FullName -ceq $FullName } |
            Select-Object -First 1

        if (-not $type) {
            throw "Missing managed type: $FullName"
        }

        return $type
    }

    function Get-RequiredMethod(
        [string] $TypeName,
        [string] $MethodName,
        [string[]] $ParameterTypes = @()
    ) {
        $type = Get-RequiredType $TypeName
        $matches = @($type.Methods | Where-Object {
            if ($_.Name -cne $MethodName -or $_.Parameters.Count -ne $ParameterTypes.Count) {
                return $false
            }

            for ($index = 0; $index -lt $ParameterTypes.Count; $index++) {
                if ($_.Parameters[$index].ParameterType.FullName -cne $ParameterTypes[$index]) {
                    return $false
                }
            }

            return $true
        })

        if ($matches.Count -ne 1) {
            throw "Expected one method $TypeName::$MethodName($($ParameterTypes -join ', ')); found $($matches.Count)."
        }

        return $matches[0]
    }

    function Assert-Condition([bool] $Condition, [string] $Message) {
        if (-not $Condition) {
            throw $Message
        }

        Write-Output "PASS $Message"
        $script:PassCount++
    }

    function Assert-Call(
        [Mono.Cecil.MethodDefinition] $Method,
        [string] $CalledMethodPattern,
        [string] $Label
    ) {
        $matches = @($Method.Body.Instructions | Where-Object {
            $_.Operand -and $_.Operand.ToString() -like $CalledMethodPattern
        })

        Assert-Condition ($matches.Count -gt 0) $Label
    }

    $handlerType = Get-RequiredType "VehicleHandlers.Employees.HandlerEmployee"
    Assert-Condition ($handlerType.IsPublic) "HandlerEmployee is public for IL2CPP registration"
    Assert-Condition ($handlerType.BaseType.FullName -ceq "Il2CppScheduleOne.Employees.Employee") "HandlerEmployee derives directly from Employee"

    $nativeConstructor = Get-RequiredMethod "VehicleHandlers.Employees.HandlerEmployee" ".ctor" @("System.IntPtr")
    Assert-Condition ($nativeConstructor.IsPublic) "HandlerEmployee exposes the native-pointer constructor"

    $awake = Get-RequiredMethod "VehicleHandlers.Employees.HandlerEmployee" "Awake"
    Assert-Condition ($awake.IsPublic -and $awake.IsVirtual) "HandlerEmployee overrides public virtual Awake"
    Assert-Call $awake "*Il2CppScheduleOne.Employees.Employee::Awake()*" "HandlerEmployee Awake preserves the base employee initialization"
    Assert-Call $awake "*Il2CppScheduleOne.Employees.Employee::set_Type*" "HandlerEmployee Awake applies the Handler role"

    $initializeAppearance = Get-RequiredMethod "VehicleHandlers.Employees.HandlerEmployee" "InitializeAppearance" @("System.Boolean", "System.Int32")
    Assert-Condition ($initializeAppearance.IsPublic -and $initializeAppearance.IsVirtual) "HandlerEmployee overrides public virtual InitializeAppearance"
    Assert-Call $initializeAppearance "*Il2CppScheduleOne.Employees.Employee::InitializeAppearance*" "Handler appearance preserves base generated appearance"
    Assert-Call $initializeAppearance "*VehicleHandlers.Employees.HandlerAppearance::TryApply*" "Handler appearance applies the role-specific overlay"

    $register = Get-RequiredMethod "VehicleHandlers.Runtime.HandlerRuntimeRegistration" "Register"
    Assert-Call $register "*ClassInjector::IsTypeRegisteredInIl2Cpp*" "Runtime registration is idempotent"
    Assert-Call $register "*ClassInjector::RegisterTypeInIl2Cpp*" "Runtime registration injects HandlerEmployee"

    $tryApply = Get-RequiredMethod "VehicleHandlers.Employees.HandlerAppearance" "TryApply" @("Il2CppScheduleOne.AvatarFramework.Avatar")
    Assert-Call $tryApply "*Il2CppScheduleOne.AvatarFramework.Avatar::SetBodyLayer*" "Handler appearance uses the verified avatar body-layer API"

    Write-Output "Vehicle Handlers runtime worker verification passed ($script:PassCount checks)."
}
finally {
    $assembly.Dispose()
}
