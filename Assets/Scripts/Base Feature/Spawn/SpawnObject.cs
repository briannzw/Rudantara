using System.Collections;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public EnemySpawner Spawner;
    public virtual void Release()
    {
        if (Spawner == null) return;

        Spawner.OnRelease(this);
    }

    public void ReleaseAfter(float time)
    {
        StartCoroutine(Count(time));
    }

    private IEnumerator Count(float time)
    {
        yield return new WaitForSeconds(time);

        Release();
    }
}
