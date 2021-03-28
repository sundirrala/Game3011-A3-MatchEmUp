using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ChibiBehaviour : MonoBehaviour
{
 
    public List<ChibiBehaviour> chibiNeighbourList = new List<ChibiBehaviour>();

    [SerializeField]
    private GameObject selector;

    [SerializeField]
    private bool isSelected = true;

    public int chibiID;

    public bool matchFound;

    [SerializeField]
    private int score;

    Vector3[] checkDirections = new Vector3[4] { Vector3.left, Vector3.right, Vector3.up, Vector3.down };

    
    public bool possiblePoints = true;

    void Start()
    {
        ToggleSelector();
        StartCoroutine(DestroyChibis());
    }

    void OnMouseDown()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            // --------- if the board is NOT moving, then you can use the click, if it is, do nothing ------- //
            /*if(BoardManager.instance.CheckIfBoardIsMoving())
            {
                return;
            }*/
            GetAllNeighbours();

            if (!BoardManager.instance.isSwapping)
            {
                ToggleSelector();
                // ------ swap the chibis ---------- //
                BoardManager.instance.SwapChibi(this);
            }
        }

    }

    public void ToggleSelector()
    {
        isSelected = !isSelected;
        selector.SetActive(isSelected);
    }

    void GetAllNeighbours()
    {
        chibiNeighbourList.Clear();
        for (int i = 0; i < checkDirections.Length; i++)
        {
            chibiNeighbourList.Add(GetNeighbour(checkDirections[i]));
        }

    }

    public ChibiBehaviour GetNeighbour(Vector3 checkDirection)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, checkDirection, out hit))
        {
            if(hit.collider != null)
            {
                return hit.collider.GetComponent<ChibiBehaviour>();
            }
        }
        return null;
    }

    public ChibiBehaviour GetNeighbour(Vector3 checkDirection, int id)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, checkDirection, out hit, 0.8f))
        {
            ChibiBehaviour temp = hit.collider.GetComponent<ChibiBehaviour>();
            if (temp != null && temp.chibiID == id)
            {
                return temp;
            }
        }
        return null;
    }

    public bool CheckIfNeighbour(ChibiBehaviour chibi)
    {
        if (chibiNeighbourList.Contains(chibi))
        {
            return true;
        }
        return false;
    }

    List<ChibiBehaviour> FindMatch(Vector3 checkDirection)
    {
        List<ChibiBehaviour> matchList = new List<ChibiBehaviour>();
        List<ChibiBehaviour> checkList = new List<ChibiBehaviour>();

        checkList.Add(this);
        for (int i = 0; i < checkList.Count; i++)
        {
            ChibiBehaviour temp = checkList[i].GetNeighbour(checkDirection, chibiID);
            if(temp != null && !checkList.Contains(temp))
            {
                checkList.Add(temp);
            }
        }
        matchList.AddRange(checkList);
        return matchList;

    }

    void ClearMatch(Vector3[] directions)
    {
        List<ChibiBehaviour> matchChibi = new List<ChibiBehaviour>();
        List<ChibiBehaviour> sortedList = new List<ChibiBehaviour>();

        for (int i = 0; i < directions.Length; i++)
        {
            matchChibi.AddRange(FindMatch(directions[i]));
        }

        // ------ erase all doubles --------- //
        for (int i = 0; i < matchChibi.Count; i++)
        {
            if(!sortedList.Contains(matchChibi[i]))
            {
                sortedList.Add(matchChibi[i]);
            }
        }

        // ------ check for more than three ------- //
        if(sortedList.Count >= 3)
        {
            for (int i = 0; i < sortedList.Count; i++)
            {
                sortedList[i].matchFound = true;

                // -------- clear the chibi list in board -------- //

                // -------- destroy the chibi ---------- //

                // --------- create a new chibi ---------- //
            }
        }
    }

    public void ClearAllMatches()
    {
        ClearMatch(new Vector3[2] { Vector3.left, Vector3.right });
        ClearMatch(new Vector3[2] { Vector3.up, Vector3.down });

        // ---------- report to board done clearing all matches -------- //
        BoardManager.instance.ReportTurnDonezo();

    }

    IEnumerator DestroyChibis()
    {
        while (true)
        {
            if(matchFound)
            {

                // ------ report a score ------- //
                if(possiblePoints)
                {
                    GameManager.instance.UpdateScore(score);
                }
                // ------ create a new chibi ------ //

                // ------ remove an older one ----- //
                BoardManager.instance.CreateNewChibi(this, transform.position);
                Destroy(gameObject);

                yield break;
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void StopAll()
    {
        StopAllCoroutines();
    }

    // ---------------- board has at least ONE match ---------------- //

    public bool CheckNeighbours(ChibiBehaviour chibi)
    {
        GetAllNeighbours();
        if(chibiNeighbourList.Contains(chibi))
        {
            return true;
        }
        return false;
    }
    
    public bool CheckForExisitingMatches()
    {
        if(CheckMatch(new Vector3[2] { Vector3.left, Vector3.right }) || CheckMatch(new Vector3[2] { Vector3.up, Vector3.down }))
        {
            return true;
        }
        return false;
    }

    bool CheckMatch(Vector3[] directions)
    {
        List<ChibiBehaviour> matchChibi = new List<ChibiBehaviour>();
        List<ChibiBehaviour> sortedList = new List<ChibiBehaviour>();

        for (int i = 0; i < directions.Length; i++)
        {
            matchChibi.AddRange(FindMatch(directions[i]));
        }

        // ------ erase all doubles --------- //
        for (int i = 0; i < matchChibi.Count; i++)
        {
            if (!sortedList.Contains(matchChibi[i]))
            {
                sortedList.Add(matchChibi[i]);
            }
        }

        // ------ check for more than three ------- //
        if (sortedList.Count >= 3)
        {
            return true;
        }
        return false;
    }

}
