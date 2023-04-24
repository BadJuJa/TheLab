using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionGenerator : MonoBehaviourSingleton<SectionGenerator>
{
    public float offset = 20;
    public float deletePoint = -20;

    public GameObject sectionPrefab;
    private List<GameObject> sections = new();

    [SerializeField]
    private float minSpeed = 5f;
    [SerializeField]
    private float maxSpeed = 20f;
    [SerializeField]
    private float speedIncrement = 1f;
    
    public float speed { get; private set; }

    public int maxSectionCount = 5;

    private void Start()
    {
        ResetLevel();

        GameManager.Instance.OnResetGame += ResetLevel;
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

        if (speed < maxSpeed)
            speed += speedIncrement * Time.deltaTime;
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
        speed = minSpeed;
        //SwipeManager.Instance.enabled = true;
        SwipeManager.Instance.canSendSwipe = true;
    }
    public void ResetLevel()
    {
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
        //SwipeManager.Instance.enabled = false;
        SwipeManager.Instance.canSendSwipe = false;
        MapGenerator.Instance.ResetMaps();
    }
}
