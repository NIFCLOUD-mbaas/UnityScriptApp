using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha : MonoBehaviour {
	// 各フィールド
	private string gachaObjectId = "";
	private uint cost;
	private List<uint> rewards;

	//-----------------------------------------------------------
	// get
	public string GachaObjectId{ get{ return gachaObjectId; } }
	public uint Cost{ get{ return cost; } }

	public int CountRewards(){ return rewards.Count; }
	public uint GetReward(int i){ return rewards[i]; }

	//-----------------------------------------------------------
	// 初期化
	public Material[] Materials;
	public void InitGachaCube(string id, uint cost, List<uint> rewards, uint number)
	{
		// フィールド値
		this.gachaObjectId = id;
		this.cost = cost;
		this.rewards = rewards;

		// オブジェクトのマテリアル設定
		number = number % 3; 
		this.GetComponent<Renderer>().material = Materials[number];
		GameObject light = gameObject.transform.Find("CubeLight").gameObject;
		light.GetComponent<Light>().color = Materials[number].color;
	}

	//-----------------------------------------------------------
}
