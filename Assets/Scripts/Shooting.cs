using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Shooting : MonoBehaviour
{
    [SerializeField] float speedBullet;
    [SerializeField] float cdBetweenShots;
    [SerializeField] float cdBetweenReload;
    [SerializeField] int countBulletsInMagaz;

    [SerializeField] bool isOnline = false;
    [SerializeField] bool isAutoShoot = false;

    PoolManager poolManager;
    PhotonView photonView;

    bool allowShoot; //флаг, доступен ли выстрел 
    int bulletsInMagazine;
    const string BULLETS_POOL_NAME = "Bullet";

    // Start is called before the first frame update
    void Start()
    {
        allowShoot = true;
        poolManager = PoolManager.instance;
        bulletsInMagazine = countBulletsInMagaz;
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if ((photonView.IsMine || !isOnline) && !isAutoShoot)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (allowShoot)
                {
                    if (--bulletsInMagazine > 0)
                    {                    
                        StartCoroutine(CoroutineShotCd());
                    }
                    else
                    {
                        StartCoroutine(CoroutineReloadCd());
                    }
                    Debug.Log("inMagazine: " + bulletsInMagazine);

                    if (isOnline)
                    {
                        photonView.RPC("RpcShoot", RpcTarget.All);
                    }
                    else
                    {
                        OfflineShoot();
                    }
                
                }
            }

        }
            if (isAutoShoot && allowShoot)
            {
                OfflineShoot();
                StartCoroutine(CoroutineShotCd());
            }
    }

    [PunRPC]
    public void RpcShoot()
    {
        Vector3 spawnBulletPos = transform.TransformPoint(new Vector3(0, 1, 0.5f));
        Quaternion spawnBulletRot = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90, 0, 0));
        //"new Vector3(90, 0, 0)" справедливо для тестового префаба пули, возможно для нормального варианта не потребуется
        GameObject Bullet = poolManager.GetObject(BULLETS_POOL_NAME, spawnBulletPos, spawnBulletRot);
        Debug.Log("spawn bullet from " + photonView.ViewID);
        Bullet.GetComponent<MovementBullet>().StartBullet(speedBullet);
    }

    void OfflineShoot()
    {
        Vector3 spawnBulletPos = transform.TransformPoint(new Vector3(0, 1, 0.5f));
        Quaternion spawnBulletRot = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(90, 0, 0));
        //"new Vector3(90, 0, 0)" справедливо для тестового префаба пули, возможно для нормального варианта не потребуется
        GameObject Bullet = poolManager.GetObject(BULLETS_POOL_NAME, spawnBulletPos, spawnBulletRot);
        Debug.Log("spawn bullet from " + photonView.ViewID);
        Bullet.GetComponent<MovementBullet>().StartBullet(speedBullet);
    }    

    IEnumerator CoroutineShotCd()
    {
        Debug.Log("shot off last for " + cdBetweenShots);
        allowShoot = false;
        yield return new WaitForSeconds(cdBetweenShots);
        allowShoot = true;
        Debug.Log("shot on");
    }

    IEnumerator CoroutineReloadCd()
    {
        Debug.Log("reload gun last for " + cdBetweenReload);
        allowShoot = false;
        yield return new WaitForSeconds(cdBetweenReload);
        allowShoot = true;
        bulletsInMagazine = countBulletsInMagaz;
        Debug.Log("reload complete");
    }
}
