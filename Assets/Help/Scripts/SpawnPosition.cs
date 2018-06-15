using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    public List<SpawnPos> pos;
}

[Serializable]
public class SpawnPos
{
    public GameObject pos;
    public int index;
}