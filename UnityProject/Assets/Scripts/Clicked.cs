using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// オブジェクトがクリックされたことを外から読み取るためのクラス
// コルーチンの待機処理のために利用
public class Clicked : MonoBehaviour
{
	private bool isClicked = false;

	//-----------------------------------------------------------
	// １フレームだけクリックされたフラグを立てる
	// 		Hierarchy > Canvas > ResultPopup > Button 
	//			-> Inspector > Button(Script) > On Click()
	public void SetClicked()
	{
		StartCoroutine(onlyThisFrame());
	}

	//-----------------------------------------------------------
	// コルーチン
	private	IEnumerator onlyThisFrame()
	{
		if(isClicked) yield break;
		isClicked = true;
		yield return null;
		isClicked = false;
	}
	//-----------------------------------------------------------
	public bool IsClicked
	{
		get{ return isClicked; }
	}
	//-----------------------------------------------------------

}
