using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class GlobalState {
	public GlobalState () {
		
	}

	public void ProcessMessage(OscMessage m) {
		this.xINC = (float) m.Values [(int) (InteractionController.enOscCentralControlMessageIndices.xINC)];
		this.yINC = (float) m.Values [(int) (InteractionController.enOscCentralControlMessageIndices.yINC)];
		this.zINC = (float) m.Values [(int) (InteractionController.enOscCentralControlMessageIndices.zINC)];
		this.globalForce = (float)m.Values [(int)(InteractionController.enOscCentralControlMessageIndices.global_force)];
//		Debug.Log (xINC + " " + yINC + " " + zINC + " " + globalForce);
	}

	public bool dirty = false;

	public float xINC = 0.01f;
	public float XINC {
		get {
			return Util.Map (xINC, 0.001f, 0.5f);
		}
	}

	public float yINC = 0.01f;
	public float YINC {
		get {
			return Util.Map (yINC, 0.001f, 0.5f);
		}
	}

	public float zINC = 0.001f;
	public float ZINC {
		get {
			return Util.Map (zINC, 0.001f, 0.5f);
		}
	}

	public float globalForce = 0f;
	public float GlobalForce {
		get {
			//return Util.Map (globalForce, 0, 100f);
			return 0;
			// we say do this cos we don't want global force to have an influence
		}
	}

	public int gridScale = 100;
	public int GridScale {
		get {
			return gridScale;
		}
		set {
			gridScale = value;
			dirty = true;
		}
	}

    public float screenAlpha = 0f;

	public void SetGridScale(float value) {
		int prevValue = gridScale;
		int newValue = (int)Util.Map (value, 10, 200);
		if (prevValue != newValue) {
			dirty = true;
			gridScale = newValue;
		}
	}

    public void SetScreenAlpha(float value) {
        Debug.Log("Setting screen alpha: " + value);
		screenAlpha = Util.Map (value, 0f, 1f);
	}
		
	public bool start = false;
	public bool Start {
		get  {
			return start;
		}
		set {
			start = value;
			stop = !value;
		}
	}

	public bool stop = true;
	public bool Stop {
		get {
			return stop;
		}
		set {
			start = !value;
			stop = value;
		}
	}

	public bool reset = false;
	public bool Reset {
		get {
			return reset;
		}
	}

	public int globalPreset = -1;
	public int GlobalPreset {
		get {
			return globalPreset;
		}
	}

	public float superstructureAlpha = 1f;
	private float globalForceInfluence = 0.7f;
	public float SuperstructureAlpha {
		get {
			return (1-globalForceInfluence) * superstructureAlpha + globalForceInfluence * globalForce * superstructureAlpha;
			//return superstructureAlpha;
		}
		set {
			superstructureAlpha = value;
		}
	}

	public int superstructureMaxMemory = 1000000;
	public int SuperstructureMaxMemory {
		get {
			return superstructureMaxMemory;
		}
		set  {
			superstructureMaxMemory = value;
		}
	}

}

