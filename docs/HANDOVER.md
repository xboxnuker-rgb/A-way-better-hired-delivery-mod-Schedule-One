# Current Handover

## Active claim

- Backlog: `VH-M2-001`
- Owner: Codex
- Branch: `feat/vh-m2-001-handler-runtime`
- Status: `IN_PROGRESS`

## Current outcome

Foundation PR #1, exact API-verifier PR #2, and contract PR #3 are merged. `VH-M2-001` is claimed to add the genuine Handler runtime worker and role-specific appearance without yet changing hiring, work lifecycle, assignments, reservations, or movement.

## Compatibility target

- Schedule I: `0.4.6f13`
- Backend: IL2CPP
- MelonLoader: `0.7.0 Open-Beta` reference layout
- `Assembly-CSharp.dll` SHA-256: `0D2EB364F3E84120AF7CCC9FA6BAFD597D42D495EBACC3A260CB4CA0CF0513DA`
- `Il2CppFishNet.Runtime.dll` SHA-256: `202E89D1E5B27E19403F66DA1779CA82E6E08D17199F2D2593C86CFDC65EF0F4`

Game references remain outside this repository.

## Verification status

- Strict static API verification passed all 95 checks against the out-of-tree reference set.
- Dependency-free contract verification passed all 110 assertions, including the complete 9-by-9 trip transition matrix.
- An intentionally incorrect `Assembly-CSharp.dll` hash was rejected before API inspection.
- Release build completed with zero warnings and zero errors using .NET SDK `8.0.425`.
- Verified contract-layer DLL SHA-256: `CCE4B75045CEAE2725D6AA1D77867AF604392539B849376D47F9B7DC989BA058`.
- The production project now compiles only `Properties/AssemblyInfo.cs` and `Source/**/*.cs`; test build artifacts cannot leak into the mod DLL.
- Runtime behavior remains untested because gameplay implementation has not started.

Commands:

```powershell
.\scripts\verify-game-api.ps1 -MelonLoaderRoot "<out-of-tree-MelonLoader>"
.\scripts\verify-contracts.ps1 -DotNet "<dotnet-8.0.425>"
.\scripts\build-il2cpp.ps1 -MelonLoaderRoot "<out-of-tree-MelonLoader>" -DotNet "<dotnet-8.0.425>"
```

## Next work

1. Verify the exact employee construction, prefab selection, IL2CPP registration, and appearance APIs needed by the Handler worker.
2. Implement and statically verify the distinct Handler runtime type and appearance boundary without taking over base hiring or lifecycle behavior.
3. Review and merge the `VH-M2-001` PR before claiming `VH-M2-002` or `VH-M2-003`.
