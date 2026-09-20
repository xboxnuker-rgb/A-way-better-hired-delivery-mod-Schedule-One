# Current Handover

## Active claim

- Backlog: `VH-M2-001`
- Owner: Codex
- Branch: `feat/vh-m2-001-handler-runtime`
- Status: `VERIFYING`

## Current outcome

Foundation PR #1, exact API-verifier PR #2, and contract PR #3 are merged. Draft PR #4 implements `VH-M2-001`: `HandlerEmployee` is a direct injected subclass of `Employee`, forces the existing Handler role around preserved base initialization, and adds a fail-safe orange outer-layer appearance. Hiring, prefab/network wiring, work lifecycle, assignments, reservations, and movement remain unchanged.

## Compatibility target

- Schedule I: `0.4.6f13`
- Backend: IL2CPP
- MelonLoader: `0.7.0 Open-Beta` reference layout
- `Assembly-CSharp.dll` SHA-256: `0D2EB364F3E84120AF7CCC9FA6BAFD597D42D495EBACC3A260CB4CA0CF0513DA`
- `Il2CppFishNet.Runtime.dll` SHA-256: `202E89D1E5B27E19403F66DA1779CA82E6E08D17199F2D2593C86CFDC65EF0F4`
- `Il2CppInterop.Runtime.dll` SHA-256: `A1154A31EC72D4097A0A1DB50E046D97AA6C9B60BCFEB4D0489DF6230960F674`

Game references remain outside this repository.

## Verification status

- Strict static API verification passed all 113 checks against the out-of-tree reference set.
- Dependency-free contract verification passed all 110 assertions, including the complete 9-by-9 trip transition matrix.
- Compiled Handler runtime verification passed all 12 checks for inheritance, native construction, base delegation, role assignment, appearance dispatch, and idempotent injection.
- An intentionally incorrect `Assembly-CSharp.dll` hash was rejected before API inspection.
- Release build completed with zero warnings and zero errors using .NET SDK `8.0.425`.
- Verified Handler-worker DLL SHA-256: `011F3E72E251AB74B99023DD3C5AEDE06FAE6B2862C32B772D4AEFD8C52D3764`.
- The production project now compiles only `Properties/AssemblyInfo.cs` and `Source/**/*.cs`; test build artifacts cannot leak into the mod DLL.
- In-game behavior remains untested: IL2CPP registration, a later FishNet prefab, and rendered appearance require isolated runtime evidence.

Commands:

```powershell
.\scripts\verify-game-api.ps1 -MelonLoaderRoot "<out-of-tree-MelonLoader>"
.\scripts\verify-contracts.ps1 -DotNet "<dotnet-8.0.425>"
.\scripts\build-il2cpp.ps1 -MelonLoaderRoot "<out-of-tree-MelonLoader>" -DotNet "<dotnet-8.0.425>"
```

## Next work

1. Run isolated in-game verification that mod initialization registers `HandlerEmployee` without an interop error.
2. Exercise Handler construction through the M2-002 prefab path, confirm the role remains Handler, and visually inspect the outer-layer appearance without changing skin/body color.
3. Mark `VH-M2-001` `DONE` only with that evidence, then review and merge PR #4 before claiming `VH-M2-002` or `VH-M2-003`.
