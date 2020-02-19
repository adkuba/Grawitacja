using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGController : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private GameObject player;
    public float scrollSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 0, 10);

        Vector2 offset = new Vector2(player.transform.position.x * scrollSpeed, player.transform.position.y * scrollSpeed);

        meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
