using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public float chaseRadius = 10f;
    Transform target;
    NavMeshAgent agent;
    public float attackRadius = 2f;
    float ultRadius = 8f;
    PlayerManager enemy;
    CharacterStats enemyStats;
    CharacterStats thisStats;
    private float nextBasic;
    private float nextQ;
    private float nextE;
    private float nextF;
    private float nextUlt;
    public event System.Action EnemyOnBasic;
    public event System.Action EnemyOnQ;
    public event System.Action EnemyOnDash;
    public event System.Action EnemyOnBlock;
    public event System.Action EnemyOnUlt;
    public bool fighting = false;
    public bool blocking = false;
    public float dashTime = 0.25f;
    public float dashSpeed = 20;
    public float ultTime = 0.75f;
    public float ultSpeed = 10;
    CharacterController control;
    float distance;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        thisStats = GetComponent<CharacterStats>();
        enemy = PlayerManager.instance;
        enemyStats = enemy.player.transform.gameObject.GetComponent<CharacterStats>();
        control = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        distance = Vector3.Distance(target.position, transform.position);

        if(distance <= chaseRadius)
        {
            agent.SetDestination(target.position);

            if(distance <= agent.stoppingDistance)
            {
                FaceTarget();
            }
        }
    }

    void FaceTarget() {
        Vector3 direction = (target.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime *5f);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

    void BasicAttack(CharacterStats targetStats){
        if(Time.time > nextBasic){
            StartCoroutine(DoDamage(targetStats, 0.2f, thisStats.basicDamage));
            nextBasic = Time.time + thisStats.basicCooldown;
            if(EnemyOnBasic != null)
            {
                EnemyOnBasic();
            }
        }
        StartCoroutine(FightDuration(0.5f));
    }

    void QAttack(CharacterStats targetStats){
        if(Time.time > nextQ){
            StartCoroutine(DoDamage(targetStats, 0.3f, thisStats.qDamage));
            nextQ = Time.time + thisStats.qCooldown;
            if(EnemyOnQ != null)
            {
                EnemyOnQ();
            }
        }
        StartCoroutine(FightDuration(0.7f));
    }

    void Dash(){
        if(Time.time > nextE){

            StartCoroutine(DashRout());
            
            nextE = Time.time + thisStats.eCooldown;
            if(EnemyOnDash != null)
            {
                EnemyOnDash();
            }
        }
        StartCoroutine(FightDuration(0.5f));
    }

    void Block(CharacterStats targetStats){
        if(Time.time > nextF){
            blocking = true;
            nextF = Time.time + thisStats.fCooldown;
            StartCoroutine(Blocking());
            if(EnemyOnBlock != null)
            {
                EnemyOnBlock();
            }
        }
        StartCoroutine(FightDuration(0.2f));
    }

    void UltAttack(CharacterStats targetStats){
        if(Time.time > nextUlt){
            StartCoroutine(DoDamage(targetStats, 0.8f, thisStats.ultDamage));
            nextUlt = Time.time + thisStats.ultCooldown;
            if(EnemyOnUlt != null)
            {
                EnemyOnUlt();
            }
            StartCoroutine(UltRout());
        }
        StartCoroutine(FightDuration(1.2f));
    }
    IEnumerator DoDamage (CharacterStats stats, float delay, int damage)
    {
        yield return new WaitForSeconds(delay);

        if(!enemy.player.transform.gameObject.GetComponent<Controller>().blocking){
            stats.TakeDamage(damage);
        }
        fighting = false;
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

    public void AgentAttack(int choice){
        if(distance <= attackRadius)
        {
            if (choice == 0 && !fighting)
            {
                fighting = true;
                BasicAttack(enemyStats);
            } else if(choice == 1  && !fighting)
            {
                QAttack(enemyStats);
                fighting = true;
            }else if(choice == 2 && !fighting)
            {
                
                Block(enemyStats);
            }

            if(choice == 3 && !fighting)
            {
                blocking = true;
                Dash();
            }else if(choice == 4  && !fighting)
            {
                fighting = true;
                UltAttack(enemyStats);
            }
        }
    }
}
