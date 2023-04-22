using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviourSingleton<MapGenerator>
{
    public float laneOffset = 3f;

    int itemSpace = 15;
    int itemCountInMap = 5;

    int coinsCountInItem = 10;
    float coinsHeight = .5f;

    int mapSize;

    enum TrackPos { Left = -1, Center = 0, Right = 1}
    enum CoinStyle { Line, Jump, Ramp }

    public GameObject obstacleTopPrefab;
    public GameObject obstacleBottomPrefab;
    public GameObject rampPrefab;
    public GameObject coinPrefab;

    public List<GameObject> maps;
    public List<GameObject> activeMaps;

    struct MapItem
    {
        public void SetValues(GameObject obstacle, TrackPos trackPos, CoinStyle coinStyle)
        {
            this.obstacle = obstacle;
            this.trackPos = trackPos;
            this.coinStyle = coinStyle;
        }

        public GameObject obstacle { get; private set; }
        public TrackPos trackPos { get; private set; }
        public CoinStyle coinStyle { get; private set; }
    }

    private void Awake()
    {
        mapSize = itemCountInMap * itemSpace;

        maps = new();
        activeMaps = new();

        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
        maps.Add(MakeMap1());
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
                                new Vector3(0, 0, 10);

        maps.RemoveAt(r);
        activeMaps.Add(go);
    }

    private GameObject MakeMap1()
    {
        GameObject result = new GameObject("Map1");
        result.transform.SetParent(transform);
        MapItem item = new();
        for (int i = 0; i < itemCountInMap; i++)
        {
            item.SetValues(null, TrackPos.Center, CoinStyle.Line);
            
            if (i == 2) { item.SetValues(obstacle: rampPrefab, trackPos: TrackPos.Left, coinStyle: CoinStyle.Ramp); }
            else if (i == 3) { item.SetValues(obstacle: obstacleBottomPrefab, trackPos: TrackPos.Right, coinStyle: CoinStyle.Jump); }
            else if (i == 4) { item.SetValues(obstacle: obstacleTopPrefab, trackPos: TrackPos.Right, coinStyle: CoinStyle.Line); }

            Vector3 obstaclePos = new Vector3((int)item.trackPos * laneOffset, 0, i * itemSpace);

            CreateCoins(item.coinStyle, obstaclePos, result);

            if (item.obstacle != null)
            {
                GameObject go = Instantiate(item.obstacle, obstaclePos, Quaternion.identity);
                go.transform.SetParent(result.transform);
            }
        }
        return result;
    }

    private void CreateCoins(CoinStyle style, Vector3 pos, GameObject parentObject)
    {
        Vector3 coinPos = Vector3.zero;
        if (style == CoinStyle.Line)
        {
            for (int i = -coinsCountInItem/2; i < coinsCountInItem/2; i++)
            {
                coinPos.y = coinsHeight;
                coinPos.z = i * ((float)itemSpace/coinsCountInItem);
                GameObject go = Instantiate(coinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);
            }
        }
        else if (style == CoinStyle.Jump)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = Mathf.Max(-1/2f * Mathf.Pow(i,2), coinsHeight);
                coinPos.z = i * ((float)itemSpace / coinsCountInItem);
                GameObject go = Instantiate(coinPrefab, coinPos + pos, Quaternion.identity);
                go.transform.SetParent(parentObject.transform);
            }
        }
        else if (style == CoinStyle.Ramp)
        {
            for (int i = -coinsCountInItem / 2; i < coinsCountInItem / 2; i++)
            {
                coinPos.y = Mathf.Min(Mathf.Max(0.7f * (i+2), coinsHeight), 3.0f);
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
