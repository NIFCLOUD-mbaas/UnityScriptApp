using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

//---------------------------------------------------------------
// スクリプトからのJSON形式のレスポンスをパースして保持するための構造体
public struct ScriptResponse
{
	// ポイントの変化量
	public int pointDiff;
	// お金の変化量
	public int moneyDiff;
}
//---------------------------------------------------------------

// キューブオブジェクトがクリックされた際の処理を扱うクラス
public class GachaCubeClick : MonoBehaviour
{
	// クリックされたキューブオブジェクトを保持
	private GameObject clickedGachaCube;

	// スクリプトからのレスポンスを保持
	private ScriptResponse scriptResponse;

	// UIの制御
	private UIController uiCntrler;

	//---------------------------------------------------------------------------------------------
	// アプリ起動時に呼ばれる関メソッド(初期化)
	//---------------------------------------------------------------------------------------------
	void Start ()
	{
		clickedGachaCube = null;
		scriptResponse = new ScriptResponse();
		uiCntrler = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
	}

	//---------------------------------------------------------------------------------------------
	// 毎フレーム実行されるメソッド（キューブオブジェクトがクリックされるのを監視）
	//---------------------------------------------------------------------------------------------
	void Update ()
	{
		bool isPointerOnUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
		// クリックされたか，かつ，クリックした位置はUI上ではないか
		if(Input.GetMouseButtonDown(0) && !isPointerOnUI){
			// カメラからクリックしたスクリーン座標にレイを飛ばす
			Ray ray = new Ray();
			RaycastHit hit = new RaycastHit();
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// レイが衝突したオブジェクトは存在するか
			if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity)){
				// レイが衝突したオブジェクトはガチャキューブオブジェクトか
				if(hit.collider.gameObject.CompareTag("GachaCube")){
					// ガチャを回す処理を実行する
					StartCoroutine(rollGachaCoroutine(hit));
				}
			}
		}
	}

	//---------------------------------------------------------------------------------------------
	// ガチャを回す処理を実行するメソッド
	//---------------------------------------------------------------------------------------------
	private bool isRollingGacha = false;
	public bool IsRollingGacha
	{
		get{ return isRollingGacha; }
	}
	private IEnumerator rollGachaCoroutine(RaycastHit hit)
	{
		// ガチャがすでに回っているのならば処理を終了する (コルーチンの唯一性も保証される)
		if(isRollingGacha) yield break;
		// ガチャを回している
		isRollingGacha = true;
		
		// クリックされたキューブ
		clickedGachaCube = hit.collider.gameObject;
		// クリックされたキューブは現在選択されている（画面に表示されている）キューブか
		int currGachaIndex = gameObject.GetComponent<HorizontalFlick>().SelectedGachaIndex;
		if(clickedGachaCube.name == "GachaCube"+ currGachaIndex.ToString()){

			// クリックされたキューブの回転を制御するコンポーネント
			CubeRotationController cubeRotCntrler = clickedGachaCube.GetComponent<CubeRotationController>();
			// クリックされたと伝える（回転速度が変化し始め，最終的に止まる）
			cubeRotCntrler.startDynamicRot();

			// "Cost: XXX" を表示する テキスト オブジェクトを非表示にする
			uiCntrler.EnableCostText(false);
			
			NCMBUser currUser = NCMBUser.CurrentUser;
			if(currUser != null){
				string gachaId = clickedGachaCube.GetComponent<Gacha>().GachaObjectId;
				// NCMB スクリプトによりガチャ結果を計算する関数を実行する（処理終了までストップ）
				yield return StartCoroutine(executeGachaLogicScriptCoroutine(gachaId, currUser));
				// サーバ側のユーザ情報の更新
				yield return StartCoroutine(updateUserPointAndMoney(currUser));
				// 該当ユーザのログを取得
				yield return StartCoroutine(LogController.GetLog(currUser.ObjectId));
			} else {
				// 出るはずはないエラーメッセージ
				Debug.Log("Failed to roll Gacha(No Current User)");
			}

			// ガチャの回転が止まるまで以下の行でストップ
			yield return new WaitWhile(()=>{return cubeRotCntrler.IsRotating;});
			
			// ガチャ結果テキストを更新する
			uiCntrler.UpdateResultText(scriptResponse.pointDiff);
			// ガチャ結果を表示する
			uiCntrler.EnableResultPopup(true);

			// ガチャ結果プレート上の "OK" ボタンがクリックされるまで以下の行でストップ
			yield return new WaitWhile(()=>{ return !(GameObject
													 .FindGameObjectWithTag("ResultPopupButton")
													 .GetComponent<Clicked>()
													 .IsClicked); });
			// ガチャ結果を非表示にする
			uiCntrler.EnableResultPopup(false);
			yield return new WaitForSeconds(0.5f);

			// 各UIテキストの更新・表示
			uiCntrler.UpdateMoneyText(scriptResponse.moneyDiff);
			uiCntrler.UpdatePointText(scriptResponse.pointDiff);
			LogController.DisplayLogs();
			uiCntrler.EnableCostText(true);

			// キューブを再び回転させる
			cubeRotCntrler.RotatingFlagOn();
		}
		// ガチャを回す処理終了
		clickedGachaCube = null;
		isRollingGacha = false;
	}

	//---------------------------------------------------------------------------------------------
	// スクリプトでガチャロジックを実行するメソッド
	//---------------------------------------------------------------------------------------------	
	private IEnumerator executeGachaLogicScriptCoroutine(string gachaId, NCMBObject currUser)
	{
		// scriptResponse 初期化
		scriptResponse.pointDiff = 0;
		scriptResponse.moneyDiff = 0;

		// 実行するファイル名とリクエストのタイプでインスタンス生成
		NCMBScript gachaLogicScript = new NCMBScript("gacha.js", NCMBScript.MethodType.GET);
		// スクリプトに渡すクエリ（ガチャのIDとユーザID）を設定する
		Dictionary<string, object> query = new Dictionary<string, object> (){ 
														{"gachaId", gachaId},
														{"userId", currUser.ObjectId} };
		// スクリプトを実行する
		bool isCalculating = true;
		gachaLogicScript.ExecuteAsync(null, null, query, (byte[] result, NCMBException e) => {
			if(e != null){
				// スクリプト実行失敗
				Debug.Log(e.ErrorCode +":"+ e.ErrorMessage);
			}else{
				// スクリプト実行成功(JSONパース（byte[] -> string -> ScriptResponse)
				string resultStr = System.Text.Encoding.ASCII.GetString(result);
				scriptResponse = JsonUtility.FromJson<ScriptResponse>(resultStr);
			}
			// スクリプト処理終了
			isCalculating = false;
		});
		// スクリプトでの処理が終了するまで待機する
		yield return new WaitWhile(()=>{return isCalculating;});
	}

	//---------------------------------------------------------------------------------------------
	// サーバ側のユーザの情報（お金とポイント）を更新するメソッド
	//---------------------------------------------------------------------------------------------	
	private IEnumerator updateUserPointAndMoney(NCMBObject currUser)
	{
		currUser["money"] = System.Convert.ToInt32(currUser["money"]) + scriptResponse.moneyDiff;
		currUser["points"] = System.Convert.ToInt32(currUser["points"]) + scriptResponse.pointDiff;
		// 保存
		currUser.SaveAsync((NCMBException e) => {
			if(e != null){
				// 保存失敗
				Debug.Log(e.ErrorCode + ": " + e.ErrorMessage);
			}else{
				// 保存成功
				Debug.Log("Succeeded to update the user data)");
			}
		});
		// 今回は特に同期処理にする必要なし
		yield return null;
	}
}