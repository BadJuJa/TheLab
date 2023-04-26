using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Level/New map")]
public class MapObject : ScriptableObject {

    [Serializable]
    public struct Obstacle {
        public GameObject obstacle;
        public MapGenerator.TrackPos trackPos;
        public MapGenerator.CoinStyle coinStyle;
    }

    [Serializable]
    public struct ObstacleRow {
        public Obstacle left;
        public Obstacle center;
        public Obstacle right;

        public Obstacle[] GetObstacles()
        {
            return new Obstacle[] { left, center, right };
        }
    }

    public string Name = "Map";
    public int itemSpace = 15;
    public ObstacleRow[] rows;
}