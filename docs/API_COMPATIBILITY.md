# Verified Game API Surface

Status: Verified
Target: Schedule I 0.4.6f13 IL2CPP

## Pinned inputs

| Assembly | SHA-256 |
| --- | --- |
| `Assembly-CSharp.dll` | `0D2EB364F3E84120AF7CCC9FA6BAFD597D42D495EBACC3A260CB4CA0CF0513DA` |
| `Il2CppFishNet.Runtime.dll` | `202E89D1E5B27E19403F66DA1779CA82E6E08D17199F2D2593C86CFDC65EF0F4` |

The verifier also requires every assembly referenced by `VehicleHandlers.Il2Cpp.csproj` to exist in the supplied MelonLoader root. Game assemblies and toolchains remain outside the repository.

## Compatibility gate

`scripts/verify-game-api.ps1` performs 95 exact checks before compilation:

- required MelonLoader and generated IL2CPP reference layout
- pinned game and FishNet assembly hashes
- public type existence and exact base types
- singleton and network-singleton accessors
- Handler enum value, hiring entry points, and virtual employee lifecycle methods
- GUID lookup and owned property/vehicle selection collections
- loading-bay availability, occupancy, parking, alignment, and vehicle transition APIs
- FishNet spawn, server, client, ownership, and network-object signals
- save and load events and active save-path identifiers
- method return types, parameter types, generic arity, static/instance status, and selected virtual flags
- property types, static/instance status, and getter/setter shape

Run:

```powershell
.\scripts\verify-game-api.ps1 -MelonLoaderRoot "<path-to-MelonLoader>"
```

A mismatched hash or changed signature fails immediately with the expected and observed values. The build script runs this gate before invoking the compiler.

## Verified result

- All 95 checks passed against the pinned 0.4.6f13 IL2CPP reference set.
- A deliberately incorrect `Assembly-CSharp.dll` hash was rejected.
- The release build completed with zero warnings and zero errors using .NET SDK 8.0.425.
- Output DLL SHA-256: `E7A2DC413DAA588C64AC0EE60151EE2862295F550E45902921E07C6B75CC24CC`.

## Boundary

This is static compatibility evidence, not runtime proof. It does not validate game-side behavior, multiplayer replication, save recovery, UI integration, or delivery conflicts. Those remain gated by M2-M5 runtime work. M1-002 freezes the mod-owned identifiers, state machine, validation results, service interfaces, and reservation ownership in [Internal Contracts](INTERNAL_CONTRACTS.md).
