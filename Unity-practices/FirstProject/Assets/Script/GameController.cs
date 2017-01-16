using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public UnityEngine.UI.Text scoreLabel;
	public GameObject winnerLabelObject;

	private int item_max_num;     // 最初の数

	void Start ()
	{
		item_max_num = GameObject.FindGameObjectsWithTag ("Item").Length;
	}
	public void Update ()
	{
		int count = GameObject.FindGameObjectsWithTag ("Item").Length;
		scoreLabel.text = (item_max_num - count).ToString ();

		if (count == 0) {
			// オブジェクトをアクティブにする
			winnerLabelObject.SetActive (true);
		}
	}
}
