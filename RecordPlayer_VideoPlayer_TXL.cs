
using System;
using Texel;
using UdonSharp;
using UdonVR.RecordPlayer;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RecordPlayer_VideoPlayer_TXL : RecordPlayer_VideoplayerClass
{
    public SyncPlayer txl;
    public AudioManager txl_audio;

    private int lastState = -1;
    private bool isLoading = false;

    private void Start()
    {
        txl_audio._RegisterControls(this);
        txl._Register(TXLVideoPlayer.EVENT_VIDEO_STATE_UPDATE, this, "_VideoStateUpdate");
        txl._Register(TXLVideoPlayer.EVENT_VIDEO_LOCK_UPDATE, this, "_VideoLockUpdate");
        //txl._Register(TXLVideoPlayer.EVENT_VIDEO_TRACKING_UPDATE, this, "_VideoTrackingUpdate");
        txl._Register(TXLVideoPlayer.EVENT_VIDEO_INFO_UPDATE, this, "_OnVideoInfoUpdate");
        //txl._Register(TXLVideoPlayer.EVENT_VIDEO_PLAYLIST_UPDATE, this, "_VideoPlaylistUpdate");
        txl._Register(TXLVideoPlayer.EVENT_VIDEO_READY, this, "_VideoStateReady");
    }

    #region Videoplayer Callbacks

    public void _VideoStateUpdate()
    {
        switch (txl.playerState)
        {
            case 0:
                //stopped
                if (lastState == 0 || isLoading) break;
                lastState = 0;
                recordPlayer._Log($"[TXL] received Stopped");
                _End();
                break;
            case 1:
                //loading
                if (lastState == 1) break;
                lastState = 1;
                recordPlayer._Log($"[TXL] received Loading");
                _OnVideoLoad();
                break;
            case 2:
                //playing
                if (lastState == 2) break;
                lastState = 2;
                isLoading = false;
                recordPlayer._Log($"[TXL] received Playing");
                break;
            case 3:
                //error
                if (lastState == 3) break;
                lastState = 3;
                recordPlayer._Log($"[TXL] received Error");
                isLoading = false;
                _PlaybackEnded();
                break;
        }
    }
    public void _VideoLockUpdate()
    {
        
    }
    public void _VideoTrackingUpdate()
    {
        
    }
    public void _OnVideoInfoUpdate()
    {
        
    }
    public void _VideoStateReady()
    {
        recordPlayer.volumeSlider._Internal_UpdateValue(_GetVolume(), false);
    }
    public void _AudioManagerUpdate()
    {
        recordPlayer.volumeSlider._Internal_UpdateValue(_GetVolume(), false);
    }

    #endregion

    #region Overrides

    public override void _LoadUrl(VRCUrl _url)
    {
        isLoading = true;
        txl._ChangeUrl(_url);
    }
    public override float _GetPlaybackTime()
    {
        return Remap(txl.trackPosition, 0, txl.trackDuration, 0, 1);
    }
    public override float _GetPlaybackDuration_Current()
    {
        return txl.trackPosition;
    }
    public override float _GetPlaybackDuration_End()
    {
        return txl.trackDuration;
    }
    public override bool _IsPaused()
    {
        return txl.paused;
    }
    public override bool _CanPlayVideo()
    {
        return txl._CanTakeControl();
    }
    public override bool _IsVideoplayerOwner()
    {
        return Networking.IsOwner(txl.gameObject);
    }
    public override void _Stop()
    {
        txl._TriggerStop();
    }
    public override void _Pause()
    {
        txl._TriggerPause();
    }
    public override void _loopCurrentUrl()
    {
        
    }
    public override void _SetVolume(float _volume)
    {
        txl_audio._SetMasterVolume(_volume);
    }
    public override float _GetVolume()
    {
        return txl_audio.masterVolume;
    }
    public override void _SetTitle(RecordPlayer_Disk _disk)
    {
        
    }
    public override void _ChangePlaybackTime(float _time)
    {
        txl._SetTargetTime(_time);
    }

    #endregion

}
