﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UIテキストの管理をするためのクラス
public class TextManager : MonoBehaviour {

	public string GetText()
	{
		return this.GetComponent<Text>().text;
	}

	public void SetText(string text)
	{
		this.GetComponent<Text>().text = text;
	}

	// 表示・非表示
	public void EnableText(bool enabled)
	{
		GetComponent<Text>().enabled = enabled;
	}
}
