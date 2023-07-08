using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Controller : MonoBehaviour {

    public int size = 16;
    float cursorX,cursorZ;
    public float x,y,z;
    public float blockDropCooldown = 0.3f;
    public float gravityCooldown = 0.1f;
    public float moveCooldown = 0.075f;
    ElementGenerator generator;
    private float cd_gravity;
    private float cd_move;

    private bool running = false;
    private bool update = false;
    List<Vector3> v3Line;

    public VoxelPillar voxelPillars
    int frozenVoxels = 0;
    float m;
    // Start is called before the first frame update
    void Start() {
        NodeRules.gridHalfLength = Vector3.one*size/2;
        y = m = size;
        cursorX = m/2; 
        cursorZ = m/2;
        generator = gameObject.GetComponent<ElementGenerator>();
        generator.Activate();
        running = true;
    }

    void Update() {
        if(Time.time >= cd_move) {
            if      (Input.GetKey(KeyCode.A)) cursorX++;
            else if (Input.GetKey(KeyCode.D)) cursorX--;
            else if (Input.GetKey(KeyCode.W)) cursorZ++;
            else if (Input.GetKey(KeyCode.S)) cursorZ--;
            cd_move = Time.time + moveCooldown;
        }
        cursorX = Math.Clamp(cursorX, 0, size);
        cursorZ = Math.Clamp(cursorZ, 0, size);

        if (ElementGenerator.currentPillars.Length == 0) {
            Destroy(generator.current.gameObject);
            generator.dropPillarGroup(new Vector3(cursorX-NodeRules.gridHalfLength.x+.5f, y, cursorZ - NodeRules.gridHalfLength.x + .5f));
            cd_gravity = Time.time + gravityCooldown;
        } 
        else if (Time.time >= cd_gravity) {
            Queue<ElementBlock> newPillars = new Queue<ElementBlock>();
            foreach(ElementBlock pillar in ElementGenerator.currentPillars) {
                int i = (int)(pillar.transform.position.x+cursorX + size * pillar.transform.position.z + cursorZ);
                if (pillar.transform.position.y <= ElementGenerator.heightMap[i]) {
                    ElementGenerator.heightMap[i] = pillar.pillarHeight + (int)pillar.transform.position.y;
                    var voxels = pillar.GetComponentsInChildren<Transform>();
                    pillar.freeze(generator.parentInDeath);

                    frozenVoxels += pillar.pillarHeight;
                } else
                    newPillars.Enqueue(pillar);
            }
            ElementGenerator.currentPillars = newPillars.ToArray();
            generator.current.transform.Translate(Vector3.down, Space.Self);
            cd_gravity = Time.time + gravityCooldown;
        }
    }

    private void FixedUpdate() {
        update = true;
        while (frozenVoxels > 0) {
            frozenVoxels--;

        }
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, Vector3.one*size);
        if (update) {
            Gizmos.color = new Color(.65f, .65f, .65f, .9f);
            v3Line = getSelectionLineVectors();
            update = false;
        }
        if (running) {
            Gizmos.DrawLine(v3Line[0], v3Line[1]);
            Gizmos.DrawLine(v3Line[2], v3Line[3]);
            Gizmos.DrawLine(v3Line[4], v3Line[5]);
            Gizmos.DrawLine(v3Line[6], v3Line[7]);
        }
    }

    public List<Vector3> getSelectionLineVectors() {
        int offset = 1,halfLen=(int)NodeRules.gridHalfLength.x;

        v3Line = new List<Vector3>();

        v3Line.Add(new Vector3(-halfLen, y, cursorZ-halfLen));
        v3Line.Add(new Vector3(halfLen, y, cursorZ-halfLen));
        v3Line.Add(new Vector3(-halfLen, y, cursorZ-halfLen + offset));
        v3Line.Add(new Vector3(halfLen, y, cursorZ-halfLen + offset));
        v3Line.Add(new Vector3(cursorX-halfLen, y, -halfLen));
        v3Line.Add(new Vector3(cursorX-halfLen, y, halfLen));
        v3Line.Add(new Vector3(cursorX-halfLen + offset, y, -halfLen));
        v3Line.Add(new Vector3(cursorX-halfLen + offset, y, halfLen));
        return v3Line;
    }

}

public static class NodeRules {
    public static Vector3 gridHalfLength;

    public static Dictionary<string, Predicate<Vector3>> conditions = new Dictionary<string, Predicate<Vector3>>(){
        {"air", pos => pos.y > (gridHalfLength.y*0.6f) },
        {"rock", pos => pos.y < (gridHalfLength.y*0.4f) }
    };
}

struct VoxelPillar {
    public string material;
    Queue<Dictionary<int, Tuple<int, int>>> voxelVectors;
    Transform[] voxels;
}