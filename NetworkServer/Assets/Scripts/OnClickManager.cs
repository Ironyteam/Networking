using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnClickManager : MonoBehaviour {

    public Button cancelBTN;
    public ServerManager networkThing;

	// Use this for initialization
	void Start () {
        cancelBTN.onClick.AddListener(() => destroyMyself());
	}
        
    void destroyMyself()
    {
        networkThing = GameObject.Find("Network Manager").GetComponent<ServerManager>();
        networkThing.forceRemoveGame(cancelBTN.transform.parent.gameObject);
        Debug.Log(cancelBTN.transform.parent.gameObject);
        Destroy(cancelBTN.transform.parent.gameObject);
    }
}
