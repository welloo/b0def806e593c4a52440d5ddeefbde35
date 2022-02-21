using System.Collections;
using UnityEngine;

public interface IGameState
{
    GameManager CurrentGameManager { get; }
    float AISpawnedChance { get; }
    int AsteroidsMaxHP { get; }
    void StartSpawningByDelay(float delay);
    void StopSpawning();
}

public class BaseGameState : IGameState
{
    private GameManager _currentGameManager;
    private float _aiSpawnedChance;
    private int _asteroidsMaxHP;
    private bool _onStopSpawning = false;

    public GameManager CurrentGameManager { get => _currentGameManager; }
    public float AISpawnedChance { get => _aiSpawnedChance; }
    public int AsteroidsMaxHP { get => _asteroidsMaxHP; }

    public BaseGameState(GameManager gameManager, float aiSpawnedChance, int asteroidsMaxHP)
    {
        _currentGameManager = gameManager;
        _aiSpawnedChance = aiSpawnedChance;
        _asteroidsMaxHP = asteroidsMaxHP;
        if (_currentGameManager.AsteroidPrefab.TryGetComponent<IDamagebleObject>(out var damageble))
        {
            damageble.SetHealth(Random.Range(1, _asteroidsMaxHP + 1));
        }
        if (_currentGameManager.AIPrefab.TryGetComponent<AIBehaviour>(out var aiBehaviour))
        {
            aiBehaviour.SetBonuseSpawnerState(false);
        }
    }

    public void StartSpawningByDelay(float delay)
    {
        _currentGameManager.SpawnedObjects.Clear();
        _currentGameManager.StartCoroutine(StartSpawner(delay, _aiSpawnedChance));
    }

    private IEnumerator StartSpawner(float delay, float aiSpawnedChance)
    {
        if (!_onStopSpawning)
        {
            var factor = Random.Range(1, 101);
            var spawnObject = factor > aiSpawnedChance ?
                _currentGameManager.AsteroidPrefab :
                _currentGameManager.AIPrefab;
            var spawnObjectInstance = _currentGameManager.InstantiateGameObject(spawnObject);
            yield return new WaitForSeconds(delay);
            _currentGameManager.StartCoroutine(StartSpawner(delay, aiSpawnedChance));
        }
        else
        {
            _onStopSpawning = false;
            CurrentGameManager.SpawnedObjects.Clear();
        }
    }

    public void StopSpawning() => _onStopSpawning = true;
}

public class HardGameState : BaseGameState
{
    public HardGameState(GameManager gameManager, float aiSpawnedChance, int asteroidsMaxHP) : base(gameManager, aiSpawnedChance, asteroidsMaxHP)
    {
        if (CurrentGameManager.AIPrefab.TryGetComponent<AIBehaviour>(out var aiBehaviour))
        {
            aiBehaviour.SetBonuseSpawnerState(true);
        }
    }
}
