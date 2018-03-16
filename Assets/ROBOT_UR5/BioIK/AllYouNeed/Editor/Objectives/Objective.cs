using UnityEngine;
using UnityEditor;

namespace BioIK {
	[CustomEditor(typeof(Objective))]
	public class Objective_CE : Editor {
		protected Objective Target;
		void Awake() {
			Target = (Objective)target;
		}
	}
}