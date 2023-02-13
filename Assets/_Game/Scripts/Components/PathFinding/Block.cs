using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private bool _isLock, _isLockTop, _isLockRight, _isLockBot, _isLockLeft, _isTrap;
    public bool isLock
    {
        get
        {
            return _isLock;
        }
        set
        {
            _isLock = value;
        }
    }
    public bool isLockBot => _isLockLeft; 
    public bool isLockTop => _isLockRight; 
    public bool isLockLeft => _isLockBot;
    public bool isLockRight => _isLockTop;
    public bool isTrap
    {
        get
        {
            return _isTrap;
        }
        set
        {
            _isTrap = value;
        }
    }

    public List<Direction> trapLock;

    [SerializeField]
    private GameObject obLockAll, obLockLeft, obLockRight, obLockBot, obLockTop, obStep;


    public void SetActiveLock()
    {
        SetActive(obLockAll, _isLock);
        SetActive(obLockLeft, _isLockLeft);
        SetActive(obLockRight, _isLockRight);
        SetActive(obLockBot, _isLockBot);
        SetActive(obLockTop, _isLockTop);
    }

    private void SetActive(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }

    public void SetActiveStep(bool active,Vector2 direction)
    {
        if (obStep != null)
        {
            obStep.SetActive(active);
            if (direction != null)
            {
                obStep.transform.rotation = Quaternion.Euler(new Vector3(90, direction.x == 1 ? 90
                    : (direction.x == -1 ? -90 : (direction.y == 1 ? 0 : 180))
                    , 0));
            }
        }
    }
    public bool IsLockAnotherSide(Vector2 direction)
    {
        if (trapLock.Count == 0) return false;
        else
        {
            if (direction.y < 0) {
                return traplock(Direction.DOWN);
            }
            if (direction.y > 0) {
                return traplock(Direction.UP);
            }
            if (direction.x < 0) {
                return traplock(Direction.LEFT);
            }
            if (direction.x > 0) {
                return traplock(Direction.RIGHT);
            }
        }
        return false;
    }
    private bool traplock(Direction direction)
    {
        foreach (Direction dir in trapLock)
        {
            if (dir == direction)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsLockSide(Vector2 direction)
    {
        if (direction.x < 0) { return isLockTop; }
        if (direction.x > 0) { return isLockBot; }
        if (direction.y < 0) { return isLockRight; }
        if (direction.y > 0) { return isLockLeft; }
        return false;
    }

    public bool IsLockSideSelf(Vector2 direction)
    {
        if (direction.x < 0) { return isLockBot; }
        if (direction.x > 0) { return isLockTop; }
        if (direction.y < 0) { return isLockLeft; }
        if (direction.y > 0) { return isLockRight; }
        return false;
    }
}
