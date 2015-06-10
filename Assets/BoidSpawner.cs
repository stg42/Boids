using UnityEngine;
using System.Collections;

public class BoidSpawner : MonoBehaviour {

	static public BoidSpawner	S;

	public int 			numBoids = 100;
	public GameObject	boidPrefab;
	public float		spawnRadius = 100f;
	public float		spawnVelocity = 10f;
	public float		minVelocity = 0f;
	public float		maxVelocity = 30f;
	public float		nearDist = 30f;
	public float		collisionDist = 5f;
	public float		velocityMatchingAmt = 0.01f;
	public float		flockCenteringAmt = 0.15f;
	public float		collisionAvoidanceAmt = -0.5f;
	public float		mouseAttractionAmt = 0.01f;
	public float		mouseAvoidanceAmt = 0.75f;
	public float		mouseAvoidanceDist = 15f;
	public float		velocityLerpAmt = 0.25f;
	
	public bool _________;
	
	public Vector3		mousePos;
	
	void Start(){
		S = this;
		
		for (int i=0; i<numBoids; i++) {
			Instantiate (boidPrefab);
		}
	}
	
	void LateUpdate(){
		Vector3 mousePos2d = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, this.transform.position.y);
		mousePos2d = this.camera.ScreenToWorldPoint (mousePos2d);
	}
}
