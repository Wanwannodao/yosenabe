using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Threading;

namespace MLPlayer
{
	public class SceneController : MonoBehaviour
	{
		//singleton
		protected static SceneController instance;

		public static SceneController Instance {
			get {
				if (instance == null) {
					instance = (SceneController)FindObjectOfType (typeof(SceneController));
					if (instance == null) {
						Debug.LogError ("An instance of" + typeof(SceneController) + "is needed in the scene,but there is none.");
					}
				}
				return instance;
			}
		}

		[SerializeField] float cycleTimeStepSize;
		[SerializeField] float episodeTimeLength;
		[Range (0.1f, 10.0f)]
		[SerializeField] float timeScale = 1.0f;

		[SerializeField] public Agent agent;
		public static AIServer server;
		public static bool FinishFlag = false;
		private Vector3 firstLocation;

		[SerializeField] Environment environment;
		private float lastSendTime;
		private float episodeStartTime = 0f;
		public static ManualResetEvent received = new ManualResetEvent (false);

		public static Queue<int> obj_q = new Queue<int>();

		void Start ()
		{
			server = new AIServer (agent);
			firstLocation = new Vector3 ();
			firstLocation = agent.transform.position;
			StartNewEpisode ();
			lastSendTime = -cycleTimeStepSize;
		}

		public void TimeOver ()
		{
			agent.EndEpisode ();
		}

		public void StartNewEpisode ()
		{
			episodeStartTime = Time.time;
			environment.OnReset ();
			agent.transform.position = firstLocation;
			agent.StartEpisode ();
		}

		public void FixedUpdate ()
		{
			if (FinishFlag == false) {
				Time.timeScale = timeScale;
				if (lastSendTime + cycleTimeStepSize <= Time.time) {
					lastSendTime = Time.time;
	
					if (Time.time - episodeStartTime > episodeTimeLength) {
						TimeOver ();
					}
					if (agent.state.endEpisode) {
						StartNewEpisode ();
					}
					received.Reset ();


					// deactivate observed objects
					while (0 < obj_q.Count) {
						int id = obj_q.Dequeue ();
						//Object[] gobj = environment.getGameObjs ();

						if (environment.deactivateGameObj(id)) {
							Debug.Log ("deactiveate:" + id);
						} else {
							Debug.Log ("deactivation failed");
						}
					}

					agent.UpdateState ();

					// set positions of all game objects as state
					Object[] objs = environment.getGameObjs();
					int itemCnt = objs.Length;
					Debug.Log (itemCnt);
					int j = 0;
					agent.state.obj_pos = new float[itemCnt][];
					agent.state.obj_angle = new float[itemCnt][];
					agent.state.obj_id = new int[itemCnt];
					agent.state.agent_pos = new float[3];
					agent.state.agent_angle = new float[3];

					foreach (GameObject obj in objs) {
						agent.state.obj_pos [j] = new float[3];
						agent.state.obj_angle [j] = new float[3];
						agent.state.obj_pos [j] [0] = obj.transform.position.x;
						agent.state.obj_pos [j] [1] = obj.transform.position.y;
						agent.state.obj_pos [j] [2] = obj.transform.position.z;
						agent.state.obj_angle [j] [0] = obj.transform.eulerAngles.x;
						agent.state.obj_angle [j] [1] = obj.transform.eulerAngles.y;
						agent.state.obj_angle [j] [2] = obj.transform.eulerAngles.z;
						agent.state.obj_id [j] = obj.GetInstanceID ();

						agent.state.agent_pos [0] = agent.transform.position.x;
						agent.state.agent_pos [1] = agent.transform.position.y;
						agent.state.agent_pos [2] = agent.transform.position.z;
						agent.state.agent_angle [0] = agent.transform.eulerAngles.x;
						agent.state.agent_angle [1] = agent.transform.eulerAngles.y;
						agent.state.agent_angle [2] = agent.transform.eulerAngles.z;

						j++;
					}
					agent.state.obj_cnt = itemCnt;

					server.PushAgentState (agent.state);
					received.WaitOne ();
					agent.ResetState ();
				}
			} else {
				EditorApplication.isPlaying = false;
			}
		}
	}
}
