using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DumbDie : MonoBehaviour, IDie
{
    [SerializeField] private List<GameObject> _gameObjectSides = new List<GameObject>(); 
    [SerializeField] private Vector3 _rotationStep;
    [SerializeField] private short _xSides, _ySides, _zSides;
    [SerializeField] private float _secondsPerRoll;
    [SerializeField] private short _rotationsPerRoll;
    [SerializeField] private float _timeTillNextRoll;
    [SerializeField] private float _currentDieRollTime;

    public IReadOnlyCollection<IDieSide> Sides { get { return _gameObjectSides.Select(side => side.GetComponent<IDieSide>()).Where(side => side != null).ToList(); } }

    public void ForceDieRoll(IDieSide dieSide)
    {
        print("TODO: Stop animation here");
    }

    public IEnumerator RollDie()
    {
        print("The dice are rolling");
        var standardTimePerRotation = _secondsPerRoll/(float)_rotationsPerRoll;

        var i = 0;
        var currRollTime = 0f;
        var currHalfRollTime = 0f;
        var stepTime = standardTimePerRotation;//Mathf.Max(0, Mathf.Min(standardTimePerRotation, _secondsPerRoll - (float)stopWatch.GetSecondsSince(currTime)));
        var halfStepTime = (stepTime / 2f);

        while (i < _rotationsPerRoll)
        {
            var randomRot = GetRandomVector(_xSides, _ySides, _zSides);
            var literalRot = Vector3.Scale(randomRot, _rotationStep);

            var startRot = transform.rotation;
            var endRot = Quaternion.Euler(literalRot);
            var halfRot = GetHalfPointRotation(literalRot, startRot, endRot);

            while(currHalfRollTime < halfStepTime)
            {
                currHalfRollTime += Time.deltaTime;
                currRollTime += Time.deltaTime;

                var rotation = GetLerpRotation(startRot, halfRot, currHalfRollTime, halfStepTime);
                transform.rotation = rotation;
                yield return null;
            }
            print($"Total time for fiirst half roll: {currHalfRollTime}");

            currHalfRollTime = 0;
            while (currHalfRollTime < halfStepTime)
            {
                currHalfRollTime += Time.deltaTime;
                currRollTime += Time.deltaTime;

                var rotation = GetLerpRotation(halfRot, endRot, currHalfRollTime, halfStepTime);
                transform.rotation = rotation;
                yield return null;
            }
 
            print($"Total time for second half roll: {currHalfRollTime}");
            i++;
            currHalfRollTime = 0f;
            yield return null;
        }

        print($"Total time for roll: {currRollTime}");
        var sidesFacingCamera = Sides.Where(x => x.FacingCamera());
        print($"We have {sidesFacingCamera.Count()} facing you, the first of which is {sidesFacingCamera.First().Side}");
        Events.CallOnDiceRolled(sidesFacingCamera.First().Side);
    }

    private Quaternion GetHalfPointRotation(Vector3 end, Quaternion startRot, Quaternion endRot)
    {
        Vector3 halfPoint;
        Quaternion halfRot;

        if (!HasSignificantRotation(startRot, endRot))
        {
            halfPoint = new Vector3(end.x + 180, end.y + 180, end.z + 180);
        }
        else if (end == Vector3.zero)
        {
            halfPoint = transform.rotation.eulerAngles / 2;
        }
        else
        {
            halfPoint = end / 2;
        }

        halfRot = Quaternion.Euler(halfPoint);

        return halfRot;
    }

    void Start()
    {
        Random.InitState((int)(DateTime.Now.Ticks % int.MaxValue));

        _rotationStep = CaclualateRotationVector(_xSides, _ySides, _zSides);
    }

    void FixedUpdate()
    {
        _currentDieRollTime += Time.deltaTime;

        if(_currentDieRollTime >= _timeTillNextRoll) 
        {
            print("Lets roll the dice at " + DateTime.Now.Second + " " + DateTime.Now.Millisecond);
            StartCoroutine(RollDie());

            _currentDieRollTime = 0;
        }
    }

    private static Quaternion GetLerpRotation(Quaternion starting, Quaternion ending, float currentTime, float maxTime)
    {
        var currStepPercent = (float)(currentTime / maxTime);
        var newRot = Quaternion.Lerp(starting, ending, currStepPercent);

        return newRot;
    }

    private static Vector3 GetRandomVector(short xSides, short ySides, short zSides)
    {
        var xStep = Random.Range(0, xSides);
        var yStep = Random.Range(0, ySides);
        var zStep = Random.Range(0, zSides);

        return new Vector3(xStep, yStep, zStep);
    }

    private static Vector3 CaclualateRotationVector(short xSides, short ySides, short zSides)
    {
        var xRot = CaclulateRotationAngle(xSides);
        var yRot = CaclulateRotationAngle(ySides);
        var zRot = CaclulateRotationAngle(zSides);

        return new Vector3(xRot, yRot, zRot);
    }

    private static float CaclulateRotationAngle(short sideCount)
    {
        if(sideCount == 0)
        {
            return 0;
        }

        return (float)(360 / sideCount);
    }

    private static bool HasSignificantRotation(Quaternion start, Quaternion end)
    {
        if(start == end)
        {
            return false;
        }

        if(    start == end
            || start.x == end.x && start.y == end.y
            || start.x == end.x && start.z == end.z
            || start.y == end.y && start.z == end.z)
        { 
            return false; 
        }

        return true;
    }
}