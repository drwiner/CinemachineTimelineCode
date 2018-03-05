// This is an adaption for Cinema Suites "Camera Body" class to use Cinemachine Virtual Camera instead of regular Camera.

using Cinemachine;
using CinemaProCams;
using UnityEngine;

/// <summary>
/// Camera body.
/// 
/// This script contains GUI elements to make Unity's built in camera operate like a realistic camera.
/// </summary>
[ExecuteInEditMode]
public class CinemachineCameraBody : MonoBehaviour {
	
	// Static
	private static RenderTexture 	_cameraPreview;
	public 	static RenderTexture 	CameraPreview {get{return _cameraPreview;}set{_cameraPreview = value;}}

    [SerializeField]
    private UnitOfMeasure _unitOfMeasure;
    public UnitOfMeasure UnitOfMeasure { get { return _unitOfMeasure; } set { _unitOfMeasure = value; } }

    [SerializeField]
    private string _filmFormatName;
    public string FilmFormatName { get { return _filmFormatName; } set { _filmFormatName = value; } }

    [SerializeField]
    private string _screenSizeName;
    public string ScreenSizeName { get { return _screenSizeName; } set { _screenSizeName = value; } }

	[SerializeField]
	private CinemachineVirtualCamera _nodalCamera;
	public CinemachineVirtualCamera NodalCamera {get{return _nodalCamera;}set{_nodalCamera = value;}}
	
	[SerializeField]
	private CSDOFScatter[] 	_dofComponents;
    public CSDOFScatter[] DepthOfField { get { return _dofComponents; } set { _dofComponents = value; } }
	
	[SerializeField]
	private Transform				_focusTransform;
	public	Transform				FocusTransform {get{return _focusTransform;}set{_focusTransform = value;}}
	
	[SerializeField]
	private Transform				_rigTransform;
	public	Transform				RigTransform {get{return _rigTransform;}set{_rigTransform = value;}}
	
	[SerializeField]
	private Vector3 				_fpOffset;
	public	Vector3					FilmPlaneOffset {get{return _fpOffset;}set{_fpOffset = value;}}
	
	[SerializeField]
	private ProCamsLensDataTable.FOVData[] _lensFOVList;
	public ProCamsLensDataTable.FOVData[] LensFOVList { get { return _lensFOVList; } set { _lensFOVList = value; } }
	
	[SerializeField]
	private int						_fstopIndex;
	public	int						IndexOfFStop {get{return _fstopIndex;}set{_fstopIndex = value;}}
	
	[SerializeField]
	private int						_lensIndex;
	public 	int						IndexOfLens {get{return _lensIndex;}set{_lensIndex = value;}}
	
	[SerializeField]
	private	int 					_lensKitIndex;
	public 	int						IndexOfLensKit {get{return _lensKitIndex;}set{_lensKitIndex = value;}}
	
	[SerializeField]
	private string					_cameraSpecs;
	public 	string					CameraSpecs {get{return _cameraSpecs;}set{_cameraSpecs = value;}}
	
	[SerializeField]
	private string 					_cameraDesc;
	public 	string					CameraDescription {get{return _cameraDesc;}set{_cameraDesc = value;}}
	
	[SerializeField]
	private string 					_lensKitName;
	public 	string					LensKitName {get{return _lensKitName;}set{_lensKitName = value;}}
	
	[SerializeField]
	private float					_dofFarLimit;
	public	float					DOFFarLimit {get{return _dofFarLimit;}set{_dofFarLimit = value;}}
	
	[SerializeField]
	private float					_dofNearLimit; // feet
	public	float					DOFNearLimit {get{return _dofNearLimit;}set{_dofNearLimit = value;}}
	
	[SerializeField]
	private float 					_dofDistTotal;
	public	float					DOFTotal {get{return _dofDistTotal;}set{_dofDistTotal = value;}}
	
	[SerializeField]
	private float 					_focusDist;
	public	float					FocusDistance {get{return _focusDist;}set{_focusDist = value;}}
	
	[SerializeField]
	private bool					_centerOnSubject;
	public 	bool					CenterOnSubject {get{return _centerOnSubject;}set{_centerOnSubject = value;}}
	
	[SerializeField]
	private bool					_clickToFocus;
	public	bool					ClickToFocus {get{return _clickToFocus;}set{_clickToFocus = value;}}
	
