# Design: Robust 3rd Person Camera Integration

## Camera Reconfiguration
- **Component Priority**: `FishingCameraFollow.cs` will search for any active Cinemachine virtual camera and reconfigure its `PositionControl` to use `CinemachineFollow` and its `RotationControl` to use `CinemachineLookAt`.
- **Force Offset**: The script will explicitly set `followOffset = (0, 4.5f, -8f)` and `BindingMode = LockToTargetWithWorldUp`.
- **LookAt Smoothing**: Use the `CameraTarget` child for the `LookAt` target, but keep the `Follow` target on the root for more stable movement.

## Error Prevention
- Check for `CinemachineHardLockToTarget` and `CinemachineHardLookAt` which might be causing the 1st person "snap" effect, and disable them if found.
- Ensure the `CinemachineBrain` on the Main Camera is active.

## Network Sync
- This camera setup only runs on the local player (`isLocalPlayer`) as per project networking conventions.
