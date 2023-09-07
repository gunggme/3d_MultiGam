using System.Collections;
using System.Collections.Generic;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform[] spawnPoints;

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            foreach (var item in Runner.ActivePlayers)
            {
                SpawnPlayer(item);
            }
        }
    }

    private void SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            var index = playerRef % this.spawnPoints.Length;
            var spawnPoint = this.spawnPoints[index].transform.position;
            var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);
            
            Debug.Log(playerRef);
            
            Runner.SetPlayerObject(playerRef, playerObject);
        }
    }

    private void DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
            {
                Runner.Despawn(playerNetworkObject);
            }
            
            Runner.SetPlayerObject(playerRef, null);
        }
    }
    
    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
}
