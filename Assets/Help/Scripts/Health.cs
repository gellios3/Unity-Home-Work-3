using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

	public const int maxHeath = 100;
	
	[SyncVar(hook="OnChangeHealth")] 
	public int currentHealth  = maxHeath;

	private SyncListBool list = new SyncListBool();
	
	public RectTransform healthBar; 

	public bool destroyOnDeath;

	public override bool OnSerialize(NetworkWriter writer, bool initialState)
	{
		return base.OnSerialize(writer, initialState);
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		base.OnDeserialize(reader, initialState);
	}

	private NetworkStartPosition[] spawnPoints;

	void Start(){
		if (isLocalPlayer) {
			spawnPoints = FindObjectsOfType <NetworkStartPosition>();
		}
	}

	public void TakeDamage(int amount){

		if (!isServer) {
			return;
		}


		currentHealth -= amount;

		if (currentHealth <= 0) {

			if (destroyOnDeath) {
				Destroy (gameObject);
			} 
			else {
				currentHealth = maxHeath;
				RpcRespawn ();
			}

		}

	}	
	
	void OnChangeHealth(int health){
		healthBar.sizeDelta = new Vector2 (health * 2, healthBar.sizeDelta.y);
	}

	[ClientRpc]
	void RpcRespawn(){
		if (isLocalPlayer) {

			Vector3 spawnPoint = Vector3.zero;
			if (spawnPoints != null && spawnPoints.Length > 0) {
				spawnPoint = spawnPoints [ Random.Range (0, spawnPoints.Length) ].transform.position;
			}

			transform.position = spawnPoint;
 		}
	}

}
