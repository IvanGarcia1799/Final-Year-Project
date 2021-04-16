using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class CharacterStats : MonoBehaviour
{
    public SceneManager sceneManager;
    public int health = 100;
    public int basicDamage = 5;
    public int qDamage = 10;
    public int block = 20;
    public int ultDamage = 20;
    public float basicCooldown = 1f;
    public float qCooldown = 3f;
    public float eCooldown = 4f;
    public float fCooldown = 4f;
    public float ultCooldown = 20f;
    public int currentHealth;
    GameObject info;
    public int seconds = 0;
    void Awake () {
        currentHealth = health;
    }

    public void TakeDamage (int damage)
    {

        currentHealth -= damage;
        Debug.Log("Took " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            Debug.Log("Dead ");
        }
    }

    public virtual void Die() 
    {
        info = GameObject.Find("Info");
        seconds  = info.transform.gameObject.GetComponent<Timer>().seconds;
        Seconds high = new Seconds();
        high.score = seconds;
        System.IO.File.WriteAllText(Application.dataPath + "/High.json",JsonUtility.ToJson(high));
        if(gameObject.name == "Enemy"){
            Debug.Log("Enemy died");
            SceneManager.LoadScene(2);

        }else{
            SceneManager.LoadScene(3);
        }
    }

    public void ResetHealth(){
        currentHealth = 100;
    }

    private class Seconds{
        public int score;
    }
}
