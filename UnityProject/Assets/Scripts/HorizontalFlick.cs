using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 水平方向のフリック入力がされた際の処理を扱うクラス
public class HorizontalFlick : MonoBehaviour {
	// 選択されているガチャ
	private int selectedGachaIndex;
	public int SelectedGachaIndex{　get{ return selectedGachaIndex; }　}

	// フリック入力検知のためのメンバ変数
	private float startPos;
	private float endPos;
	private readonly float minFlickLength = (float)(Screen.width) / 5.0f;

	GachaCubeGenerator GCgenerator;
	UIController UICntrler;

	//-----------------------------------------------------------
	// アプリ起動時に呼ばれるメソッド
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

	//-----------------------------------------------------------
	// 毎フレーム呼ばれる関数
	void Update () {
		// フリック入力検知
		if(Input.GetMouseButtonDown(0)){
			startPos = Input.mousePosition.x;
		}
		if(Input.GetMouseButtonUp(0)){
			// ガチャが回転していないときだけフリック入力を受け付ける
			if(!(gameObject.GetComponent<GachaCubeClick>().IsRollingGacha)){
				endPos = Input.mousePosition.x;				
				// フリック入力は右方向か左方向か
				if(startPos > endPos + minFlickLength){
					if(selectedGachaIndex + 1 < GCgenerator.NumOfGacha){
						changeGacha(1);
					}
				} else if(startPos < endPos - minFlickLength){
					if(selectedGachaIndex - 1 >= 0){
						changeGacha(-1);
					}
				}
				// 選択中のガチャが変更されたのでUIの表示を変更
				UICntrler.UpdateGachaText(GCgenerator.GetGachaId(selectedGachaIndex));
			}
		}
	}

	// 選択中のガチャを変更，画面に映るガチャも変更(カメラ移動)
	private void changeGacha(int xDir){	// 1 or -1
		selectedGachaIndex += xDir;
		Vector3 NewCamPos = Camera.main.transform.position;
		NewCamPos.x += (float)xDir * GCgenerator.CubePositionOffset;
		Camera.main.transform.position = NewCamPos;
	}
}
