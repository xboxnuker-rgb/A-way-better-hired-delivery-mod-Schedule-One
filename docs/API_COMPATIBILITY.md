# Verified Game API Surface

Status: Verified
Target: Schedule I 0.4.6f13 IL2CPP

## Pinned inputs

| Assembly | SHA-256 |
| --- | --- |
| `Assembly-CSharp.dll` | `0D2EB364F3E84120AF7CCC9FA6BAFD597D42D495EBACC3A260CB4CA0CF0513DA` |
| `Il2CppFishNet.Runtime.dll` | `202E89D1E5B27E19403F66DA1779CA82E6E08D17199F2D2593C86CFDC65EF0F4` |
| `Il2CppInterop.Runtime.dll` | `A1154A31EC72D4097A0A1DB50E046D97AA6C9B60BCFEB4D0489DF6230960F674` |
| `UnityEngine.CoreModule.dll` | `269552723AB92FBBCA5DFD1C7EC7D875A95406AB048A1CC9B1BAA78C9B9B62E8` |
| `0Harmony.dll` | `6C898933B52149E8BCF6722305C7E4D94ADD47C9E53B364096F91721624C0877` |

The verifier also requires every assembly referenced by `VehicleHandlers.Il2Cpp.csproj` to exist in the supplied MelonLoader root. Game assemblies and toolchains remain outside the repository.

## Compatibility gate

`scripts/verify-game-api.ps1` performs 124 exact checks before compilation:

- required MelonLoader and generated IL2CPP reference layout
- pinned game and FishNet assembly hashes
- public type existence and exact base types
- singleton and network-singleton accessors
- Handler enum value, hiring entry points, and virtual employee lifecycle methods
- GUID lookup and owned property/vehicle selection collections
- loading-bay availability, occupancy, parking, alignment, and vehicle transition APIs
- FishNet spawn, server, client, ownership, and network-object signals
- save and load events and active save-path identifiers
- IL2CPP class injection, Unity component composition, Handler role assignment, Harmony patch attributes, avatar access, and non-base body-layer appearance APIs
- method return types, parameter types, generic arity, static/instance status, and selected virtual flags
- property types, static/instance status, and getter/setter shape

Run:

```powershell
.\scripts\verify-game-api.ps1 -MelonLoaderRoot "<path-to-MelonLoader>"
```

A mismatched hash or changed signature fails immediately with the expected and observed values. The build script runs this gate before invoking the compiler.

## Verified result

- All 124 checks passed against the pinned 0.4.6f13 IL2CPP, Unity, MelonLoader interop, and Harmony reference set.
- A deliberately incorrect `Assembly-CSharp.dll` hash was rejected.
- The release build completed with zero warnings and zero errors using .NET SDK 8.0.425.
- Latest verified and startup-tested output DLL SHA-256: `6E976CB9EBABAFFABE60E1EC3188BDE24506DE6D506C78BBCFBE233A4543E3EE`.

## Boundary

This is static compatibility evidence, not runtime proof. It does not validate game-side behavior, multiplayer replication, save recovery, UI integration, or delivery conflicts. Those remain gated by M2-M5 runtime work. M1-002 freezes the mod-owned identifiers, state machine, validation results, service interfaces, and reservation ownership in [Internal Contracts](INTERNAL_CONTRACTS.md).
