using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviourSingleton<MapGenerator> {
    public float laneOffset = 3f;

    public int itemSpace = 15;
    public int itemCountInMap = 5;

    public int coinsCountInItem = 10;
    private float coinsHeight = .5f;
    private int mapSize;

    public enum TrackPos { Left = -1, Center = 0, Right = 1 }

    public enum CoinStyle { Line, Jump, Ramp }

    public GameObject coinPrefab;

    public MapObject[] mapObjects;
    public List<GameObject> maps;
    public List<GameObject> activeMaps;

  
    private void Awake()
    {
        mapSize = itemCountInMap * itemSpace;

        maps = new();
        activeMaps = new();

        foreach (MapObject mapData in mapObjects)
        {
            maps.Add(MakeMap(mapData));
        }

        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }
    }

    private void Update()
    {
        if (SectionGenerator.Instance.speed == 0) return;

        foreach (GameObject map in activeMaps)
        {
            map.transform.position -= new Vector3(0, 0, SectionGenerator.Instance.speed * Time.deltaTime);
        }
        if (activeMaps[0].transform.position.z < -mapSize)
        {
            RemoveFirstActiveMap();
            AddActiveMap();
        }
    }

    private void RemoveFirstActiveMap()
    {
        activeMaps[0].SetActive(false);
        maps.Add(activeMaps[0]);
        activeMaps.RemoveAt(0);
    }

    private void AddActiveMap()
    {
        int r = Random.Range(0, maps.Count);
        GameObject go = maps[r];
        go.SetActive(true);
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetActive(true);
        }
        go.transform.position = activeMaps.Count > 0 ?
                                activeMaps[activeMaps.Count - 1].transform.position + Vector3.forward * mapSize :
                                new Vector3(0, 0, 15);

        maps.RemoveAt(r);
        activeMaps.Add(go);
    }

    private GameObject MakeMap(MapObject mapData)
    {
        GameObject result = new GameObject(mapData.name);
        result.transform.SetParent(transform);

        for (int i = 0; i < mapData.rows.Length; i++)
        {
            var row = mapData.rows[i];
            foreach (MapObject.Obstacle item in row.GetObstacles())
            {
                Vector3 obstaclePos = new Vector3((int)item.trackPos * laneOffset, 0, i * itemSpace);

                if (item.obstacle != null)
                {
                    CreateCoins(item.coinStyle, obstaclePos, result);
                    GameObject go = Instantiate(item.obstacle, obstaclePos, Quaternion.identity);
                    go.transform.SetParent(result.transform);
                }
            }
        }
        return result;
    }

    private void CreateCoins(CoinStyle style, Vector3 pos, GameObject parentObject)
    {
        if (Random.Range(0f, 1f) > 0.35f) return;

        Vector3 coinPos = Vector3.zero;
        if (style == CoinStyle.Line)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = coinsHeight;
                coinPos.z = i * ((float)itemSpace / coinsCountInItem);
                GameObject go = Instantiate(coinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);
            }
        }
        else if (style == CoinStyle.Jump)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = Mathf.Max(-1 / 2f * Mathf.Pow(i, 2) + 3, coinsHeight);
                coinPos.z = i * ((float)itemSpace / coinsCountInItem);
                GameObject go = Instantiate(coinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);
            }
        }
        else if (style == CoinStyle.Ramp)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = Mathf.Min(Mathf.Max(0.7f * (i + 2), coinsHeight), 3.0f);
                coinPos.z = i * ((float)itemSpace / coinsCountInItem);
                GameObject go = Instantiate(coinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);
            }
        }
    }

    public void ResetMaps()
    {
        while (activeMaps.Count > 0)
        {
            RemoveFirstActiveMap();
        }
        AddActiveMap();
        AddActiveMap();
    }
}