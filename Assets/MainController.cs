using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainController : MonoBehaviour {

    public WorldController world;
    public bool showHeightMap = false;
    public int size = 16;
    public float xOffset,yOffset,zOffset;
    public float cursorX, cursorY, cursorZ;
    public float blockDropCooldown = 0.3f;
    public float gravityCooldown = 0.1f;
    public float moveCooldown = 0.075f;

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
        takeInputs();
        cursorX = Math.Clamp(cursorX, 1, size - 2);
        cursorZ = Math.Clamp(cursorZ, 1, size - 2);

        if (Time.time >= cd_gravity) {
            cursorY--;
            if (world.currentPillars.Length == 0) 
                world.currentPillars = generator.dropPillarGroup(new Vector3(cursorX, cursorY = size+6, cursorZ), ref world.current);
            world.updatePillars(new Vector2(cursorX, cursorZ), cursorY);
            cd_gravity = Time.time + gravityCooldown;
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
        Gizmos.DrawWireCube(transform.position+(Vector3.one-Vector3.up)*WorldController.halfLength, (Vector3.one*size)-Vector3.one);
        if (update) {
            Gizmos.color = new Color(.65f, .65f, .65f, .9f);
            v3Line = getSelectionLineVectors();
            update = false;
        }
        if (running) {
            float lineLength = WorldController.halfLength*2;
            Gizmos.DrawLine(v3Line[0], new Vector3(v3Line[0].x+lineLength, v3Line[0].y, v3Line[0].z));
            Gizmos.DrawLine(v3Line[1], new Vector3(v3Line[1].x+lineLength, v3Line[1].y, v3Line[1].z));
            Gizmos.DrawLine(v3Line[2], new Vector3(v3Line[2].x, v3Line[2].y, v3Line[2].z+lineLength));
            Gizmos.DrawLine(v3Line[3], new Vector3(v3Line[3].x, v3Line[3].y, v3Line[3].z+lineLength));
            Gizmos.color = new Color(.65f, .65f, .65f, .3f);
            Gizmos.DrawCube(new Vector3(cursorX- WorldController.halfLength, WorldController.halfLength,cursorZ- WorldController.halfLength),
                                        (Vector3.up*size)+Vector3.one*0.45f);
            Gizmos.color = new Color(.65f, .65f, .65f, .9f);
            if (showHeightMap) {
                Gizmos.color = new Color(.9f, .1f, .1f, .75f);
                int xH=0, zH=0;
                for (int i = 0; i < WorldController.rockHeightMap.Length; i++) {
                    if (WorldController.rockHeightMap[i] != 0) {
                        xH = i % WorldController.worldSize;
                        zH = i / WorldController.worldSize;
                        Gizmos.DrawCube(new Vector3(xH, WorldController.rockHeightMap[i], zH), Vector3.one * 1.1f);
                    }
                }

                Gizmos.color = new Color(.65f, .65f, .65f, .9f);
            }
        }
    }

    public List<Vector3> getSelectionLineVectors() {
        int offset = 1;

        v3Line = new List<Vector3>();
        v3Line.Add(new Vector3(0, yOffset-.5f, cursorZ + offset) + (Vector3.one - Vector3.up) / 2);
        v3Line.Add(new Vector3(0, yOffset - .5f, cursorZ) + (Vector3.one-Vector3.up)/ 2);
        v3Line.Add(new Vector3(cursorX+offset, yOffset - .5f, 0) + (Vector3.one-Vector3.up)/ 2);
        v3Line.Add(new Vector3(cursorX, yOffset - .5f, 0) + (Vector3.one-Vector3.up)/ 2);
        return v3Line;
    }


}