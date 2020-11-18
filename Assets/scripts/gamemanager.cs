using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    //modele
    public GameObject hit_cross;
    public GameObject miss_naught;
    public GameObject plane;

    //hitboxes
    private int[,] hitbox = new int[10, 10];
    private int[,] hitbox_bot = new int[10, 10];

    //lista avioane
    private List<Vector2>[] lista_avioane = new List<Vector2>[4];
    private List<Vector2>[] lista_avioane_bot = new List<Vector2>[4];

    // bot atac
    private Queue<Vector2> bot_atac = new Queue<Vector2>();

    //UI
    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject beginningPanel;
    public Text beginningText;
    public GameObject pozinvPanel;
    public Text pozinvText;

    //contoare
    private int nravioane = 3;
    private int nravioane_bot = 3;
    public int contor = 1;
    public int contor_bot = 1;
    private int ult_lov = 0;

    

    private void Start()
    {
        //UI inactiv + setare coordonate
        beginningPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        pozinvPanel.SetActive(false);
        gameOverPanel.transform.localScale = new Vector3(1.1f, 0.5f, 1);
        gameOverPanel.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        beginningPanel.transform.localScale = new Vector3(0.7f, 0.4f, 1);
        beginningPanel.transform.position = new Vector3(Screen.width / 2, Screen.height * 5 / 6, 0);
        pozinvPanel.transform.localScale = new Vector3(0.4f, 0.34f, 1);
        pozinvPanel.transform.position = new Vector3(Screen.width / 2, Screen.height * 3 / 7, 0);
        beginningText.text = "Plaseaza 3 avioane pe tabla din stanga";
        pozinvText.text = "Pozitie Invalida";

        //initializari
        lista_avioane[0] = new List<Vector2>();
        lista_avioane[1] = new List<Vector2>();
        lista_avioane[2] = new List<Vector2>();
        lista_avioane[3] = new List<Vector2>();
        lista_avioane_bot[0] = new List<Vector2>();
        lista_avioane_bot[1] = new List<Vector2>();
        lista_avioane_bot[2] = new List<Vector2>();
        lista_avioane_bot[3] = new List<Vector2>();

        //botul isi plaseaza avioanele
        while (contor_bot < 4)
        {
            int x = Random.Range(1, 10);
            int y = Random.Range(1, 10);
            if(posibil(x, y, hitbox_bot) == true)
            {
                createhitbox(x, y, hitbox_bot, lista_avioane_bot,contor_bot);
                contor_bot++;
            }
        }
    }
    public void SquareClicked2(GameObject square)
    {
        //nr patrat
        int squareNumber = square.GetComponent<clickabelsquare2>().squareNumber;

        //adaugare avioane
        if (contor < 4)
        {
            pozinvPanel.SetActive(false);
            int x = squareNumber % 10;
            int y = squareNumber / 10 % 10;
            if (posibil(x, y, hitbox) == true)
            {
                createhitbox(x, y, hitbox, lista_avioane, contor);
                contor++;
                SpawnPlane(square.transform.position);
                if (contor == 4)
                    beginningPanel.SetActive(false);
            }
            else
            {
                pozinvPanel.SetActive(true);
            }
            
        }
        
    }
    public void SquareClicked(GameObject square)
    {
        if (contor == 4)
        {
            // nr patrat
            int squareNumber = square.GetComponent<clickabelsquare>().squareNumber;

            //desfasurare runda
            Runda(square.transform.position, hitbox_bot, lista_avioane_bot, 1);

            //tura bot
            if (ult_lov == 0)
            {
                Vector2 atac;
                if (bot_atac.Count == 0)
                {
                    int x, y;
                    do
                    {
                        x = Random.Range(-14, -4);
                        y = Random.Range(-3, 7);
                    } while (hitbox[x + 14, y + 3] == 3 || hitbox[x + 14, y + 3] == 2);
                    atac = new Vector2(x, y);
                }
                else
                {
                    do
                    {
                        atac = bot_atac.Dequeue();
                    } while (hitbox[(int)atac.x + 14, (int)atac.y + 3] == 3 || hitbox[(int)atac.x + 14, (int)atac.y + 3] == 2);
                }
                Runda(atac, hitbox, lista_avioane, 2);
            }
        }
    }
    private bool posibil(int x, int y, int[,]hitbox)
    {
        //verifica pozitia plasarii avionului
        if (x > 0 && x < 9)
            if (y > 0 && y < 9)
                if (hitbox[x, y] == 0)
                    if (hitbox[x, y + 1] == 0)
                        if (hitbox[x, y - 1] == 0)
                            if (hitbox[x + 1, y] == 0)
                                if (hitbox[x - 1, y] == 0)
                                    return true;
        return false;

    }
    private bool dead(int x, int y, int[,] hitbox, List<Vector2>[] lista_avioane)
    {
        //verifica daca avionul e distrus
        int nr = 0;
        Vector2 poz = new Vector2( x, y );
        for (int i = 1; i < 4; i++)
            foreach (var el in lista_avioane[i])
            {
                if (el == poz)
                {
                    nr = i;
                    break;
                }

            }
        bool ok = true;
        foreach (var el in lista_avioane[nr])
        {
            if (hitbox[(int)el.x, (int)el.y] == 1)
            {
                ok = false;
                break;
               }
        }
        return ok;
    }
    private void createhitbox(int x, int y, int[,]hitbox, List<Vector2>[]lista_avioane, int contor)
    {
        //creare hitbox pentru avion
        hitbox[x, y] = 1;
        hitbox[x, y + 1] = 1;
        hitbox[x, y - 1] = 1;
        hitbox[x + 1, y] = 1;
        hitbox[x - 1, y] = 1;
        Vector2 a = new Vector2(x, y);
        lista_avioane[contor].Add(a);
        a.x++;
        lista_avioane[contor].Add(a);
        a.x -= 2;
        lista_avioane[contor].Add(a);
        a.x++;
        a.y++;
        lista_avioane[contor].Add(a);
        a.y -= 2;
        lista_avioane[contor].Add(a);
    }
    private void SpawnPlane(Vector3 position)
    {
        //crearea modelului avionului
        position.x += 1.3f;
        position.y += 0.2f;
        position.z = 0;
        GameObject av = Instantiate(plane, position, Quaternion.identity);
        av.transform.localScale = new Vector3(12, 14, 0);
    }
    private void Runda(Vector3 position, int[,] hitbox, List<Vector2>[] lista_avioane,int player)
    {
        //desfasurare runda
        position.z = -1;
        int x = (int)position.x;
        int y = (int)position.y;
        if(x<0)
        {
            x += 14;
            y += 3;
        }
        position.y = y;
        if (hitbox[x, y] == 1)
        {
            Instantiate(hit_cross, position, Quaternion.identity);
            hitbox[x, y] = 2;
            if (player == 2)
            {
                if (x + 1 <= 9)
                    bot_atac.Enqueue(new Vector2(x - 13, y - 3));
                if (x - 1 >= 0)
                    bot_atac.Enqueue(new Vector2(x - 15, y - 3));
                if (y + 1 <= 9)
                    bot_atac.Enqueue(new Vector2(x - 14, y - 2));
                if (y - 1 >= 0)
                    bot_atac.Enqueue(new Vector2(x - 14, y - 4));
            }
            if (dead(x, y, hitbox, lista_avioane))
            {
                if (player == 1)
                    nravioane_bot--;
                else
                {
                    nravioane--;
                    if(nravioane>0)
                        golire_coada();         
                }
              //  print("AVION MORT");

                if (nravioane_bot == 0)
                {
                    gameOverPanel.SetActive(true);
                    gameOverText.color = Color.green;
                    gameOverText.text = "VICTORIE!";
                    foreach (var a in GameObject.FindObjectsOfType<clickabelsquare>())
                        Destroy(a);
                    ult_lov = 1;
                }
                else
                if (nravioane == 0)
                {
                    gameOverPanel.SetActive(true);
                    gameOverText.color = Color.red;
                    gameOverText.text = "ÎNFRÂNGERE!";
                    foreach (var a in GameObject.FindObjectsOfType<clickabelsquare>())
                        Destroy(a);
                }
            }
        }
        else
        {
            Instantiate(miss_naught, position, Quaternion.identity);
            hitbox[x, y] = 3;
        }

    }
    private void golire_coada()
    {
        //strategie bot
        bool ok = false;
        int i = 0;
        while(ok==false)
        {
            i++;
            ok = true;
            foreach(var a in lista_avioane[i])
            {
                if (hitbox[(int)a.x, (int)a.y] != 1)
                {
                    ok = false;
                    break;
                }
            }
        }
        bot_atac.Clear();
        for(int j=1;j<4;j++)
        {
            ok = dead((int)lista_avioane[j][0].x, (int)lista_avioane[j][0].y, hitbox, lista_avioane);
            if (ok == false)
            {
                foreach (var a in lista_avioane[j])
                {
                    if (hitbox[(int)a.x, (int)a.y] == 2)
                    {
                        if (a.x + 1 <= 9)
                            bot_atac.Enqueue(new Vector2(a.x - 13, a.y - 3));
                        if (a.x - 1 >= 0)
                            bot_atac.Enqueue(new Vector2(a.x - 15, a.y - 3));
                        if (a.y + 1 <= 9)
                            bot_atac.Enqueue(new Vector2(a.x - 14, a.y - 2));
                        if (a.y - 1 >= 0)
                            bot_atac.Enqueue(new Vector2(a.x - 14, a.y - 4));
                    }
                }
            }
        }
    }
    public void restart()
    {
        SceneManager.LoadScene(0);
    }

}
