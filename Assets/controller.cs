using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class controller : MonoBehaviour {

    public float x = 0, y=0, z = 0;
    public static bool update = true;
    public float blockCooldown = 0.3f;
    public float moveCooldown = 0.075f;
    ElementGenerator generator;
    ElementBlock[]? pillars = new ElementBlock[4];
    private float cd_blockDrop;
    private float cd_move;
    List<Vector3> v3Line;

    private bool running=false;

    float m;
    // Start is called before the first frame update
    void Start() {
        generator = gameObject.GetComponent<ElementGenerator>();
        NodeRules.gridSize = transform.localScale/2;
        m = NodeRules.gridSize.x;
        y = m;
        v3Line = getSelectionLineVectors();
        
        cd_blockDrop = Time.time + blockCooldown;
        running = true;
    }

    void Update() {
        
        if (Input.GetKey(KeyCode.A) && Time.time >= cd_move) {
            x++;
            cd_move = Time.time + moveCooldown;
        }
        if (Input.GetKey(KeyCode.D) && Time.time >= cd_move) {
            x--;
            cd_move = Time.time + moveCooldown;
        }
        if (Input.GetKey(KeyCode.W) && Time.time >= cd_move) {
            z++;
            cd_move = Time.time + moveCooldown;
        }
        if (Input.GetKey(KeyCode.S) && Time.time >= cd_move) {
            z--;
            cd_move = Time.time + moveCooldown;
        }
        Math.Clamp(x, -m, m);
        Math.Clamp(y, -m, m);
        Math.Clamp(z, -m, m);

        
        
        if (cd_blockDrop == Mathf.Infinity || pillars == null) {
            generator.dropBlock2(new Vector3(x, y, z));

            pillars = pillars.getPillars(); 

            cd_blockDrop = Time.time + blockCooldown;
        }
        if (Time.time >= cd_blockDrop && ElementBlock.children.Count != 0)
            cd_blockDrop = pillars.refresh();

    }

    private void FixedUpdate() {
        update = true;
    }

    void generateNodes() {
        int arraySize = (int)(transform.lossyScale.x * transform.lossyScale.y * transform.lossyScale.z);

        Vector3 pos = new Vector3(0, 10, 0);
        Debug.Log(NodeRules.conditions["rock"](pos));

    }

    List<Vector3> getSelectionLineVectors() {
        int offset = 1;
        v3Line = new List<Vector3>();
        v3Line.Add(new Vector3(-m, y, z));
        v3Line.Add(new Vector3(m, y, z));
        v3Line.Add(new Vector3(-m, y, z + offset));
        v3Line.Add(new Vector3(m, y, z + offset));
        v3Line.Add(new Vector3(x, y, -m));
        v3Line.Add(new Vector3(x, y, m));
        v3Line.Add(new Vector3(x + offset, y, -m));
        v3Line.Add(new Vector3(x + offset, y, m));
        return v3Line;
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
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
}

struct Voxel {
    bool flushed;
    Vector3 pos;
    Color rgba;
    Voxel(Vector3 p, Color col) {
        flushed = false;
        pos = p;
        rgba = col;
    }
}
struct Mana {
    Vector3 pos;
    Color rgba;
    Mana(Vector3 p, Color col) {
        pos = p;
        rgba = col;
    }
}

public static class NodeRules {
    public static Vector3 gridSize;

    public static Dictionary<string, Predicate<Vector3>> conditions = new Dictionary<string, Predicate<Vector3>>(){
        {"air", pos => pos.y > (gridSize.y*0.6f) },
        {"rock", pos => pos.y < (gridSize.y*0.4f) }
    };
}

