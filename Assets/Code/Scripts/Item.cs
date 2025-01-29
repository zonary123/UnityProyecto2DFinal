using UnityEngine;

namespace Code.Scripts{
	public class Item{
		private ScriptableObject item;
		public string id{ get; set; } = "";
		public string name{ get; set; } = "";
		public string description{ get; set; } = "";
		public Sprite icon{ get; set; }
		public int quantity{ get; set; } = 1;
	}
}