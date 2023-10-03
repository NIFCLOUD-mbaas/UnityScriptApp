using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キューブオブジェクトの動き（回転）を制御するクラス
public class CubeRotationController : MonoBehaviour
{
	// 回転速度（１フレームあたりの回転角度(degree)）
	private float rotationScale = 1.0f;

	// クリックされたか
	private bool isDynamic = false;
	// 回転中か
	private bool isRotating = true;

	// クリックされてからのフレーム数
	private int frameCount = 0;
	// クリックされてから回転速度が変化し続けるフレーム数
	private readonly int maxFrameCount = 360;
	
	// 毎フレーム呼ばれるメソッド
	void Update ()
	{
		if(isRotating){
			// 回転
			if(isDynamic){
				// クリックされた
				frameCount++;
				// sine関数に従って回転速度を変化
				float sinArg = (float)frameCount * Mathf.PI / (float)maxFrameCount;
				rotationScale = 1.0f + 12.0f * Mathf.Sin(sinArg);
				if(frameCount == maxFrameCount){
					// 一定フレーム数のち
					isRotating = false;	// 回転停止
					isDynamic = false;
				}
			}
			// オブジェクト回転
			transform.Rotate(new Vector3(0.0f, rotationScale, 0.0f), Space.World);
		}
	}

	//-----------------------------------------------------------
	// 動的に回転速度を変化させるフラグをオン
	public void startDynamicRot()
	{
		isDynamic = true;
		frameCount = 0;
	}

	//-----------------------------------------------------------
	// 停止 → 回転
	public void RotatingFlagOn(){
		isRotating = true;
		rotationScale = 1.0f;
	}
	//-----------------------------------------------------------
	public bool IsRotating{
		get{ return isRotating; }
	}
	//-----------------------------------------------------------
}
