using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gacha : MonoBehaviour {

	private string gachaObjectId = "";
	private uint cost;
	private List<uint> rewards;

	public string GachaObjectId{ get{ return gachaObjectId; } }
	public uint Cost{ get{ return cost; } }
	public int CountRewards(){
		return rewards.Count;
	}
	public uint GetReward(int i){
		return rewards[i];
	}

	public Material[] Materials;
	public void InitGachaCube(string id, uint cost, List<uint> rewards, uint number)
	{
		number = number % 3; 
		// Assign gacha information
		this.gachaObjectId = id;
		this.cost = cost;
		this.rewards = rewards;
		// Set materials and components to GachaCube
		this.GetComponent<Renderer>().material = Materials[number];
		GameObject light = gameObject.transform.Find("CubeLight").gameObject;
		light.GetComponent<Light>().color = Materials[number].color;
	}
}
