using System;
using NeoGXP.NeoGXP.Core;
using static NeoGXP.NeoGXP.SoLoud.Soloud;

namespace NeoGXP.NeoGXP.SoLoud;

public class SoloudSoundSystem : SoundSystem
{
    private IntPtr _device;

    public override void Init() {
        _device = Soloud_create();
        Soloud_init(_device);
    }

    public override void Deinit()
    {
        if (_device == IntPtr.Zero) return;
        Soloud_stopAll(_device);
        Soloud_deinit(_device);
        _device = IntPtr.Zero;
    }

    public override IntPtr CreateStream(string filename, bool looping)
    {
        IntPtr id = WavStream_create();
        WavStream_load(id, filename);
        WavStream_setLooping(id, looping);
        if (id == IntPtr.Zero)
        {
            throw new Exception("Stream file not loaded: " + filename);
        }
        return id;
    }

    public override IntPtr LoadSound(string filename, bool looping)
    {
        IntPtr id = Wav_create();
        Wav_load(id, filename);
        Wav_setLooping(id, looping);
        if (id == IntPtr.Zero)
        {
            throw new Exception("Sound file not loaded: " + filename);
        }
        return id;
    }

    public override void Step()
    {
        //empty
    }

    public override uint PlaySound(IntPtr id, uint channelId, bool paused)
    {
        return id == IntPtr.Zero ? 0 : Soloud_playEx(_device, id, 1.0f, 0.0f, paused, 0);
    }

    public override uint PlaySound(IntPtr id, uint channelId, bool paused, float volume, float pan)
    {
        return id == IntPtr.Zero ? 0 : Soloud_playEx(_device, id, volume, pan, paused, 0);
    }


    public override float GetChannelFrequency(uint channelId)
    {
        return Soloud_getSamplerate(_device, channelId);
    }

    public override void SetChannelFrequency(uint channelId, float frequency)
    {
        Soloud_setSamplerate(_device, channelId, frequency);
    }

    public override float GetChannelPan(uint channelId)
    {
        return Soloud_getPan(_device, channelId);
    }

    public override void SetChannelPan(uint channelId, float pan)
    {
        Soloud_setPan(_device, channelId, pan);
    }

    public override bool GetChannelPaused(uint channelId)
    {
        return Soloud_getPause(_device, channelId);
    }

    public override void SetChannelPaused(uint channelId, bool pause)
    {
        Soloud_setPause(_device, channelId, pause);
    }

    public override bool ChannelIsPlaying(uint channelId)
    {
        return Soloud_isValidVoiceHandle(_device, channelId) && !Soloud_getPause(_device, channelId);
    }

    public override void StopChannel(uint channelId)
    {
        Soloud_stop(_device, channelId);
    }

    public override float GetChannelVolume(uint channelId)
    {
        return Soloud_getVolume(_device, channelId);
    }

    public override void SetChannelVolume(uint channelId, float volume)
    {
        Soloud_setVolume(_device, channelId, volume);
    }

}
