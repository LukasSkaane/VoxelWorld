using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour{
    public static float halfLength;
    public int pillarCount;
    static public int worldSize;
    static public int[] rockHeightMap;
    static public int[] waterHeightMap;
    static public int[] airHeightMap;
    public int frozenVoxelsBufferLength;
    public Dictionary<Vector2, Queue<Transform>> voxelPillars;
    public static int depth=1, width=1;
    public GameObject rock_plate;
    public GameObject current;
    public Pillar[] currentPillars;
    public Queue<Transform> bufferPillarTransforms;

    public void Activate(int worldSize) {
        WorldController.worldSize = worldSize;
        halfLength = worldSize / 2;
        rockHeightMap = generateStartWorld();
        //rockHeightMap = new int[worldSize * worldSize];
        voxelPillars = new();
        bufferPillarTransforms = new();
        currentPillars = new Pillar[0]; 
    }

    public void updatePillars(Vector3 cursorPos, ref Vector3 velocity) {
        current.transform.position += velocity;
        velocity = Vector3.zero;
        Queue<Pillar> newPillars = new();
        foreach (Pillar pillar in currentPillars) {
            Vector2 key = new(pillar.transform.position.x,
                              pillar.transform.position.z);
            cursorPos = avoidPillarCollision(cursorPos, new(key.x, cursorPos.y, key.y));

            int i = getPosIndex(key);
            if (rockHeightMap[i] >= cursorPos.y) {
                rockHeightMap[i] = pillar.pillarHeight + (int)cursorPos.y;
                //if (!voxelPillars.ContainsKey(key)) voxelPillars[key] = new Queue<Transform>();
                //pillar.getVoxelTransforms();
                pillar.transform.parent = transform;
                pillar.freeze();

            } else
                newPillars.Enqueue(pillar);
        }
        currentPillars = newPillars.ToArray();
        if (currentPillars.Length==0)
            Destroy(current);
    }

    public Vector3 avoidPillarCollision(Vector3 nextCursorPos, Vector3 pillarPos) {
        Vector3 dir = new Vector3(nextCursorPos.x+pillarPos.x, 0, nextCursorPos.z+pillarPos.z);
        
        //if (dir != Vector3.zero)
        //    Debug.Log(dir);
        if (getHeightMapOccupiedAt(pillarPos)) {
            pillarPos -= dir;
            //Debug.Log("Collision.");
        }
        return pillarPos;
    }

    public bool getHeightMapOccupiedAt(Vector3 pos) {
        int i = getPosIndex(pos);
        if (rockHeightMap[i] >= pos.y)
            return true;
        return false;
    }

    private int getPosIndex(Vector2 pos) { return (int)(pos.x + worldSize * pos.y); }
    private int getPosIndex(Vector3 pos) { return (int)(pos.x + worldSize * pos.z); }

    private int[] generateStartWorld() {
        int[] map = new int[worldSize*worldSize];
        for (int i = 0; i < map.Length;) {
            int x = (i%worldSize);
            int z = (i/worldSize);
            map[i] = 1;
            if (x % 4 == 0 && z % 4 == 0) 
                Instantiate(rock_plate, this.transform.position+new Vector3(x, 0 ,z), Quaternion.identity);
            i++;
        }
        return map;
    }
}