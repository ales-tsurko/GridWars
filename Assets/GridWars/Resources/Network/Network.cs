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
	public string zeusEndpoint = "159.8.0.207:24000";


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
		BoltLauncher.Shutdown();
		StartShutdownTimer();
	}

	void CheckForShutdown() {
		//Debug.Log("CheckForShutdown: " + BoltNetwork.isRunning + "," + Bolt.Zeus.IsConnected);
		if (!BoltNetwork.isRunning && !Bolt.Zeus.IsConnected) {
			isShuttingDown = false;
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
		BoltNetwork.RegisterTokenClass<ServerToken>();
	}
		
	public override void BoltStartDone() {
		base.BoltStartDone();
		networkDelegate.BoltStartDone();
	}

	public override void ZeusConnected(UdpKit.UdpEndPoint endpoint) {
		base.ZeusConnected(endpoint);
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
		networkDelegate.SessionListUpdated(sessionList);
	}

	public override void ConnectRequest(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRequest(endpoint, token);
		networkDelegate.ConnectRequest(endpoint, token);
	}

	//Game was full
	public override void ConnectRefused(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token) {
		base.ConnectRefused(endpoint, token);
		networkDelegate.ConnectRefused(endpoint, token);
	}

	public override void Connected(BoltConnection connection) {
		base.Connected(connection);
		this.connection = connection;
		networkDelegate.Connected(connection);
	}

	public override void Disconnected(BoltConnection connection) {
		base.Disconnected(connection);
		this.connection = null;
		networkDelegate.Disconnected(connection);
	}

	public override void BoltShutdownBegin(Bolt.AddCallback registerDoneCallback) {
		base.BoltShutdownBegin(registerDoneCallback);
		App.shared.Log("BoltShutdownBegin", this);
	}

	public override void BoltStartFailed() {
		base.BoltStartFailed();
		networkDelegate.BoltStartFailed();
	}
}
