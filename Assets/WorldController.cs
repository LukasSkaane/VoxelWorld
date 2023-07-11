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
    public GameObject current;
    public Pillar[] currentPillars;
    public Queue<Transform> bufferPillarTransforms;

    public void Activate(int worldSize) {
        WorldController.worldSize = worldSize;
        halfLength = worldSize / 2;
        rockHeightMap = new int[worldSize * worldSize];
        voxelPillars = new();
        bufferPillarTransforms = new();
        currentPillars = new Pillar[0]; 
    }
    private void Update() {
        
    }

    public void updatePillars(Vector2 cursorPos, float height) {
        Queue<Pillar> newPillars = new Queue<Pillar>();
        for (int i = 0; i < currentPillars.Length; i++) {
            updatePillar(ref currentPillars[i]);
        }
        currentPillars = newPillars.ToArray();
        current.transform.position = new Vector3(cursorPos.x, height, cursorPos.y);
        if (currentPillars.Length==0)
            Destroy(current);

        void updatePillar(ref Pillar pillar) {
            Vector2 key = new(cursorPos.x + pillar.transform.localPosition.x,
                              cursorPos.y + pillar.transform.localPosition.z);

            int i = (int)(key.x + worldSize * key.y);
            //Debug.Log("Position: " + key.x + "," + height + "," + key.y + "   ---   hm:" + rockHeightMap[(int)(key.x + worldSize*key.y)]);
            if (rockHeightMap[i] >= height) {
                //Debug.Log("freezing pillar at" + key + " with height" + height + ", pillarHeight:" + pillar.pillarHeight);
                rockHeightMap[i] = pillar.pillarHeight + (int)height;
                if (!voxelPillars.ContainsKey(key)) voxelPillars[key] = new Queue<Transform>();
                pillar.getVoxelTransforms();
                pillar.transform.parent = transform;
                pillar.freeze();

            } else
                newPillars.Enqueue(pillar);
        }
    }

    public static void getHeightMapXY(int x,int y, int index) {
        
    }
}