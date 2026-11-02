# UdonVR Record Player Module: VideoTXL

Compatibility hook bridging the **UdonVR Record Player** (`RecordPlayer_VideoplayerClass`) with **Texel VideoTXL**.

## Feature Compatibility

| Capability | Status | Implementation Details |
| :--- | :---: | :--- |
| **Editor Auto-Setup** | `None` | Requires manual inspector assignment |
| **Load URL** | `Full` | Calls `txl._ChangeUrl(_url)` |
| **External Load Detection** | `Full` | `playerState == 1` triggers `_OnVideoLoad()` to stop vinyl if TXL is used externally |
| **Play / Pause / Stop** | `Full` | Uses `txl._TriggerPause()` and `txl._TriggerStop()` |
| **Seeking & Scrubbing** | `Full` | Calls `txl._SetTargetTime(_time)` |
| **Tonearm Progress** | `Full` | Remaps `txl.trackPosition` / `txl.trackDuration` (`0..1`) |
| **2-Way Volume Sync** | `Full` | Syncs via `AudioManager` (`_VideoStateReady` & `_AudioManagerUpdate`) |
| **Permission / Lock Check** | `Full` | Checks `txl._CanTakeControl()` |
| **Album Auto-Advance & Shuffle** | `Broken` | Natural track end enters `playerState == 0` (`_End()`), stopping the player instead of calling `_PlaybackEnded()` (`_NextTrack_Button` / `_Shuffle`) |
| **Single-Track Loop** | `None` | `_loopCurrentUrl()` is unimplemented |
| **Custom Track Titles** | `None` | Not supported by `SyncPlayer._ChangeUrl`; `_SetTitle()` is empty |

## Inspector Setup
1. Add `RecordPlayer_VideoPlayer_TXL` to your module GameObject.
2. Assign your `RecordPlayer` instance to **`recordPlayer`**.
3. Assign your scene's `SyncPlayer` instance to **`txl`**.
4. Assign your scene's `AudioManager` instance to **`txl_audio`**.

<!-- MODULE_MANIFEST_START -->
```json
{
  "schemaVersion": "1.1.0",
  "id": "TXL",
  "displayName": "Texel VideoTXL",
  "repo": "[https://github.com/UdonVR/RecordPlayerModule_TXL](https://github.com/UdonVR/RecordPlayerModule_TXL)",
  "scriptPath": "RecordPlayer_VideoPlayer_TXL.cs",
  "className": "RecordPlayer_VideoPlayer_TXL",
  "baseClass": "UdonVR.RecordPlayer.RecordPlayer_VideoplayerClass",
  "autoSetup": false,
  "requiredNamespaces": [
    "Texel"
  ],
  "inspectorReferences": [
    {
      "fieldName": "recordPlayer",
      "fieldType": "UdonVR.RecordPlayer.RecordPlayer",
      "required": true,
      "description": "Core RecordPlayer controller"
    },
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
    "autoSetup": "none",
    "loadUrl": "full",
    "externalLoadDetection": "full",
    "playPauseStop": "full",
    "seekTime": "full",
    "playbackProgress": "full",
    "volumeSync": "bidirectional",
    "permissionCheck": "full",
    "autoAdvanceAndShuffle": "broken",
    "loopTrack": "none",
    "customTitleMetadata": "none"
  },
  "notes": [
    "No auto-setup support; requires manual inspector references.",
    "Class is in the global namespace rather than UdonVR.RecordPlayer.",
    "State 0 (Stopped) calls _End() for both manual stops and natural track completion, preventing _PlaybackEnded() from advancing tracks or shuffling unless State 3 (Error) occurs.",
    "_loopCurrentUrl() and _SetTitle() are empty."
  ]
}
```
<!-- MODULE_MANIFEST_END -->
