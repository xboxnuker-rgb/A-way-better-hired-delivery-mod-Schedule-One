# Handler Runtime Worker

Status: Implemented; awaiting in-game verification

Target: Schedule I 0.4.6f13 IL2CPP

## Runtime type

`HandlerEmployee` derives directly from the game's `Employee` wrapper and exposes the native-pointer constructor required when IL2CPP creates an injected component. `HandlerRuntimeRegistration.Register` idempotently registers the type with `ClassInjector` during mod initialization and fails mod initialization if registration cannot be confirmed.

`Awake` sets `Employee.Type` to the existing `EEmployeeType.Handler` value before and after base initialization. This preserves the base NPC/employee initialization path while ensuring the non-virtual `EmployeeType` getter identifies the worker as Handler.

M2-001 does not manually construct or spawn an employee. The hiring integration in M2-002 must instantiate the registered type through an IL2CPP/Unity construction path and wire it into the network prefab before spawn.

## Appearance

`InitializeAppearance` delegates to the game's normal generated employee appearance first. It then recolors the highest populated non-base body layer with a high-visibility orange role color:

- body layers are considered from layer 8 down to layer 2;
- layer 1 is never recolored because it can be the base body/skin layer;
- no shared `AvatarSettings` asset is mutated;
- if there is no suitable layer or the override throws, the generated base appearance remains intact and a warning is logged.

The worker therefore keeps the game's gender, body, face, hair, voice, and mugshot selection while gaining a visible Handler role cue when the selected employee appearance contains an outer body layer.

## Scope boundary

This task supplies the injected runtime class and its appearance behavior only. It does not:

- patch `EmployeeManager.GetEmployeePrefab` or either creation method;
- clone or register a FishNet spawnable prefab;
- change hiring, wages, beds, lockers, firing, saving, or leave/despawn;
- implement assignments, management UI, reservations, or vehicle movement.

Those behaviors remain assigned to M2-002 and later backlog items.

## Verification

The pinned API verifier now includes `Il2CppInterop.Runtime.dll`, the injection methods, employee role field, avatar access, appearance initialization, body-layer paths, and body-layer mutation API.

After compilation, `scripts/verify-handler-runtime.ps1` inspects the produced assembly and verifies:

- public direct inheritance from `Employee`;
- the native-pointer constructor;
- virtual `Awake` and `InitializeAppearance` overrides;
- calls to base employee initialization;
- Handler role assignment;
- role-specific appearance dispatch and body-layer use;
- idempotent IL2CPP registration.

Static verification cannot prove that the target game accepts the injected subclass, that a future FishNet prefab is wired correctly, or that the appearance renders as intended. Those require isolated in-game evidence before this item moves from `VERIFYING` to `DONE`.
