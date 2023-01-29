using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemysKilled : MonoBehaviour
{
    public dooropen door;
    public NpcHealth[] enemysToBeKilled;
    private int killedEnemys;

    void Start()
    {
        killedEnemys = 0;
    }


    // Update is called once per frame
    void Update()
    {
        killedEnemys = 0;
        foreach (NpcHealth enemy in enemysToBeKilled)
        {
            if (enemy.health <= 0f)
            {
                killedEnemys++;
            }
        }

        if (killedEnemys >= enemysToBeKilled.Length)
        {
            door.DoorOpen = true;
        }
    }
}
