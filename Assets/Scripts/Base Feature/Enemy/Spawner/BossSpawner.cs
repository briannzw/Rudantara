using System.Collections;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private float SpawnTime = 1f * 60;
    [SerializeField] private GameObject boss;

    private DungeonNavMesh dungeonNavMesh;

    private void Start()
    {
        GameManager.Instance.bossHUD.SetBoss(boss);
        boss.SetActive(false);
        dungeonNavMesh = GameManager.Instance.DungeonNavMesh;
        DungeonGenerator.Instance.OnDungeonComplete += () => StartCoroutine(Spawn());
    }


    private IEnumerator Spawn()
    {
        transform.parent.Find("Chunk(1, 1)/Ground").gameObject.layer = LayerMask.NameToLayer("BossArea");
        dungeonNavMesh.Initialize();
        yield return new WaitForSeconds(SpawnTime);
        GameManager.Instance.LevelManager.SetBoss(boss.GetComponent<Character>());
        boss.SetActive(true);
        GameManager.Instance.bossHUD.Spawned();
        boss.GetComponent<CharacterLeveling>().Initialize(GameManager.Instance.LevelManager.CurrentEnemyBaseLevel);
    }
}
