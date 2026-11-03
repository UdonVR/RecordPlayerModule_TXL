using Texel;
using UdonVR.RecordPlayer;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;

namespace com.udonvr.recordplayer.txlEditor
{
    public class RecordPlayerAutoSetup : IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder => -9999;

        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            return true;
        }

        [PostProcessScene(-9999)]
        private static void ProcessScene()
        {
            RecordPlayer recordPlayer = GameObject.FindObjectOfType<RecordPlayer>();
            SyncPlayer _txl = GameObject.FindObjectOfType<SyncPlayer>();
            AudioManager _txlAudio = GameObject.FindObjectOfType<AudioManager>();

            if (recordPlayer == null)
            {
                Debug.LogWarning("[UdonVR][TXL AutoSetup] Record Player was not found");
                return;
            }

            if (_txl == null)
            {
                Debug.LogError("[UdonVR][TXL AutoSetup] TXL SyncPlayer was not found");
                return;
            }

            if (recordPlayer.videoplayerModule != null && recordPlayer.videoplayerModule.GetType() != typeof(RecordPlayer_VideoPlayer_TXL))
            {
                if (!EditorUtility.DisplayDialog(
                    "Existing Module found",
                    "There was a Module found attached to the record player, but it wasn't TXL. Do you want to continue TXL Auto Setup?",
                    "Yes", "No"))
                {
                    Debug.LogError($"[UdonVR][TXL AutoSetup] Module found [{recordPlayer.videoplayerModule.name}] but is not for TXL, Canceling AutoSetup");
                    return;
                }
            }

            RecordPlayer_VideoplayerClass module = GameObject.FindObjectOfType<RecordPlayer_VideoPlayer_TXL>();
            if (module == null)
            {
                var loadedObject = AssetDatabase.LoadAssetAtPath("Packages/com.udonvr.recordplayer.txl/RecordPlayerModule-TXL.prefab", typeof(UnityEngine.Object));
                if (loadedObject == null)
                {
                    string[] guids = AssetDatabase.FindAssets("RecordPlayerVideoModule - TXL t:Prefab");
                    if (guids.Length > 0)
                    {
                        loadedObject = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(UnityEngine.Object));
                    }
                }

                if (loadedObject == null)
                {
                    Debug.LogError("[UdonVR][TXL AutoSetup] No Videoplayer module prefab found in Packages or Assets");
                    return;
                }

                if (!((GameObject)PrefabUtility.InstantiatePrefab(loadedObject)).transform.TryGetComponent(out module))
                {
                    Debug.LogError("[UdonVR][TXL AutoSetup] No Videoplayer module found, failed to instantiate");
                    return;
                }
                else
                {
                    Debug.Log($"[UdonVR][TXL AutoSetup] Instantiate TXL videoplayer module: [{module.name}]");
                    recordPlayer.videoplayerModule = module;
                    if (module.GetType() != typeof(RecordPlayer_VideoPlayer_TXL))
                    {
                        Debug.LogError("[UdonVR][TXL AutoSetup] Created module found but is not for TXL, Canceling AutoSetup");
                        return;
                    }
                }
            }
            else if (recordPlayer.videoplayerModule == null)
            {
                recordPlayer.videoplayerModule = module;
            }

            if (module.recordPlayer == null)
            {
                module.recordPlayer = recordPlayer;
                Debug.Log($"[UdonVR][TXL AutoSetup] Module's recordPlayer Reference was null, binding to [{recordPlayer.name}]");
            }
            else
            {
                Debug.Log($"[UdonVR][TXL AutoSetup] Module's recordPlayer Reference already filled in, skipping");
            }

            RecordPlayer_VideoPlayer_TXL txlModule = (RecordPlayer_VideoPlayer_TXL)module;
            if (txlModule.txl == null)
            {
                txlModule.txl = _txl;
                Debug.Log($"[UdonVR][TXL AutoSetup] Module's txl Reference was null, binding to [{_txl.name}]");
            }
            else
            {
                Debug.Log($"[UdonVR][TXL AutoSetup] Module's txl Reference already filled in, skipping");
            }

            if (txlModule.txl_audio == null && _txlAudio != null)
            {
                txlModule.txl_audio = _txlAudio;
                Debug.Log($"[UdonVR][TXL AutoSetup] Module's txl_audio Reference was null, binding to [{_txlAudio.name}]");
            }
            else if (_txlAudio == null)
            {
                Debug.LogWarning("[UdonVR][TXL AutoSetup] TXL AudioManager was not found in scene; volume sync reference left unassigned");
            }

            EditorUtility.SetDirty(recordPlayer);
            EditorUtility.SetDirty(txlModule);
        }
    }
}