using UnityEngine;
using System.Collections;

namespace MLPlayer {
	public class State {
		public float reward;
		public bool endEpisode;
		public byte[][] image;
		public byte[][] depth;

		// x, y, z
		public int obj_cnt;
		public int[] obj_id;
		public float[][] obj_pos; 
		public float[][] obj_angle;
		public float[] agent_pos;
		public float[] agent_angle;

		public void Clear() {
			reward = 0;
			endEpisode = false;
			image = null;
			depth = null;
		}
	}
}