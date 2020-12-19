using UnityEngine;

public class Camera : MonoBehaviour
{
    [Header("目標")]
    public Transform target;
    [Header("追蹤速度"), Range(0, 100)]
    public float speed = 1.5f;
    [Header("攝影機下方與上方的限制")]
    public Vector2 limit = new Vector2(0, 3.5f);

    /* 認識插值
    public float a = 0;
    public float b = 100;

    public Vector2 v2A = new Vector2(0, 0);
    public Vector2 v2B = new Vector2(100, 100);

    private void Start()
    {
        // 插值
        // 取得兩點間的某個值
        // 0 - 10 取得 50% 得到 5
        print(Mathf.Lerp(0, 100, 0.7f));
    }

    private void Update()
    {
        a = Mathf.Lerp(a, b, 0.1f);

        v2A = Vector2.Lerp(v2A, v2B, 0.1f);
    }
    */

    /// <summary>
    /// 追蹤
    /// </summary>
    private void Track()
    {
        Vector3 posA = transform.position;                              // 取得攝影機座標
        Vector3 posB = target.position;                                 // 取得目標座標

        posB.z = -10;                                                   // 固定 Z 軸
        posB.y = Mathf.Clamp(posB.y, limit.x, limit.y);                 // 將 Y 軸夾在限制範圍內

        // 一幀的時間 Time.deltaTime

        posA = Vector3.Lerp(posA, posB, speed * Time.deltaTime);        // 插值

        transform.position = posA;                                      // 攝影機 座標 = A 點
    }

    private void LateUpdate()
    {
        Track();
    }
}