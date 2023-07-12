using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public enum Elements : int { ROCK = 0, WATER = 1, AIR = 2, FIRE = 4 };


 public class Generator : MonoBehaviour {

    Dictionary<int, GameObject[]> elem_groups;
    public GameObject[] rocks;
    public GameObject rock_plate;
    public GameObject[] waters;
    static public Pillar[] currentPillars;

    public void Activate() {
        Generator.currentPillars = new Pillar[0];
        elem_groups = new Dictionary<int, GameObject[]>();
        elem_groups.Add((int)Elements.ROCK, rocks);
        elem_groups.Add((int)Elements.WATER, waters);
    }

    public int[] generateStartWorld(Transform transformParent) {
        int[] map = new int[WorldController.worldSize * WorldController.worldSize];
        GameObject obj;
        for (int i = 0; i < map.Length;) {
            int x = (i % WorldController.worldSize);
            int z = (i / WorldController.worldSize);
            if (x % 4 == 0 && z % 4 == 0) {
                obj = Instantiate(rock_plate, this.transform.position + new Vector3(x, 0, z), Quaternion.identity);
                obj.transform.parent = transformParent;
            }
            i++;
        }
        return map;
    }

    public List<Pillar> dropPillarGroup(Vector3 pos, ref GameObject current) {
        int pillarCount = new();
        current = Instantiate(getRandomGroup(ref pillarCount), pos, Quaternion.identity);
        List<Pillar> pillars = new();
        current.transform.parent = transform;
        

        for (int i = 0; i <pillarCount; i++) {
            pillars.Add(current.transform.GetChild(i).gameObject.AddComponent<Pillar>());

            if (WorldController.depth < pillars[i].transform.localPosition.z+1)
                WorldController.depth = (int)pillars[i].transform.localPosition.z+1;
            if (WorldController.width < pillars[i].transform.localPosition.x+1)
                WorldController.width = (int)pillars[i].transform.localPosition.x+1;

            pillars[i].pillarHeight = (pillars[i].transform.childCount>0) ? pillars[i].transform.childCount : 1;
            //Debug.Log(child.name + " - " + eb.pillarHeight);
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
