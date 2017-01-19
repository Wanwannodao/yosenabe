using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSharp.Net;
using System.Threading;
using MsgPack;

namespace MLPlayer
{
	public class AIServer : MonoBehaviour
	{
		private WebSocketServer wssv;

		[SerializeField] string domain;
		[SerializeField] int port;                                          
		public Queue<byte[]> agentMessageQueue;
		private Queue<byte[]> aiMessageQueue;
		private Mutex mutAgent;
		public Agent agent;
		private MsgPack.CompiledPacker packer;

		public AIServer (Agent _agent)
		{
			agent = _agent;
			mutAgent = new Mutex ();
			packer = new MsgPack.CompiledPacker ();
			agentMessageQueue = new Queue<byte[]> ();
			aiMessageQueue = new Queue<byte[]> ();
		}

		public class CommunicationGym : WebSocketBehavior
		{
			public Agent agent { set; get; }
			MsgPack.BoxingPacker packer = new MsgPack.BoxingPacker ();
			private bool SendFlag=true;

			protected override void OnMessage (MessageEventArgs e)
			{
				//receive message 
				Dictionary<System.Object, System.Object> msg = (Dictionary<System.Object,System.Object>)packer.Unpack (e.RawData);
				var originalKey = new Dictionary<string, byte[]>();
				var idKey = new Dictionary<string, byte[]> ();
				foreach (byte[] key in msg.Keys) {
					string k = System.Text.Encoding.UTF8.GetString (key);
					if (k == "command") {
						originalKey.Add (k, key);
					} else {
						idKey.Add (k, key);
					}
					Debug.Log ("key:" + System.Text.Encoding.UTF8.GetString(key) + " value:" + msg[key]);
				}
		
				// string:
				string command = System.Text.Encoding.UTF8.GetString((byte[])msg [originalKey ["command"]]);
				// int:
				//int i = (int)action [originalKey ["command"]];
				// float:
				//float f = float.Parse (System.Text.Encoding.UTF8.GetString((byte[])action [originalKey ["value"]]));
				agent.action.Set (command);

			
				// unique id of game object
				foreach (string key in idKey.Keys) {
					int obj_id = Convert.ToInt32 (msg [idKey[key]]);
					//Debug.Log (obj_id);
					SceneController.obj_q.Enqueue (obj_id);
				}
			

				//agent.action.Set ((Dictionary<System.Object,System.Object>)packer.Unpack (e.RawData));
				SceneController.received.Set ();
				Debug.Log ("Rotate=" + agent.action.rotate + " Forword=" + agent.action.forward + " Jump=" + agent.action.jump);

				//send state data 
				Sendmessage();
			
			}

			protected override void OnOpen ()
			{
				Debug.Log ("Socket Open");
				SceneController.received.Set ();
				Sendmessage ();
			}

			protected override void OnClose(CloseEventArgs e)
			{
				SceneController.FinishFlag=true;
				SceneController.received.Set ();
			}
				
			private void Sendmessage(){
				SendFlag = true;
				//send state data 
				while (SendFlag == true) {
					if (SceneController.server.agentMessageQueue.Count > 0) {
						byte[] data = SceneController.server.PopAgentState ();
						Send (data);
						SendFlag = false;
					}
				}
			}
		}

		CommunicationGym instantiate ()
		{
			CommunicationGym service = new CommunicationGym ();
			service.agent = agent;
			return service;
		}

		string GetUrl(string domain,int port){
			return "ws://" + domain + ":" + port.ToString ();
		}

		void Awake ()
		{
			Debug.Log (GetUrl(domain,port));
			wssv = new WebSocketServer (GetUrl(domain,port));
			wssv.AddWebSocketService<CommunicationGym> ("/CommunicationGym", instantiate);
			wssv.Start ();


			if (wssv.IsListening) {
				Debug.Log ("Listening on port " + wssv.Port + ", and providing WebSocket services:");
				foreach (var path in wssv.WebSocketServices.Paths)
					Debug.Log ("- " + path);
			}
		}

		public void PushAIMessage (byte[] msg)
		{
			throw new System.NotImplementedException ();
		}

		public byte[] PopAIMessage ()
		{
			throw new System.NotImplementedException ();
		}

		public void PushAgentState (State s)
		{
			byte[] msg = packer.Pack (s);  
			mutAgent.WaitOne ();
			agentMessageQueue.Enqueue (msg);  
			mutAgent.ReleaseMutex ();
		}

		public byte[] PopAgentState ()
		{
			byte[] received = null;

			mutAgent.WaitOne ();
			if (agentMessageQueue.Count > 0) {
				received = agentMessageQueue.Dequeue ();
			}
			mutAgent.ReleaseMutex ();

			return received;
		}

		void OnApplicationQuit ()
		{
			wssv.Stop ();
			Debug.Log ("websocket server exiteed");
		}
	}
}
