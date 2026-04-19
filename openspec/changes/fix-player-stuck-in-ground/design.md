# Design: Fix Player Stuck in Ground

## Component Fixes
- `GamePlayer.prefab`'s `CharacterController.m_Enabled` set to `1` (true).
- `GamePlayer.prefab`'s `CapsuleCollider.m_Center` set to `{x: 0, y: 1.05, z: 0}`.
- `Mirror.Components.PlayerControllerReliable.m_Enabled` set to `1` (true).
- `Rigidbody.m_IsKinematic` set to `1` (true) but ensuring `CharacterController` is handling the movement.

## Code Adjustments
- If `FishingPlayer.cs` has logic to reset position on start, verify its compatibility with the new collider offset.
- `SmartEscapeRoutine` in `FishingPlayer.cs` should be audited to ensure it doesn't conflict with the fixed collider.

## Networking Strategy
- All movements are handled via Mirror's `PlayerControllerReliable`.
- Client-side prediction and server-side validation are handled by Mirror automatically.
- Ensure the local player has authority (`isLocalPlayer`) when processing inputs.
