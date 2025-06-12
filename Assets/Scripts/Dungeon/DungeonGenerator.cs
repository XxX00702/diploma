using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private GameObject[] _corridor;
    [SerializeField] private GameObject _spawnRoom;
    [SerializeField] private GameObject[] _bossRoom;
    [Space(20)] 
    [SerializeField] private GameObject[] _genericRooms;
    [SerializeField] private GameObject[] _treasureRooms;

    [Header("Generate configuration")] 
    [SerializeField, Min(3)] private int _floorCount = 3;
    [SerializeField, Min(3)] private int _maxRoomsCount = 5;
    [SerializeField, Min(2)] private int _minRoomsCount = 6;
    [SerializeField, Min(1)] private int _maxTreasureRooms = 2;
    [SerializeField] private UnityEvent _onDungeonExit;
    private Queue<GameObject> _roomsForGeneration = new Queue<GameObject>();
    private List<GameObject> _generatedRooms = new List<GameObject>();
    private List<GameObject> _dungeonRooms = new List<GameObject>();

    private Player _player;
    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        GenerateDungeon();
    }

    public void RegenerateDungeon()
    {
        if (_floorCount <= 0)
        {
            _onDungeonExit?.Invoke();
            return;
        }
        _roomsForGeneration.Clear();
        _generatedRooms.Clear();
        for (int i = 0; i < _dungeonRooms.Count; i++)
        {
            Destroy(_dungeonRooms[i].gameObject);
        }
        _dungeonRooms.Clear();
        GenerateDungeon();
        var player = GameObject.FindGameObjectWithTag("Player");
    }

    public void GenerateDungeon()
    {
        _player.transform.position = Vector2.zero;
        _player.GetComponent<Collider2D>().enabled = false;
        Debug.Log("Generating dungeon");
        _floorCount--;
        SetupSpawnRoom();
        int genericRoomsCount = Random.Range(_minRoomsCount, _maxRoomsCount + 1);
        while (genericRoomsCount > 0)
        {
            if (genericRoomsCount > 1)
            {
                GenerateRoomRoots(_roomsForGeneration.Peek());
                _roomsForGeneration.Dequeue();
            }
            else
            {
                GenerateRoomRoots(_roomsForGeneration.Peek(), true);
                _roomsForGeneration.Dequeue();
            }
            _player.GetComponent<Collider2D>().enabled = true;
            genericRoomsCount--;
        }
        Debug.Log("Dungeon generated. Remaining floors - " + _floorCount);
    }

    private void SetupSpawnRoom()
    { 
        var spawnRoom = Instantiate(_spawnRoom, Vector2.zero, Quaternion.identity);
        _dungeonRooms.Add(spawnRoom);
        GenerateRoomRoots(spawnRoom);
    }

    private bool _isRoomGenerated;

    private void GenerateRoomRoots(GameObject originRoom, bool generateBossRoom = false)
    {
        List<GameObject> corridors = GenerateCorridors(originRoom);
        foreach (var corridor in corridors)
        {
            _isRoomGenerated = false;
            if (generateBossRoom)
            {
                while (_isRoomGenerated == false)
                {
                    GenerateRoomFromCorridor(corridor, GetRandomRoom(_bossRoom));
                }
            }
            else
            {
                float treasureRoomChance = Random.value;
                bool isTreasureRoom = false;
                if (_maxTreasureRooms > 0 && treasureRoomChance < 0.25f)
                {
                    isTreasureRoom = true;
                    _maxTreasureRooms--;
                }
                while (_isRoomGenerated == false)
                {
                    if (isTreasureRoom)
                    {
                        GenerateRoomFromCorridor(corridor, GetRandomRoom(_treasureRooms));
                    }
                    else
                    {
                        GenerateRoomFromCorridor(corridor, GetRandomRoom(_genericRooms));
                    }
                }
            }
        }
    }

    private GameObject GetRandomRoom(GameObject[] rooms)
    {
        return rooms[Random.Range(0, rooms.Length)];
    }

    private void AlignRoomPosition(Door doorPoint, Room generatedRoom, int index)
    {
        Vector2 position = doorPoint.transform.position + 
                           (generatedRoom.Doors[index].transform.position * (-1));
        generatedRoom.transform.position = position;
        doorPoint.IsAvailable = false;
        generatedRoom.Doors[index].IsAvailable = false;
    }

    private bool HasNeededDoor(DoorDirection neededDoorDirection, Room room)
    {
        foreach (var door in room.Doors)
        {
            if (door._doorDirection == neededDoorDirection) return true;
        }
        return false;
    }

    private void GenerateRoomFromCorridor(GameObject originCorridor, GameObject roomToGenerate)
    {
        var originalCorridor = originCorridor.GetComponent<Room>();
        Door availableDoor = null;
        int i;
        for (i = 0; i < originalCorridor.Doors.Length; i++)
        {
            if (originalCorridor.Doors[i].IsAvailable)
            {
                availableDoor = originalCorridor.Doors[i];
                break;
            }
        }
        if(availableDoor == null) return;
        DoorDirection neededNextDoorDirection = availableDoor.GetOppositeDirection();
        var generatedRoom = Instantiate(roomToGenerate, Vector2.zero, Quaternion.identity).GetComponent<Room>();
        for (int j = 0; j < generatedRoom.Doors.Length; j++)
        {
            if (generatedRoom.Doors[j]._doorDirection == neededNextDoorDirection)
            {
                AlignRoomPosition(availableDoor, generatedRoom, j);
                generatedRoom.Doors[j].IsAvailable = false;
                originalCorridor.Doors[i].IsAvailable = false;
                _isRoomGenerated = true;

                _roomsForGeneration.Enqueue(generatedRoom.gameObject);
                _dungeonRooms.Add(generatedRoom.gameObject);
                _generatedRooms.Add(generatedRoom.gameObject);
                return;
            }
        }
        _isRoomGenerated = false;
        Destroy(generatedRoom.gameObject);
    }
    
    private List<GameObject> GenerateCorridors(GameObject originRoom)
    {
        List<GameObject> generatedCorridors = new List<GameObject>();
        var originalRoom = originRoom.GetComponent<Room>();
        for (int i = 0; i < originalRoom.Doors.Length; i++)
        {
            if(originalRoom.Doors[i].IsAvailable == false) continue;
            DoorDirection neededNextDoorDirection = originalRoom.Doors[i].GetOppositeDirection();
            var randomCorridor = GetRandomRoom(_corridor).GetComponent<Room>();
            while (HasNeededDoor(neededNextDoorDirection, randomCorridor) == false)
            {
                randomCorridor = GetRandomRoom(_corridor).GetComponent<Room>();
            }

            var generatedCorridor = Instantiate(randomCorridor, Vector2.zero, Quaternion.identity).GetComponent<Room>();
            for (int j = 0; j < generatedCorridor.Doors.Length; j++)
            {
                if (generatedCorridor.Doors[j]._doorDirection == neededNextDoorDirection)
                {
                    AlignRoomPosition(originalRoom.Doors[i], generatedCorridor, j);
                    generatedCorridor.Doors[j].IsAvailable = false;
                    originalRoom.Doors[i].IsAvailable = false;
                    generatedCorridors.Add(generatedCorridor.gameObject);
                    _dungeonRooms.Add(generatedCorridor.gameObject);
                }
                else
                {
                    Destroy(generatedCorridor);
                }
            }
        }

        return generatedCorridors;
    }
}
