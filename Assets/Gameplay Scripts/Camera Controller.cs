using UnityEngine;

public class CameraFollowWithMouseOffset : MonoBehaviour
{
    public Transform player;
    public float offsetAmount = 3f; // Kameranın mouse yönüne ne kadar kayacağı
    public float smoothSpeed = 5f;  // Takip yumuşaklığı

    void LateUpdate()
    {
        // Mouse'un dünya üzerindeki pozisyonunu bul
        Plane plane = new Plane(Vector3.up, player.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0f;
        Vector3 mouseWorldPos = player.position;
        if (plane.Raycast(ray, out enter))
        {
            mouseWorldPos = ray.GetPoint(enter);
        }

        // Hedef pozisyon: oyuncudan mouse yönüne offset
        Vector3 targetPos = player.position + (mouseWorldPos - player.position).normalized * offsetAmount;
        targetPos.y = player.position.y; // Yükseklik sabit kalsın

        // Takip objesini yumuşakça hareket ettir
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
    }
}
