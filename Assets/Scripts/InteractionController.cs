using UnityEngine;
using System.Collections;
using Exception = System.Exception;

public class InteractionController : MonoBehaviour 
{
	public static string MasterMicAddress = "/Audio/A1";
	public static string AudioOSCAddressRoot = "/Audio/A";

	public static string EmitterControlOSCAddressRoot = "/Control/E";
	public static string EmitterControlRangeOSCAddressRoot = "/Control/R";

	public static string GlobalControlOSCAddressRoot = "/Control/";
    public static string ZoneRotationAddressRoot = "/rotation_offset/";
    public static string ZoneAzimuthAddressRoot = "/azimuth/";

	public static string CentralControlOSCAddress = "/Control/Central";
	public string[] AudioOSCAddresses;
	public string[] EmitterControlOSCAddresses;
	public string[] EmitterControlRangeOSCAddresses;
    public string[] EmitterControlRangeMessageTypes;
	public string[] GlobalControlOSCAddresses;

	public ControlStore store;

	[Tooltip("number of emitters to map to the messages")]
	public int NumEmitters = 2;

	HexagonParticleSystem[] emitters;
	bool debug = false;

	[Range(1, 100)]
	[Tooltip("Hex Lattice scale")]
	public float amplitudeGain = 60;

	public struct AudioMessage {
		public float Onset;
		public float Amplitude;
		public float Pitch;
		public float Centroid;
		public float Spread;
		public float Flatness;
		public float LowER; // low energy ratio
		public float Flux;
		public float Slope;
		public float HarmonicER; // harmonic energy ratio
		public float OddEvenER;
		public float Inharmonicity;
	}
		
	enum enOscAudioMessageIndices {
		Onset,
		Amplitude,
		Pitch,
		Centroid,
		Slope,
		Spread,
		Flatness,
		LowER,
		Flux,
		HarmonicER,
		OddEvenER,
		Inharmonicity,
		numIndices // we do this in case we iterate. Hat's off Sean ;_;
	}

	public enum EmitterMessageType {
		CurrentLife_min,
		CurrentLife_max,
		LifeDecay_min,
		LifeDecay_max,
		Bloom_min,
		Bloom_max,
		Hue_min,
		Hue_max,
		Force_min,
		Force_max,
		RandomMovement_min,
		RandomMovement_max,
		LocalForce_min,
		LocalForce_max,
		ParticleTrail_min,
		ParticleTrail_max,
		MaxSpeed_min,
		MaxSpeed_max,
		NoiseTimeOffset_min,
		NoiseTimeOffset_max,
		SpreadFactor_min,
		SpreadFactor_max,
		DrawMode,
		numIndices
	}

    public static string getEmitterControlRangeMessageTypeString(EmitterMessageType type) {

        switch (type) {
            case EmitterMessageType.CurrentLife_min:     return "current_life_min";
            case EmitterMessageType.CurrentLife_max:     return "current_life_max";
            case EmitterMessageType.LifeDecay_min:       return "life_decay_min";
            case EmitterMessageType.LifeDecay_max:       return "life_decay_max";
            case EmitterMessageType.Bloom_min:           return "bloom_min";
            case EmitterMessageType.Bloom_max:           return "bloom_max";
            case EmitterMessageType.Hue_min:             return "hue_min";
            case EmitterMessageType.Hue_max:             return "hue_max";
            case EmitterMessageType.Force_min:           return "force_min";
            case EmitterMessageType.Force_max:           return "force_max";
            case EmitterMessageType.RandomMovement_min:  return "random_movement_min";
            case EmitterMessageType.RandomMovement_max:  return "random_movement_max";
            case EmitterMessageType.LocalForce_min:      return "local_force_min";
            case EmitterMessageType.LocalForce_max:      return "local_force_max";
            case EmitterMessageType.ParticleTrail_min:   return "particle_trail_min";
            case EmitterMessageType.ParticleTrail_max:   return "particle_trail_max";
            case EmitterMessageType.MaxSpeed_min:        return "max_speed_min";
            case EmitterMessageType.MaxSpeed_max:        return "max_speed_max";
            case EmitterMessageType.NoiseTimeOffset_min: return "noise_time_offset_min";
            case EmitterMessageType.NoiseTimeOffset_max: return "noise_time_offset_max";
            case EmitterMessageType.SpreadFactor_min:    return "spread_factor_min";
            case EmitterMessageType.SpreadFactor_max:    return "spread_factor_max";
		 	case EmitterMessageType.DrawMode:           return "draw_mode";
            default: return "UNKNOWN";
        }
    }

