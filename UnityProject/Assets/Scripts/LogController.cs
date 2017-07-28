using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

//---------------------------------------------------------------
// ガチャのログを保持するためのクラス
public class GachaLog
{
	public string date;
	public int pointDiff;
	public int moneyDiff;

	public GachaLog()
	{
		date = "";
		pointDiff = 0;
		moneyDiff = 0;
	}
}
//---------------------------------------------------------------

public class LogController : MonoBehaviour
{
	// ガチャログのリスト(5個)
	public static int NumOfLog = 5;
	private static GachaLog[] logList;

	//---------------------------------------------------------------------------------------------
	// データストアから該当ユーザのログを取得するメソッド
	//---------------------------------------------------------------------------------------------
	public static IEnumerator GetLog(string userId)
	{
		// ログのリストを初期化
		logList = new GachaLog[NumOfLog];
		for(int i = 0; i < logList.Length; i++){
			logList[i] = new GachaLog();
		}

		bool isConnecting = true;
		// データストアの "GachaLog"クラスに接続
		NCMBQuery<NCMBObject> logQuery = new NCMBQuery<NCMBObject>("GachaLog");
		// ユーザIDが一致するものを
		logQuery.WhereEqualTo("userId", userId);
		// numOfLog の数だけ
		logQuery.Limit = NumOfLog;
		// 日付に関して降順で
		logQuery.OrderByDescending ("createDate");
		// 検索して取得	
		logQuery.FindAsync((List<NCMBObject> response, NCMBException e) => {
			if(e != null){
				// データ取得失敗
				Debug.Log(e.ErrorCode + ": " + e.ErrorMessage);
			}else{
				// データ取得成功
				Debug.Log("Success(Getting the gacha log)");
				// 取得したデータから必要なものだけ取り出す: response(List<NCMBObject>) -> logList(GachaLog[])
				for(int i = 0; i < response.Count; i++){
					System.DateTime date = (System.DateTime)(response[i].CreateDate);
					logList[i].date = (response[i].CreateDate).ToString();
					logList[i].date = date.AddHours(9.0).ToString();
					logList[i].moneyDiff = System.Convert.ToInt32(response[i]["moneyDiff"]);
					logList[i].pointDiff = System.Convert.ToInt32(response[i]["pointDiff"]);
				}
			}
			// データストアからログ取得処理終了
			isConnecting = false;
		});
		// データストアからログ取得処理が終了するまで以下の行でストップ
		yield return new WaitWhile(()=>{ return isConnecting; });
	}

	//---------------------------------------------------------------------------------------------
	// logListに保持されたデータをUIに反映させるメソッド
	//---------------------------------------------------------------------------------------------	
	public static void DisplayLogs(){
		UIController uiCntrler = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
		uiCntrler.UpdateLogText(logList);
	}
}
