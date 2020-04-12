using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGController : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private GameObject player;
    public float scrollSpeed = 0.1f;

    
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0, 0, 40);

        Vector2 offset = new Vector2(player.transform.position.x * scrollSpeed, player.transform.position.y * scrollSpeed);

        meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
