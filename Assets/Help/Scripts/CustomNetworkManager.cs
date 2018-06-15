using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public struct BusyPositions
{
	public short playerId;
	public int spawnPoint;
}

public class BusyPosList : SyncListStruct<BusyPositions> { }
