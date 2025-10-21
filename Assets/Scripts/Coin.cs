using UnityEngine;

public class Coin : MonoBehaviour
{
    public Vector2 gpsPosition;
    public bool collected = false;

    public AudioClip coinSound;  // ✅ 반드시 public 이어야 함

    public void PlaySound()
    {
        if (coinSound != null)
        {
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
        }
    }
}