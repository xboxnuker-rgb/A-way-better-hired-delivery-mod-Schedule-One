# Current Handover

## Active claim

- Backlog: `VH-M1-002`
- Owner: Codex
- Branch: `feat/vh-m1-002-contracts`
- Status: `IN_PROGRESS`

## Current outcome

Foundation PR #1 and the exact API-verifier PR #2 are merged. `VH-M1-002` is claimed to freeze the shared identifiers, validation results, state transitions, service boundaries, and reservation ownership required by later milestones.

## Compatibility target

- Schedule I: `0.4.6f13`
- Backend: IL2CPP
- MelonLoader: `0.7.0 Open-Beta` reference layout
- `Assembly-CSharp.dll` SHA-256: `0D2EB364F3E84120AF7CCC9FA6BAFD597D42D495EBACC3A260CB4CA0CF0513DA`
- `Il2CppFishNet.Runtime.dll` SHA-256: `202E89D1E5B27E19403F66DA1779CA82E6E08D17199F2D2593C86CFDC65EF0F4`

Game references remain outside this repository.

## Verification status

- Strict static API verification passed all 95 checks against the out-of-tree reference set.
- An intentionally incorrect `Assembly-CSharp.dll` hash was rejected before API inspection.
- Release build completed with zero warnings and zero errors using .NET SDK `8.0.425`.
- Verified scaffold DLL SHA-256: `E7A2DC413DAA588C64AC0EE60151EE2862295F550E45902921E07C6B75CC24CC`.
- Runtime behavior remains untested because gameplay implementation has not started.

Commands:

```powershell
.\scripts\verify-game-api.ps1 -MelonLoaderRoot "<out-of-tree-MelonLoader>"
.\scripts\build-il2cpp.ps1 -MelonLoaderRoot "<out-of-tree-MelonLoader>" -DotNet "<dotnet-8.0.425>"
```

## Next work

1. Freeze identifiers, assignment and reservation keys, validation result types, state transitions, service interfaces, and reservation ownership.
2. Review and merge the `VH-M1-002` contract PR with owner approval.
3. Begin M2, M3, and M4 only after those shared contracts are merged.
