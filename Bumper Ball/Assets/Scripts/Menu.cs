using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {

	public Canvas tempMenu;
	// Use this for initialization
	void Start () {
		if (tempMenu != null) {
			tempMenu = tempMenu.GetComponent<Canvas> ();
			tempMenu.enabled = false;
		}
	}
	
	// Update is called once per frame
	public void PressPlay(){
		Application.LoadLevel ("scene1");
	}

	public void PressQuit(){
		Application.Quit ();
	}

	public void PressBack(){
		Application.LoadLevel ("Menu");
	}

	public void EnableGameOverMenu(){
		tempMenu.enabled = true;
	}

}
