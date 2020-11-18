using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickabelsquare2 : MonoBehaviour
{
    private int Getnumber()
    {
        Vector3 p = gameObject.transform.position;
        return 13+((int)p.x + 1)+ (10*(int)p.y);
    }
    public int squareNumber = 0;

    private void Start() => squareNumber = Getnumber();
    private void OnMouseDown()
    {
        GameObject.Find("game manager").SendMessage("SquareClicked2", gameObject);
        Destroy(this);
    }

}
