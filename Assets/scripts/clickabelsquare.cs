using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickabelsquare : MonoBehaviour
{
    private int Getnumber()
    {
        Vector3 p = gameObject.transform.position;
        return ((int)p.x + 1)+ (10*(int)p.y);
    }
    public int squareNumber = 0;

    private void Start() => squareNumber = Getnumber();
    private void OnMouseDown()
    {
        GameObject.Find("game manager").SendMessage("SquareClicked", gameObject);
        int contor = GameObject.Find("game manager").GetComponent<gamemanager>().contor;
        if (contor > 3)
            Destroy(this);
    }

}
