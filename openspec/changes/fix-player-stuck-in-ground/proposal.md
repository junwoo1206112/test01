# Proposal: Fix Player Stuck in Ground

## What is the problem?
Currently, when a player spawns into the world, they are stuck in the ground and unable to move. This is caused by multiple factors in the `GamePlayer` prefab:
1. Movement-related components (`CharacterController`, `PlayerControllerReliable`) are disabled.
2. The `CapsuleCollider`'s center (y=2) is set too high, causing the physics engine to calculate the player's position below the surface.
3. The `Rigidbody` is set to `IsKinematic` without an active `CharacterController` to handle movement, resulting in no physics or input processing.

## What is the goal?
The goal is to enable smooth player movement and ensure they spawn correctly on the ground surface by fixing the prefab configuration and aligning with the project's Mirror networking conventions.

## Proposed Changes
1. **Enable Components**: Activate `CharacterController` and `PlayerControllerReliable` on the `GamePlayer` prefab.
2. **Fix Collider Offset**: Adjust `CapsuleCollider.m_Center` to `(0, 1.05, 0)` to align with the character's height and the ground level.
3. **Verify Movement Logic**: Ensure `FishingPlayer.cs` and `PlayerControllerReliable.cs` are correctly processing inputs when the components are enabled.
4. **Clean up Scene**: Ensure no `GamePlayer` instances are manually placed in the Scene hierarchy, as they should only be spawned via the `NetworkRoomManager`.
