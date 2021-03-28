using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    [SerializeField]
    private GameObject[] chibiList; // prefab list

    List<ChibiBehaviour> allChibiList = new List<ChibiBehaviour>();

    // ------ below variables are for swapping -------- //
    public bool isSwapping;
    bool turnChecked;
    ChibiBehaviour lastChibi;
    ChibiBehaviour chibiOne; // ---- storing the chibis
    ChibiBehaviour chibiTwo; // ---- storing the chibis
    Vector3 chibiOneStartPos;
    Vector3 chibiOneEndPos;
    Vector3 chibiTwoStartPos;
    Vector3 chibiTwoEndPos;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        FillBoard();
        StartCoroutine(PermaBoardCheck());
    }

    void FillBoard()
    {
        int[] previousLeft = new int[height]; // ------ last left column
        int previousBelow = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                /*
                int randomChibi = Random.Range(0, chibiList.Length);
                GameObject newChibi = Instantiate(chibiList[randomChibi], new Vector4(transform.position.x + i, transform.position.y + j), Quaternion.identity) as GameObject;

                allChibiList.Add(newChibi.GetComponent<ChibiBehaviour>());

                newChibi.transform.parent = this.transform; 
                */

                List<GameObject> possibleChibis = new List<GameObject>();
                possibleChibis.AddRange(chibiList);

                // --------- recursive loop, going backwards --------- // 
                for (int k = possibleChibis.Count - 1; k >= 0; k--)
                {
                    int idChibi = possibleChibis[k].GetComponent<ChibiBehaviour>().chibiID;
                    if(idChibi == previousLeft[j] || idChibi == previousBelow)
                    {
                        possibleChibis.RemoveAt(k);
                    }
                }

                int randomChibi = Random.Range(0, possibleChibis.Count);
                GameObject newChibi = Instantiate(possibleChibis[randomChibi], new Vector4(transform.position.x + i, transform.position.y + j), Quaternion.identity) as GameObject;

                ChibiBehaviour nC = newChibi.GetComponent<ChibiBehaviour>();
                allChibiList.Add(nC);

                newChibi.transform.parent = this.transform;

                previousBelow = nC.chibiID;
                previousLeft[j] = nC.chibiID;
            }
        }
    
    }

    void TogglePhysics(bool on)
    {
        for (int i = 0; i < allChibiList.Count; i++)
        {
            allChibiList[i].GetComponent<Rigidbody>().isKinematic = on;
        }
    }

    public void SwapChibi(ChibiBehaviour currentChibi)
    {
        if (lastChibi == null)
        {
            lastChibi = currentChibi;
        }
        else if (lastChibi == currentChibi)
        {
            lastChibi = null;
        }
        else
        {
            // ----- check if neighbour ------ //
            if (lastChibi.CheckIfNeighbour(currentChibi))
            {
                // ----- the swapping! ------------ //
                chibiOne = lastChibi;
                chibiTwo = currentChibi;

                chibiOneStartPos = lastChibi.transform.position;
                chibiOneEndPos = currentChibi.transform.position;

                chibiTwoStartPos = currentChibi.transform.position;
                chibiTwoEndPos = lastChibi.transform.position;

                // ---- here is for the swapping mechanic! ------- //
                StartCoroutine(SwapChibi());
            }
            else
            {
                // ------- no swap pls ---------- //
                lastChibi.ToggleSelector();
                lastChibi = currentChibi;
            }
        }
    }

    IEnumerator SwapChibi()
    {
        if (isSwapping)
        {
            yield break;
        }
        isSwapping = true;
        TogglePhysics(true);

        // --- finally doing the swapping! ------- //
        while (MoveToSwapLocation(chibiOne, chibiOneEndPos) && MoveToSwapLocation(chibiTwo, chibiTwoEndPos))
        {
            yield return null;
        }

        // --------- has been a match --------- //
        chibiOne.ClearAllMatches();
        chibiTwo.ClearAllMatches();

        while (!turnChecked)
        {
            yield return null; // this waits until check is done.
        }
        bool chibiOneMatchFound = chibiOne.matchFound;
        bool chibiTwoMatchFound = chibiTwo.matchFound;
        if(!chibiOneMatchFound && !chibiTwoMatchFound)
        {
            // ----- swap back in no match -------- //
            while (MoveToSwapLocation(chibiOne, chibiOneStartPos) && MoveToSwapLocation(chibiTwo, chibiTwoStartPos))
            {
                yield return null;
            }
        }

        // ---------- report actual match donezo ----------------- //
        if(chibiOneMatchFound || chibiTwoMatchFound)
        {
            GameManager.instance.UpdateMatches();
        }
        turnChecked = false;

        // ----- toggle all the physics ------- //

        isSwapping = false;
        // ------ reset all things ------- //
        TogglePhysics(false);
        lastChibi = null;
        chibiOne.ToggleSelector();
        chibiTwo.ToggleSelector();
    }

    bool MoveToSwapLocation(ChibiBehaviour chibi, Vector3 swapGoal)
    {
        return chibi.transform.position != (chibi.transform.position = Vector3.MoveTowards(chibi.transform.position, swapGoal, 2 * Time.deltaTime));
    }

    public void CreateNewChibi(ChibiBehaviour chibi, Vector3 posiition)
    {
        allChibiList.Remove(chibi);

        // ------ create a new one! ------- //
        int random = Random.Range(0, chibiList.Length);
        GameObject newChibi = Instantiate(chibiList[random], new Vector3(posiition.x, posiition.y + 9.0f, posiition.z), Quaternion.identity);

        allChibiList.Add(newChibi.GetComponent<ChibiBehaviour>());
        newChibi.transform.parent = transform;
    }

    public void ReportTurnDonezo()
    {
        turnChecked = true;
    }

    public bool CheckIfBoardIsMoving()
    {
        for (int i = 0; i < allChibiList.Count; i++)
        {
            if(allChibiList[i].transform.localPosition.y > 10.6f)
            {
                return true;
            }

            //if(allChibiList[i].GetComponent<Rigidbody>().velocity.y > 0.001f)
            if(!allChibiList[i].GetComponent<Rigidbody>().IsSleeping())
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator PermaBoardCheck()
    {
        yield return new WaitForSeconds(0f);
        while (true)
        {
            if(!isSwapping && !CheckIfBoardIsMoving())
            {
                for (int i = 0; i < allChibiList.Count; i++)
                {
                    allChibiList[i].ClearAllMatches();
                }

                if(!CheckForPossibleMatches())
                {
                    Debug.Log("NO MATCHES LEFT!");
                    for (int i = 0; i < allChibiList.Count; i++)
                    {
                        allChibiList[i].possiblePoints = false;
                        allChibiList[i].matchFound = true;
                    }
                }    
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void StopGame()
    {
        StopAllCoroutines();
        foreach(ChibiBehaviour chibi in allChibiList)
        {
            chibi.StopAll();
        }
    }

    // ---------------- board has at least ONE match ---------------- //

    bool CheckForPossibleMatches()
    {
        TogglePhysics(true);
        for (int i = 0; i < allChibiList.Count; i++)
        {
            for (int j = 0; j < allChibiList.Count; j++)
            {
                if(allChibiList[i].CheckNeighbours(allChibiList[j]))
                {
                    // --------- temp swap chibi --------- //
                    ChibiBehaviour c1 = allChibiList[i];
                    ChibiBehaviour c2 = allChibiList[j];

                    // ------------ create a neighbour list --------- //
                    List<ChibiBehaviour> tempNeighbour = new List<ChibiBehaviour>(c1.chibiNeighbourList);

                    // ------- swapping the location ------- //
                    Vector3 c1TempPos = c1.transform.position;
                    Vector3 c2TempPos = c2.transform.position;
                    c1.transform.position = c2TempPos;
                    c2.transform.position = c1TempPos;

                    // --------- swapping the neighbours ---------- //
                    c1.chibiNeighbourList = c2.chibiNeighbourList;
                    c2.chibiNeighbourList = tempNeighbour;

                // ------------- check matches, reset everything ----------- //
                if(c1.CheckForExisitingMatches())
                {
                    c1.transform.position = c1TempPos;
                    c2.transform.position = c2TempPos;

                    c2.chibiNeighbourList = c1.chibiNeighbourList;
                    c1.chibiNeighbourList = tempNeighbour;

                    TogglePhysics(false);
                    return true;
                }
                if (c2.CheckForExisitingMatches())
                {
                    c1.transform.position = c1TempPos;
                    c2.transform.position = c2TempPos;

                    c2.chibiNeighbourList = c1.chibiNeighbourList;
                    c1.chibiNeighbourList = tempNeighbour;

                    TogglePhysics(false);
                    return true;
                }

                c1.transform.position = c1TempPos;
                c2.transform.position = c2TempPos;

                c2.chibiNeighbourList = c1.chibiNeighbourList;
                c1.chibiNeighbourList = tempNeighbour;

                TogglePhysics(false);
                    
                }
            }
    }
    return false;
}

                    /*if unity editor will only run if in editor, but won't show in the standalone game.*/
#if UNITY_EDITOR
                    void OnDrawGizmos()
    {
        for (int i = 0; i < width; i++)
			{
                for (int j = 0; j < height; j++)
			        {
                        Gizmos.DrawWireCube(new Vector3(transform.position.x + i, transform.position.y + j, 0), new Vector3(1.0f, 1.0f, 1.0f));
			        }
			}
    }

    #endif
}
