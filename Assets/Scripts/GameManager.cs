using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] GameObject car;
    [SerializeField] GameObject ghostCar;
    [SerializeField] GameObject ball;

    RLInput inputActions;
    Vector3 orientation; public static float angle;

    public List<TideView> tideViews = new List<TideView>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inputActions = new RLInput();
        inputActions.Enable();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        tideViews = new List<TideView>();
    }

    void Update()
    {
        orientation = Input.acceleration;
        float y = Mathf.Min(orientation.y, orientation.z);

        var t = Mathf.Atan2(y, orientation.x) * Mathf.Rad2Deg + 90;
        if (t > -2 && t < 2) t = 0;
        angle = Mathf.Lerp(angle, t, 10 * Time.deltaTime);
    }

    void OnGUI()
    {
        Rect labelRect = new Rect(20, 10, 400, 100);
        GUI.Label(labelRect, $"{orientation}\n{Mathf.Round(angle)}°", new GUIStyle { fontSize = 50 });
    }

    public void deleteCar(int ID)
    {
        for (int i = 0; i < tideViews.Count; i++)
        {
            if (tideViews[i].ID != ID) continue;
            Destroy(instance.tideViews[i].gameObject);
            instance.tideViews.RemoveAt(i);
        }
    }

    public void CreateBall()
    {
        myBall = Instantiate(Resources.Load<GameObject>(ball.name), ball.transform.position, ball.transform.rotation);
    }

    public void CreateCar(ushort spawnerID)
    {
        GameObject myCar = Instantiate(Resources.Load<GameObject>(car.name), car.transform.position, car.transform.rotation);
        var view = myCar.GetComponent<TideView>();

        if (view != null)
        {
            view.ID = spawnerID;
            view.IsMine = ClientManager.instance.Active ? (spawnerID == ClientManager.instance.getID) : false;
            view.Owner = spawnerID;

            tideViews.Add(view);
            tideViews = tideViews.OrderBy(x => x.ID).ToList();
        }
    }

    public void CreateGhostCar(SnapShot shot)
    {
        GameObject myCar = Instantiate(Resources.Load<GameObject>(ghostCar.name), car.transform.position, car.transform.rotation);
        myCar.transform.position = shot.Position.get();
        myCar.transform.rotation = shot.Rotation.get();
    }

    public void PhotonSpawnCar()
    {
        PhotonNetwork.Instantiate(car.name, car.transform.position, car.transform.rotation);
    }

    GameObject myBall;
    Ball gameBall;
    public Ball Ball
    {
        get
        {
            if (!gameBall)
                gameBall = myBall?.GetComponent<Ball>();

            return gameBall;
        }
    }
}
