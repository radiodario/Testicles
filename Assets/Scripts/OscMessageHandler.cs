using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class OscMessageHandler : MonoBehaviour  {

	// OSC Parameters
	public static string remoteIP = "127.0.0.1";//"192.168.1.169";
	public static int sendToPort = 9002; //the port you will be sending from
	public static int listenerPort = 9000; //the port you will be listening on
    public GameObject textObject;
    //public Text oscMonitor;
    public string latestOSCAddress;
	private Osc handler;

	private bool showMessages = false;
    public static OscMessage messageToSend;
	public static Dictionary<string, OscMessage> OscMessages = new Dictionary<string, OscMessage> ();

	// Use this for initialization
	void Start () {
		UDPPacketIO udp = GetComponent<UDPPacketIO>();
		udp.init (remoteIP, sendToPort, listenerPort);
		handler = GetComponent<Osc> ();
		handler.init (udp);
		handler.SetAllMessageHandler (OscMsgHandler);
        //oscMonitor = textObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

        if (messageToSend != null){
            Debug.Log("Sending " + messageToSend.Address + " vale: " + messageToSend.Values[0]);
            handler.Send (messageToSend);
            messageToSend =  null;
        }
	}

    public static void sendRequestZoneParametersMessage()
    {
        OscMessage requestMessage = new OscMessage();
        requestMessage.Address = InteractionController.GlobalControlOSCAddressRoot  
                               + InteractionController.getGlobalControlMessageTypeString (InteractionController.GlobalControlMessageType.request_zones_parameters);
        requestMessage.Values.Add (1.0f);
        messageToSend = requestMessage;
    }

	void OscMsgHandler (OscMessage oscMessage)
	{
		OscMessages[oscMessage.Address] = oscMessage;
        latestOSCAddress = oscMessage.Address;
		if (showMessages) { 
			Debug.Log (Osc.OscMessageToString (oscMessage));
		}
	}
}
