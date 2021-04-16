using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Controller : MonoBehaviour
{
    public float dashTime = 0.25f;
    public float dashSpeed = 20;
    public float ultTime = 0.75f;
    public float ultSpeed = 10;
    public float walkSpeed = 6;
    public float runSpeed = 12;
    public float scale = 50;
    public float rotTime = 0.1f;
    public float gravity = -12;
    public float rotSpeed;
    Animator animator;
    Transform follower;
    float characterRotation;
    CharacterController control;
    float fallVelocity;
    Camera cam;
    public bool lockOn = false;
    Transform target;
    float distance;
    float attackRadius =  2f;
    float ultRadius = 8f;
    EnemyManager enemy;
    CharacterStats enemyStats;
    CharacterStats thisStats;
    private float nextBasic;
    private float nextQ;
    private float nextE;
    private float nextF;
    private float nextUlt;
    public event System.Action OnBasic;
    public event System.Action OnQ;
    public event System.Action OnDash;
    public event System.Action OnBlock;
    public event System.Action OnUlt;
    public bool fighting = false;
    public bool blocking = false;
    // Start is called before the first frame update
    void Start()
    {
        thisStats = GetComponent<CharacterStats>();
        enemy = EnemyManager.instance;
        enemyStats = enemy.enemy.transform.gameObject.GetComponent<CharacterStats>();
        animator = GetComponentInChildren<Animator>();
        follower = Camera.main.transform;
        control = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y == -2)
        {
            SceneManager.LoadScene(3);
        }
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputNor = input.normalized;

        Move(inputNor);

    }

    void Move(Vector2 inputNor)
    {
        
        bool walking = Input.GetKey(KeyCode.W);
        bool running = Input.GetKey(KeyCode.LeftShift);
        bool backing = Input.GetKey(KeyCode.S);

        if (inputNor != Vector2.zero && !backing) {
			characterRotation = Mathf.Atan2 (inputNor.x, inputNor.y) * Mathf.Rad2Deg + follower.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, characterRotation, ref rotSpeed, rotTime);
		}
        
        float speed  = ((walking) ? .88f : 0f)*inputNor.magnitude;
        speed  = ((running) ? runSpeed : walkSpeed)*inputNor.magnitude;

        float animationSpeed =  ((running) ? 1.5f : .66f)*inputNor.magnitude;
        animator.SetFloat("speedPercent", animationSpeed, 0.1f, Time.deltaTime);

        fallVelocity += Time.deltaTime*gravity;
        if(control.isGrounded) {
            fallVelocity = 0;
        }

        if(!backing) {
            Vector3 velocity = transform.forward*speed + Vector3.up*fallVelocity;
            control.Move(velocity*Time.deltaTime);
        }

        if(backing)
        {
            rotSpeed = 100;
            animationSpeed = 1;
            speed = 4;
            animator.SetFloat("speedPercent", animationSpeed, 0.0f, Time.deltaTime);

            // Move backward
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            speed = speed * Input.GetAxis("Vertical");
            control.SimpleMove(forward * speed);

            float r = Input.GetAxis("Horizontal");
            transform.Rotate(0, r * rotSpeed * Time.deltaTime, 0);

        }

        if (Input.GetMouseButtonDown(0) && !lockOn)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.name == "Enemy")
                {
                    target = hit.transform;
                    // enemy = hit.transform.gameObject;
                    lockOn = true;
                    Debug.Log("We hit" + hit.collider.name + " at " + hit.point);
                }
            }
        }

        if (lockOn)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            Vector3 direction = (target.position - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime *5f);

            if (distance <= attackRadius)
            {
                if (Input.GetMouseButtonDown(0) && !fighting)
                {
                    fighting = true;
                    BasicAttack(enemyStats);
                } else if(Input.GetKey(KeyCode.Q)  && !fighting)
                {
                    fighting = true;
                    QAttack(enemyStats); 
                }else if(Input.GetKey(KeyCode.F) && !fighting)
                {
                    Block(enemyStats);
                }
            }

            if(Input.GetKey(KeyCode.E) && !fighting)
            {
                blocking = true;
                Dash();
            }else if(Input.GetKey(KeyCode.R)  && !fighting)
            {
                fighting = true;
                UltAttack(enemyStats);
            }

            if((Input.GetKey(KeyCode.Space) && distance > attackRadius) || !target.transform){
                lockOn = false;
            }
        }

    }

    void BasicAttack(CharacterStats targetStats){
        if(Time.time > nextBasic){
            StartCoroutine(DoDamage(targetStats, 0.4f, thisStats.basicDamage));
            nextBasic = Time.time + thisStats.basicCooldown;
            if(OnBasic != null)
            {
                OnBasic();
            }
        }
        StartCoroutine(FightDuration(0.4f));
    }

    void QAttack(CharacterStats targetStats){
        if(Time.time > nextQ){
            StartCoroutine(DoDamage(targetStats, 0.7f, thisStats.qDamage));
            nextQ = Time.time + thisStats.qCooldown;
            if(OnQ != null)
            {
                OnQ();
            }
        }
        StartCoroutine(FightDuration(0.7f));
    }

    void Dash(){
        if(Time.time > nextE){

            StartCoroutine(DashRout());
            
            nextE = Time.time + thisStats.eCooldown;
            if(OnDash != null)
            {
                OnDash();
            }
        }
        StartCoroutine(FightDuration(0.5f));
    }

    void Block(CharacterStats targetStats){
        if(Time.time > nextF){
            nextF = Time.time + thisStats.fCooldown;
            blocking = true;
            if(OnBlock != null)
            {
                OnBlock();
            }
             StartCoroutine(Blocking());
        }
        StartCoroutine(FightDuration(0.2f));
    }

    void UltAttack(CharacterStats targetStats){
        if(Time.time > nextUlt){
            StartCoroutine(DoDamage(targetStats, 1.5f, thisStats.ultDamage));
            nextUlt = Time.time + thisStats.ultCooldown;
            if(OnUlt != null)
            {
                OnUlt();
            }
            StartCoroutine(UltRout());
        }
        StartCoroutine(FightDuration(1f));
    }
    IEnumerator DoDamage (CharacterStats stats, float delay, int damage)
    {
        yield return new WaitForSeconds(delay);

        stats.TakeDamage(damage);
    }

    IEnumerator DashRout(){
        float startTime = Time.time;
        while(Time.time < startTime + dashTime){
            control.Move(transform.forward * dashSpeed * Time.deltaTime);

            yield return null;
        }
    }
    IEnumerator UltRout(){
        float ultStartTime = Time.time;
        while(Time.time < ultStartTime + ultTime){
            control.Move(transform.forward * ultSpeed * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator Blocking(){
        yield return new WaitForSeconds(1);

        blocking = false;
    }
    IEnumerator FightDuration( float delay)
    {
        yield return new WaitForSeconds(delay);

        fighting = false;
    }

}
