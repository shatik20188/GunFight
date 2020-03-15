using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCharacters : MonoBehaviour
{
    CharacterController charController;

    [SerializeField] float moveSpeed;
    [SerializeField] float gravity;
    [SerializeField] float moveBound;
    [SerializeField] float lengthDash;
    [SerializeField] float speedDash;
    [SerializeField] float delayBtwDash;

    bool isCanDash;
    int dashForward; //если 1 то вправо, -1 влево
    float dashedDist; //какая дистанция уже пройдена дашем

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        isCanDash = true;
        dashForward = 0;
        dashedDist = 0;
    }

    // Update is called once per frame
    void Update()
    {        
        if (isCanDash)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                dashForward = -1;
                StartCoroutine(CoroutineDash());
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                dashForward = 1;
                StartCoroutine(CoroutineDash());
            }
        }

        if (dashForward == 0)
        {
            MovePers(Input.GetAxis("Horizontal") * moveSpeed);
        }
        else
        {
            if (dashedDist < lengthDash)
            {
                MovePers(speedDash * dashForward);
                dashedDist += (speedDash * Time.deltaTime);
            }
            else
            {
                dashedDist = 0;
                dashForward = 0;
            }
        }

    }

    void MovePers(float deltaX)
    {
        Vector3 movement = new Vector3(deltaX, gravity, 0);
        movement *= Time.deltaTime;

        float newPosition = transform.position.x + movement.x;
        if (newPosition >= moveBound)
        {
            movement.x -= (newPosition - moveBound);
        }
        else if (newPosition <= -moveBound)
        {
            movement.x -= (newPosition + moveBound);
        }

        charController.Move(movement);
    }

    IEnumerator CoroutineDash()
    {
        Debug.Log("dash off");
        isCanDash = false;
        yield return new WaitForSeconds(delayBtwDash);
        isCanDash = true;
        Debug.Log("dash on");
    }
}
