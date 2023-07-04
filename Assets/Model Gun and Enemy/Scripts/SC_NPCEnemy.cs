using UnityEngine;
using UnityEngine.AI;


public class SC_NPCEnemy : MonoBehaviour, IEntity
{
    public float npcHP = 100;
    public float knockAwayForce = 20f;

    [HideInInspector]
    public SC_EnemySpawner es;

    // Start is called before the first frame update
    void Start()
    {
        //Set Rigidbody to Kinematic to prevent hit register bug
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void ApplyDamage(float points)
    {
        npcHP -= points;
        if(npcHP <= 0)
        {
            //Destroy the NPC
            es.EnemyEliminated(this);
            Destroy(gameObject);
        }
    }
}