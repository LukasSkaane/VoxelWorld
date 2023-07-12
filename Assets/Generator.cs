using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public enum Elements : int { ROCK = 0, WATER = 1, AIR = 2, FIRE = 4 };


 public class Generator : MonoBehaviour {

    Dictionary<int, GameObject[]> elem_groups;
    public GameObject[] rocks;
    public GameObject[] waters;
    static public Pillar[] currentPillars;

    public void Activate() {
        Generator.currentPillars = new Pillar[0];
        elem_groups = new Dictionary<int, GameObject[]>();
        elem_groups.Add((int)Elements.ROCK, rocks);
        elem_groups.Add((int)Elements.WATER, waters);
    }
    public List<Pillar> dropPillarGroup(Vector3 pos, ref GameObject current) {
        int pillarCount = new();
        current = Instantiate(getRandomGroup(ref pillarCount), pos, Quaternion.identity);
        List<Pillar> pillars = new();
        current.transform.parent = transform;

        for (int i = 0; i <pillarCount; i++) {
            Transform child = current.transform.GetChild(i);
            Pillar eb = child.gameObject.AddComponent<Pillar>();

            if (WorldController.depth < child.localPosition.z+1)
                WorldController.depth = (int)child.localPosition.z+1;
            if (WorldController.width < child.localPosition.x+1)
                WorldController.width = (int)child.localPosition.x+1;

            eb.pillarHeight = (child.childCount>0) ? child.childCount : 1;
            //Debug.Log(child.name + " - " + eb.pillarHeight);
            pillars[i] = eb;
        }
        return pillars;
    }
    ref GameObject getRandomGroup(ref int pillarCount) {
        int element = Random.Range(0, elem_groups.Keys.Count);
        int index = Random.Range(0, elem_groups[element].Length);
        pillarCount = elem_groups[element][index].transform.childCount;
        return ref elem_groups[element][index];
    }
}

public class Pillar : MonoBehaviour {
    public int pillarHeight;
    public void freeze(Transform parent) {
        if (transform.name == "pillar") {
            foreach (Transform obj in transform.gameObject.GetComponentsInChildren<Transform>())
                obj.parent = parent;
        } else
            transform.parent = parent;
    }
    public Transform[] getVoxelTransforms() {
        return (transform.name != "pillar") ? new Transform[1] { transform } : transform.GetComponentsInChildren<Transform>();
    }
}
