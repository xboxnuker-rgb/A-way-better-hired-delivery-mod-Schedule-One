# Current Handover

## Active claim

- Backlog: `VH-M1-002`
- Owner: Codex
- Branch: `feat/vh-m1-002-contracts`
- Status: `DONE`

## Current outcome

Foundation PR #1 and exact API-verifier PR #2 are merged. Draft PR #3 completes `VH-M1-002` with contract version 1: strongly typed identities, immutable revisioned assignments, stable validation outcomes, an explicit trip transition policy, owner-and-trip reservation leases, and server-authoritative service boundaries. M2, M3, and M4 can implement against these contracts after PR #3 is merged.

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

1. Review and merge draft PR #3 with owner approval.
2. After merge, claim `VH-M2-001`, `VH-M3-001`, or `VH-M4-001` on separate branches; these milestone roots may now proceed independently.
3. Preserve contract version 1 semantics and coordinate any contract change through the handover before parallel work consumes it.
