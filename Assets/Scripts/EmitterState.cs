using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class EmitterState {

	public EmitterState() {
		this.ranges = new EmitterControlRanges (true);
	}

	public EmitterState(OscMessage msg, EmitterControlRanges ranges) {
		this.ranges = ranges;
		this.ProcessMessage (msg);
	}

	public void ProcessMessage(OscMessage msg) {
		this.currentLife = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.CurrentLife];
		this.lifeDecay = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.LifeDecay];
		this.bloom = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.Bloom];
		this.hue = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.Hue];

        //Hack to get around re-building Wekinator brains and retraining: 
		this.birthPositionOffsetY = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.RandomMovement];

        this.force = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.Force];
		this.localForce = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.LocalForce];
		this.particleTrail = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.ParticleTrail];
		this.maxSpeed = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.MaxSpeed];
		this.noiseTimeOffset = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.NoiseTimeOffset];
		this.spreadFactor = (float) msg.Values [(int)InteractionController.enOscEmitterControlMessageIndices.SpreadFactor];
	}

	public EmitterControlRanges ranges;

	[Range(0, 1)]
	public float currentLife = 0;
	public float CurrentLife {
		get {
			return Util.Map (currentLife, ranges.CurrentLife_min, ranges.CurrentLife_max) * 100;
		}
	}

	[Range(0, 1)]
	public float lifeDecay = 0;
	public float LifeDecay {
		get {
			return Util.Map (lifeDecay, ranges.LifeDecay_min, ranges.LifeDecay_max) * 10f;
		}
	}

	[Range(0, 1)]
	public float bloom = 0;
	public float Bloom {
		get {
			return Util.Map (bloom, ranges.Bloom_min, ranges.Bloom_max) * 5f;
		}
	}

	[Range(0, 1)]
	public float hue = 0;
	public float Hue {
		get {
			return Util.Map (hue, ranges.Hue_min, ranges.Hue_max) * 1;
		}
	}

	[Range(0, 1)]
	public float force = 0;
	public float Force {
		get {
			return Util.Map (force, ranges.Force_min, ranges.Force_max) * 100f;
		}
	}

	[Range(0, 1)]
	public float heightOffset = 0;
	public float HeightOffset {
		get {
			return Util.Map (heightOffset, ranges.RandomMovement_min, ranges.RandomMovement_max); }
	}

	[Range(0, 1)]
	public float particleTrail = 0;
	public float ParticleTrail {
		get {
			return Util.Map (particleTrail, ranges.ParticleTrail_min, ranges.ParticleTrail_max) * 50;
		}
	}

	[Range(0, 1)]
	public float maxSpeed = 0;
	public float MaxSpeed {
		get {
			return Util.Map (maxSpeed, ranges.MaxSpeed_min, ranges.MaxSpeed_max) * 200f;
		}
	}

	[Range(0, 1)]
	public float localForce = 0;
	public float LocalForce {
		get {
			return Util.Map (localForce, ranges.LocalForce_min, ranges.LocalForce_max) * 100f;
		}
	}

	[Range(0, 1)]
	public float noiseTimeOffset = 0;
	public float NoiseTimeOffset {
		get {
			return Util.Map (noiseTimeOffset, ranges.NoiseTimeOffset_min, ranges.NoiseTimeOffset_max) * 2 - 1f;
		}
	}

	[Range(0, 1)]
	public float spreadFactor = 0;
	public float SpreadFactor {
		get {
			return Util.Map (spreadFactor, ranges.SpreadFactor_min, ranges.SpreadFactor_max) * 1f;
		}
	}

    [Range(0, 1)]
    public float birthPositionOffsetY = 0.5f;
    public float BirthPositionOffsetY {
        get {
			float pos = birthPositionOffsetY;
            return Util.Map (pos, ranges.RandomMovement_min, ranges.RandomMovement_max);
        }
    }

    [Range(-1, 1)]
    public float birthPositionOffsetX = 0;
    public float BirthPositionOffsetX {
        get {
            float x = birthPositionOffsetX - azimuthOffset;
            if (x < 0.0f)
                x += 1.0f;
            return Util.Map (x % 1.0f, ranges.BirthOffsetX_min, ranges.BirthOffsetX_max);
        }
    }

    public float azimuthOffset = 0.0f;

	public enum enDrawMode
	{
		lines = 1,
		triangles = 4,
		triangle_strip = 5,
		quads = 7
	}

	public enDrawMode drawMode = enDrawMode.lines;
	public int DrawMode {
		get {
			return (int)drawMode;
		}
		set {
			this.drawMode = (enDrawMode) value;
		}
	}
}
