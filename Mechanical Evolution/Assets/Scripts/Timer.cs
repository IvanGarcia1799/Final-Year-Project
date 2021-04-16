using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    float timer = 0.0f;
    public int minutes;
    public int seconds;
    EnemyManager enemy;
    PlayerManager player;
    CharacterStats playerStats;
    CharacterStats enemyStats;
    public bool count = true;
    // Start is called before the first frame update
    void Start()
    {
        enemy = EnemyManager.instance;
        enemyStats = enemy.enemy.transform.gameObject.GetComponent<CharacterStats>();
        player = PlayerManager.instance;
        playerStats = player.player.transform.gameObject.GetComponent<CharacterStats>();
        DontDestroyOnLoad (transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerStats.currentHealth != 0 && enemyStats.currentHealth != 0 && count){
        timer += Time.deltaTime;
        seconds = (int)timer % 60;
        minutes = (int)seconds % 60;
        }else{
            count = false;
        }
    }
}
