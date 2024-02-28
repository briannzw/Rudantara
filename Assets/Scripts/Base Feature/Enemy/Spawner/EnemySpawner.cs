using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private float spawnRate = 15;
    [SerializeField] private float spawnCount = 3;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Additional")]
    [SerializeField] private int tinySpawnChance = 10;

    private void Start()
    {
        StartCoroutine(DoSpawn());
    }

    private IEnumerator DoSpawn()
    {
        while (true)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius + spawnOffset;
                randomPos.y = 0f;

                if (NavMesh.SamplePosition(randomPos, out var hit, StatsConst.NAV_SAMPLE_POS_MAX_DISTANCE, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    // Check Tiny Spawn
                    if (Random.Range(0, 100) < tinySpawnChance)
                    {
                        GameObject go = Instantiate(spawnObject, hit.position + spawnOffset, Quaternion.identity);
                        go.transform.localScale = Vector3.one * .5f;
                        Character chara = go.GetComponent<Character>();
                        chara.ModifyStat(DynamicStatEnum.Health, new Kryz.CharacterStats.StatModifier(-.4f, Kryz.CharacterStats.StatModType.PercentMult));
                        chara.ModifyStat(StatEnum.Speed, new Kryz.CharacterStats.StatModifier(.5f, Kryz.CharacterStats.StatModType.PercentMult));
                        go.GetComponent<EnemyController>().StopDistance = StatsConst.STOPPING_DISTANCE / 2f;
                    }
                    else
                    {
                        Instantiate(spawnObject, hit.position + spawnOffset, Quaternion.identity);
                    }
                }
                else yield return null;
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + spawnOffset, spawnRadius);
    }
}
