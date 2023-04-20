using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionGenerator : MonoBehaviour
{
    public bool isRunning { get; private set; }

    public float offset = 10;
    public float deletePoint = -15;

    public GameObject sectionPrefab;
    private List<GameObject> sections = new();
    public float maxSpeed = 10f;
    private float speed = 0;

    public int maxSectionCount = 5;

    public static SectionGenerator instance;
    void Awake() { instance = this; }

    private void Start()
    {
        ResetLevel();
        //StartLevel();
    }

    private void Update()
    {
        if (speed == 0) return;

        foreach (GameObject section in sections)
        {
            section.transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
        }

        if (sections[0].transform.position.z < deletePoint)
        {
            Destroy(sections[0]);
            sections.RemoveAt(0);

            CreateNextSection();
        }
    }

    public void CreateNextSection()
    {
        Vector3 pos = Vector3.zero;
        if (sections.Count > 0) { pos = sections[sections.Count - 1].transform.position + new Vector3(0, 0, offset); }
        GameObject go = Instantiate(sectionPrefab, pos, Quaternion.identity);
        go.transform.SetParent(transform);
        sections.Add(go);
        
    }


    public void StartLevel()
    {
        isRunning = true;
        speed = maxSpeed;
        SwipeManager.instance.enabled = true;
    }
    public void ResetLevel()
    {
        isRunning = false;
        speed = 0;
        while(sections.Count > 0)
        {
            Destroy(sections[0]);
            sections.RemoveAt(0);
        }

        for (int i = 0; i < maxSectionCount; i++)
        {
            CreateNextSection();
        }
        SwipeManager.instance.enabled = false;
    }
}
