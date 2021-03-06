using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    public static Action OnLineStarted;


    public GameObject linePrefab;
    public GameObject currentLine;

    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;

    public List<Vector2> fingerPositions;
    public float minDistance;

    public float maxDistance;

    public float distanceTraveled;
    public int maxDrawing;
    public int drawnLine;

    public bool isDrawing;

    private Camera mainCam;

    private bool _isActive = false;

    Vector2 firstPos;

    
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += Activate;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= Activate;
    }

    private void Activate(GameManager.GameState state)
    {
        _isActive = state == GameManager.GameState.Attacking;

        if (state == GameManager.GameState.Idle)
        {
            drawnLine = 0;
            distanceTraveled = 0;
        }
        else if (state == GameManager.GameState.Act || state == GameManager.GameState.Idle || state == GameManager.GameState.Lose || state == GameManager.GameState.Win)
        {
            transform.DestroyChildren();
        }
        else if (_isActive)
        {
            transform.DestroyChildren();
        }
    }

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        // Debug.Log(mainCam.name);
    }

    private void Update()
    {
        if (!_isActive) return;
        if (Input.GetMouseButtonDown(0))
        {

            CreateLine();


        }
        if (Input.GetMouseButton(0))
        {
            if(distanceTraveled < maxDistance && drawnLine < maxDrawing) 
            {
                Vector2 tempFingerPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(tempFingerPos,fingerPositions[fingerPositions.Count -1]) > minDistance)
            {
                distanceTraveled += Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]);
                // Debug.Log(Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count - 1]));
                UpdateLine(tempFingerPos);
                

            }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isDrawing)
            {
                BossInteraction();
                isDrawing = false;
                drawnLine++;
                distanceTraveled = 0;
            }
        }


    }

    void CreateLine()
    {
        maxDistance = LevelBehaviour.Instance.MaxDrawDistance * LevelManager.Instance.LineLengthMultiplier;
        maxDrawing = LevelBehaviour.Instance.MaxDrawCount + LevelManager.Instance.BonusLine;
        
        if (drawnLine > maxDrawing) return;
        
        
        isDrawing = true;
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, transform);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        fingerPositions.Clear();
        fingerPositions.Add(mainCam.ScreenToWorldPoint(Input.mousePosition));
        fingerPositions.Add(mainCam.ScreenToWorldPoint(Input.mousePosition));
        firstPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);
        edgeCollider.points = fingerPositions.ToArray();
        
        OnLineStarted?.Invoke();
        
    }

    void UpdateLine(Vector2 newFingerPos)
    {
        
        fingerPositions.Add(newFingerPos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount-1,newFingerPos);
        edgeCollider.points = fingerPositions.ToArray();

    }

    public void BossInteraction()
    {
        if (GameObject.FindGameObjectWithTag("Boss") != null)
        {
            
            BossController bossScript;
            bossScript = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>();


            if (bossScript.isDead) return;

            float xDistance = firstPos.x - mainCam.ScreenToWorldPoint(Input.mousePosition).x;
            float yDistance = firstPos.y - mainCam.ScreenToWorldPoint(Input.mousePosition).y;
            // Debug.Log("x:" + xDistance + " y:" + yDistance);
            if(bossScript.BState == BossController.BossState.PrepareDefend)
            {
                if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
                {
                    bossScript.Block(2);
                }
                else if (yDistance > 0)
                {
                    bossScript.Block(1);
                }
                else if (yDistance <= 0)
                {
                    bossScript.Block(0);
                }
            }else if (bossScript.BState == BossController.BossState.PrepareAttack)
            {
                if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
                {
                    bossScript.Attack(2);
                }
                else if (yDistance > 0)
                {
                    bossScript.Attack(1);
                }
                else if (yDistance <= 0)
                {
                    bossScript.Attack(0);
                }
            }
            
        }
    }




}
