# Handler Runtime Worker

Status: Startup verified; awaiting worker-construction and appearance verification

Target: Schedule I 0.4.6f13 IL2CPP

## Runtime type

`HandlerEmployee` is an injected `MonoBehaviour` role component with the native-pointer constructor required when IL2CPP creates it. It binds to the real game `Employee` component on the same object and sets that component's `Type` to the existing `EEmployeeType.Handler` value. The game-owned Employee remains responsible for NPC, FishNet, lifecycle, and save behavior.

`HandlerRuntimeRegistration.Register` idempotently registers the role component with `ClassInjector` during mod initialization and fails mod initialization if registration cannot be confirmed.

The first runtime attempt used direct inheritance from `Employee`. Schedule I's pinned interop runtime rejected that injected type while resolving the inherited `NPCMovement.WalkResult` virtual signature. Composition avoids that unsupported injection boundary while retaining a genuine base-game Employee as the networked worker.

M2-001 does not manually construct or spawn an employee. The hiring integration in M2-002 must attach the registered role component to its employee construction donor before spawn.

## Appearance

A Harmony postfix observes the game's normal `Employee.InitializeAppearance` completion. It ignores every non-Handler employee, then recolors the highest populated non-base body layer with a high-visibility orange role color:

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

- public `MonoBehaviour` injection boundary;
- the native-pointer constructor;
- Unity `Awake` binding to the real Employee component;
- Handler role assignment;
- Handler-filtered Harmony appearance dispatch and body-layer use;
- idempotent IL2CPP registration.

## Runtime evidence

On 2026-09-25, the exact built DLL was installed into the game Mods directory and Schedule I was launched twice:

1. Direct `Employee` inheritance failed during mod initialization with `Couldn't find System.Type for Il2Cpp type: NPCMovement+WalkResult`. No worker was created and the game remained responsive at the main menu.
2. After changing to the composed role component, MelonLoader logged `Registered mono type VehicleHandlers.Employees.HandlerEmployee in il2cpp domain`, followed by the Vehicle Handlers initialization banner. No Vehicle Handlers exception occurred.

The successful installed DLL SHA-256 is `6E976CB9EBABAFFABE60E1EC3188BDE24506DE6D506C78BBCFBE233A4543E3EE`. The failed DLL is retained outside the repository with a disabled extension and cannot be loaded by MelonLoader.

Startup registration is now proven. A future FishNet prefab, component attachment, role persistence, and rendered appearance still require isolated in-game evidence through M2-002 before this item moves from `VERIFYING` to `DONE`.
