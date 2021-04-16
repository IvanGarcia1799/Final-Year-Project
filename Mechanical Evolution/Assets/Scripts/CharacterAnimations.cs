using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimations : MonoBehaviour
{
    const float smoothTime = .1f;
    NavMeshAgent agent;
    NavMeshAgent enemyAgent;
    protected Animator animator;
    public Controller player;
    public EnemyController enemy;
    EnemyManager enemyMan;
    PlayerManager playerMan;
    protected Animator enemyAnimator;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        enemyMan = EnemyManager.instance;
        playerMan = PlayerManager.instance;
        agent = playerMan.player.transform.gameObject.GetComponent<NavMeshAgent>();
        enemyAgent = enemyMan.enemy.transform.gameObject.GetComponent<NavMeshAgent>();
        animator = playerMan.player.transform.gameObject.GetComponentInChildren<Animator>();
        enemyAnimator = enemyMan.enemy.transform.gameObject.GetComponentInChildren<Animator>();
        player = playerMan.player.transform.gameObject.GetComponent<Controller>();
        enemy = enemyMan.enemy.transform.gameObject.GetComponent<EnemyController>();
        player.OnBasic += OnBasic;
        player.OnQ += OnQ;
        player.OnDash += OnDash;
        player.OnBlock += OnBlock;
        player.OnUlt += OnUlt;
        enemy.EnemyOnBasic += OnBasicEnemy;
        enemy.EnemyOnQ += OnQEnemy;
        enemy.EnemyOnDash += OnDashEnemy;
        enemy.EnemyOnBlock += OnBlockEnemy;
        enemy.EnemyOnUlt += OnUltEnemy;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, smoothTime, Time.deltaTime);
        float enemySpeedPercent = enemyAgent.velocity.magnitude / enemyAgent.speed;
        enemyAnimator.SetFloat("speedPercent", enemySpeedPercent, smoothTime, Time.deltaTime);
    }
    protected virtual void OnBasic()
    {
        animator.SetTrigger("basic");
    }

    protected virtual void OnQ()
    {
        animator.SetTrigger("q");
    }
    protected virtual void OnDash()
    {
        animator.SetTrigger("dash");
    }
    protected virtual void OnBlock()
    {
        animator.SetTrigger("block");
    }
    protected virtual void OnUlt()
    {
        animator.SetTrigger("ult");
    }
    protected virtual void OnBasicEnemy()
    {
        enemyAnimator.SetTrigger("basic");
    }

    protected virtual void OnQEnemy()
    {
        enemyAnimator.SetTrigger("q");
    }
    protected virtual void OnDashEnemy()
    {
        enemyAnimator.SetTrigger("dash");
    }
    protected virtual void OnBlockEnemy()
    {
        enemyAnimator.SetTrigger("block");
    }
    protected virtual void OnUltEnemy()
    {
        enemyAnimator.SetTrigger("ult");
    }
}
