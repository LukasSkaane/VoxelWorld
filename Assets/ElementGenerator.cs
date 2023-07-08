using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public enum Elements : int { ROCK = 0, WATER = 1, AIR = 2, FIRE = 4 };


public class ElementGenerator : MonoBehaviour {
    public GameObject parentInDeath;
    public float voxelDropSpeed;
    public GameObject[] rocks;
    public GameObject[] waters;
    static public int[] heightMap;
    Dictionary<int, GameObject[]> elem_groups = new Dictionary<int, GameObject[]>();

    public GameObject current;
    public int pillarCount = 0;
    static public ElementBlock[] currentPillars;

    public void Activate() {
        ElementGenerator.currentPillars = new ElementBlock[0];
        heightMap = new int[(int)NodeRules.gridHalfLength.x * (int)NodeRules.gridHalfLength.y * 4];

        elem_groups.Add((int)Elements.ROCK, rocks);
        elem_groups.Add((int)Elements.WATER, waters);
    }
    public void dropPillarGroup(Vector3 pos) {
        current = Instantiate(getRandomGroup(), pos, Quaternion.identity);
        ElementBlock[] pillars = new ElementBlock[pillarCount];
        current.transform.parent = transform;

        for (int i = 0; i <pillarCount; i++) {
            pos = new Vector3(pos.x, NodeRules.gridHalfLength.y, pos.z);
            Transform child = current.transform.GetChild(i);
            ElementBlock eb = child.gameObject.AddComponent<ElementBlock>();
            eb.pillarHeight = (child.childCount>0) ? child.childCount : 1;
            pillars[i] = eb;
        }
        currentPillars = pillars;
    }
    ref GameObject getRandomGroup() {
        int element = Random.Range(0, elem_groups.Keys.Count);
        int index = Random.Range(0, elem_groups[element].Length);
        pillarCount = elem_groups[element][index].transform.childCount;
        return ref elem_groups[element][index];
    }
}
public class ElementBlock : MonoBehaviour {
    public int pillarHeight;
    public void freeze(GameObject newParent) {
        if (transform.name == "pillar") {
            foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
                t.parent = newParent.transform;
            }
        }else
            transform.parent = newParent.transform;
        Destroy(this.gameObject);
    }
}
