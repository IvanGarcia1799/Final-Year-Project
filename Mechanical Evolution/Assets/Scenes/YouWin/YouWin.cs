using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class YouWin : MonoBehaviour
{
    public int minutes;
    GameObject info;
    public TMP_Text win;
    Text winText;
    Timer timer;
    public int score;
    bool setText = false;
    // Start is called before the first frame update
    void Start()
    {
        string json = File.ReadAllText(Application.dataPath + "/High.json");
        Debug.Log((json));
        Seconds seconds = JsonUtility.FromJson<Seconds>(json);
        score = seconds.score;
        Debug.Log((score));
        transform.gameObject.GetComponent<TMP_Text>().text = "You took " + score + " seconds. Try beat your time next time!";
    }

    // Update is called once per frame
    void Update()
    {
    }

    private class Seconds{
        public int score;
    }
}
