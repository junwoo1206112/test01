# Tasks: Robust 3rd Person Camera

- [x] Audit and Rewrite `Assets/Scripts/Gameplay/FishingCameraFollow.cs`
  - [x] Remove manual transform update logic (clean up messy code).
  - [x] Use standard `CinemachineFollow` and `CinemachineRotationComposer` components.
  - [x] Ensure `isLocalPlayer` check is robust using `NetworkClient`.
- [ ] Final verification in Unity.
