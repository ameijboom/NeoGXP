using System;
using NeoGXP.NeoGXP.Core;
using static NeoGXP.NeoGXP.FMOD.FMOD;

namespace NeoGXP.NeoGXP.FMOD;

public class FMODSoundSystem : SoundSystem
{
    private static IntPtr _system = IntPtr.Zero;

    private IntPtr GetSystem()
    {
        if (_system == IntPtr.Zero)
        {
            // if fmod not initialized, create system and init default
            System_Create(out _system);
            System_Init(_system, 32, 0, 0);
        }
        return _system;
    }

    public override void Init()
    {
        //setup is done on-demand
    }

    public override void Deinit()
    {
        //clean up is never done?
    }

    public override IntPtr CreateStream(string filename, bool looping)
    {
        uint loop = FMOD_LOOP_OFF; // no loop
        if (looping) loop = FMOD_LOOP_NORMAL;

        System_CreateStream(GetSystem(), filename, loop, 0, out IntPtr id);
        if (id == IntPtr.Zero)
        {
            throw new Exception("Sound file not found: " + filename);
        }
        return id;
    }

    public override IntPtr LoadSound(string filename, bool looping)
    {
        uint loop = FMOD_LOOP_OFF; // no loop
        if (looping) loop = FMOD_LOOP_NORMAL;

        System_CreateSound(GetSystem(), filename, loop, 0, out IntPtr id);
        return id;
    }

    public override void Step()
    {
        if (_system != IntPtr.Zero)
        {
            System_Update(_system);
        }
    }

    public override uint PlaySound(IntPtr id, uint channelId, bool paused)
    {
        System_PlaySound(GetSystem(), channelId, id, paused, out uint outId);
        return outId;
    }

    public override uint PlaySound(IntPtr id, uint channelId, bool paused, float volume, float pan)
    {
        System_PlaySound(GetSystem(), channelId, id, true, out uint outId);
        SetChannelVolume (outId, volume);
        SetChannelPan (outId, pan);
        SetChannelPaused (outId, paused);
        return outId;
    }

    public override float GetChannelFrequency(uint channelId)
    {
        Channel_GetFrequency(channelId, out float frequency);
        return frequency;
    }

    public override void SetChannelFrequency(uint channelId, float frequency)
    {
        Channel_SetFrequency(channelId, frequency);
    }

    public override float GetChannelPan(uint channelId)
    {
        Channel_GetPan(channelId, out float pan);
        return pan;
    }

    public override void SetChannelPan(uint channelId, float pan)
    {
        Channel_SetPan(channelId, pan);
    }

    public override bool GetChannelPaused(uint channelId)
    {
        Channel_GetPaused(channelId, out bool pause);
        return pause;
    }

    public override void SetChannelPaused(uint channelId, bool pause)
    {
        Channel_SetPaused(channelId, pause);
    }

    public override bool ChannelIsPlaying(uint channelId)
    {
        Channel_IsPlaying(channelId, out bool playing);
        return playing;
    }

    public override void StopChannel(uint channelId)
    {
        Channel_Stop(channelId);
    }

    public override float GetChannelVolume(uint channelId)
    {
        Channel_GetVolume(channelId, out float volume);
        return volume;
    }

    public override void SetChannelVolume(uint channelId, float volume)
    {
        Channel_SetVolume(channelId, volume);
    }

}
