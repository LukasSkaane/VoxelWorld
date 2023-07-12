using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainController : MonoBehaviour {

    public WorldController world;
    public bool showHeightMap = false;
    public int size = 16;
    public float xOffset,yOffset,zOffset;
    public float blockDropCooldown = 0.3f;
    public float gravityCooldown = 0.1f;
    public float moveCooldown = 0.075f;
    public Vector3 cursor;

    Vector3 velocity;
    private float cursorX=0,cursorY, cursorZ=0;
    private bool running = false;
    private bool update = false;
    private float cd_gravity;
    private float cd_move;
    private List<Vector3> v3Line;
    private Generator generator;

    void Start() {
        generator = gameObject.GetComponent<Generator>();
        
        world.Activate(size);
        generator.Activate();
        cursorX = cursorZ = 0;
        cursorY = size;
        yOffset = size;
        running = true;
    }

    void Update() {
        float prevCursorX = cursorX, prevCursorZ = cursorZ;
        takeInputs();

        cursorX = Math.Clamp(cursorX, -1, 1);
        cursorZ = Math.Clamp(cursorZ, -1, 1);
        Vector3 newPos = new(cursorX, cursorY, cursorZ);
        if (velocity == Vector3.zero && (cursorX != 0 || cursorZ != 0))
            velocity = new(cursorX, -1, cursorZ);
        //if (!world.getHeightMapOccupiedAt(newPos)) {
        //    cursorX = prevCursorX; 
        //    cursorZ = prevCursorZ;
        //}            

        Debug.Log(velocity);
        if (Time.time >= cd_gravity) {
            cd_gravity = Time.time + gravityCooldown;
            cursorY--;
            if (world.currentPillars.Length == 0) {
                cursorY = size + 4;
                newPos = new Vector3(newPos.x, cursorY, newPos.z);
                world.currentPillars = generator.dropPillarGroup(newPos, ref world.current);
            }
            world.updatePillars(newPos, ref velocity);
            newPos = Vector3.zero;
        }
    }

    private void FixedUpdate() {
        update = true;
        while (world.frozenVoxelsBufferLength > 0) {
            world.frozenVoxelsBufferLength--;

        }
    }

    private void takeInputs() {
        if (Time.time >= cd_move) {
            cd_move = Time.time + moveCooldown;
            if (Input.GetKey(KeyCode.A)) cursorX--;
            if (Input.GetKey(KeyCode.D)) cursorX++;
            if (Input.GetKey(KeyCode.W)) cursorZ++;
            if (Input.GetKey(KeyCode.S)) cursorZ--;
        }
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(Vector3.one *WorldController.halfLength - ((Vector3.one - Vector3.up)/2), (Vector3.one*size)-Vector3.up);
        if (update) {
            Gizmos.color = new Color(.65f, .65f, .65f, .9f);
            v3Line = getSelectionLineVectors();
            update = false;
        }
        if (running) {
            Gizmos.DrawLine(v3Line[0], new Vector3(v3Line[0].x+size, v3Line[0].y, v3Line[0].z));
            Gizmos.DrawLine(v3Line[1], new Vector3(v3Line[1].x+size, v3Line[1].y, v3Line[1].z));
            Gizmos.DrawLine(v3Line[2], new Vector3(v3Line[2].x, v3Line[2].y, v3Line[2].z+size));
            Gizmos.DrawLine(v3Line[3], new Vector3(v3Line[3].x, v3Line[3].y, v3Line[3].z+size));
            Gizmos.color = new Color(.65f, .65f, .65f, .35f);
            Gizmos.DrawCube(new Vector3(cursorX, WorldController.halfLength, cursorZ),
                                        (Vector3.up*(size))+ (Vector3.one-Vector3.up)*0.45f);
            Gizmos.color = new Color(.65f, .65f, .65f, .9f);
            if (showHeightMap) {
                Gizmos.color = new Color(.9f, .1f, .1f, .45f);
                int xH=0, zH=0;
                for (int i = 0; i < WorldController.rockHeightMap.Length; i++) {
                    if (WorldController.rockHeightMap[i] != 0) {
                        xH = i % WorldController.worldSize;
                        zH = i / WorldController.worldSize;
                        Gizmos.DrawCube(new Vector3(xH, WorldController.rockHeightMap[i]-1, zH), Vector3.one * 1.1f);
                    }
                }

                Gizmos.color = new Color(.65f, .65f, .65f, .9f);
            }
        }
    }

    public List<Vector3> getSelectionLineVectors() {
        int offset = 1;
        int w = WorldController.depth, h = WorldController.width;

        v3Line = new List<Vector3>();
        v3Line.Add(new Vector3(0, yOffset, cursorZ - offset) + (Vector3.one - 2 * Vector3.up) / 2);
        v3Line.Add(new Vector3(0, yOffset, cursorZ - offset + h) + (Vector3.one - 2 * Vector3.up) / 2);
        v3Line.Add(new Vector3(cursorX - offset, yOffset, 0) + (Vector3.one - 2 * Vector3.up) / 2);
        v3Line.Add(new Vector3(cursorX - offset + w, yOffset, 0) + (Vector3.one - 2 * Vector3.up) / 2);
        return v3Line;
    }


}