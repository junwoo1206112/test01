# Proposal: Fix Camera Perspective to 3rd Person

## Problem
The player's camera remains in a 1st-person perspective (inside the character model) despite multiple script adjustments. This suggests that the Cinemachine Camera in the scene has high-priority components or settings (like Hard Lock or specific Position/Rotation Control modes) that override the `CinemachineFollow.FollowOffset` set in the script.

## Goal
Force a reliable 3rd-person top-down perspective where the player character is clearly visible from behind and above.

## Proposed Changes
1. **Audit Scene Camera**: Inspect `GamePlay.unity` to identify the specific Cinemachine components on the virtual camera.
2. **Robust Script Update**: Rewrite `FishingCameraFollow.cs` to not just set offsets, but to re-initialize the Cinemachine components to the correct "3rd Person" types (Position: Follow, Rotation: LookAt) and ensure they are active.
3. **Hierarchy Fix**: Ensure the `CameraTarget` in the player prefab is at the correct height (around 1.5m to 1.8m) to serve as a proper look-at point.
