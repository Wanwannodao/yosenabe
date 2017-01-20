using UnityEngine;
using System.Collections.Generic;

namespace MLPlayer {
	public class Environment : MonoBehaviour {

		[SerializeField] int itemCount;
		[SerializeField] int chairCount;
		[SerializeField] float areaSize;
		[SerializeField] List<GameObject> itemPrefabs;
		[SerializeField] List<GameObject> chairPrefabs;
	

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void OnReset() {
			foreach(Transform i in transform) {
				Destroy (i.gameObject);
			}
			for (int i=0; i<itemCount; i++) {	
				int itemId = UnityEngine.Random.Range(0, itemPrefabs.Count);

				Vector3 pos = new Vector3 (
					UnityEngine.Random.Range (-areaSize, areaSize),
					itemPrefabs[itemId].transform.position.y,
					UnityEngine.Random.Range (-areaSize, areaSize));
				Quaternion q = Quaternion.Euler (
					270,
					UnityEngine.Random.Range (0f, 360f),
					0
				);

				pos += transform.position;

				GameObject obj = (GameObject)GameObject.Instantiate 
					(itemPrefabs[itemId], pos, q);
				obj.transform.parent = transform;
			}

			for (int i=0; i<chairCount; i++) {	
				int itemId = UnityEngine.Random.Range(0, chairPrefabs.Count);
				Vector3 pos = new Vector3 (
					UnityEngine.Random.Range (-areaSize, areaSize),
					chairPrefabs[itemId].transform.position.y,
					UnityEngine.Random.Range (-areaSize, areaSize));
				Quaternion q = Quaternion.Euler (
					chairPrefabs[itemId].transform.rotation.x,
					UnityEngine.Random.Range (0f, 360f),
					0
					);

				pos += transform.position;
				GameObject obj = (GameObject)GameObject.Instantiate 
					(chairPrefabs[itemId], pos, q);
				obj.transform.parent = transform;
			}
		}
		// return a all active game objects
		public Object[] getGameObjs() {
			return UnityEngine.Object.FindObjectsOfType (typeof(GameObject));
		}
		// return a game object indicated by given id
		public GameObject getGameObj(int id) {
			foreach(GameObject obj 
				in UnityEngine.Object.FindObjectsOfType(typeof(GameObject))) {
				if (obj.GetInstanceID () == id) {
					return obj;
				}
			}
			return null;
		} 
		// deactivate the game object indicated by given id
		public bool deactivateGameObj(int id) {
			GameObject obj = getGameObj (id);
			if (obj != null) { 
				obj.SetActive (false);
				return true;
			} else {
				return false;
			}
		}
	}
}
