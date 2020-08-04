using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public enum LevelStatus {Closed, Opened, Complited};
    public bool isNotEmpty;
    public LevelStatus levelStatus;
    public int currentGoal;
    public int score;
    public int shieldEnergy;
    public Dictionary<GameObject, Transform> asteroids = new Dictionary<GameObject, Transform>();
    public GameObject playerShip;

}
