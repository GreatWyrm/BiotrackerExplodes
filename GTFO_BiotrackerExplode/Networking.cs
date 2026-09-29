using AK;
using GTFO.API;
using UnityEngine;

namespace BiotrackerExplode;

public class Networking
{
    
    private static CellSoundPlayer soundPlayer;
    private static readonly string NetworkEvent = "BiotrackerExplode";
    
    public static void init()
    {
        NetworkAPI.RegisterEvent<Vector3>(NetworkEvent, OnExplosionReceive);
        soundPlayer = new();
    }

    public static void BroadcastAndPlayToSelf(Vector3 position)
    {
        NetworkAPI.InvokeEvent(NetworkEvent, position);
        soundPlayer.Post(EVENTS.EXPLODEREXPLODE, position);
    }

    public static void OnExplosionReceive(ulong id, Vector3 position)
    {
        
    }
}