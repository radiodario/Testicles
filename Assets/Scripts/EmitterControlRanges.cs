using UnityEngine;
using System.Collections;

[System.Serializable]
public struct EmitterControlRanges {

	public EmitterControlRanges(bool UseDefault) {
		CurrentLife_min = 0f;
		CurrentLife_max = 0.25f;
		LifeDecay_min = 0.1f;
		LifeDecay_max = 0.5f;
		Bloom_min = 0.0f;
		Bloom_max = 0.1f;
		Hue_min = 0.0f;
		Hue_max = 1.0f;
		RandomMovement_min = -0.5f;
		RandomMovement_max = 0.5f;
		LocalForce_min = 0.0f;
		LocalForce_max = 1.0f;
		Force_min = 0.0f;
		Force_max = 1.0f;
		MaxSpeed_min = 0.0f;
		MaxSpeed_max = 1.0f;
		ParticleTrail_min = 0.1f;
		ParticleTrail_max = 1.0f;
		NoiseTimeOffset_min = 0.0f;
		NoiseTimeOffset_max = 1.0f;
		SpreadFactor_min = 0.0f;
		SpreadFactor_max = 1.0f;
        BirthOffsetY_min = -1.0f;
        BirthOffsetY_max = 1.0f;
        BirthOffsetX_min = 0.0f;
        BirthOffsetX_max = 1.0f;
	}

	[Range(0, 1)]
	public float CurrentLife_min;
	[Range(0, 1)]
	public float CurrentLife_max;
	[Range(0, 1)]
	public float LifeDecay_min;
	[Range(0, 1)]
	public float LifeDecay_max;
	[Range(0, 1)]
	public float Bloom_min;
	[Range(0, 1)]
	public float Bloom_max;
	[Range(0, 1)]
	public float Hue_min;
	[Range(0, 1)]
	public float Hue_max;
	[Range(0, 1)]
	public float LocalForce_min;
	[Range(0, 1)]
	public float LocalForce_max;
	[Range(0, 1)]
	public float Force_min;
	[Range(0, 1)]
	public float Force_max;
	[Range(0, 1)]
	public float RandomMovement_min;
	[Range(0, 1)]
	public float RandomMovement_max;
	[Range(0, 1)]
	public float ParticleTrail_min;
	[Range(0, 1)]
	public float ParticleTrail_max;
	[Range(0, 1)]
	public float MaxSpeed_min;
	[Range(0, 1)]
	public float MaxSpeed_max;
	[Range(0, 1)]
	public float NoiseTimeOffset_min;
	[Range(0, 1)]
	public float NoiseTimeOffset_max;
	[Range(0, 1)]
	public float SpreadFactor_min;
	[Range(0, 1)]
	public float SpreadFactor_max;
    [Range(-1, 1)]
    public float BirthOffsetY_min;
    [Range(-1, 1)]
    public float BirthOffsetY_max;
    [Range(0, 1)]
    public float BirthOffsetX_min;
    [Range(0, 1)]
    public float BirthOffsetX_max;
}
