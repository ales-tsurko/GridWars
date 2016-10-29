using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using AssemblyCSharp;
using System.Net.NetworkInformation;
using SocketIO;

public class Network : Bolt.GlobalEventListener {
	// public interface

	public string listenEndpoint = "0.0.0.0";
	//public string zeusEndpoint = "159.8.0.207:24000";
    public string zeusEndpoint = "107.170.5.61:24000";


	//Bolt

	public NetworkDelegate networkDelegate;
	public bool isShuttingDown;
	public BoltConnection connection;

	public void ShutdownBolt() {
		if (isShuttingDown) {
			return;
		}

		isShuttingDown = true;
		connection = null;
		StartShutdownTimer();
		if (BoltNetwork.isRunning) {
			App.shared.Log("ShutdownBolt", this);
			BoltLauncher.Shutdown();
		}
	}

	void CheckForShutdown() {
		//Debug.Log("CheckForShutdown: " + BoltNetwork.isRunning + "," + Bolt.Zeus.IsConnected);
		if (!BoltNetwork.isRunning && !Bolt.Zeus.IsConnected) {
			isShuttingDown = false;
			App.shared.Log("BoltShutdownCompleted", this);
			networkDelegate.BoltShutdownCompleted();
		}
		else {
			StartShutdownTimer();
		}
	}

	void StartShutdownTimer() {
		var timer = App.shared.timerCenter.NewTimer();
		timer.action = CheckForShutdown;
		timer.timeout = 0.2f;
		timer.Start();
	}

	public override void BoltStartBegin() {
		base.BoltStartBegin();
		App.shared.Log("BoltStartBegin", this);
		BoltNetwork.RegisterTokenClass<ServerToken>();
	}
		
	public override void BoltStartDone() {
		base.BoltStartDone();
		App.shared.Log("BoltStartDone", this);
		networkDelegate.BoltStartDone();
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);
		App.shared.Log("ZeusConnected", this);
		networkDelegate.ZeusConnected(endpoint);
	}

	public override void ZeusConnectFailed(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnectFailed(endpoint);
		//TODO: implement this
		App.shared.Log("ZeusConnectFailed", this);
	}

	public override void ZeusDisconnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusDisconnected(endpoint);
		App.shared.Log("ZeusDisconnected", this);
		networkDelegate.ZeusDisconnected();
	}
		
	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		base.SessionListUpdated(sessionList);
		App.shared.Log("SessionListUpdated", this);
		networkDelegate.SessionListUpdated(sessionList);
	}

	public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRequest(endpoint, token);
		App.shared.Log("ConnectRequest", this);
		networkDelegate.ConnectRequest(endpoint, token);
	}

	//Game was full
	public override void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRefused(endpoint, token);
		App.shared.Log("ConnectRefused", this);
		networkDelegate.ConnectRefused(endpoint, token);
	}

	public override void Connected(BoltConnection connection) {
		base.Connected(connection);
		App.shared.Log("Connected", this);
		this.connection = connection;
		networkDelegate.Connected(connection);
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);
		App.shared.Log("Disconnected", this);
		this.connection = null;
		networkDelegate.Disconnected(connection);
	}

	public override void BoltShutdownBegin(Bolt.AddCallback registerDoneCallback) {
		base.BoltShutdownBegin(registerDoneCallback);
		App.shared.Log("BoltShutdownBegin", this);
	}

	public override void BoltStartFailed() {
		base.BoltStartFailed();
		App.shared.Log("BoltStartFailed", this);
		networkDelegate.BoltStartFailed();
	}

	public override void ConnectAttempt(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectAttempt(endpoint, token);
		App.shared.Log("ConnectAttempt: " + endpoint.Address.ToString(), this);
	}

	public override void ConnectFailed(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectFailed(endpoint, token);
		App.shared.Log("ConnectFailed: " + endpoint.Address.ToString(), this);
	}

	public override void PortMappingChanged(Bolt.INatDevice device, Bolt.IPortMapping portMapping) {
		base.PortMappingChanged(device, portMapping);
		App.shared.Log("PortMappingChanged", this);
	}

	public override void SessionConnectFailed(UdpKit.UdpSession session, Bolt.IProtocolToken token) {
		base.SessionConnectFailed(session, token);
		App.shared.Log("SessionConnectFailed", this);
	}

	public override void ZeusNatProbeResult(UdpKit.NatFeatures features) {
		base.ZeusNatProbeResult(features);

		App.shared.Log("ZeusNatProbeResult: " + features.ToString(), this);
	}

	public override void OnEvent(RequestRematchEvent evnt) {
		base.OnEvent(evnt);

		if (!evnt.FromSelf) {
			App.shared.Log("RequestRematchEvent", this);
			networkDelegate.ReceivedRematchRequest();
		}
	}

	public override void OnEvent(ConcedeEvent evnt) {
		base.OnEvent(evnt);

		if (!evnt.FromSelf) {
			App.shared.Log("ConcedeEvent", this);
			networkDelegate.ReceivedConcede();
		}
	}

	public override void OnEvent(AcceptRematchEvent evnt) {
		base.OnEvent(evnt);

		if (!evnt.FromSelf) {
			App.shared.Log("AcceptRematchEvent", this);
			networkDelegate.ReceivedAcceptRematch();
		}
	}
}
