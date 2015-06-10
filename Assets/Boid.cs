using UnityEngine;
using System.Collections;
using System.Collections.Generic;  // necessary for Lists

public class Boid : MonoBehaviour {
	// static list shared among all boid objects
	static public List<Boid>	boids;
	
	// not using rigidbocy. moving boids directly
	public Vector3	velocity;
	public Vector3	newVelocity;
	public Vector3	newPosition;
	
	public List<Boid> 	neighbors;		// nearby boids
	public List<Boid> 	collisionRisks; // boids that are too close
	public Boid			closest;		// single closest boid
	
	
	// Use this for initialization
	void Awake () {
		if (boids == null) {
			boids = new List<Boid>();
		}
		
		boids.Add (this);
		
		//Give the boid a random pos and vel
		Vector3 randPos = Random.insideUnitSphere * BoidSpawner.S.spawnRadius;
		randPos.y = 0;
		
		this.transform.position = randPos;
		velocity = Random.onUnitSphere;
		velocity*= BoidSpawner.S.spawnVelocity;
		
		//Initialize the two Lists
		neighbors = new List<Boid>();
		collisionRisks = new List<Boid>();
		
		// make this.transform a child of the Boids GO
		this.transform.parent = GameObject.Find ("Boids").transform;
		
		Color randColor = Color.black;
		while (randColor.r + randColor.g + randColor.b < 1.0f) {
			randColor = new Color(Random.value, Random.value, Random.value);
		}
		
		Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer r in rends) {
			r.material.color = randColor;
		}
	} // Awake
	
	// Update is called once per frame
	void Update () {
		
		List<Boid> neighbors = GetNeighbors (this);

		newVelocity = velocity;
		newPosition = this.transform.position;

		Vector3 neighborVel = GetAverageVelocity (neighbors);
		newVelocity += neighborVel * BoidSpawner.S.velocityMatchingAmt;

		Vector3 neighborCenterOffset = GetAveragePosition (neighbors) - this.transform.position;
		newVelocity += neighborCenterOffset * BoidSpawner.S.flockCenteringAmt;

		Vector3 dist;
		if (collisionRisks.Count > 0) {
			Vector3 collisionAveragePos=GetAveragePosition(collisionRisks);
			dist = collisionAveragePos - this.transform.position;
			newVelocity += dist * BoidSpawner.S.collisionAvoidanceAmt;
		}

		dist = BoidSpawner.S.mousePos - this.transform.position;
		if (dist.magnitude > BoidSpawner.S.mouseAvoidanceDist) {
			newVelocity += dist * BoidSpawner.S.mouseAttractionAmt;
		} else {
			newVelocity -= dist.normalized*BoidSpawner.S.mouseAvoidanceDist*BoidSpawner.S.mouseAvoidanceAmt;
		}
	}

	void LateUpdate(){
		velocity = (1 - BoidSpawner.S.velocityLerpAmt) * velocity + BoidSpawner.S.velocityLerpAmt * newVelocity;

		if (velocity.magnitude > BoidSpawner.S.maxVelocity) {
			velocity = velocity.normalized*BoidSpawner.S.maxVelocity;
		}
		if (velocity.magnitude < BoidSpawner.S.minVelocity) {
			velocity = velocity.normalized*BoidSpawner.S.minVelocity;
		}

		newPosition = this.transform.position + velocity * Time.deltaTime;
		newPosition.y = 0;
		this.transform.LookAt (newPosition);
		this.transform.position = newPosition;
		
	}
	
	public List <Boid> GetNeighbors (Boid boi) {
		float closestDist = float.MaxValue;
		Vector3 delta;
		float dist;
		neighbors.Clear ();
		collisionRisks.Clear ();
		
		foreach (Boid b in boids) {
			
			if (b == boi)
				continue;
			delta = b.transform.position - boi.transform.position;
			dist = delta.magnitude;
			
			if (dist < closestDist) {
				closestDist = dist;
				closest = b;
			}
			
			if (dist < BoidSpawner.S.nearDist) {
				neighbors.Add (b);
			}
			
			if (dist < BoidSpawner.S.collisionDist) {
				collisionRisks.Add (b);
			}
		}
		
		if (neighbors.Count == 0) {
			neighbors.Add (closest);
		}
		
		return(neighbors);
		
	}
	
	public Vector3 GetAveragePosition (List<Boid> someBoids) {
		Vector3 sum = Vector3.zero;
		foreach (Boid b in someBoids) {
			sum += b.transform.position;
		}
		Vector3 center = sum / someBoids.Count;
		return (center);
	}
	
	public Vector3 GetAverageVelocity (List<Boid> someBoids) {
		Vector3 sum = Vector3.zero;
		foreach (Boid b in someBoids) {
			sum += b.velocity;
		}
		Vector3 avg = sum / someBoids.Count;
		return (avg);
	}
}