using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class MovementCharacters : MonoBehaviour
{
    CharacterController charController;

    [SerializeField] float moveSpeed;
    [SerializeField] float gravity;
    [SerializeField] private float moveBound;
    public float _moveBound { get { return moveBound; } }
    [SerializeField] float lengthDash;
    [SerializeField] float speedDash;
    [SerializeField] float delayBtwDash;
    [SerializeField] bool isOnline = true;

    GameObject Camera;
    bool allowDash; //флаг, доступен ли даш 
    int dashForward; //если 1 то вправо, -1 влево
    float dashedDist; //какая дистанция уже пройдена дашем
    private bool _inverse = false;
    public bool inverse { get { return _inverse; } }
    private float movementX;
    public float _movementX { get { return movementX; } } //движение в данный момент (нужно для playerDie)

    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        allowDash = true;
        dashForward = 0;
        dashedDist = 0;
        if (transform.rotation.eulerAngles.y == 180) _inverse = true;
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (photonView.IsMine || !isOnline)
        {
            if (allowDash)
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
    }

    void MovePers(float deltaX)
    {
        if (_inverse) deltaX = -deltaX;
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

        movementX = movement.x;
        charController.Move(movement);

    }

    IEnumerator CoroutineDash()
    {
        Debug.Log("dash off last for " + delayBtwDash);
        allowDash = false;
        yield return new WaitForSeconds(delayBtwDash);
        allowDash = true;
        Debug.Log("dash on");
    }
}
