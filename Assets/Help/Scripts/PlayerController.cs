using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Xml;

public class PlayerController : NetworkBehaviour {

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	[SerializeField]
	private float xSpeed = 150f;
	
	[SerializeField]
	private float zSpeed = 30f;

	private float x, z;
	
	void Update () {

		if (!isLocalPlayer) {
			return;
		}
		
		Camera.main.transform.LookAt(gameObject.transform);
		
		x = Input.GetAxis("Horizontal");
		z = Input.GetAxis("Vertical");

		if(Input.GetKeyDown(KeyCode.Space)){
			CmdFire();
		}

	}

	private void FixedUpdate()
	{
		var fixedX = x * Time.fixedDeltaTime * xSpeed;
		var fixedZ = z * Time.fixedDeltaTime * zSpeed;
		
		transform.Rotate (0, fixedX, 0);
		transform.Translate (0, 0, fixedZ);
	}


	[Command]
	public void CmdFire(){
		GameObject bullet = (GameObject)Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * 6.0f;

		// Spawn the bullet on the client
		NetworkServer.Spawn(bullet);

		Destroy (bullet, 2f);

	}

	public override void OnStartClient()
	{		
		
	}
}
