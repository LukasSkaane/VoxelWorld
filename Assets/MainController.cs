using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainController : MonoBehaviour {

    public WorldController world;
    public int size = 16;
    public float x,y,z;
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
        world.Activate(size);

        cursorX = cursorZ = 0;
        cursorY = size;
        y = size;
        generator = gameObject.GetComponent<Generator>();
        generator.Activate();
        running = true;
    }

    void Update() {
        takeInputs();
        cursorX = Math.Clamp(cursorX, 0, size-1);
        cursorZ = Math.Clamp(cursorZ, 0, size-1);

        if (Time.time >= cd_gravity) {
            cursorY--;

            if (world.currentPillars.Length == 0) {
                cursorY = size;
                world.currentPillars = generator.dropPillarGroup(new Vector3(cursorX, cursorY, cursorZ), ref world.current);
            }
            world.updatePillars(new Vector2(cursorX,cursorZ), cursorY);
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
            if (Input.GetKey(KeyCode.A)) cursorX--;
            if (Input.GetKey(KeyCode.D)) cursorX++;
            if (Input.GetKey(KeyCode.W)) cursorZ++;
            if (Input.GetKey(KeyCode.S)) cursorZ--;
            cd_move = Time.time + moveCooldown;
        }
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, Vector3.one*size-Vector3.one);
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

        }
    }

    public List<Vector3> getSelectionLineVectors() {
        int offset = 1;

        v3Line = new List<Vector3>();
        v3Line.Add(new Vector3(-WorldController.halfLength, y - offset, cursorZ - WorldController.halfLength-offset)+Vector3.one/2);
        v3Line.Add(new Vector3(-WorldController.halfLength, y - offset, cursorZ - WorldController.halfLength) + Vector3.one / 2);
        v3Line.Add(new Vector3(cursorX - WorldController.halfLength-offset, y - offset, -WorldController.halfLength) + Vector3.one / 2);
        v3Line.Add(new Vector3(cursorX - WorldController.halfLength, y - offset, -WorldController.halfLength) +Vector3.one/2);
        return v3Line;
    }


}