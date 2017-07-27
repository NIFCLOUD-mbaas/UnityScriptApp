using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotationController : MonoBehaviour {
	private float rotationScale = 1.0f;

	private bool isClicked = false;
	private bool isRotating = true;

	private int frameCount = 0;
	private readonly int maxFrameCount = 360;

	// Use this for initialization
	void Start () {
		rotationScale = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(isRotating){
			// Change rotationScale by sine func until counting maxFrameCount
			if(isClicked){
				frameCount++;
				float sinArg = (float)frameCount * Mathf.PI / (float)maxFrameCount;
				rotationScale = 1.0f + 12.0f * Mathf.Sin(sinArg);
				if(frameCount == maxFrameCount){
					isRotating = false;		// Stop rotating until the flag is on again
					isClicked = false;
				}
			}
			transform.Rotate(new Vector3(0.0f, rotationScale, 0.0f), Space.World);
		}
	}

	public void ClickedFlagOn(){
		if(!isClicked){
			isClicked = true;
			frameCount = 0;
		}
	}

	public void RotatingFlagOn(){
		if(!isRotating){
			isRotating = true;
			rotationScale = 1.0f;
		}
	}

	public bool IsClicked{
		get{
			return isClicked;
		}
	}

	public bool IsRotating{
		get{
			return isRotating;
		}
	}
}
