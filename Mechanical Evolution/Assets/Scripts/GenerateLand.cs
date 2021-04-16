using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

class Tile
{
    public GameObject theTile;
    public float creationTime;

    public Tile(GameObject t, float cTime) {
        {
            theTile = t;
            creationTime = cTime;
        }
    }
}
public class GenerateLand : MonoBehaviour
{

    public GameObject plane;
    public GameObject player;

    int planeSize = 6;
    int renderSize = 4;

    Vector3 startPos;

    Hashtable tiles = new Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.position = Vector3.zero;
        startPos = Vector3.zero;

        float updateTime = Time.realtimeSinceStartup;

        for(int i = -renderSize; i < renderSize; i++)
        {
            for(int j = -renderSize; j < renderSize; j++)
            {
                Vector3 pos = new Vector3((i * planeSize + startPos.x), 0, (j * planeSize + startPos.z));

                GameObject t = (GameObject) Instantiate(plane, pos, Quaternion.identity);

                string tileName = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();
                t.name = tileName;
                Tile tile = new Tile(t, updateTime);
                tiles.Add(tileName, tile);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        int xMove = (int)(player.transform.position.x - startPos.x);
        int zMove = (int)(player.transform.position.z - startPos.z);

            //if player moves a distance bigger than plane size
        if(Mathf.Abs(xMove) >= planeSize || Mathf.Abs(zMove) >= planeSize)
        {
            float updateTime = Time.realtimeSinceStartup;

            //round down players position
            int playerX = (int)(Mathf.Floor(player.transform.position.x/planeSize)*planeSize);
            int playerZ = (int)(Mathf.Floor(player.transform.position.z/planeSize)*planeSize);

            for(int i = -renderSize; i < renderSize; i++)
            {
                for(int j = -renderSize; j < renderSize; j++)
                {
                    Vector3 pos = new Vector3((i * planeSize + playerX), 0, (j * planeSize + playerZ));

                    string tileName = "Tile_" + ((int)(pos.x)).ToString() + "_" + ((int)(pos.z)).ToString();

                    if(!tiles.ContainsKey(tileName))
                    {   
                        GameObject t = (GameObject) Instantiate(plane, pos, Quaternion.identity);

                        t.name = tileName;
                        Tile tile = new Tile(t, updateTime);
                        tiles.Add(tileName, tile);
                    }
                    else
                    {
                        (tiles[tileName] as Tile).creationTime = updateTime;
                    }
                }
            }
    
        //destroy old tiles and add new tiles
        Hashtable tempTerrain = new Hashtable();
        foreach(Tile ts in tiles.Values)
        {
            if(ts.creationTime != updateTime)
            {
                Destroy(ts.theTile);
            }
            else{
                tempTerrain.Add(ts.theTile.name, ts);
            }
        }

        tiles = tempTerrain;

        startPos = player.transform.position;
        }
    }
}
