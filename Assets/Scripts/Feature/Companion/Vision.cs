using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class Vision : MonoBehaviour
{
    private const int MAX_STEPS = 20;

    [Header("Parameters")]
    [SerializeField] private float viewingDistance = 5f;
    [SerializeField] private float viewingHeight = 6f;
    [SerializeField] private LayerMask viewingMask;
    [SerializeField] private float fovHorizontal = 150f;
    [SerializeField, TagField] private string[] viewTags;

    private Personality owner;

    private new MeshCollider collider;
    private MeshColliderCookingOptions cookingOptions =
        MeshColliderCookingOptions.UseFastMidphase & MeshColliderCookingOptions.CookForFasterSimulation;

    private HashSet<Describable> seen = new();

    public HashSet<Describable> Seen { get => seen; }

    private void OnEnable()
    {
        if (viewingMask == default) Debug.LogWarning("Viewing Mask is not set yet!");
    }

    private void Awake()
    {
        owner = GetComponentInParent<Personality>();
        collider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        SetFOV();
    }

    private void SetFOV()
    {
        Mesh visionMesh = GenerateVisionMesh();
        Physics.BakeMesh(visionMesh.GetInstanceID(), false, cookingOptions);
        collider.sharedMesh = visionMesh;
    }

    //https://forum.unity.com/threads/trouble-with-mesh-generation-for-a-field-of-view.1406002/#post-8853907
    private Mesh GenerateVisionMesh()
    {
        Mesh mesh = new();
        // Initial pos + extra vertices for steps
        Vector3[] vertices = new Vector3[2 * (MAX_STEPS + 1) + 1];
        int[] tris = new int[2 * (MAX_STEPS * 3 * 2) + 6];

        // Complex
        var origin = Vector3.zero;
        var dir = Vector3.forward;
        var radius = viewingDistance;

        var forwardLimitPos = origin + dir;
        var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - origin.z, forwardLimitPos.x - origin.x);
        var initialPos = origin;
        var posA = initialPos;
        var stepAngles = fovHorizontal / MAX_STEPS;
        var angle = srcAngles - fovHorizontal / 2;

        // Index 0
        vertices[0] = posA;

        // Tris 0
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;

        int vertexIndex = 1;
        int trisIndex = 3;

        for (int i = 0; i <= MAX_STEPS; i++)
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            // Below
            vertices[vertexIndex++] = new Vector3(posB.x, -viewingHeight / 2, posB.z);
            // Above
            vertices[vertexIndex] = new Vector3(posB.x, viewingHeight / 2, posB.z);

            // Except last step
            if (i < MAX_STEPS)
            {
                // Below segment to origin
                tris[trisIndex] = 0;
                tris[trisIndex + 1] = vertexIndex - 1;
                tris[trisIndex + 2] = vertexIndex + 1;

                trisIndex += 3;

                // Tris that connects below and above segment
                tris[trisIndex] = vertexIndex - 1;
                tris[trisIndex + 1] = vertexIndex;
                tris[trisIndex + 2] = vertexIndex + 1;

                trisIndex += 3;

                tris[trisIndex] = vertexIndex;
                tris[trisIndex + 1] = vertexIndex + 1;
                tris[trisIndex + 2] = vertexIndex + 2;

                trisIndex += 3;

                // Above segment to origin
                tris[trisIndex] = 0;
                tris[trisIndex + 1] = vertexIndex;
                tris[trisIndex + 2] = vertexIndex + 2;

                trisIndex += 3;
            }

            // Next Segment
            vertexIndex++;

            angle += stepAngles;
        }

        // Last tris
        tris[trisIndex] = 0;
        tris[trisIndex + 1] = vertexIndex - 2;
        tris[trisIndex + 2] = vertexIndex - 1;

        mesh.vertices = vertices;
        mesh.triangles = tris;
        return mesh;
    }

    private void CheckVision(GameObject obj, Describable describable)
    {
        // Prevent too realistic vision, making gameplay not fun
        //if (!IsObscured(obj))
        //{
        seen.Add(describable);
        owner.DescribeVisual("You saw [" + describable.Name + "]");
        owner.DescribeVisual(describable.InitialReport);
        describable.OnEvent += owner.DescribeVisual;

        if (!describable.CompareTag("Enemy")) return;

        Character chara = obj.GetComponent<Character>();
        if (chara != null)
        {
            chara.OnCharacterDie += OnCharaDied;
            owner.EnemyDetected(chara);
        }
        //}
    }

    private bool IsObscured(GameObject other)
    {
        Vector3 dir = (other.transform.position - transform.position).normalized;
        float distance = (other.transform.position - transform.position).magnitude;

        Physics.Raycast(transform.position, dir, out var hit, distance, viewingMask);

        if (hit.collider == null) return true;

        return hit.collider.gameObject != other;
    }

    private void OnTriggerEnter(Collider other)
    {
        Describable describable = null;

        foreach(var tag in viewTags)
        {
            if (other.CompareTag(tag))
                describable = other.GetComponentInChildren<Describable>();
        }

        if (describable != null)
        {
            if (seen.Contains(describable)) return;

            if (other.GetComponent<Rigidbody>() != null)
            {
                CheckVision(other.gameObject, describable);
            }
            else
            {
                seen.Add(describable);

                owner.DescribeVisual(describable.InitialReport);
                describable.OnEvent += owner.DescribeVisual;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Describable describable = null;

        foreach (var tag in viewTags)
            if (other.CompareTag(tag))
        {
                describable = other.GetComponentInChildren<Describable>();
        }

        if (describable != null)
        {
            if (seen.Contains(describable))
            {
                seen.Remove(describable);
                //owner.DescribeVisual("You can no longer see [" + describable.Name + "]");
                describable.OnEvent -= owner.DescribeVisual;

                if (describable.CompareTag("Enemy")) owner.EnemyRemoved(describable.GetComponent<Character>());
            }
        }
    }

    private void OnCharaDied(Character chara)
    {
        owner.EnemyRemoved(chara);
        chara.OnCharacterDie -= OnCharaDied;
    }
}
