using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class ArcadeWaitForBoltState : NetworkDelegateState {
    public override void EnterFrom(AppState state) {
        base.EnterFrom(state);

        if (BoltNetwork.isRunning) {
            network.ShutdownBolt();
        }
        else {
            StartBolt();
        }
    }

    void StartBolt() {
        BoltLauncher.StartServer();
    }

    // NetworkDelegate

    public override void BoltStartDone() {
        base.BoltStartDone();

        TransitionTo(new ArcadePlayingGameState());
    }

    //might not be shutdown yet from previous game
    public override void BoltShutdownCompleted() {
        base.BoltShutdownCompleted();

        StartBolt();
    }
}
