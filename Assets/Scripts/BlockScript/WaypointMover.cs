using System;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [Header("Cài đặt đường đi")]
    [SerializeField] private Transform[] _waypoints; // Kéo các Point A, B, C vào đây
    [SerializeField] private float _speed = 5f;      // Tốc độ di chuyển
    [SerializeField] private float _waitAtPoint = 0f;// Thời gian nghỉ tại mỗi điểm
    public bool _isActive;
    
    private int _currentPointIndex = 0;
    private float _waitTimer = 0f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        _isActive = true;
    }

    private void Update()
    {
        if(!_isActive) return;
        // 1. Kiểm tra nếu chưa gán waypoint nào thì không làm gì cả
        if (_waypoints.Length == 0) return;

        // 2. Lấy vị trí điểm đến hiện tại
        Transform targetWaypoint = _waypoints[_currentPointIndex];

        // 3. Di chuyển từ vị trí hiện tại đến target
        // Vector3.MoveTowards(Vị trí hiện tại, Đích đến, Quãng đường đi được trong 1 frame)
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, _speed * Time.deltaTime);
        AudioManager.Instance.PlayElevator();
        // 4. Kiểm tra xem đã đến nơi chưa (Khoảng cách < 0.1f)
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // Nếu có thời gian chờ -> Chờ chút
            if (_waitTimer < _waitAtPoint)
            {
                _waitTimer += Time.deltaTime;
            }
            else
            {
                // Reset thời gian chờ và chuyển sang điểm tiếp theo
                _waitTimer = 0f;
                _currentPointIndex++;

                // Nếu đi hết danh sách thì quay về điểm đầu tiên (Loop)
                if (_currentPointIndex >= _waypoints.Length)
                {
                    _currentPointIndex = 0;
                }
            }
        }
    }
    
    // Vẽ đường đi trong Scene để dễ nhìn (Debug)
    private void OnDrawGizmos()
    {
        if (_waypoints == null || _waypoints.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < _waypoints.Length; i++)
        {
            // Vẽ điểm
            Gizmos.DrawWireSphere(_waypoints[i].position, 0.3f);
            
            // Vẽ đường nối
            Transform current = _waypoints[i];
            Transform next = _waypoints[(i + 1) % _waypoints.Length]; // Nối điểm cuối về đầu
            if (current != null && next != null)
            {
                Gizmos.DrawLine(current.position, next.position);
            }
        }
    }
}