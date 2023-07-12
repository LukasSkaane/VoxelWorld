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

    private float cursorX=0,cursorY, cursorZ=0;
    private float prevCursorX, prevCursorZ;
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
        WorldController.rockHeightMap = generator.generateStartWorld(world.transform);
        cursorX = cursorZ = prevCursorX = prevCursorZ = WorldController.halfLength;
        cursorY = size;
        yOffset = size;
        running = true;
    }

    void Update() {
        
        prevCursorX = cursorX;
        
        prevCursorZ = cursorZ;
        takeInputs();

        cursorX = Math.Clamp(cursorX, 0, WorldController.worldSize);
        cursorZ = Math.Clamp(cursorZ, 0, WorldController.worldSize);
        prevCursorX = Math.Clamp(prevCursorX, 0, WorldController.worldSize);
        prevCursorZ = Math.Clamp(prevCursorZ, 0, WorldController.worldSize);
        Vector3 newPos = new(cursorX, cursorY,cursorZ);

        //if(cursorX!=prevCursorX || cursorZ != prevCursorZ)
        //    Debug.Log(velocity + ", " + cursorX + ", " + prevCursorX + ", " + cursorZ + ", " + prevCursorZ);
        if (Time.time >= cd_gravity) {
            cd_gravity = Time.time + gravityCooldown;
            cursorY--;
        }
        //Debug.Log(new Vector2(cursorX, cursorZ) + "," + new Vector2(prevCursorX, prevCursorZ));
        if (world.currentPillars.Count == 0) {
            cursorY = size + 4;
            newPos = new Vector3(newPos.x, cursorY, newPos.z);
            world.currentPillars = generator.dropPillarGroup(newPos, ref world.current);    
        }
        world.updatePillarGroup(newPos);
        
        world.flushFreezeBuffer();
    }

    private void FixedUpdate() {
        update = true;
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
            if (WorldController.drawLastCollision) {
                Gizmos.color = new Color(.1f, .3f, .7f, .45f);
                Gizmos.DrawCube(WorldController.collisionDebugCube.transform.position, Vector3.one*1.1f);
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