	[SerializeField]
	private bool					_showGizmos = false;
	public	bool					ShowGizmos {get{return _showGizmos;} set{_showGizmos = value;}}
	
	public	bool					ShowBody = false;
	
	void Awake () 
    {
		_nodalCamera = GetComponent<CinemachineVirtualCamera>();
		_dofComponents = GetComponents<CSDOFScatter>();
		
		//if (_nodalCamera == null) {
		//	CreateNodalCamera ();
		//	_nodalCamera = GetComponentInChildren<CinemachineVirtualCamera>();
		//}
	}
	
	void Start () {
		// Populate lens data
        ProCamsLensDataTable.FilmFormatData curFilmFormat = ProCamsLensDataTable.Instance.GetFilmFormat(_filmFormatName);
		if(curFilmFormat != null)
		{
			ProCamsLensDataTable.LensKitData lensKitData = curFilmFormat.GetLensKitData(_lensKitIndex);
			if(lensKitData != null)
			{
				// Set available lens data for this film format
				_lensFOVList = lensKitData._fovDataset.ToArray();
			}
		}
	}
	
	void Update () {
		CallUpdate();
	}

	/// <summary>
	/// Raises the draw gizmos event.
	/// 
	/// Will draw the camera frustum along with the near,focus and far planes.
	/// </summary>
	void OnDrawGizmos () {
		
		// Draw Near Plane Line
		if (_showGizmos && _lensFOVList != null) 
        {
            float dofNearM = _dofNearLimit * 0.3048f;// feet to meters
            float dofFarM = _dofFarLimit * 0.3048f;	// feet to meters
            float dofFocusM = _focusDist;	// feet to meters

            if (_unitOfMeasure == UnitOfMeasure.Imperial)
            {
                dofNearM = _dofNearLimit * 0.3048f;// feet to meters
                dofFarM = _dofFarLimit * 0.3048f;	// feet to meters
                dofFocusM = _focusDist * 0.3048f;	// feet to meters
            }

			Transform camTransform = _nodalCamera.transform;
			
			Gizmos.matrix = Matrix4x4.TRS(
			camTransform.position, camTransform.rotation, camTransform.lossyScale);
		
			Vector3 toNear = 		Vector3.forward * dofNearM;
			Vector3 toFar = 		Vector3.forward * dofFarM;
			Vector3 toFocus = 		Vector3.forward * dofFocusM;
			Vector3 toNearClip =	Vector3.forward * _nodalCamera.m_Lens.NearClipPlane;
			Vector3 toFarClip = 	Vector3.forward * _nodalCamera.m_Lens.FarClipPlane;
			
			float fov = 0;
            ProCamsLensDataTable.FilmFormatData curFilmFormat = ProCamsLensDataTable.Instance.GetFilmFormat(_filmFormatName);
			if(curFilmFormat != null)
			{
				fov = Lens.GetHorizontalFOV(curFilmFormat._aspect, _lensFOVList[_lensIndex]._unityVFOV);
			}
			
			float ang = Mathf.Tan(Mathf.Deg2Rad * (fov / 2f));
			
			float oppNear = 		ang * dofNearM;
			float oppFar = 			ang * dofFarM;
			float oppFocus = 		ang * dofFocusM;
			float oppNearClip = 	ang * _nodalCamera.m_Lens.NearClipPlane;
			float oppFarClip = 		ang * _nodalCamera.m_Lens.FarClipPlane;
			
			Vector3 toNearR = 		Vector3.right * oppNear;
			Vector3 toFarR = 		Vector3.right * oppFar;
			Vector3 toFocusR = 		Vector3.right * oppFocus;
			Vector3 toNearClipR = 	Vector3.right * oppNearClip;
			Vector3 toFarClipR = 	Vector3.right * oppFarClip;
			
			fov = _lensFOVList[_lensIndex]._unityVFOV;
			ang = Mathf.Tan(Mathf.Deg2Rad * (fov / 2f));
			
			oppNear = 				ang * dofNearM;
			oppFar = 				ang * dofFarM;
			oppFocus = 				ang * dofFocusM;
			oppNearClip = 			ang * _nodalCamera.m_Lens.NearClipPlane;
            oppFarClip = 			ang * _nodalCamera.m_Lens.FarClipPlane;

            Vector3 toNearT = 		Vector3.up * oppNear;
			Vector3 toFarT = 		Vector3.up * oppFar;
			Vector3 toFocusT = 		Vector3.up * oppFocus;
			Vector3 toNearClipT = 	Vector3.up * oppNearClip;
			Vector3 toFarClipT = 	Vector3.up * oppFarClip;
			
			// Calculate all the points (L = Left, R = Right, T = top, B = bottom)
			Vector3 nearLB = 	toNear 	- toNearR 	- toNearT;	
			Vector3 nearLT = 	toNear 	- toNearR 	+ toNearT;		
			Vector3 nearRB = 	toNear 	+ toNearR 	- toNearT;	
			Vector3 nearRT = 	toNear 	+ toNearR 	+ toNearT;
			Vector3 focusLB = 	toFocus	- toFocusR 	- toFocusT;
			Vector3 focusLT = 	toFocus	- toFocusR 	+ toFocusT;
			Vector3 focusRB = 	toFocus	+ toFocusR 	- toFocusT;
			Vector3 focusRT = 	toFocus + toFocusR 	+ toFocusT;
			Vector3 farLB = 	toFar 	- toFarR 	- toFarT;
			Vector3 farLT = 	toFar 	- toFarR 	+ toFarT;
			Vector3 farRB = 	toFar	+ toFarR 	- toFarT;
			Vector3 farRT = 	toFar	+ toFarR 	+ toFarT;
			Vector3 nearCLB = 	toNearClip - toNearClipR - toNearClipT;
			Vector3 nearCLT = 	toNearClip - toNearClipR + toNearClipT;
			Vector3 nearCRB = 	toNearClip + toNearClipR - toNearClipT;
			Vector3 nearCRT = 	toNearClip + toNearClipR + toNearClipT;
			Vector3 farCLB = 	toFarClip - toFarClipR - toFarClipT;
			Vector3 farCLT = 	toFarClip - toFarClipR + toFarClipT;
			Vector3 farCRB = 	toFarClip + toFarClipR - toFarClipT;
			Vector3 farCRT = 	toFarClip + toFarClipR + toFarClipT;
			
			Gizmos.color = Color.white;
			
			// far clip rectangle
			Gizmos.DrawLine(farCLB, farCRB);
			Gizmos.DrawLine(farCLT, farCRT);
			Gizmos.DrawLine(farCLB, farCLT);
			Gizmos.DrawLine(farCRB, farCRT);
			
			// camera frustum
			Gizmos.DrawLine(nearCLB, farCLB);
			Gizmos.DrawLine(nearCRB, farCRB);
			Gizmos.DrawLine(nearCLT, farCLT);
			Gizmos.DrawLine(nearCRT, farCRT);
			
			// near rectangle
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(nearLB, nearRB);
			Gizmos.DrawLine(nearLB, nearLT);
			Gizmos.DrawLine(nearLT, nearRT);
			Gizmos.DrawLine(nearRB, nearRT);
			
			if (_dofFarLimit >= 0) {
				// far rectangle
				Gizmos.DrawLine(farLB, farRB);
				Gizmos.DrawLine(farLT, farRT);
				Gizmos.DrawLine(farLB, farLT);
				Gizmos.DrawLine(farRB, farRT);
				
				// dof frustum
				Gizmos.DrawLine(nearLB, farLB);
				Gizmos.DrawLine(nearRB, farRB);
				Gizmos.DrawLine(nearLT, farLT);
				Gizmos.DrawLine(nearRT, farRT);
			}
			
			// focus rectangle
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(focusLB, focusLT);
			Gizmos.DrawLine(focusLB, focusRB);
			Gizmos.DrawLine(focusRB, focusRT);
			Gizmos.DrawLine(focusLT, focusRT);
						
			Gizmos.DrawLine(Vector3.zero, toFocus);
			
			//Gizmos.matrix = Matrix4x4.TRS(
			//transform.position, transform.rotation, transform.lossyScale);
			
			if (ShowBody) {
				Gizmos.color = Color.green;
				Gizmos.DrawWireCube(
					Vector3.zero, new Vector3(0.01f, 0.01f, 0.02f));
				Gizmos.DrawWireCube(Vector3.forward * -0.1f, new Vector3(0.05f, 0.08f, 0.175f));
			}
		}
	}
	