	public enum GlobalControlMessageType {
		start = 0,
		stop,
		reset,
		grid_scale,
        screen_alpha,
        request_zones_parameters,
		global_preset,
		superstructure_alpha,
		superstructure_max_memory,
		numGlobalMessages
	}

	public static string getGlobalControlMessageTypeString(GlobalControlMessageType type) {
		switch (type) {
            case GlobalControlMessageType.start:                    return "start";
            case GlobalControlMessageType.stop:                     return "stop";
            case GlobalControlMessageType.reset:                    return "reset";
            case GlobalControlMessageType.grid_scale:               return "grid_scale";
            case GlobalControlMessageType.screen_alpha:             return "screen_alpha";
            case GlobalControlMessageType.request_zones_parameters: return "request_zones_parameters";
            case GlobalControlMessageType.global_preset:            return "global_preset";
			case GlobalControlMessageType.superstructure_alpha:		return "superstructure_alpha";
			case GlobalControlMessageType.superstructure_max_memory:return "superstructure_max_memory";
			default:
				return "UNKNOWN";
		}
	}

    public static string getZoneRotationAddress (int zone){
        //Zones are indexed from 1 in alains osc spec
        return ZoneRotationAddressRoot + (zone + 1);
    }

    public static string getZoneAzimuthAddress (int zone){
        return ZoneAzimuthAddressRoot + (zone + 1);
    }

	public enum enOscCentralControlMessageIndices {
		xINC,
		yINC,
		zINC,
		global_force,
		numIndices
	}

	public enum enOscEmitterControlMessageIndices  {
		CurrentLife,
		LifeDecay,
		Bloom,
		Hue,
		Force,
		RandomMovement,
		ParticleTrail,
		MaxSpeed,
		LocalForce,
		NoiseTimeOffset,
		SpreadFactor,
		DrawMode,
        PositionOffsetX,
        PositionOffsetY,
		numIndices
	}

	//for spout
	string spoutCameraName = "Main Camera.SpoutSender";
	//for screen
	string screenCameraName = "Main Camera";

	// Use this for initialization
	void Start () {
		AudioOSCAddresses = new string[NumEmitters];
		for (var i = 0; i < NumEmitters; i++) {
			AudioOSCAddresses [i] = AudioOSCAddressRoot + i;	
		}

		EmitterControlOSCAddresses = new string[NumEmitters];
		for (var i = 0; i < NumEmitters; i++) {
			EmitterControlOSCAddresses [i] = EmitterControlOSCAddressRoot + (i); 
		}

		EmitterControlRangeOSCAddresses = new string[NumEmitters];
		for (var i = 0; i < NumEmitters; i++) {
			EmitterControlRangeOSCAddresses [i] = EmitterControlRangeOSCAddressRoot + (i); 
		}

        EmitterControlRangeMessageTypes = new string[(int) EmitterMessageType.numIndices];
		for (var i = 0; i < (int) EmitterMessageType.numIndices; i++) {
			EmitterControlRangeMessageTypes [i] = getEmitterControlRangeMessageTypeString((EmitterMessageType) i); 
            Debug.Log (EmitterControlRangeOSCAddresses[0] + "/" + EmitterControlRangeMessageTypes[i]);
		}

		GlobalControlOSCAddresses = new string[(int)GlobalControlMessageType.numGlobalMessages];
		for (var i = 0; i < (int)GlobalControlMessageType.numGlobalMessages; i++) {
			GlobalControlOSCAddresses [i] = GlobalControlOSCAddressRoot + getGlobalControlMessageTypeString ((GlobalControlMessageType)i);
		}
			
		emitters = GameObject.FindObjectsOfType<HexagonParticleSystem> ();
		store = new ControlStore (NumEmitters);
	}
	
	// Update is called once per frame
	void Update () {
		CheckForCentralControlMessages ();
		CheckForEmitterControlMessages ();
		CheckForEmitterControlRangeMessages ();
		CheckForGlobalControlMessages ();
        CheckForZoneRotationMessages();
        CheckForZoneAzimuthMessages();

		GlobalState newGlobalState = store.GetGlobalState ();
		for (var i = 0; i < emitters.Length; i++) {
			EmitterState state = store.GetStateFor (i);
			emitters [i].SetState (state, newGlobalState);
		}
	}

	private void CheckForCentralControlMessages() {
		if (OscMessageHandler.OscMessages.ContainsKey (CentralControlOSCAddress)) {
			OscMessage msg = OscMessageHandler.OscMessages [CentralControlOSCAddress];
			store.HandleOscCentralControlMessage (msg);
		}
	}


