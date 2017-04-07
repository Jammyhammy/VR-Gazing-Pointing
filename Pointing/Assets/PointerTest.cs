using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PointerTest : MonoBehaviour {

    public Transform Controller;

    public float PointSensitivity = 10;
    
    private float pointer_pitch = 0;
    private float pointer_yaw = 0;
	public float timeStep = 0.1f;
	//Approximate max distance in unity units
	public float maxDistance = 5f;  
    public Color goodSpotCol = new Color(0, 0.6f, 1f, 0.2f);
  	public float arcLineWidth = 0.05f;
    public Vector2 texMovementSpeed = new Vector2(-0.035f, 0);
    public float matScale = 5;
    public GameObject tipPrefab;
	private GameObject _tipInstance;
	private Transform _vrCamera;
	private Transform _vrPlayArea;
	private LineRenderer _lineRenderer;
	private Vector3 _destination;
	private Vector3 _destinationNormal;

    void Start() {
        _vrCamera = Camera.main.transform;
        _vrPlayArea = transform;
		GameObject arcParentObject = new GameObject("ArcTeleporter");
		arcParentObject.transform.localScale = _vrPlayArea.localScale;        
        GameObject arcLine1 = new GameObject("ArcLine1");
		arcLine1.transform.SetParent(arcParentObject.transform);
		_lineRenderer = arcLine1.AddComponent<LineRenderer>();
        _lineRenderer.SetWidth(arcLineWidth*_vrPlayArea.localScale.magnitude, arcLineWidth*_vrPlayArea.localScale.magnitude);
        _lineRenderer.material = new Material(Shader.Find("Custom/ArcShader"));
        _lineRenderer.material.SetColor("_Color", goodSpotCol);
        if (tipPrefab != null) {
            _tipInstance = (GameObject)Instantiate(tipPrefab, Vector3.zero, Quaternion.identity);
        } else {
            _tipInstance = new GameObject("TipInstance");
        }
        _tipInstance.transform.SetParent(arcParentObject.transform);
        _tipInstance.transform.localScale = Vector3.one;
        _tipInstance.SetActive(true);
    }

    void Update() {
        if(!Input.GetButton("Switch Control"))
        {
            pointer_pitch += -Input.GetAxis("Mouse Y") * PointSensitivity;
            pointer_yaw += Input.GetAxis("Mouse X") * PointSensitivity;
            Controller.localRotation = Quaternion.Euler(pointer_pitch, pointer_yaw, 0);
        }

        if(_tipInstance != null && _tipInstance.activeSelf)
        {
            _tipInstance.transform.position = _destination+(_destinationNormal*0.05f);
            _tipInstance.transform.rotation = Quaternion.identity;

        }
        CalculateLine();
        
    }

    //Regardless of if it hits something or not, create a game object at the end that a camera can be created to look at
    //if it's outside of the frustum view.
    private void CalculateLine() {
        List<Vector3> positions1 = new List<Vector3>();

        RaycastHit hit = new RaycastHit();
        float totalDistance1 = 0;

        Quaternion currentRotation = transform.rotation;
        Vector3 currentPosition;
        currentPosition = transform.position;
        Vector3 lastPosition;
        positions1.Add(currentPosition);

        lastPosition = transform.position-transform.forward;
        Vector3 currentDirection = transform.forward;
        // Change to straight line
        Vector3 downForward = new Vector3(transform.forward.x*0.01f, transform.forward.y*0.01f, transform.forward.z*0.01f);
        int i = 0;
        bool hitObject = false;

        while(i<400) {
				i++;
				//	Make ray for new direction
				Ray newRay = new Ray(currentPosition, currentPosition-lastPosition);
				float length = (maxDistance*0.01f)*_vrPlayArea.localScale.magnitude;

				float raycastLength = length*1.1f;

				//	Check if we hit something
				bool hitSomething = false;
                hitSomething = Physics.Raycast(newRay, out hit, raycastLength);

				if (hitSomething)
				{
					//	Depending on whether we had switched to the first or second line renderer
					//	add the point and finish calculating the total distance
                    totalDistance1 += (currentPosition-hit.point).magnitude;
                    positions1.Add(hit.point);
					_destinationNormal = hit.normal;
                    hitObject = true;
                    
					//	And we're done
					break;
				}

				//	Convert the rotation to a forward vector and apply to our current position
				currentDirection = currentRotation * Vector3.forward;
				lastPosition = currentPosition;
				currentPosition += currentDirection*length;

                totalDistance1 += length;
                positions1.Add(currentPosition);
        }

        if(!hitObject) {
            _destination = lastPosition;
            _destinationNormal = Vector3.zero;
        }

        _lineRenderer.enabled = true;
        _destination = positions1[positions1.Count-1];
        _lineRenderer.SetColors(goodSpotCol, goodSpotCol);
        _lineRenderer.material.SetColor("_Color", goodSpotCol);
		_lineRenderer.SetVertexCount(positions1.Count);
		_lineRenderer.SetPositions(positions1.ToArray());
		_lineRenderer.material.mainTextureScale = new Vector2((totalDistance1*matScale)/_vrPlayArea.localScale.magnitude, 1);
		_lineRenderer.material.mainTextureOffset = new Vector2(_lineRenderer.material.mainTextureOffset.x+texMovementSpeed.x, _lineRenderer.material.mainTextureOffset.y+texMovementSpeed.y);
        
        if( _tipInstance != null) _tipInstance.SetActive(true);


    }
}