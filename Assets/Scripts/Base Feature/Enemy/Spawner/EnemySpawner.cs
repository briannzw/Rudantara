using Pool;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    private PoolManager poolManager;
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private GameObject spawnEffect;

    [Header("Parameters")]
    [SerializeField] private float spawnRate = 15;
    [SerializeField] private float spawnCount = 3;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private int maxSpawn = 50;

    [Header("Additional")]
    [SerializeField] private int tinySpawnChance = 10;

    public Action<SpawnObject> OnRelease;

    private void Start()
    {
        poolManager = PoolManager.Instance;
        Debug.Log(spawnObject);
        Debug.Log(maxSpawn);
        poolManager.Add(spawnObject, maxSpawn);
        OnRelease += Release;
        StartCoroutine(DoSpawn());
    }

    private IEnumerator DoSpawn()
    {
        while (true)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                if (poolManager.Pools[spawnObject].CountActive >= maxSpawn)
                {
                    //Debug.LogWarning("Max Object in Pool has reached its limit, " + spawnObject.name + " can't be spawned.");
                    break;
                }

                Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius + spawnOffset;
                randomPos.y = 0f;

                if (NavMesh.SamplePosition(randomPos, out var hit, StatsConst.NAV_SAMPLE_POS_MAX_DISTANCE, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    GameObject go = poolManager.Pools[spawnObject].Get();
                    go.transform.position = hit.position + spawnOffset;
                    go.transform.rotation = Quaternion.identity;


                    if (go.GetComponent<SpawnObject>() == null) go.AddComponent<SpawnObject>();
                    go.GetComponent<SpawnObject>().Spawner = this;
                    go.SetActive(true);

                    Describable desc = go.GetComponent<Describable>();
                    if (desc.Name == spawnObject.GetComponent<Describable>().Name)
                        desc.Name += " " + poolManager.Pools[spawnObject].CountActive.ToString();

                    // Check Tiny Spawn
                    if (Random.Range(0, 100) < tinySpawnChance)
                    {
                        go.transform.localScale = Vector3.one * .5f;

                        Character chara = go.GetComponent<Character>();
                        chara.ModifyStat(DynamicStatEnum.Health, new Kryz.CharacterStats.StatModifier(-.4f, Kryz.CharacterStats.StatModType.PercentMult));
                        chara.ModifyStat(StatEnum.Speed, new Kryz.CharacterStats.StatModifier(.5f, Kryz.CharacterStats.StatModType.PercentMult));
                        go.GetComponent<AgentController>().StopDistance = StatsConst.STOPPING_DISTANCE / 2f;
                    }

                    // VFX
                    GameObject vfx = Instantiate(spawnEffect, hit.position + spawnOffset, Quaternion.identity);
                    Destroy(vfx, vfx.GetComponent<UnityEngine.VFX.VisualEffect>().GetFloat("Duration"));
                }
                else yield return null;
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void Release(SpawnObject obj)
    {
        if (obj == null) return;
        poolManager.Pools[spawnObject].Release(obj.gameObject);
    }

    private void OnDestroy()
    {
        OnRelease -= Release;
        if (poolManager != null) poolManager.Remove(spawnObject);

        StopAllCoroutines();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + spawnOffset, spawnRadius);
    }
}
