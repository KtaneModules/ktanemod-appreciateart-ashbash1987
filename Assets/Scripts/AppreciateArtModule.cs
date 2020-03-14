using System.Collections;
using UnityEngine;

public class AppreciateArtModule : MonoBehaviour
{
    public float MinimumAppreciationTime;
    public float MaximumAppreciationTime;

    public float MaximumAppreciationDistance;
    public float MaximumAppreciationAngle;
    public float MaximumAppreciationSeparationAngle;

    public AudioClip[] AppreciationStartedClip;
    public AudioClip AppreciationAmbientClip;
    public AudioClip AppreciationInterruptedClip;
    public AudioClip AppreciationEnlightenedClip;

    public Material PostProcessMaterial;

    private bool IsAppreciatingArt
    {
        get
        {
            Vector3 position = _transform.position;
            Vector3 cameraPosition = _mainCameraTransform.position;
            Vector3 facing = _transform.up;
            Vector3 cameraFacing = _mainCameraTransform.forward;

            float distanceToArt = Vector3.Distance(position, cameraPosition);
            float angleToArt = Vector3.Angle(-facing, cameraFacing);
            float separationAngleToArt = Vector3.Angle(cameraFacing, (position - cameraPosition).normalized);

            return distanceToArt <= MaximumAppreciationDistance &&
                   angleToArt <= MaximumAppreciationAngle &&
                   separationAngleToArt <= MaximumAppreciationSeparationAngle;
        }
    }

    private Transform _transform = null;
    private Transform _mainCameraTransform = null;
    private KMBombModule _module = null;
    private KMAudio _audio = null;
    private KMAudio.KMAudioRef _ambientRef = null;
    private CameraPostProcess _postProcess = null;

    private float? _appreciationStartTime = null;
    private float _appreciationRequiredDuration = 0.0f;
    private float _defaultGameMusicVolume = 0.0f;
    private bool _solved = false;

    private void Awake()
    {
        _module = GetComponent<KMBombModule>();
        _module.GenerateLogFriendlyName();
        _module.Log("There is some art to appreciate.");

        _audio = GetComponent<KMAudio>();

        _transform = transform;
        _mainCameraTransform = Camera.main.transform;
        _appreciationRequiredDuration = Random.Range(MinimumAppreciationTime, MaximumAppreciationTime);

        _defaultGameMusicVolume = GameMusicControl.GameMusicVolume;
    }

    private void Update()
    {
        if (!_solved)
        {
            UpdateForArtAppreciation();
        }
        else
        {
            UpdateForExcessiveArtAppreciation();
        }
    }

    private void UpdateForArtAppreciation()
    {
        if (IsAppreciatingArt)
        {
            if (_appreciationStartTime.HasValue)
            {
                if (Time.time - _appreciationStartTime >= _appreciationRequiredDuration)
                {
                    Enlighten();
                }
            }
            else
            {
                StartAppreciatingArt();
            }
        }
        else if (_appreciationStartTime.HasValue)
        {
            StopAppreciatingArt();
        }
    }

    private void UpdateForExcessiveArtAppreciation()
    {

    }

    private void StartAppreciatingArt()
    {
        _module.Log("Art is being appreciated.");
        _appreciationStartTime = Time.time;

        _audio.PlaySoundAtTransform(AppreciationStartedClip.RandomPick().name, _transform);
        _ambientRef = _audio.PlaySoundAtTransformWithRef(AppreciationAmbientClip.name, _transform);

        GameMusicControl.GameMusicVolume = 0.0f;

        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    private void StopAppreciatingArt()
    {
        StopAmbient();
        StopAllCoroutines();
        StartCoroutine(FadeOut(100000.0f));

        _module.Log("Art was not sufficiently appreciated. More appreciation time will be required.");
        _audio.PlaySoundAtTransform(AppreciationInterruptedClip.name, _transform);

        _appreciationStartTime = null;
    }

    private void Enlighten()
    {
        StopAmbient();
        StopAllCoroutines();
        StartCoroutine(FadeOut());

        _module.Log("Art has been sufficiently appreciated. You feel enlightened.");
        _module.HandlePass();

        _solved = true;

        _audio.PlaySoundAtTransform(AppreciationEnlightenedClip.name, _transform);
    }

    private void StopAmbient()
    {
        if (_ambientRef != null)
        {
            _ambientRef.StopSound();
            _ambientRef = null;
        }

        GameMusicControl.GameMusicVolume = _defaultGameMusicVolume;
    }

    private IEnumerator FadeIn(float speed = 1.0f)
    {
        if (_postProcess != null)
        {
            DestroyImmediate(_postProcess);
        }

        _postProcess = _mainCameraTransform.gameObject.AddComponent<CameraPostProcess>();
        _postProcess.PostProcessMaterial = new Material(PostProcessMaterial);

        for (float progress = 0.0f; progress < 1.0f; progress += Time.deltaTime * speed)
        {
            _postProcess.Vignette = progress * 1.6f;
            _postProcess.Grayscale = progress * 0.35f;

            yield return null;
        }

        _postProcess.Vignette = 1.6f;
        _postProcess.Grayscale = 0.35f;
    }

    private IEnumerator FadeOut(float speed = 1.0f)
    {
        for (float progress = 1.0f - Time.deltaTime * speed; progress >= 0.0f; progress -= Time.deltaTime * speed)
        {
            _postProcess.Vignette = progress * 1.6f;
            _postProcess.Grayscale = progress * 0.35f;

            yield return null;
        }

        if (_postProcess != null)
        {
            DestroyImmediate(_postProcess);
            _postProcess = null;
        }
    }
}
