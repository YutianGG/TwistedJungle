using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("移動速度"), Range(0, 1000)]
    public float speed = 10.5f;
    [Header("跳躍高度"), Range(0, 3000)]
    public int jump = 100;
    [Header("是否在地板上"), Tooltip("檢查玩家是否站在地板上")]
    public bool isGrounded = false;
    [Header("子彈"), Tooltip("存放要生成的子彈預製物")]
    public GameObject bullet;
    [Header("子彈生成點"), Tooltip("子彈生成位置")]
    public Transform point;
    [Header("子彈速度"), Range(0, 5000)]
    public int speedBullet = 800;
    [Header("開槍音效")]
    public AudioClip soundFire;
    [Header("生命數量"), Range(0, 10)]
    public int live = 3;
    [Header("檢查地面位移")]
    public Vector2 offset;
    [Header("檢查地面半徑")]
    public float radius = 0.3f;

    private int score;
    private AudioSource aud;
    private Rigidbody2D rig;
    private Animator ani;

    private GameManager gm;
    private BOSS bb;
    private void Awake()
    {
        // 剛體 = 取得元件<剛體元件>()；
        // 抓到角色身上的剛體元件存放到 rig 欄位內
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        // 透過<類型>取得物件
        // 僅限於此<類型>在場景上只有一個
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        Move();
        Fire();
        Jump();
        NextLevel();
    }

    /// <summary>
    /// 前往下一關
    /// </summary>
    private void NextLevel()
    {
        if (inDoor )                      // 如果 在門裡面 並且 按下 W
        {
            int lvIndex = SceneManager.GetActiveScene().buildIndex;     // 取得當前場景的編號

            lvIndex++;                                                  // 編號加一

            SceneManager.LoadScene(lvIndex);                            // 載入下一關
        }
    }

    /// <summary>
    /// 移動功能
    /// </summary>
    private void Move()
    {
        float h = Input.GetAxis("Horizontal");  // 水平浮點數 = 輸入 的 取得軸向("水平") - 左右AD
        rig.velocity = new Vector2(h * speed, rig.velocity.y); // 剛體 的 加速度 = 新 二維向量(水平浮點數 * 速度，剛體的加速度的Y)
        // 動畫 的 設定布林值(參數名稱，水平 不等於 零時勾選)
        // != 不等於，傳回布林值
        ani.SetBool("RUN", h != 0);

        // KeyCode 列舉(下拉式選單) - 所有輸入的項目 滑鼠、鍵盤、搖桿
        if (Input.GetKeyDown(KeyCode.D))
        {
            // transform 此物件的變形元件
            // eulerAngles 歐拉角度 0 - 180 - 270 - 360...
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    /// <summary>
    /// 跳躍功能
    /// </summary>
    private void Jump()
    {
        // 如果 角色在地面上 並且 按下空白鍵 才能跳躍
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rig.AddForce(transform.up * jump);
            ani.SetBool("JUMP", true);
        }
        // 如果 物理 圓形範圍 碰到 圖層 8 的地板物件
        else if (Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y) + offset, radius, 1 << 8))
        {
            isGrounded = true;
        }
        // 沒有碰到地板物件
        else
        {
            isGrounded = false;                     // 不在地面上了
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ani.SetBool("JUMP", false);
        }
    }

    /// <summary>
    /// 開槍功能
    /// </summary>
    private void Fire()
    {
        // 按下左鍵之後
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ani.SetBool("SHOT", true);
            // 音源 的 播放一次音效(音效，隨機大小聲)
            aud.PlayOneShot(soundFire, Random.Range(0.8f, 1.5f));
            // 生成 子彈在槍口
            // 生成(物件，座標，角度)
            GameObject temp = Instantiate(bullet, point.position, point.rotation);
            // 讓子彈飛
            // 上 綠 transform.up
            // 右 紅 transform.right
            // 前 藍 transform.forward
            temp.GetComponent<Rigidbody2D>().AddForce(transform.right * speedBullet + transform.up * 100);
            
           
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ani.SetBool("SHOT", false);
        }
    }

    /// <summary>
    /// 死亡功能
    /// </summary>
    /// <param name="obj">碰到物件的名稱</param>
    public void Dead(string obj)
    {
        // 或者 ||
        // 如果 物件名稱 等於 死亡區域 或者 物件名稱 等於 敵人子彈
        // 等於 ==
        if (obj == "DIEE" || obj == "Eshoot(Clone)" || obj == "BOSS" || obj == "01")
        {
            ani.SetBool("DIE", true);

            this.enabled = true;                    // 此腳本 關閉
            //GetComponent<CapsuleCollider2D>().enabled = false;
            rig.Sleep();
            

            // 延遲呼叫("方法名稱"，延遲時間)
            // 靜態成員
            // 類別名稱.靜態成員 存取
            if (GameManager.live > 1) Invoke("Replay", 2f);
            if (GameManager.live == 0) enabled = false;
            // 呼叫 GM 處理玩家死亡
            gm.PlayerDead();
        }
    }

    /// <summary>
    /// 重新遊戲：重新載入當前的關卡
    /// </summary>
    private void Replay()
    {
        // 載入場景(當前場景 的 名稱)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 是否在傳送門裡面
    /// </summary>
    public bool inDoor;

    // 觸發事件：
    // 兩個碰撞物件有其中一個勾選 IsTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Door") inDoor = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Door") inDoor = false;
    }

    // OCE 碰撞時執行一次的事件
    // collision 碰到物件的資訊
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Dead(collision.gameObject.name);
    }


    // 繪製圖示：僅顯示魚場景面板
    private void OnDrawGizmos()
    {
        // 圖示 顏色
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        // 圖示 繪製球體(中心點，半徑)
        Gizmos.DrawSphere(new Vector2(transform.position.x, transform.position.y) + offset, radius);
    }
}