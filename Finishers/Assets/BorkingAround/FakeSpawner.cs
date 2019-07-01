using UnityEngine;

public class FakeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnGroup;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Instantiate(spawnGroup, transform.position, transform.rotation);
        }
    }
}