using UnityEngine;

public class DrumInput3D : MonoBehaviour
{
    public CarRig3D car;
    [Header("Audio")]
    public AudioSource srcQ; // 左
    public AudioSource srcW; // 正前
    public AudioSource srcE; // 右
    public AudioSource srcR; // 特效/摇

    public float strength = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) OnQ();
        if (Input.GetKeyDown(KeyCode.W)) OnW();
        if (Input.GetKeyDown(KeyCode.E)) OnE();
        if (Input.GetKeyDown(KeyCode.R)) OnR();
    }
    public void OnQ() { srcQ?.Play(); car?.PulseLeft(strength); }
    public void OnW() { srcW?.Play(); car?.PulseCenter(strength); }
    public void OnE() { srcE?.Play(); car?.PulseRight(strength); }
    public void OnR() { srcR?.Play(); car?.PulseShake(strength); }
}
