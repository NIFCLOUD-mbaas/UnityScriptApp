using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalFlick : MonoBehaviour {

	private int selectedGachaIndex;
	public int SelectedGachaIndex{
		get{ return selectedGachaIndex; }
	}
	GachaCubeGenerator GCgenerator;
	UIController UICntrler;

	// For detecting flick input
	private float startPos;
	private float endPos;
	private readonly float minFlickLength = (float)(Screen.width) / 5.0f;

	// Use this for initialization
	IEnumerator Start () {
		enabled = false;

		selectedGachaIndex = 0;
		GCgenerator = GameObject
			.FindGameObjectWithTag("GachaCubeGenerator")
			.GetComponent<GachaCubeGenerator>();
		UICntrler = GameObject.
			FindGameObjectWithTag("Canvas")
			.GetComponent<UIController>();
		
		yield return new WaitWhile(()=>{return !(GCgenerator.IsInitialized);});

        if (GCgenerator.NumOfGacha > 0) 
        {
            UICntrler.UpdateGachaText(GCgenerator.GetGachaId(selectedGachaIndex));
        }

		enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		// Detect flick input
		if(Input.GetMouseButtonDown(0)){
			startPos = Input.mousePosition.x;
		}
		if(Input.GetMouseButtonUp(0)){
			// Not proceed if the
			if(!(gameObject.GetComponent<GachaCubeClick>().IsRollingGacha)){
				endPos = Input.mousePosition.x;				
				// Flicked Right or Left ?
				if(startPos > endPos + minFlickLength){
					if(selectedGachaIndex + 1 < GCgenerator.NumOfGacha){
						changeGacha(1);
					}
				} else if(startPos < endPos - minFlickLength){
					if(selectedGachaIndex - 1 >= 0){
						changeGacha(-1);
					}
				}
				UICntrler.UpdateGachaText(GCgenerator.GetGachaId(selectedGachaIndex));
			}
		}
	}

	// Transform camera position in X-direction 
	private void changeGacha(int xDir){	// 1 or -1
		selectedGachaIndex += xDir;
		Vector3 NewCamPos = Camera.main.transform.position;
		NewCamPos.x += (float)xDir * GCgenerator.CubePositionOffset;
		Camera.main.transform.position = NewCamPos;
	}
}
