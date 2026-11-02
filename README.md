# UdonVR Record Player Module: VideoTXL

Compatibility hook bridging the **UdonVR Record Player** (`RecordPlayer_VideoplayerClass`) with **Texel VideoTXL**.

## Feature Compatibility

| Capability | Status | Implementation Details |
| :--- | :---: | :--- |
| **Load URL** | `Full` | Calls `txl._ChangeUrl(_url)` |
| **Play / Pause / Stop** | `Full` | Uses `txl._TriggerPause()` and `txl._TriggerStop()` |
| **Seeking & Scrubbing** | `Full` | Calls `txl._SetTargetTime(_time)` |
| **Tonearm Progress** | `Full` | Remaps `txl.trackPosition` / `txl.trackDuration` (`0..1`) |
| **2-Way Volume Sync** | `Full` | Syncs via `AudioManager` (`_VideoStateReady` & `_AudioManagerUpdate`) |
| **Permission / Lock Check** | `Full` | Checks `txl._CanTakeControl()` |
| **Track End Auto-Advance** | `Partial` | `playerState == 0` fires `_End()` for both manual stop and natural end; `_PlaybackEnded()` only fires on `playerState == 3` (Error) |
| **Single-Track Loop** | `None` | `_loopCurrentUrl()` is unimplemented |
| **Custom Track Titles** | `None` | Not supported by `SyncPlayer._ChangeUrl`; `_SetTitle()` is empty |

## Inspector Setup
1. Add the `RecordPlayer_VideoPlayer_TXL` component to your module GameObject.
2. Assign your scene's `SyncPlayer` instance to **`txl`**.
3. Assign your scene's `AudioManager` instance to **`txl_audio`**.
4. Reference this module on your core `RecordPlayer` script.

<!-- MODULE_MANIFEST_START -->
```json
{
  "schemaVersion": "1.0.0",
  "id": "TXL",
  "displayName": "Texel VideoTXL",
  "repo": "[https://github.com/UdonVR/RecordPlayerModule_TXL](https://github.com/UdonVR/RecordPlayerModule_TXL)",
  "scriptPath": "RecordPlayer_VideoPlayer_TXL.cs",
  "className": "RecordPlayer_VideoPlayer_TXL",
  "baseClass": "UdonVR.RecordPlayer.RecordPlayer_VideoplayerClass",
  "requiredNamespaces": [
    "Texel"
  ],
  "inspectorReferences": [
    {
      "fieldName": "txl",
      "fieldType": "Texel.SyncPlayer",
      "required": true,
      "description": "TXL SyncPlayer component"
    },
    {
      "fieldName": "txl_audio",
      "fieldType": "Texel.AudioManager",
      "required": true,
      "description": "TXL AudioManager component for volume sync"
    }
  ],
  "capabilities": {
    "loadUrl": "full",
    "playPauseStop": "full",
    "seekTime": "full",
    "playbackProgress": "full",
    "volumeSync": "bidirectional",
    "permissionCheck": "full",
    "loopTrack": "none",
    "customTitleMetadata": "none",
    "naturalEndDetection": "partial"
  },
  "notes": [
    "Class is currently in the global namespace rather than UdonVR.RecordPlayer.",
    "Requires both SyncPlayer and AudioManager inspector references.",
    "State 0 (Stopped) calls _End() for both manual stops and natural track completion; _PlaybackEnded() only triggers on State 3 (Error).",
    "_loopCurrentUrl() and _SetTitle() are empty."
  ]
}
```
<!-- MODULE_MANIFEST_END -->
