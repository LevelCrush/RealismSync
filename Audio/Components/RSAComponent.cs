using Fika.Core.Coop.Players;
using Fika.Core.Coop.Utils;
using Fika.Core.Networking;
using RealismMod;
using UnityEngine;

namespace RealismModSync.Audio.Components;

public class RSAComponent : MonoBehaviour
{
    CoopPlayer player;
    
    private AudioSource _foodPoisoningSfx;
    private AudioSource _gasMaskAudioSource;
    private AudioSource _gasAnalyserSource;
    private AudioSource _geigerAudioSource;
    private AudioSource _toggleDeviceSource;
    
    private void Start()
    {
        player = GetComponent<CoopPlayer>();
        if(player == null)
        {
            Destroy(this);
            return;
        }
    }
    
    // copy pasted from RealismMod and adjusted for RealismSync if needed
    private void SetUpAudio(AudioSource source, float vol = 1f, float spatialBlend = 1f, float minDistance = 1f, float maxDistance = 30f) 
    {
        source.volume = vol * GameWorldController.GetGameVolumeAsFactor();
        source.spatialBlend = spatialBlend; 
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;
        source.spatialize = true;
        source.rolloffMode = AudioRolloffMode.Linear; // for now try linear?
    }

}