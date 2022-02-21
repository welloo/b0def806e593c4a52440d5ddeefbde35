using System.Collections.Generic;
using UnityEngine;
using static Extensions;

public class GameManager : Singleton<GameManager>
{
    public enum GameMode
    {
        LOW, MEDIUM, HARD
    }

    private GameMode _gameMode;
    public List<GameObject> _spawnedObjects;

    public float spawningDelay = 1;
    public float MinOffsetX, MaxOffsetX = 0;
    public GameObject BulletPrefab = null;
    public GameObject AsteroidPrefab = null;
    public GameObject BonusPrefab = null;
    public GameObject AIPrefab = null;

    public IGameState gameState { get; private set; }
    public GameMode currentGameMode { get => _gameMode; }
    public List<GameObject> SpawnedObjects { get => _spawnedObjects; }

    #region Unity Methods
    #endregion

    #region Public Methods
    public void StartGameMode(GameMode mode)
    {
        PlayerController.Instance.enabled = true;
        _spawnedObjects = new List<GameObject>();
        _gameMode = mode;
        gameState = _gameMode switch
        {
            GameMode.LOW => new BaseGameState(this, 0, 1),
            GameMode.MEDIUM => new BaseGameState(this, 20, 2),
            GameMode.HARD => new HardGameState(this, 40, 3)
        };
        gameState.StartSpawningByDelay(spawningDelay);
    }

    public void StartGameMode(string mode)
    {
        if (System.Enum.TryParse<GameMode>(mode, out var result))
        {
            StartGameMode(result);
        }
        else
            Debug.LogError("Incorrect input mode.");
    }

    public GameObject InstantiateGameObject(GameObject prefab)
    {
        var spawnPosition = GetRandomPositionForSpawning(MinOffsetX, MaxOffsetX);
        var objInstance = Instantiate(prefab, spawnPosition, new Quaternion());
        objInstance.transform.SetParent(transform);
        ResgisterElement(objInstance);
        return objInstance;
    }

    public void ResgisterElement(GameObject element)
    {
        if (_spawnedObjects.Contains(element) || !element)
        {
            return;
        }
        _spawnedObjects.Add(element);
    }
    public void UnResgisterElement(GameObject element)
    {
        if (!_spawnedObjects.Contains(element) || !element)
        {
            return;
        }
        _spawnedObjects.Remove(element);
    }

    public void DisableSpawner()
    {
        gameState.StopSpawning();
    }

    public void DestroyChildObjects()
    {
        foreach (Transform obj in transform)
        {
            Destroy(obj.gameObject);
        }
    }
    #endregion

    #region Private Methods
    private Vector3 GetRandomPositionForSpawning(float min, float max) => new Vector3(Random.Range(min, max), 7, 0);

    #endregion
}
