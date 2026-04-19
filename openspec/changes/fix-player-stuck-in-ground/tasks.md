# Tasks: Fix Player Stuck in Ground

- [x] Modify `Assets/Prefabs/Player/GamePlayer.prefab`
  - [x] Set `CharacterController.m_Enabled` to `1`
  - [x] Set `CapsuleCollider.m_Center` to `{x: 0, y: 1.05, z: 0}`
  - [x] Set `PlayerControllerReliable.m_Enabled` to `1`
- [x] Audit `Assets/Scripts/Gameplay/FishingPlayer.cs`
  - [x] Verify `SmartEscapeRoutine` doesn't conflict with collider fixes
- [x] Instruction for user: Remove manual `GamePlayer` instances from the Scene hierarchy.

