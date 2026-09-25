# Current Handover

## Active claim

- Backlog: `VH-M2-001`
- Owner: Codex
- Branch: `feat/vh-m2-001-handler-runtime`
- Status: `VERIFYING`

## Current outcome

Foundation PR #1, exact API-verifier PR #2, and contract PR #3 are merged. Draft PR #4 implements `VH-M2-001`: `HandlerEmployee` is an injected role component that binds to a genuine game `Employee`, applies the existing Handler role, and adds a fail-safe orange outer-layer appearance through a Handler-filtered Harmony postfix. Hiring, prefab/network wiring, work lifecycle, assignments, reservations, and movement remain unchanged.

## Compatibility target

- Schedule I: `0.4.6f13`
- Backend: IL2CPP
- MelonLoader: `0.7.0 Open-Beta` reference layout
- `Assembly-CSharp.dll` SHA-256: `0D2EB364F3E84120AF7CCC9FA6BAFD597D42D495EBACC3A260CB4CA0CF0513DA`
- `Il2CppFishNet.Runtime.dll` SHA-256: `202E89D1E5B27E19403F66DA1779CA82E6E08D17199F2D2593C86CFDC65EF0F4`
- `Il2CppInterop.Runtime.dll` SHA-256: `A1154A31EC72D4097A0A1DB50E046D97AA6C9B60BCFEB4D0489DF6230960F674`
- `UnityEngine.CoreModule.dll` SHA-256: `269552723AB92FBBCA5DFD1C7EC7D875A95406AB048A1CC9B1BAA78C9B9B62E8`
- `0Harmony.dll` SHA-256: `6C898933B52149E8BCF6722305C7E4D94ADD47C9E53B364096F91721624C0877`

Game references remain outside this repository.

## Verification status

- Strict static API verification passed all 124 checks against the out-of-tree reference set.
- Dependency-free contract verification passed all 110 assertions, including the complete 9-by-9 trip transition matrix.
- Compiled Handler runtime verification passed all 12 checks for inheritance, native construction, base delegation, role assignment, appearance dispatch, and idempotent injection.
- An intentionally incorrect `Assembly-CSharp.dll` hash was rejected before API inspection.
- Release build completed with zero warnings and zero errors using .NET SDK `8.0.425`.
- Verified and installed Handler-worker DLL SHA-256: `6E976CB9EBABAFFABE60E1EC3188BDE24506DE6D506C78BBCFBE233A4543E3EE`.
- The production project now compiles only `Properties/AssemblyInfo.cs` and `Source/**/*.cs`; test build artifacts cannot leak into the mod DLL.
- First in-game launch rejected direct `Employee` subclass injection because the interop runtime could not map inherited `NPCMovement.WalkResult`; the game remained responsive and no worker was created.
- Second launch with component composition registered `VehicleHandlers.Employees.HandlerEmployee` successfully and printed the mod initialization banner with no Vehicle Handlers exception.
- Worker attachment to a later FishNet prefab and rendered appearance remain untested until M2-002 supplies the construction path.

Commands:

```powershell
.\scripts\verify-game-api.ps1 -MelonLoaderRoot "<out-of-tree-MelonLoader>"
.\scripts\verify-contracts.ps1 -DotNet "<dotnet-8.0.425>"
.\scripts\build-il2cpp.ps1 -MelonLoaderRoot "<out-of-tree-MelonLoader>" -DotNet "<dotnet-8.0.425>"
```

## Next work

1. Exercise Handler component attachment through the M2-002 prefab path, confirm the genuine Employee reports Handler, and visually inspect the outer-layer appearance without changing skin/body color.
2. Mark `VH-M2-001` `DONE` only with that evidence, then review and merge PR #4 before claiming `VH-M2-002` or `VH-M2-003`.
3. Preserve the disabled failed DLL outside the repository only until the corrected PR is merged; it is not loadable by MelonLoader.
