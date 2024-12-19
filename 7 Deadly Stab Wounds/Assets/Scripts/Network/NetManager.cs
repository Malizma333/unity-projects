using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : NetworkManager
{
 
    public static NetManager networkInst;

    public bool inGame;

    public GameObject humanPrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();
    
        networkInst = this;

        NetworkServer.RegisterHandler<PlayerMessage>(OnCreateCharacter);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        if (!IsSceneActive("Main Menu"))
        {
            conn.clientOwnedObjects.Clear();
            PlayerMessage message = new PlayerMessage
            {
                playerType = Character.CharacterType.HUMAN
            };

            conn.Send(message);
        }
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {

        conn.clientOwnedObjects.Clear();
        PlayerMessage message = new PlayerMessage
        {
            playerType = Character.CharacterType.HUMAN
        };

        conn.Send(message);
    }

    void OnCreateCharacter(NetworkConnection conn, PlayerMessage message)
    {
        GameObject player;

        if (message.playerType == Character.CharacterType.HUMAN)
        {
            player = Instantiate(humanPrefab);
        }
        else
        {
            player = null;
        }

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public struct PlayerMessage : NetworkMessage
    {
        public Character.CharacterType playerType;
    }

   
}
