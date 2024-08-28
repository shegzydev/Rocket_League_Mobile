using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] GameObject car;

    public List<RipView> ripViews = new List<RipView>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void deleteCar(int ID)
    {
        for (int i = 0; i < ripViews.Count; i++)
        {
            if (ripViews[i].ID != ID) continue;

            var toRemove = instance.ripViews[i].GetComponent<PhysicsObject>();
            if (toRemove != null) { PhysicsManager.Remove(toRemove); }
            Destroy(instance.ripViews[i].car.gameObject);
            instance.ripViews.RemoveAt(i);
        }
    }

    public void CreateCar(int spawnerID)
    {
        GameObject myCar = Instantiate(Resources.Load<GameObject>(car.name), car.transform.position, car.transform.rotation);
        var view = myCar.GetComponent<RipView>();

        if (view != null)
        {
            view.ID = spawnerID;
            view.IsMine = ClientManager.instance.Active ? (spawnerID == ClientManager.instance.getID) : false;
            view.Owner = spawnerID;

            ripViews.Add(view);
            ripViews = ripViews.OrderBy(x => x.ID).ToList();
        }
    }
}
