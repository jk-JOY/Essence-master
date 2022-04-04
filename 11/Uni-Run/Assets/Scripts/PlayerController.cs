using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour
{ 
    public AudioClip deathClip; //사망시 사용할 오디오 클립
    public float jumpForce = 700f; //점프 힘

    private int jumpCount = 0;  //플레이어가 점프한 횟수
    private bool isGrounded = false;  
    private bool isDead = false;

    private Rigidbody2D playerRigidbody;
    private Animator animator;
    private AudioSource playerAudio;

    private void Start()
    {
        // 초기화
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
    }

    private void Update()
    { 
        // 사용자 입력을 감지하고 점프하는 처리
        if (isDead)
        {
            // 사망시 더이상 처리를 하지 않고 종료
            return;
        }
        //마우스 왼쪽 버튼을 눌렀고, 최대 점프 횟수 2에 도달하지 않았다면
        if (Input.GetMouseButtonDown(0) && jumpCount < 2)
        {
            //점프 카운트 증가
            jumpCount++;
            playerRigidbody.velocity = Vector2.zero; //점프직전에 속도를 순간적으로 제로0,0으로 변경
            playerRigidbody.AddForce(new Vector2(0, jumpForce)); //리지드 바디에 위쪾으로 힘주기(점프포스, 700만큼)
            playerAudio.Play();
        }

        else if (Input.GetMouseButtonUp(0) && playerRigidbody.velocity.y > 0) // 마우스 왼쪽 버튼에서 떼는순간 그리고 속도의 y값이 양수라면(위로 상승하고 있는 state)
            //현재속도를 절반으로 변경
            playerRigidbody.velocity *= 0.5f;

        animator.SetBool("Grounded", isGrounded);
    }

    private void Die()
    {//애니매이터의 값과, 소리를 변경해주고
        animator.SetTrigger("Die");
        playerAudio.clip = deathClip;
        playerAudio.Play();

        //속력값을 제로로 만든뒤 Bool 값을 isdead로 변경. 
        playerRigidbody.velocity = Vector2.zero;
        isDead = true;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Dead" && !isDead)
        {//Die함수를 따로 작성한뒤 Ontrigger에서 함수만 실행
            //충돌한 상대방의 태그가 Dead이며 아직 사망하지 않았다면 Die()함수실행
            Die();
        }
        // 트리거 콜라이더를 가진 장애물과의 충돌을 감지

    }

    // 바닥에 닿았음을 감지하는 처리
    private void OnCollisionEnter2D(Collision2D collision)
    {//어떤 콜라이더와 닿았으며, 충돌 표면이 위쪽을 바라보고 있다면
        if (collision.contacts[0].normal.y > 0.7f)
        {//isGrounded 를 true로 변경하고 누적 점프횟수를 0으로 리셋 ㅡ 
            isGrounded = true;
            jumpCount = 0;
        }
    }

    // 바닥에서 벗어났음을 감지하는 처리
    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }   
}