using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

// データストアからガチャを取得してキューブオブジェクトを生成するクラス
public class GachaCubeGenerator : MonoBehaviour
{
	// ガチャキューブオブジェクト同士の間隔
	public readonly float CubePositionOffset = 10.0f;

	// データストア->ガチャクラス から取得したレコードの数（ガチャの数）
	private uint numOfGacha;
	// すべてのガチャのID
	private List<string> allGachaId;

	// ガチャ数の取得
	public uint NumOfGacha
	{
		get{ return numOfGacha; }
	}
	// ガチャIDの取得
	public string GetGachaId(int i)
	{
		return allGachaId[i];
	}

	// Startメソッドが終了したかどうかのフラグ
	private bool isInitialized = false;
	// isInitialized の取得
	public bool IsInitialized
	{
		get{ return isInitialized; }
	}

	//---------------------------------------------------------------------------------------------
	// アプリ起動時に呼ばれるメソッド (データストアからガチャを取得)
	//---------------------------------------------------------------------------------------------
	IEnumerator Start ()
	{
		// ガチャの数とガチャIDリストの初期化
		numOfGacha = 0;
		allGachaId = new List<string>();

		// データストアにアクセスして全ガチャレコードを取得
		bool isGettingGachaData = true;
		NCMBQuery<NCMBObject> getAll = new NCMBQuery<NCMBObject>("Gacha");
		getAll.FindAsync((List<NCMBObject> allGacha, NCMBException e)=>{
			if(e != null){
				// データ取得失敗
				Debug.Log(e.ErrorCode + ":" + e.ErrorMessage);
			}else{
				//データ取得成功
				Vector3 cubePosition = new Vector3(0.0f, 0.0f, 0.0f);
				// 取得したガチャの情報をもつキューブオブジェクトを生成
				foreach(NCMBObject gacha in allGacha){
					createGachaCube(cubePosition, gacha, numOfGacha);
					// 位置をずらす
					cubePosition += new Vector3(CubePositionOffset, 0.0f, 0.0f);
					// ガチャの数を増やす
					numOfGacha++;
				}
			}
			// データ取得処理終了
			isGettingGachaData = false;
		});
		// データ取得処理が終了するまで以下の行でストップ
		yield return new WaitWhile(()=>{return isGettingGachaData;});

		// ガチャの読み込み終了
		isInitialized = true;
	}

	//---------------------------------------------------------------------------------------------
	// ガチャキューブオブジェクトを生成するメソッド
	//---------------------------------------------------------------------------------------------
	public GameObject gachaCubePrefab;
	private void createGachaCube(Vector3 posOffset, NCMBObject gacha, uint gachaNum)
	{
		// プレファブから新たにオブジェクトを生成 (位置と回転を指定)
		Vector3 pos = transform.position + posOffset;
		GameObject gachaCube = (GameObject)Instantiate(
			gachaCubePrefab, pos, transform.rotation
		);
		// 生成されたオブジェクトに名前を設定
		gachaCube.name = "GachaCube" + gachaNum.ToString();

		// ガチャの各フィールドの取得
		uint cost = System.Convert.ToUInt32(gacha["cost"]);
		ArrayList rewards_arrayList = (ArrayList)gacha["rewards"];
		List<uint> rewards = new List<uint>();
		foreach(object reward in rewards_arrayList){
			rewards.Add(System.Convert.ToUInt32(reward));
		}
		// 各プロパティをGachaクラスで管理する
		gachaCube.GetComponent<Gacha>().InitGachaCube(gacha.ObjectId, cost, rewards, gachaNum);

		// ガチャIDリストにIDを追加
		allGachaId.Add(gacha.ObjectId);
	}
}