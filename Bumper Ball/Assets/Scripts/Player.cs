using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//public attributes
	public bool isHuman;
	public GameObject[] otherPlayers;
	public GameObject menu;

	//private attributes
	private static float moveSpeed = 12;
	private GameObject target; //for IA only
	private Rigidbody body;
	private Vector3 moveInput;
	private Vector3 moveVelocity;
	private GameObject ground; //reference to the floor
	private static float groundRadius = 8;
	private bool hasLost;
	private Vector2 touchStart; //for the touch controls

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody> ();
		ground = GameObject.Find("Ground");
		if (!isHuman) { // is a bot
			ChooseTarget ();
			InvokeRepeating ("ChooseTarget", 0, 5); //decide a target every 5 seconds
		}
	}
	
	// Update is called once per frame
	void Update () {
		int i;
		//check if player is losing
		if (Vector3.Distance (transform.position, ground.transform.position) >= groundRadius) {
			//Debug.Log("Game over " + transform.name);
			hasLost = true;
			if (isHuman && menu != null){
				menu.GetComponent<Menu>().EnableGameOverMenu();
			}
		}
		//check if player is winning
		for (i=0; i< otherPlayers.Length; i++) { 
			if (!otherPlayers[i].GetComponent<Player>().GetHasLost())
				break;
		}
		if (i == otherPlayers.Length && !hasLost && menu != null) {
			//Debug.Log (transform.name + "WINS ");
			menu.GetComponent<Menu>().EnableGameOverMenu();
		}
	}

	// Fixed update for physics i.e movement
	void FixedUpdate () {
		if (!hasLost) {
			if (isHuman) {
				moveInput = Vector3.zero;
				if (Input.touchCount > 0){
					switch (Input.GetTouch(0).phase){
					case TouchPhase.Began:
						touchStart = Input.GetTouch(0).position;
						break;
					case TouchPhase.Moved:
						Vector2 dir = Input.GetTouch(0).position - touchStart;
						moveInput = new Vector3(dir.x,0f,dir.y);
						break;
					default:
						break;
					}
				}
				//moveInput = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, Input.GetAxisRaw ("Vertical"));
			} else {
				if (Vector3.Distance (transform.position, ground.transform.position) >= groundRadius-2) { //close to falling!
					moveInput = ground.transform.position - transform.position;
				} else {
					moveInput = target.transform.position - transform.position;
				}
			}
			moveInput.Normalize ();
			moveVelocity = moveInput * moveSpeed;
			body.AddForce (moveVelocity);
		}
	}

	//When colliding with another player there is a knockback relative to the other's velocity
	void OnCollisionEnter(Collision c) {
		if (c.rigidbody != null) { //it's not the ground
			Vector3 dir = c.contacts[0].point - transform.position; //get angle between the collision point and the player
			float force = c.relativeVelocity.magnitude * 6;
			dir = -dir.normalized; //get the opposite vector and normalize it
			body.AddForce(dir*force); //push back the player
		}
	}

	//This one is to make sure players always bounce from each other
	void OnCollisionStay(Collision c){
		if (c.rigidbody != null) {
			Vector3 dir = c.contacts[0].point - transform.position;
			float force = c.relativeVelocity.magnitude * 3;
			dir = -dir.normalized; 
			body.AddForce(dir*force);
		}
	}

	//IA function that chooses a target player to bump into
	void ChooseTarget (){
		int r = Random.Range (0, 3);
		if (!otherPlayers[r].GetComponent<Player>().GetHasLost()) // pick a random target that hasn't lost already
			target = otherPlayers[r];
	}

	public bool GetHasLost(){
		return hasLost;
	}

}
