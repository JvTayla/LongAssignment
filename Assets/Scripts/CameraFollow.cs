using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector3 offset;
    public Transform player;

    private KartController playerScript;

    public Vector3 origiCamPos;
    public Vector3 boostCamPos;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<KartController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
        transform.position = player.position + offset;

        if (!playerScript.GLIDER_FLY)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, 3 * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, player.eulerAngles.y, 0), 3 * Time.deltaTime);
        }
        
        if (playerScript.BoostTime > 0)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, boostCamPos, 3 * Time.deltaTime);
        }
        else
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, origiCamPos, 3 * Time.deltaTime);
        }
    }
}