	public void CallUpdate () {
		if (_focusDist < 0)														_focusDist = 0;
		if (_fstopIndex < 0 || _fstopIndex >= FStop.list.Length)				_fstopIndex = 0;

		// Populate lens data
		if(_lensFOVList == null)
		{
            ProCamsLensDataTable.FilmFormatData curFilmFormat = ProCamsLensDataTable.Instance.GetFilmFormat(_filmFormatName);
			if(curFilmFormat != null)
			{
				ProCamsLensDataTable.LensKitData lensKitData = curFilmFormat.GetLensKitData(_lensKitIndex);
				if(lensKitData != null)
				{
					// Set available lens data for this film format
					_lensFOVList = lensKitData._fovDataset.ToArray();
				}
			}
		}
		
		if (_lensIndex < 0 || _lensIndex >= _lensFOVList.Length)
			_lensIndex = 0;
		
		UpdateTransforms ();
		NodalCamera.m_Lens.FieldOfView = _lensFOVList[_lensIndex]._unityVFOV;
		UpdateDepthOfField ();
	}
	
	/// <summary>
	/// Updates the transforms.
	/// Moves the nodal point across the Z axis.
	/// </summary>
	void UpdateTransforms () {
		
		if (_rigTransform != null) {
			transform.position = _rigTransform.position;
			transform.rotation = _rigTransform.rotation;
		}
		
		//Transform t = _nodalCamera.transform;
		//t.position = t.parent.position + _fpOffset * 0.0254f + t.forward * (_lensFOVList[_lensIndex]._nodalOffset * 0.0254f);	
	}
	