	private void CheckForEmitterControlMessages() {
		for (var i = 0; i < EmitterControlOSCAddresses.Length; i++) {
			if (OscMessageHandler.OscMessages.ContainsKey (EmitterControlOSCAddresses [i])) {
				OscMessage msg = OscMessageHandler.OscMessages [EmitterControlOSCAddresses [i]];

				store.HandleOSCEmitterControlMessage (i, msg);

			}
		}
	}

	private void CheckForEmitterControlRangeMessages() {
		for (var i = 0; i < EmitterControlRangeOSCAddresses.Length; i++) {
            for (var m = 0; m < EmitterControlRangeMessageTypes.Length; m++){
                string individualMessageAddress = EmitterControlRangeOSCAddresses [i] + "/" + EmitterControlRangeMessageTypes[m];
                if (OscMessageHandler.OscMessages.ContainsKey (individualMessageAddress)){
                    OscMessage msg = OscMessageHandler.OscMessages [individualMessageAddress];

                    store.HandleOscEmitterControlRangeIndividualMessage (i, (EmitterMessageType) m, msg);
                    //Flush message address to avoid unnecessary repeated handling 
                    OscMessageHandler.OscMessages.Remove (msg.Address);
                }
            }
		}
	}

	private void CheckForGlobalControlMessages() {
		for (var i = 0; i < GlobalControlOSCAddresses.Length; i++) {
			if (OscMessageHandler.OscMessages.ContainsKey (GlobalControlOSCAddresses [i])) {
				OscMessage msg = OscMessageHandler.OscMessages [GlobalControlOSCAddresses [i]];
				store.HandleOscGlobalControlRangeMessage ((GlobalControlMessageType) i, msg);
                //Flush message address to avoid unnecessary repeated handling 
                OscMessageHandler.OscMessages.Remove (msg.Address);
			}
		}
	}

    private void CheckForZoneAzimuthMessages(){
        for (var zone = 0; zone < NumEmitters; zone++){
            string address = getZoneAzimuthAddress (zone);
            if (OscMessageHandler.OscMessages.ContainsKey (address)){
                OscMessage msg = OscMessageHandler.OscMessages[address];
                store.HandleOscZoneAzimuthMessage(zone, (float) msg.Values[0]);
                OscMessageHandler.OscMessages.Remove(msg.Address);
            }

        }
    }

    private void CheckForZoneRotationMessages(){
        for (var zone = 0; zone < NumEmitters; zone++){
            string address = getZoneRotationAddress (zone);
            if (OscMessageHandler.OscMessages.ContainsKey (address)){
                OscMessage msg = OscMessageHandler.OscMessages[address];
                store.HandleOscZoneRotationMessage(zone, (float) msg.Values[0]);
                OscMessageHandler.OscMessages.Remove(msg.Address);
            }

        }
    }

	private AudioMessage ProcessAudioMessage(OscMessage msg) {
		if (debug) {
			Debug.Log (Osc.OscMessageToString (msg));
		}

		AudioMessage aMsg;

		aMsg.Onset     = (float) msg.Values [(int) enOscAudioMessageIndices.Onset];
		aMsg.Amplitude = (float) msg.Values [(int) enOscAudioMessageIndices.Amplitude];
		aMsg.Pitch     = (float) msg.Values [(int) enOscAudioMessageIndices.Pitch];
		aMsg.Centroid  = (float) msg.Values [(int) enOscAudioMessageIndices.Centroid];
		aMsg.Spread    = (float) msg.Values [(int) enOscAudioMessageIndices.Spread];
		aMsg.Flatness  = (float) msg.Values [(int) enOscAudioMessageIndices.Flatness];
		aMsg.LowER     = (float) msg.Values [(int) enOscAudioMessageIndices.LowER];
		aMsg.Flux      = (float) msg.Values [(int) enOscAudioMessageIndices.Flux];
		aMsg.Slope     = (float) msg.Values [(int) enOscAudioMessageIndices.Slope];
		aMsg.HarmonicER = (float)msg.Values [(int) enOscAudioMessageIndices.HarmonicER];
		aMsg.OddEvenER = (float) msg.Values [(int) enOscAudioMessageIndices.OddEvenER];
		aMsg.Inharmonicity = (float)msg.Values [(int)enOscAudioMessageIndices.Inharmonicity];

		// TODO: work out why this is happening?
		aMsg.Amplitude -= 0.01f;
		if (aMsg.Amplitude < 0) {
			aMsg.Amplitude = 0;
		}
			
		return aMsg;
	}


}
