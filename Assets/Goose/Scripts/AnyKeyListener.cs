using UnityEngine;
using System.Collections;

public class AnyKeyListener : MonoBehaviour {

    public void Listen(PlayerInputs inputs, System.Action onKey, System.Action onAnyKey, InControl.PlayerAction onAction){
        StartCoroutine(ListenForConcedeOrAnyKey(inputs, onKey, onAnyKey, onAction));
    }

    public IEnumerator ListenForConcedeOrAnyKey(PlayerInputs inputs, System.Action onKey, System.Action onAnyKey, InControl.PlayerAction onAction){
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        while (!Input.anyKeyDown) {
            if (onAction.WasPressed) {
                onKey.Invoke();
                Destroy(gameObject, 0.2f);
                yield break;
            }
            //NEED an ANYBUTTON HERE TO CANCEL
            if (inputs.goBack.WasPressed) {
                onAnyKey.Invoke();
                Destroy(gameObject, 0.2f);
                yield break;
            }
            yield return null;
        }
        if (inputs.concede.WasPressed) {
            onKey.Invoke();
            Destroy(gameObject, 0.2f);
            yield break;
        }
        if (Input.GetMouseButton(0)) {
            yield return new WaitForSeconds(.2f);
        }
        onAnyKey.Invoke();
        Destroy(gameObject, 0.2f);
        yield break;
    }

}