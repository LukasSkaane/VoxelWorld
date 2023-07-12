using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class WorldController : MonoBehaviour{
    enum CollisionConsts { NO_COLLISION, HORI_COLLISION, VERT_COLLISION };
    public static float halfLength;
    public int pillarCount;
    static public int worldSize;
    static public int[] rockHeightMap;
    static public int[] waterHeightMap;
    static public int[] airHeightMap;
    static public bool drawLastCollision;
    static public GameObject collisionDebugCube;
    public int frozenVoxelsBufferLength;
    public Dictionary<Vector2, Queue<Transform>> voxelPillars;
    public static int depth=1, width=1;
    public GameObject current;
    public List < Pillar > currentPillars;
    public Queue<GameObject> pillarFreezeBuffer;

    public void Activate(int worldSize) {
        WorldController.worldSize = worldSize;
        halfLength = worldSize / 2;
        collisionDebugCube = new GameObject();
        //rockHeightMap = new int[worldSize * worldSize];
        voxelPillars = new();
        pillarFreezeBuffer = new();
        currentPillars = new();
        current = new();
        current.transform.position = new(halfLength, worldSize, halfLength);
    }
    public void updatePillarGroup(Vector3 nextPos) {
        Vector3 velocity = current.transform.position-nextPos;
        Vector3 nextPillarPos;
        int collision;
        Vector3[] pillarPositions = new Vector3[width*depth];

        for(int i=0; i < pillarPositions.Length; i++) {
            collision = evalPillarTrajectoryCollision(currentPillars[i].transform.position, ref velocity);
            nextPillarPos = currentPillars[i].transform.position - velocity;
            //if (collision != (int)CollisionConsts.NO_COLLISION) Debug.Log((collision > 1) ? "Bottom hit, freezing pillar at " : "Wall collided with by pillar at " +
            //                                                              nextPillarPos);
            int h = getHeightmapAt(nextPillarPos);
            if(collision == (int)CollisionConsts.VERT_COLLISION) {
                rockHeightMap[h] = currentPillars[i].pillarHeight + (int)currentPillars[i].transform.position.y -((currentPillars[i].pillarHeight!=1)?0:1);
                pillarFreezeBuffer.Enqueue(currentPillars[i].gameObject);
                currentPillars.RemoveAt(i);
            }
        }
        current.transform.position -= velocity;
        if (currentPillars.Count == 0) {
            Debug.Log("Destroying Current");
            Destroy(current.gameObject);
        }
    }


    public int evalPillarTrajectoryCollision(Vector3 pillarPos, ref Vector3 velocity) {
        Vector3 ogPos = pillarPos;
        pillarPos -= velocity;
        if (pillarPos.y > rockHeightMap[getHeightmapAt(pillarPos)]) 
            return (int)CollisionConsts.NO_COLLISION;
        
        pillarPos = ogPos;
        velocity = Vector3.up;
        pillarPos -= Vector3.up;
        if (pillarPos.y > rockHeightMap[getHeightmapAt(pillarPos)]) 
            return (int)CollisionConsts.HORI_COLLISION;
        velocity = Vector3.zero;
        return (int)CollisionConsts.VERT_COLLISION;
    }

    public bool getHeightMapOccupiedAt(Vector3 pos) {
        int i = getHeightmapAt(pos);
        if (rockHeightMap[i] >= pos.y)
            return true;
        return false;
    }

    public void flushFreezeBuffer() {
        for (; pillarFreezeBuffer.Count > 0;) {
            Debug.Log(pillarFreezeBuffer.Count);
            var current = pillarFreezeBuffer.Dequeue();
            current.GetComponent<Pillar>().freeze(transform);
            Destroy(current);
        }
    }

    private int getPosIndex(Vector2 pos) { return (int)(pos.x + worldSize * pos.y); }
    private int getHeightmapAt(Vector3 pos) { return (int)(pos.x + worldSize * pos.z); }

    
}