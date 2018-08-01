using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplePeerConnectionM;
public class signaling : MonoBehaviour
{
    PeerConnectionM offer;
    PeerConnectionM answer;
    void Start()
    {
        List<string> servers = new List<string>();
        servers.Add("stun: stun.skyway.io:3478");
        servers.Add("stun: stun.l.google.com:19302");

        offer = new PeerConnectionM(servers, "", "");
        offer.OnLocalSdpReadytoSend += OnLocalSdpReadytoSendOffer;
        offer.OnIceCandiateReadytoSend += setIceCandidateAnswer;
        offer.AddDataChannel();
        offer.OnLocalDataChannelReady += ConnectedOffer;
        offer.OnDataFromDataChannelReady += Received;

        answer = new PeerConnectionM(servers, "", "");
        answer.OnLocalSdpReadytoSend += OnLocalSdpReadytoSendAnswer;
        answer.OnIceCandiateReadytoSend += setIceCandidateOffer;
        answer.AddDataChannel();
        answer.OnLocalDataChannelReady += ConnectedAnswer;
        answer.OnDataFromDataChannelReady += Received;

        offer.CreateOffer();
    }

    public void Received(int id, string s)
    {
        Debug.Log("received : " + s);
    }

    public void ConnectedOffer(int id)
    {
        Debug.Log(id + ":connected");
        offer.SendDataViaDataChannel("hello from offer");
    }

    public void ConnectedAnswer(int id)
    {
        Debug.Log(id + ":connected");
        answer.SendDataViaDataChannel("hello from answer");
    }

    public void OnLocalSdpReadytoSendOffer(int id, string type, string sdp)
    {
        Debug.Log("OnLocalSdpReadytoSend called. id=" + id + " | type=" + type + " | sdp=" + sdp);
        if (type == "offer")
        {
            createAnswer(sdp);
        }
    }

    public void OnLocalSdpReadytoSendAnswer(int id, string type, string sdp)
    {
        Debug.Log("OnLocalSdpReadytoSend called. id=" + id + " | type=" + type + " | sdp=" + sdp);
        if (type == "answer")
        {
            setAnswer(sdp);
        }
    }

    public void setAnswer(string sdp)
    {
        offer.SetRemoteDescription("answer", sdp);
    }

    public void createAnswer(string sdp)
    {
        answer.SetRemoteDescription("offer", sdp);
        answer.CreateAnswer();
    }

    public void setIceCandidateOffer(int id, string candidate, int sdpMlineIndex, string sdpMid)
    {
        offer.AddIceCandidate(candidate, sdpMlineIndex, sdpMid);
    }

    public void setIceCandidateAnswer(int id, string candidate, int sdpMlineIndex, string sdpMid)
    {
        answer.AddIceCandidate(candidate, sdpMlineIndex, sdpMid);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
