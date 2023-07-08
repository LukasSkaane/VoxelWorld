using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Elements : int { ROCK = 0, WATER = 1, AIR = 2, FIRE = 4 };


public class ElementGenerator : MonoBehaviour {
    public GameObject parentInDeath;
    public float elementDropSpeed;
    public GameObject[] rocks;
    public GameObject[] waters;
    Dictionary<int, GameObject[]> elem_groups = new Dictionary<int, GameObject[]>();
    Dictionary<int, Dictionary<Vector2, GameObject[]>> elem_pillar_groups = new Dictionary<int, Dictionary<Vector2, GameObject[]>>();

    private int[] groupCounts = new int[5]; // Index 0-3 for Rock groups, index 4 for Water group
    GameObject current;

    // Start is called before the first frame update
    void Awake() {
        ElementBlock.heightMap = new int[(int)NodeRules.gridSize.x * 2, (int)NodeRules.gridSize.y * 2];
        ElementBlock.children = new Queue<GameObject>();
        ElementBlock.hitbox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(ElementBlock.hitbox.GetComponent<MeshRenderer>());

        elem_groups.Add((int)Elements.ROCK, rocks);
        elem_groups.Add((int)Elements.WATER, waters);
    }
    public ElementBlock dropBlock(Vector3 pos) {
        // Instantiate the chosen group
        int element = new();
        current = Instantiate(getRandomGroup(element), pos, Quaternion.identity);
        current.transform.parent = transform;
        ElementBlock block = current.AddComponent<ElementBlock>();
        block.Activate(elementDropSpeed, ref NodeRules.gridSize, ref parentInDeath, transform.childCount + 1);
        return block;
    }
    public void dropBlock2(Vector3 pos) {
        current = Instantiate(getRandomGroup(), pos, Quaternion.identity);
        current.transform.parent = transform;

        for (int i = 0; i <current.transform.childCount; i++) {
            current.AddComponent<ElementBlock2>().Activate(elementDropSpeed, ref parentInDeath, 4);
        }
    }

    ref GameObject getRandomGroup(ref int groupSize) {
        int element = Random.Range(0, elem_groups.Keys.Count);
        int index = Random.Range(0, elem_groups[element].Length);
        if(element == (int)Elements.ROCK)
        return ref elem_groups[element][index];
    }
}
public class ElementBlock2 : MonoBehaviour {
    static public int min = 0, max = 0;
    static public int[,] heightMap;
    static public GameObject hitbox;
    static public ElementBlock2[] pillars ;
    static private GameObject parent;
    private int x, y, z;
    private float dropSpeed;
    private Vector3 bounds;

    public void Activate(float dropSpeed, ref GameObject parent, int max, ref Vector3 pos) {
        this.dropSpeed = dropSpeed;
        ElementBlock2.parent = parent;
        ElementBlock2.max = max;
        this.x = (int)pos.x;
        this.y = (int)pos.y;
        this.z = (int)pos.z;

    }
    public static void updatePillars() {
        pillars = ElementBlock2.parent.GetComponentsInChildren<ElementBlock2>(false);
    }


    public float refresh() {
        transform.Translate(Vector3.down, Space.Self);

        Array.ForEach(pillars, (pillar) => {
            if (pillar.y - 1 <= heightMap[pillar.x,pillar.z]) {
                pillar.transform.parent = ElementBlock2.parent.transform;
                heightMap[pillar.x,pillar.z] = pillar.y;
            }
        });
        return Time.time + dropSpeed;
    }
    private void freeze() {
        if (transform.childCount == 0)
            Destroy(GetComponent<ElementBlock>());
    }
}

public class ElementBlock : MonoBehaviour {
    public int min = 0, max;
    static public int[,] heightMap;
    static public GameObject hitbox;
    static public Queue<GameObject> children;
    private float dropSpeed;
    private Vector3 bounds;
    private GameObject parent;

    public void Activate(float dropSpeed, ref Vector3 bounds, ref GameObject parent, int max) {
        this.dropSpeed = dropSpeed;
        this.bounds = bounds;
        this.parent = parent;
        this.max = max;
    }
    public static ElementBlock[] getPillars() {
        ElementBlock[] pillars = new ElementBlock[4];
        while (ElementBlock.min<max) {
            pillars[min] = transform.GetChild(min).gameObject.AddComponent<ElementBlock>();
            min++;
        }
        return pillars;
    }
}
