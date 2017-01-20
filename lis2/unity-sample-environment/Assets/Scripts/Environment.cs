using UnityEngine;
using System.Collections.Generic;

namespace MLPlayer {
	public class Environment : MonoBehaviour {

		[SerializeField] int itemCount;
		[SerializeField] int chairCount;
		[SerializeField] int wallCount;
		[SerializeField] float areaSize;
		[SerializeField] List<GameObject> itemPrefabs;
		[SerializeField] List<GameObject> chairPrefabs;
		[SerializeField] List<GameObject> wallPrefabs;

	

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

			for (int i=0; i<wallCount; i++) {	
				while (true) {
					int itemId = UnityEngine.Random.Range (0, wallPrefabs.Count);
					Vector3 pos = new Vector3 (
						UnityEngine.Random.Range (-areaSize, areaSize),
						wallPrefabs [itemId].transform.position.y,
						UnityEngine.Random.Range (-areaSize, areaSize));
					Quaternion q = Quaternion.Euler (
						wallPrefabs [itemId].transform.rotation.x,
						UnityEngine.Random.Range (0f, 360f),
						0
					);

					pos += transform.position;
					GameObject obj = (GameObject)GameObject.Instantiate 
						(wallPrefabs [itemId], pos, q);

					if (!detectIntersecton (obj)) {
						obj.transform.parent = transform;
						break;
					} else {
						Destroy (obj);
					}
				}
			}

			for (int i=0; i<itemCount; i++) {	
				while (true) {
					int itemId = UnityEngine.Random.Range (0, itemPrefabs.Count);

					Vector3 pos = new Vector3 (
						             UnityEngine.Random.Range (-areaSize, areaSize),
						             itemPrefabs [itemId].transform.position.y,
						             UnityEngine.Random.Range (-areaSize, areaSize));
					Quaternion q = Quaternion.Euler (
								270,
						              UnityEngine.Random.Range (0f, 360f),
						              0
					              );

					pos += transform.position;

					GameObject obj = (GameObject)GameObject.Instantiate 
					(itemPrefabs [itemId], pos, q);

					if (!detectIntersecton (obj)) {
						obj.transform.parent = transform;
						break;
					} else {
						Destroy (obj);
					}
				}
			}

			for (int i=0; i<chairCount; i++) {	
				while (true) {
					int itemId = UnityEngine.Random.Range (0, chairPrefabs.Count);
					Vector3 pos = new Vector3 (
						             UnityEngine.Random.Range (-areaSize, areaSize),
						             chairPrefabs [itemId].transform.position.y,
						             UnityEngine.Random.Range (-areaSize, areaSize));
					Quaternion q = Quaternion.Euler (
						              chairPrefabs [itemId].transform.rotation.x,
						              UnityEngine.Random.Range (0f, 360f),
						              0
					              );

					pos += transform.position;
					GameObject obj = (GameObject)GameObject.Instantiate 
					(chairPrefabs [itemId], pos, q);

					if (!detectIntersecton (obj)) {
						obj.transform.parent = transform;
						break;
					} else {
						Destroy (obj);
					}
				}
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
			if (obj != null && obj.tag != "light") { 
				obj.SetActive (false);
				return true;
			} else {
				return false;
			}
		}

		public bool detectIntersecton(GameObject obj) {
			Bounds a = obj.GetComponent<Renderer> ().bounds;
			foreach (Transform i in transform) {
				Bounds b = i.gameObject.GetComponent<Renderer> ().bounds;
				if (a.Intersects (b)) {
					return true;
				}
			}
			return false;
		}
	}
}
