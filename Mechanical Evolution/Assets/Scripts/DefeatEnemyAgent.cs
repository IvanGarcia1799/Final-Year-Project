using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DefeatEnemyAgent : Agent
{
    [SerializeField] private Transform targetTransform;

    public int currentHealth;
    public int tempHealth;
    public int enemyTemp;
    public int enemyHealth;
    public bool fighting;
    public bool blocking;
    public bool blockingEnemy;
    public bool fightingEnemy;
    public float healthDiff;
    public float healthLost;
    public GameObject info;
    public Timer timer;
    public int seconds;
    public float finalReward;

    public override void Initialize()
    {
        info = GameObject.Find("Info");
        timer = info.transform.gameObject.GetComponent<Timer>();
        seconds = timer.seconds;
    }
    public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation((transform.gameObject.GetComponent<CharacterStats>().currentHealth)/100f);
        sensor.AddObservation(transform.gameObject.GetComponent<EnemyController>().fighting);
        sensor.AddObservation(transform.gameObject.GetComponent<EnemyController>().blocking);
        sensor.AddObservation((targetTransform.gameObject.GetComponent<CharacterStats>().currentHealth)/100f);
        sensor.AddObservation(targetTransform.gameObject.GetComponent<Controller>().fighting);
        sensor.AddObservation(targetTransform.gameObject.GetComponent<Controller>().blocking);
        sensor.AddObservation(seconds);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        int action = actions.DiscreteActions[0];
        Debug.Log(actions.DiscreteActions[0]);
        tempHealth = transform.gameObject.GetComponent<CharacterStats>().currentHealth;
        enemyTemp = targetTransform.gameObject.GetComponent<CharacterStats>().currentHealth;

        transform.gameObject.GetComponent<EnemyController>().AgentAttack(action);
        seconds = timer.seconds;

        currentHealth = transform.gameObject.GetComponent<CharacterStats>().currentHealth;
        enemyHealth = targetTransform.gameObject.GetComponent<CharacterStats>().currentHealth;
        fighting = transform.gameObject.GetComponent<EnemyController>().fighting;
        blocking = transform.gameObject.GetComponent<EnemyController>().blocking;
        fightingEnemy = targetTransform.gameObject.GetComponent<Controller>().fighting;
        blockingEnemy = targetTransform.gameObject.GetComponent<Controller>().blocking;
        healthDiff = enemyTemp - enemyHealth;
        healthLost = tempHealth - currentHealth;

        if(seconds != 0)
        {
            finalReward = 1000f/(float)seconds;
        }

        if(currentHealth <= 0){
            SetReward(-finalReward);
            EndEpisode();
        }
        if(enemyHealth <= 0){
            SetReward(+finalReward);
            EndEpisode();
        }
        if(currentHealth == tempHealth){
            AddReward(+0f);
        }
        if((currentHealth == tempHealth) && blocking  && fightingEnemy){
            AddReward(+0.3f);
        }
        if(enemyHealth <= enemyTemp)
        {
            AddReward(+(healthDiff*0.001f));
        }
        if(currentHealth <= tempHealth)
        {
            AddReward(-(healthLost*0.0001f));
        }
        if(enemyHealth == enemyTemp && blockingEnemy && fighting){
            AddReward(-0.5f);
        }
        if(fighting){
            AddReward(+0.3f);
        }

    }
    public override void OnEpisodeBegin() {
        transform.gameObject.GetComponent<CharacterStats>().ResetHealth();
        targetTransform.gameObject.GetComponent<CharacterStats>().ResetHealth();
        transform.localPosition = new Vector3(0, 1.49f, 5.39f);
        targetTransform.localPosition = new Vector3(0, 2.35f, 1.2f);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
    }
}
