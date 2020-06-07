using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> objBFPrefabs;

    [SerializeField] private int probabInRow1 = 50,
        probabInRow2 = 35, probabInRow3 = 15; //вероятности для 1,2 или 3 обьектов подряд в %
    [SerializeField] private int probabCreateObj = 15; //вероятность того, что на растоянии 1 клетки от предыдущей точки появиться обьект (в %)
    [SerializeField] private int leftBoundOfRand = 1, rightBoundOfRand = 10;
    [SerializeField] private float leftBoundCameraView1stBF = -8; //левая граница видимости камеры для 1го поля битвы
    [SerializeField] private float xLengthCameraView = 16; //х длина видимости камеры
    [SerializeField] private float leftBoundPlayer = -6.5f; //также, только не видимость камеры, а движение игрока
    [SerializeField] private float xLengthBoundsPlayer = 13;
    [SerializeField] private float stepRand = 1;
    [SerializeField] private bool isOnline = true;

    private List<BoxCollider> objBFBoxColliders;
    private List<float> objBFAllowRotate;
    private int startLine = -6; //линия спавна - Y координата нижней границы линии 
    private int endLine = 5;

    /*int currLine;
    int indexOfCurrObj = 0, countObjInRow = 0;
    float xPointForSpawn = 0;
    bool isFirstObject = true;
    float rightBOund;
    bool isMapFull = false;
    float timer = 0; */

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (stepRand < 0.5) stepRand = 0.5f;

            FindRotAnglesAndSetColliders();
            int currLine = startLine;
        
            int indexOfCurrObj, countObjInRow = 0; //индекс текущего обьекта и колво обектов подряд
            float xPointForSpawn = 0; //крайняя правая X координата предыдущего обьекта

            bool isFirstObject = true; //первый ли обьект
            float rightBOund = leftBoundCameraView1stBF + xLengthCameraView;

            bool isMapFull = false;

            currLine = startLine;       
            rightBOund = leftBoundCameraView1stBF + xLengthCameraView;

            do
             {
                 indexOfCurrObj = Random.Range(0, objBFPrefabs.Count);
                 GameObject currGameObj = objBFPrefabs[indexOfCurrObj];
                 BoxCollider currBoxColl = objBFBoxColliders[indexOfCurrObj];
                 float rotateAngle = objBFAllowRotate[indexOfCurrObj];

                 int randVal = Random.Range(0, 100);
                 if (randVal < probabInRow1) countObjInRow = 1;
                 else if (randVal < probabInRow1 + probabInRow2) countObjInRow = 2;
                 else if (randVal < probabInRow1 + probabInRow2 + probabInRow3) countObjInRow = 3;

                 if (isFirstObject)
                 {
                     xPointForSpawn = Random.Range(-currBoxColl.size.x, 0) + leftBoundPlayer - stepRand;
                     isFirstObject = false;
                 }

                 bool isSpawnHere = false;
                /*do
                {
                    xPointForSpawn += stepRand;

                    float pointOfSpawnLastObj = currBoxColl.size.x * (countObjInRow - 1);
                    if (xPointForSpawn + pointOfSpawnLastObj > leftBoundPlayer + xLengthBoundsPlayer)
                    {
                        currLine++;
                        xPointForSpawn = Random.Range(-currBoxColl.size.x, 0) + leftBoundPlayer - stepRand;
                        if (currLine > endLine) isMapFull = true;
                    }

                    randVal = Random.Range(0, 100);
                    if (randVal < probabCreateObj)
                    {
                        isSpawnHere = true;
                    }
                } while (!isSpawnHere);*/

                int rand = Random.Range(leftBoundOfRand, rightBoundOfRand + 1);
                for (int i = 0; i < rand; i++)
                {
                    xPointForSpawn += stepRand;

                    float pointOfSpawnLastObj = currBoxColl.size.x * (countObjInRow - 1);
                    if (xPointForSpawn + pointOfSpawnLastObj > leftBoundPlayer + xLengthBoundsPlayer)
                    {
                        currLine++;
                        xPointForSpawn = Random.Range(-currBoxColl.size.x, 0) + leftBoundPlayer - stepRand;
                        if (currLine > endLine) isMapFull = true;
                    }              
                }

                 if (isMapFull) break;

                 xPointForSpawn -= Random.Range(0f, stepRand - 0.5f);
                 float zPointForSpawn = ((1 - currBoxColl.size.z) / 2) + currLine;
                 Vector3 position = new Vector3(xPointForSpawn, 0, zPointForSpawn);

                 for (int i = 0; i < countObjInRow; i++)
                 {
                     Quaternion rotation = Quaternion.Euler(Vector3.zero);
                     GameObject obj;
                     if (isOnline)
                     {
                         obj = PhotonNetwork.Instantiate(currGameObj.name, position, rotation, 0);
                     } else
                     {
                         obj = Instantiate(currGameObj, position, rotation);
                     }

                     float randRotAngle = Random.Range(-rotateAngle, rotateAngle);
                     Vector3 localRotatePoint = new Vector3(currBoxColl.center.x, 0, currBoxColl.center.z);
                     Vector3 rotatePoint = obj.transform.TransformPoint(localRotatePoint);
                     obj.transform.RotateAround(rotatePoint, Vector3.up, randRotAngle);

                     Vector3 leftBoundObjPoint;
                     float offsetZbounds;
                     Vector3 localTopLeftPointOfObj = new Vector3(0, 0, currBoxColl.size.z);
                     Vector3 topLeftPointOfObj = obj.transform.TransformPoint(localTopLeftPointOfObj);
                     bool isPlusRotate = randRotAngle < 0 ? false : true;

                     if (isPlusRotate)
                     {
                         leftBoundObjPoint = obj.transform.position;
                         offsetZbounds = (currLine + 1) - topLeftPointOfObj.z;
                     }
                     else
                     {
                         leftBoundObjPoint = topLeftPointOfObj;
                         offsetZbounds = obj.transform.position.z - currLine;
                     }

                     float offsetX = leftBoundObjPoint.x - position.x;
                     float offsetZ = Random.Range(-offsetZbounds, offsetZbounds);
                     obj.transform.Translate(offsetX, 0, offsetZ, Space.World);

                     if (isPlusRotate)
                     {
                         Vector3 localTopRightPointOfObj = new Vector3(currBoxColl.size.x, 0, currBoxColl.size.z);
                         Vector3 topRightPointOfObj = obj.transform.TransformPoint(localTopRightPointOfObj);
                         position.x = topRightPointOfObj.x;
                     }
                     else
                     {
                         Vector3 localDownRightPointOfObj = new Vector3(currBoxColl.size.x, 0, 0);
                         Vector3 downRightPointOfObj = obj.transform.TransformPoint(localDownRightPointOfObj);
                         position.x = downRightPointOfObj.x;
                     }
                 }

                 xPointForSpawn = position.x;

             } while (!isMapFull);
        }
         
    }

    private void FindRotAnglesAndSetColliders() //определяем максимальные углы поворота обьектов на линии + кешируем коллайдеры 
    {
        objBFAllowRotate = new List<float>();
        objBFBoxColliders = new List<BoxCollider>();

        foreach (GameObject objBF in objBFPrefabs)
        {
            BoxCollider boxCollider = objBF.GetComponent<BoxCollider>();
            objBFBoxColliders.Add(boxCollider);

            float distCenterToRUPoint = Vector2.Distance(new Vector2(0, 0),
                (new Vector2(boxCollider.center.x, boxCollider.center.z))); //от центра к правому верхнему углу

            float sinA = (boxCollider.center.z) / (distCenterToRUPoint);
            float angleA = (Mathf.Asin(sinA) / Mathf.PI) * 180f;

            float sinB = (0.5f / distCenterToRUPoint);
            float angleB = (Mathf.Asin(sinB) / Mathf.PI) * 180f;

            objBFAllowRotate.Add(angleA - angleB);
        }
    }

    private void Update()
    {
        /*timer += Time.deltaTime;
        if (!isMapFull && timer > 1) 
        {
            timer = 0;           
            indexOfCurrObj = Random.Range(0, objBFPrefabs.Count);
            GameObject currGameObj = objBFPrefabs[indexOfCurrObj];
            BoxCollider currBoxColl = objBFBoxColliders[indexOfCurrObj];
            float rotateAngle = objBFAllowRotate[indexOfCurrObj];

            int randVal = Random.Range(0, 100);
            Debug.Log("rand inRow: " + randVal);
            if (randVal < probabInRow1) countObjInRow = 1;
            else if (randVal < probabInRow1 + probabInRow2) countObjInRow = 2;
            else if (randVal < probabInRow1 + probabInRow2 + probabInRow3) countObjInRow = 3;

            if (isFirstObject)
            {
                xPointForSpawn = Random.Range(-currBoxColl.size.x, 0) + leftBoundPlayer - stepRand;
                isFirstObject = false;
            }

            bool isSpawnHere = false;
            do
            {
                xPointForSpawn += stepRand;

                float pointOfSpawnLastObj = currBoxColl.size.x * (countObjInRow - 1);
                if (xPointForSpawn + pointOfSpawnLastObj > leftBoundPlayer + xLengthBoundsPlayer)
                {
                    currLine++;
                    xPointForSpawn = Random.Range(-currBoxColl.size.x, 0) + leftBoundPlayer - stepRand;
                    if (currLine > endLine) isMapFull = true;
                }

                randVal = Random.Range(0, 100);
                if (randVal < probabCreateObj)
                {
                    isSpawnHere = true;
                }
            } while (!isSpawnHere);

            //if (isMapFull) break;

            xPointForSpawn -= Random.Range(0f, stepRand - 0.5f);
            float zPointForSpawn = ((1 - currBoxColl.size.z) / 2) + currLine;
            
            Vector3 position = new Vector3(xPointForSpawn, 0, zPointForSpawn);

            for (int i = 0; i < countObjInRow; i++)
            {
                Quaternion rotation = Quaternion.Euler(Vector3.zero);
                GameObject obj;
                if (isOnline)
                {
                    obj = PhotonNetwork.Instantiate(currGameObj.name, position, rotation, 0);
                }
                else
                {
                    obj = Instantiate(currGameObj, position, rotation);
                }

                float randRotAngle = Random.Range(-rotateAngle, rotateAngle);
                Debug.Log("rand rotAngle: " + randRotAngle);
                Vector3 localRotatePoint = new Vector3(currBoxColl.center.x, 0, currBoxColl.center.z);
                Vector3 rotatePoint = obj.transform.TransformPoint(localRotatePoint);
                obj.transform.RotateAround(rotatePoint, Vector3.up, randRotAngle);

                Vector3 leftBoundObjPoint;
                float offsetZbounds;
                Vector3 localTopLeftPointOfObj = new Vector3(0, 0, currBoxColl.size.z);
                Vector3 topLeftPointOfObj = obj.transform.TransformPoint(localTopLeftPointOfObj);
                bool isPlusRotate = randRotAngle < 0 ? false : true;

                if (isPlusRotate)
                {
                    leftBoundObjPoint = obj.transform.position;
                    offsetZbounds = (currLine + 1) - topLeftPointOfObj.z;
                }
                else
                {
                    leftBoundObjPoint = topLeftPointOfObj;
                    offsetZbounds = obj.transform.position.z - currLine;
                }

                float offsetX = leftBoundObjPoint.x - position.x;
                float offsetZ = Random.Range(-offsetZbounds, offsetZbounds);
                obj.transform.Translate(offsetX, 0, offsetZ, Space.World);

                if (isPlusRotate)
                {
                    Vector3 localTopRightPointOfObj = new Vector3(currBoxColl.size.x, 0, currBoxColl.size.z);
                    Vector3 topRightPointOfObj = obj.transform.TransformPoint(localTopRightPointOfObj);
                    position.x = topRightPointOfObj.x;
                }
                else
                {
                    Vector3 localDownRightPointOfObj = new Vector3(currBoxColl.size.x, 0, 0);
                    Vector3 downRightPointOfObj = obj.transform.TransformPoint(localDownRightPointOfObj);
                    position.x = downRightPointOfObj.x;
                }
            }

            xPointForSpawn = position.x;

        }
        */
    }
}
