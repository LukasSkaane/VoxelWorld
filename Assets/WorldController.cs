using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour{
    public static float halfLength;
    public int pillarCount;
    public int worldSize;
    static public int[] rockHeightMap;
    static public int[] waterHeightMap;
    static public int[] airHeightMap;
    public int frozenVoxelsBufferLength;
    public Dictionary<Vector2, Queue<Transform>> voxelPillars;
    public GameObject current;
    public Pillar[] currentPillars;

    public void Activate(int worldSize) {
        halfLength = worldSize/2;
        this.worldSize = worldSize;
        rockHeightMap = new int[(int)(worldSize * worldSize)];
        voxelPillars = new Dictionary<Vector2, Queue<Transform>>();
        currentPillars = new Pillar[0]; 
    }

    public void updatePillars(Vector2 cursorPos, float h) {
        Queue<Pillar> newPillars = new Queue<Pillar>();
        foreach (Pillar pillar in currentPillars) {
            Vector2 key = new(cursorPos.x + pillar.transform.position.x,
                              cursorPos.y + pillar.transform.position.z);

            int i = (int)(key.x +  worldSize * key.y);
            if (h <= rockHeightMap[i]) {
                rockHeightMap[i] = pillar.pillarHeight + (int)h;
                Debug.Log(key +""+ rockHeightMap[i]);
                if (!voxelPillars.ContainsKey(key)) voxelPillars[key] = new Queue<Transform>();
                voxelPillars[key].CopyTo(pillar.getVoxelTransforms(), 0);
                pillar.freeze(transform.gameObject);
            } else
                newPillars.Enqueue(pillar);
            
        }
        currentPillars = newPillars.ToArray();
        if(currentPillars.Length==0)
            Destroy(current);
        else 
            current.transform.position = new Vector3(cursorPos.x, h, cursorPos.y);
    }
}