using System;
using UnityEngine;

public class Describable : MonoBehaviour
{
    [field: SerializeReference] public string Name { get; set;  }
    public Action<string> OnEvent { get; set; }
    public string InitialReport { get; set; }
}
