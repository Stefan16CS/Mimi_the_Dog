using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
   
    public Transform player;
    public GameObject groundBirdPrefab;
    public GameObject flyingBirdPrefab;

    [Header("Spawn control")]
    
    public int maxAlive = 9999;
    public Vector2 spawnIntervalRange = new Vector2(1f, 2f);
    [Range(0f, 1f)] public float groundSpawnChance = 0.5f;
    [Range(0f, 1f)] public float spawnAheadBias = 0.8f;
    public float minEnemySeparation = 2.0f;

    [Header("Common X distance constraints")]

    public float minXDistanceFromPlayer = 20f;
    public float maxXDistanceFromPlayer = 25f;

    [Header("Ground bird placement")]
   
    public bool useGroundRaycast = true;
    public LayerMask groundLayer;
    public float groundRaycastTop = 10f;
    public float groundYOffset = 0.5f;
    public float fallbackGroundY = -4f;

    [Header("Flying bird vertical bands")]
    
    public BandMode flyingBandMode = BandMode.RandomBand;
    public Vector2 lowBandY = new Vector2(-4f, -3f);
    public Vector2 highBandY = new Vector2(-3f, 0f);
    [Range(0f, 1f)] public float lowBandChance = 0.7f;

    [Header("Spawn safety")]
    
    public float minDistanceToPlayer = 10f;
    public int maxPlacementAttempts = 6;

    [Header("Debug/Gizmos")]
    public bool drawGizmos = true;
    public Color gizmoXRangeColor = new Color(0.3f, 0.8f, 1f, 0.2f);
    public Color gizmoLowBandColor = new Color(0.2f, 1f, 0.2f, 0.15f);
    public Color gizmoHighBandColor = new Color(1f, 0.8f, 0.2f, 0.15f);

    public enum BandMode { LowOnly, HighOnly, RandomBand }

    private float _nextSpawnAt;
    private readonly List<Transform> _alive = new List<Transform>(64);
    private Vector3 _prevPlayerPos;

    void Start()
    {
        if (player != null) _prevPlayerPos = player.position;
        ScheduleNextSpawn();
    }

    void Update()
    {
        for (int i = _alive.Count - 1; i >= 0; i--)
        {
            if (_alive[i] == null) _alive.RemoveAt(i);
        }

        if (player == null || (groundBirdPrefab == null && flyingBirdPrefab == null)) return;

        if (Time.time >= _nextSpawnAt && _alive.Count < maxAlive)
        {
            TrySpawnOne();
            ScheduleNextSpawn();
        }

        if (player != null) _prevPlayerPos = player.position;
    }

    private void ScheduleNextSpawn()
    {
        float t = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
        _nextSpawnAt = Time.time + Mathf.Max(0.05f, t);
    }

    private void TrySpawnOne()
    {
        bool spawnGround = DecideGroundOrFlying();

        GameObject prefab = spawnGround ? groundBirdPrefab : flyingBirdPrefab;
        if (prefab == null) return;

        for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
        {
            Vector3 pos = spawnGround ? GetGroundSpawnPosition() : GetFlyingSpawnPosition();
            if (!IsValidSpawnPosition(pos)) continue;

            Transform inst = Instantiate(prefab, pos, Quaternion.identity).transform;
            _alive.Add(inst);
            return;
        }
        
    }

    private bool DecideGroundOrFlying()
    {
        float random = Random.value;
        if (groundBirdPrefab == null && flyingBirdPrefab != null) return false;
        if (flyingBirdPrefab == null && groundBirdPrefab != null) return true;
        return random < groundSpawnChance;
    }

    private Vector3 GetGroundSpawnPosition()
    {
        float x = ChooseSpawnX();
        float y = fallbackGroundY;

        if (useGroundRaycast)
        {
            Vector2 origin = new Vector2(x, groundRaycastTop);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, Mathf.Infinity, groundLayer);
            if (hit.collider != null) y = hit.point.y + groundYOffset;
        }
        
            return new Vector3(x, y, 0f);
    }

    private Vector3 GetFlyingSpawnPosition()
    {
        float x = ChooseSpawnX();

        float y;
        BandMode mode = flyingBandMode;
        if (mode == BandMode.RandomBand)
        {
            mode = (Random.value < lowBandChance) ? BandMode.LowOnly : BandMode.HighOnly;
        }

        if (mode == BandMode.LowOnly)
        {
            y = Random.Range(lowBandY.x, lowBandY.y);
        }
        else 
        {
            y = Random.Range(highBandY.x, highBandY.y);
        }

        return new Vector3(x, y, 0f);
    }

    private float ChooseSpawnX()
    {
        float dir = ChooseSideRelativeToPlayer();
        float dist = Random.Range(minXDistanceFromPlayer, maxXDistanceFromPlayer);
        return player.position.x + dir * dist;
    }

    private float ChooseSideRelativeToPlayer()
    {
        if (player == null) return (Random.value < 0.5f) ? -1f : 1f;

        float dx = player.position.x - _prevPlayerPos.x;
        if (Mathf.Abs(dx) < 0.001f)
        {
            
            return (Random.value < 0.5f) ? -1f : 1f;
        }

        bool movingRight = dx > 0f;
        
        if (Random.value < spawnAheadBias)
            return movingRight ? 1f : -1f;

        return movingRight ? -1f : 1f;
    }

    private bool IsValidSpawnPosition(Vector3 pos)
    {
        if (Vector2.Distance(pos, player.position) < minDistanceToPlayer) return false;

        for (int i = 0; i < _alive.Count; i++)
        {
            Transform t = _alive[i];
            if (t == null) continue;
            if (Vector2.Distance(pos, t.position) < minEnemySeparation) return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos || player == null) return;

        Gizmos.color = gizmoXRangeColor;
        float px = player.position.x;
        float y = player.position.y;
        float minX = px - maxXDistanceFromPlayer;
        float maxX = px + maxXDistanceFromPlayer;

        DrawHollowRect(new Vector3((minX + maxX) * 0.5f, y + 0.25f, 0f),
                       new Vector3((maxX - minX), 0.5f, 0f), gizmoXRangeColor);

        Gizmos.color = gizmoLowBandColor;
        DrawFilledRect(new Vector3(px, (lowBandY.x + lowBandY.y) * 0.5f, 0f),
                       new Vector3(maxXDistanceFromPlayer * 2f, (lowBandY.y - lowBandY.x), 0f), gizmoLowBandColor);

        Gizmos.color = gizmoHighBandColor;
        DrawFilledRect(new Vector3(px, (highBandY.x + highBandY.y) * 0.5f, 0f),
                       new Vector3(maxXDistanceFromPlayer * 2f, (highBandY.y - highBandY.x), 0f), gizmoHighBandColor);
    }

    private void DrawFilledRect(Vector3 center, Vector3 size, Color color)
    {
        Color prev = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawCube(center, size);
        Gizmos.color = prev;
    }

    private void DrawHollowRect(Vector3 center, Vector3 size, Color color)
    {
        float t = 0.05f;
        Vector3 half = size * 0.5f;

        Vector3 leftCenter = center + new Vector3(-half.x, 0f, 0f);
        Vector3 rightCenter = center + new Vector3(half.x, 0f, 0f);
        Vector3 topCenter = center + new Vector3(0f, half.y, 0f);
        Vector3 bottomCenter = center + new Vector3(0f, -half.y, 0f);

        Gizmos.DrawCube(leftCenter, new Vector3(t, size.y, t));
        Gizmos.DrawCube(rightCenter, new Vector3(t, size.y, t));
        Gizmos.DrawCube(topCenter, new Vector3(size.x, t, t));
        Gizmos.DrawCube(bottomCenter, new Vector3(size.x, t, t));
    }
}