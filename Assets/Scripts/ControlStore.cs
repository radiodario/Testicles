using System;
using UnityEngine;
using System.Collections.Generic;
using MessageType = InteractionController.EmitterMessageType;
using GlobalMessageType = InteractionController.GlobalControlMessageType;

public class ControlStore {

	private EmitterState[] emitterState;
	private GlobalState globalState;

	public ControlStore (int numEmitters) {
		globalState = new GlobalState ();
		emitterState = new EmitterState[numEmitters];
		for (int i = 0; i < numEmitters; i++) {
			EmitterState em = new EmitterState();
			emitterState [i] = em;
		}
	}

	public EmitterState GetStateFor(int emitterId) {
		return emitterState [emitterId];	
	}

	public GlobalState GetGlobalState() {
		return globalState;
	}

	public void HandleOSCEmitterControlMessage(int emitterId, OscMessage msg) {
		emitterState[emitterId].ProcessMessage(msg);
	}

    public void HandleOscEmitterControlRangeIndividualMessage(int emitterId, MessageType parameterType, OscMessage msg){
        float v = (float) msg.Values[0];
        switch (parameterType){
            case MessageType.CurrentLife_min:     emitterState[emitterId].ranges.CurrentLife_min = v; break;
            case MessageType.CurrentLife_max:     emitterState[emitterId].ranges.CurrentLife_max = v; break;
            case MessageType.LifeDecay_min:       emitterState[emitterId].ranges.LifeDecay_min = v; break;
            case MessageType.LifeDecay_max:       emitterState[emitterId].ranges.LifeDecay_max = v; break;
            case MessageType.Bloom_min:           emitterState[emitterId].ranges.Bloom_min = v; break;
            case MessageType.Bloom_max:           emitterState[emitterId].ranges.Bloom_max = v; break;
            case MessageType.Hue_min:             emitterState[emitterId].ranges.Hue_min = v; break;
            case MessageType.Hue_max:             emitterState[emitterId].ranges.Hue_max = v; break;
            case MessageType.Force_min:           emitterState[emitterId].ranges.Force_min = v; break;
            case MessageType.Force_max:           emitterState[emitterId].ranges.Force_max = v; break;
            case MessageType.RandomMovement_min:  emitterState[emitterId].ranges.RandomMovement_min = v; break;
            case MessageType.RandomMovement_max:  emitterState[emitterId].ranges.RandomMovement_max = v; break;
            case MessageType.LocalForce_min:      emitterState[emitterId].ranges.LocalForce_min = v; break;
            case MessageType.LocalForce_max:      emitterState[emitterId].ranges.LocalForce_max = v; break;
            case MessageType.ParticleTrail_min:   emitterState[emitterId].ranges.ParticleTrail_min = v; break;
            case MessageType.ParticleTrail_max:   emitterState[emitterId].ranges.ParticleTrail_max = v; break;
            case MessageType.MaxSpeed_min:        emitterState[emitterId].ranges.MaxSpeed_min = v; break;
            case MessageType.MaxSpeed_max:        emitterState[emitterId].ranges.MaxSpeed_max = v; break;
            case MessageType.NoiseTimeOffset_min: emitterState[emitterId].ranges.NoiseTimeOffset_min = v; break;
            case MessageType.NoiseTimeOffset_max: emitterState[emitterId].ranges.NoiseTimeOffset_max = v; break;
            case MessageType.SpreadFactor_min:    emitterState[emitterId].ranges.SpreadFactor_min = v; break;
            case MessageType.SpreadFactor_max:    emitterState[emitterId].ranges.SpreadFactor_max = v; break;
			case MessageType.DrawMode:			  emitterState[emitterId].DrawMode = (int) v; break;
            default: break;
        }
        
    }

	public void HandleOscCentralControlMessage(OscMessage msg) {
		globalState.ProcessMessage (msg);
		Debug.Log (msg);
	}

	public void HandleOscGlobalControlRangeMessage(GlobalMessageType parameterType, OscMessage msg) {
		float v = (float)msg.Values [0];
		switch (parameterType) {
		case GlobalMessageType.start:
			globalState.Start = (v > 0);
			break;
		case GlobalMessageType.stop:
			globalState.Stop = (v > 0);
			break;
		case GlobalMessageType.reset:
			globalState.reset = (v > 0);
			break;
		case GlobalMessageType.grid_scale:
			globalState.SetGridScale(v);
			break;
        case GlobalMessageType.screen_alpha:
			globalState.SetScreenAlpha(v);
			break;
		case GlobalMessageType.global_preset:
			globalState.globalPreset = (int) v;
			break;
		case GlobalMessageType.superstructure_alpha:
			globalState.SuperstructureAlpha = v;
			break;
		case GlobalMessageType.superstructure_max_memory:
			globalState.SuperstructureMaxMemory = (int)v;
			break;
		default:
			break;
		}
	}

    public void HandleOscZoneAzimuthMessage(int zone, float value){
        emitterState[zone].azimuthOffset = (value % 360.0f) / 360.0f;
    }

    public void HandleOscZoneRotationMessage(int zone, float value){
        emitterState[zone].birthPositionOffsetX = (value % 360.0f) / 360.0f;
    }

    public EmitterState GetEmitterState(int emitterId) {
		return emitterState [emitterId];
	}
		
}

