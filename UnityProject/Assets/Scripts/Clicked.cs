using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicked : MonoBehaviour {
	private bool isClicked = false;

	public void SetClicked(){
		StartCoroutine(onlyThisFrame());
	}
	private	IEnumerator onlyThisFrame(){
		if(isClicked) yield break;
		isClicked = true;
		yield return null;
		isClicked = false;
	}

	public bool IsClicked {
		get{ return isClicked; }
	}


}
