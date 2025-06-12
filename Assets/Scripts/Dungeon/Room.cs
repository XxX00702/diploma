using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour
{
    public Door[] Doors;

    public Transform GetRandomCorridorPosition()
    {
        return Doors[Random.Range(0, Doors.Length-1)].transform;
    }
}
