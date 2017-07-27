using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Globalization;

public class UIController : MonoBehaviour {
	// Update the text ----------------------------------------------------------------------------
	public void UpdateGachaText(string gachaCubeId)
	{
		GameObject[] gachaCubes = GameObject.FindGameObjectsWithTag("GachaCube");
		foreach(GameObject gachaCube in gachaCubes){
			Gacha gacha = gachaCube.GetComponent<Gacha>();
			if(gacha.GachaObjectId == gachaCubeId){
				// Update "Cost" text
				string costStr = string.Format("Cost: {0:#,0}" , gacha.Cost);
				transform.Find("CostText").GetComponent<TextManager>().SetText(costStr);
				// Update the "Rewards" texts
				int NumOfReward = gacha.CountRewards();
				for(int i = 0; i < NumOfReward; i++){
					uint reward = gacha.GetReward(i);
					string rewardStr = string.Format("{0:#,0} pt", reward);
					transform.Find("Rewards/Reward"+(i+1).ToString()+"Text")
						.GetComponent<TextManager>().SetText(rewardStr);
				}
			}
		}
	}

	public void UpdatePointText(int pointDiff)
	{
		TextManager pointTextTM = transform.Find("Points/PointsText").GetComponent<TextManager>();
		string pointStr = pointTextTM.GetText();
		int pointInt = int.Parse(pointStr, NumberStyles.AllowThousands) + pointDiff;	// Apply difference
		pointStr = string.Format("{0:#,0}", pointInt);
		pointTextTM.SetText(pointStr);
	}
	public void UpdateMoneyText(int moneyDiff)
	{
		TextManager moneyTextTM = transform.Find("Money/MoneyText").GetComponent<TextManager>();
		string moneyStr = moneyTextTM.GetText();
		int moneyInt = int.Parse(moneyStr, NumberStyles.AllowThousands) + moneyDiff;	// Apply difference
		moneyStr = string.Format("{0:#,0}", moneyInt);
		moneyTextTM.SetText(moneyStr);
	}

	public void UpdateResultText(int pointDiff)
	{
		TextManager resultTextTM = transform.Find("ResultPopup/ResultPopupText").GetComponent<TextManager>();
		resultTextTM.SetText(string.Format("{0:#,0}", pointDiff));
	}

	public void UpdateLogText(GachaLog[] logList)
	{
		for(int i = 0; i < logList.Length; i++){
			GachaLog log = logList[i];
			TextManager logTextTM = transform.Find("Log/Log"+(i+1).ToString()).GetComponent<TextManager>();
			if(log.date != ""){
				logTextTM.SetText("Date: " + log.date + 
								  ", Money: " + log.moneyDiff + 
								  ", Points: +" + log.pointDiff);
			}else{
				logTextTM.SetText("");
			}
		}
	}


	// set visible/invisible ------------------------------------------------------------------
	public void EnableCostText(bool enabled)
	{
		transform.Find("CostText").GetComponent<TextManager>().EnableText(enabled);
	}

	public void EnableResultPopup(bool enabled)
	{
		transform.Find("ResultPopup").GetComponent<Animator>().SetBool("Open", enabled);
	}

}
