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


	public void Reset() {
		if (BoltNetwork.isRunning) {
			ShutdownBolt();
			networkDelegate = null;
		}
	}

	//Bolt

	public NetworkDelegate networkDelegate;
	public bool isShuttingDown;
	public BoltConnection connection;

	public void ShutdownBolt() {
		if (isShuttingDown) {
			return;
		}

		isShuttingDown = true;
		App.shared.Log("ShutdownBolt", this);
		BoltLauncher.Shutdown();
	}

	public override void BoltStartBegin() {
		base.BoltStartBegin();
		App.shared.Log("BoltStartBegin", this);
		BoltNetwork.RegisterTokenClass<ServerToken>();
	}
		
	public override void BoltStartDone() {
		base.BoltStartDone();
		App.shared.Log("BoltStartDone", this);
		if (networkDelegate != null) {
			networkDelegate.BoltStartDone();
		}
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);
		App.shared.Log("ZeusConnected", this);
		if (networkDelegate != null) {
			networkDelegate.ZeusConnected(endpoint);
		}
	}

	public override void ZeusConnectFailed(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnectFailed(endpoint);
		App.shared.Log("ZeusConnectFailed", this);

		if (networkDelegate != null) {
			networkDelegate.ZeusConnectFailed();
		}
	}

	public override void ZeusDisconnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusDisconnected(endpoint);
		App.shared.Log("ZeusDisconnected", this);
		if (networkDelegate != null) {
			networkDelegate.ZeusDisconnected();
		}
	}
		
	public override void SessionListUpdated(UdpKit.Map<System.Guid, UdpKit.UdpSession> sessionList) {
		base.SessionListUpdated(sessionList);
		App.shared.Log("SessionListUpdated", this);
		if (networkDelegate != null) {
			networkDelegate.SessionListUpdated(sessionList);
		}
	}

	public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRequest(endpoint, token);
		App.shared.Log("ConnectRequest", this);
		if (networkDelegate != null) {
			networkDelegate.ConnectRequest(endpoint, token);
		}
	}

	//Game was full
	public override void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRefused(endpoint, token);
		App.shared.Log("ConnectRefused", this);
		if (networkDelegate != null) {
			networkDelegate.ConnectRefused(endpoint, token);
		}
	}

	public override void Connected(BoltConnection connection) {
		base.Connected(connection);
		App.shared.Log("Connected", this);
		this.connection = connection;
		if (networkDelegate != null) {
			networkDelegate.Connected(connection);
		}
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);
		App.shared.Log("Disconnected", this);
		this.connection = null;
		if (networkDelegate != null) {
			networkDelegate.Disconnected(connection);
		}
	}

	public override void BoltShutdownBegin(Bolt.AddCallback registerDoneCallback) {
		isShuttingDown = true;
		base.BoltShutdownBegin(registerDoneCallback);
		registerDoneCallback(BoltShutdownCompleted);
		App.shared.Log("BoltShutdownBegin", this);
	}

	void BoltShutdownCompleted() {
		App.shared.Log("BoltShutdownCompleted", this);
		connection = null;
		isShuttingDown = false;
		if (networkDelegate != null) {
			networkDelegate.BoltShutdownCompleted();
		}
	}

	public override void BoltStartFailed() {
		base.BoltStartFailed();
		App.shared.Log("BoltStartFailed", this);
		if (networkDelegate != null) {
			networkDelegate.BoltStartFailed();
		}
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
}