	/// <summary>
	/// Updates the depth of field.
	/// Variables for dof change the original dof script.
	/// </summary>
	void UpdateDepthOfField () {
		
		CalculateDepthOfField();
	
		_dofComponents = _nodalCamera.GetComponentsInChildren<CSDOFScatter>();
		
		foreach (CSDOFScatter dofs in _dofComponents) 
        {
			dofs.aperture = _lensFOVList[_lensIndex]._focalLength / FStop.list[_fstopIndex].fstop;
            if (_unitOfMeasure == UnitOfMeasure.Imperial)
            {
                dofs.focalLength = ProCamsUtility.Convert(_focusDist, Units.Foot, Units.Meter);
            }
            else
            {
                dofs.focalLength = _focusDist;
            }
		}
	}
	
	/// <summary>
	/// Creates the nodal camera.
	/// Handles situations where the script is dragged onto an existing object.
	/// </summary>
	private void CreateNodalCamera () 
    {
		//GameObject go = new GameObject(name);
		//go.transform.parent = transform;
		
		this.gameObject.AddComponent<Camera>();
        this.gameObject.AddComponent<CSDOFScatter>();
	}
	
	/// <summary>
	/// Calculates the depth of field.
	/// </summary>
	private void CalculateDepthOfField () 
    {
		float F = ProCamsUtility.Convert(_lensFOVList[_lensIndex]._focalLength, 
			Units.Millimeter, 
			Units.Inch);
		
		float f = FStop.list[_fstopIndex].fstop;

        float S;
        if (_unitOfMeasure == UnitOfMeasure.Imperial)
        {
            S = ProCamsUtility.Convert(_focusDist, Units.Foot, Units.Inch);
        }
        else
        {
            S = ProCamsUtility.Convert(_focusDist, Units.Meter, Units.Inch);
        }
		
		float H = (F * F) / (f * 0.001f);
		
		float DN = (H * S) / ( H + (S - F));
		float DF = (H * S) / ( H - (S - F));
		float D = DF - DN;
		
		D = ProCamsUtility.Convert(D, Units.Inch, Units.Foot);
		D = ProCamsUtility.Truncate(D, 2);
		
		DN = ProCamsUtility.Convert(DN, Units.Inch, Units.Foot);
		DN = ProCamsUtility.Truncate(DN, 2);
		
		DF = ProCamsUtility.Convert(DF, Units.Inch, Units.Foot);
		DF = ProCamsUtility.Truncate(DF, 2);
		
		_dofDistTotal = D;
		_dofNearLimit = DN;
		_dofFarLimit = DF;
		
		S = ProCamsUtility.Convert (S, 
			Units.Inch, 
			Units.Meter);
	}
}
