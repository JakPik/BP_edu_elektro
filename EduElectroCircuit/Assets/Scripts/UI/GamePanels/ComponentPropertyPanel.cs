using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ComponentPropertyPanel : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private GameObject _container;
    [SerializeField] private float _animDuration = 0.5f;
    private GameObject _mainCamera;
    private bool _visible = false;

    private Label _title;

    private ComponentPropertyBlock[] _visualElement;
    private VisualElement _root;

    private Coroutine _curCoroutine;
    private Coroutine _updateCoroutine;

    void Awake()
    {
        _root = _uiDocument.rootVisualElement.Q<VisualElement>("Display");
        _root.style.opacity = 0.0f;
        _title = _uiDocument.rootVisualElement.Q<Label>("Title");

        _visualElement = new ComponentPropertyBlock[4];

        for(int i = 0; i < 4; i++)
        {
            _visualElement[i]._block = _uiDocument.rootVisualElement.Q<VisualElement>("Property_" + (i+1));
            _visualElement[i]._label = _visualElement[i]._block.Q<Label>("Label");
            _visualElement[i]._value = _visualElement[i]._block.Q<Label>("Value");
            _visualElement[i]._block.style.display = DisplayStyle.None;
        }
        this.enabled = false;
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public void SetData(ComponentPropertyData[] componentPropertyDatas, string title)
    {
        if(_root == null) return;
        _title.text = title;
        int id = 0;
        foreach(ComponentPropertyData data in componentPropertyDatas)
        {
            _visualElement[id]._block.style.display = DisplayStyle.Flex;
            _visualElement[id]._label.text = data.label;
            _visualElement[id]._value.text = data.value;
            id++;
        }
    }

    public void Interact()
    {
        _visible = !_visible;
        if(_curCoroutine != null) StopCoroutine(_curCoroutine);
        if(_visible)
        {
            this.enabled = true;
            _updateCoroutine = StartCoroutine(UpdatePanelOrientation());
            _curCoroutine = StartCoroutine(ShowPanelAnim());
        }
        else
        {
            if(_updateCoroutine != null) StopCoroutine(_updateCoroutine);
            _curCoroutine = StartCoroutine(HidePanelAnim());
        }
    }

    public void ResetInteraction()
    {
        _visible = false;
        if(_updateCoroutine != null) StopCoroutine(_updateCoroutine);
        if(_curCoroutine != null) StopCoroutine(_curCoroutine);

        _curCoroutine = StartCoroutine(HidePanelAnim());
    }
    private IEnumerator ShowPanelAnim()
    {
        float startOpacity = _root.resolvedStyle.opacity;
        float elapsed = 0f;

        while (elapsed < _animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _animDuration);

            _root.style.opacity = Mathf.Lerp(startOpacity, 1.0f, t);

            yield return null;
        }
        _root.style.opacity = 1.0f;
        _curCoroutine = null;
    }

    private IEnumerator HidePanelAnim()
    {
        float startOpacity = _root.resolvedStyle.opacity;
        float elapsed = 0f;

        while (elapsed < _animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _animDuration);

            _root.style.opacity = Mathf.Lerp(startOpacity, 0.0f, t);

            yield return null;
        }
        _root.style.opacity = 0.0f;
        this.enabled = false;
        _curCoroutine = null;
    }

    private IEnumerator UpdatePanelOrientation()
    {
        while (true)
        {
            Vector3 direction = _container.transform.position - _mainCamera.transform.position;
            if(direction.magnitude >= 3f) break;
            if(direction.magnitude < 1.0f) 
            {
                yield return null;
                continue;
            }
            direction.y = 0f;
            direction = direction.normalized;
            Quaternion local = Quaternion.LookRotation(direction);
            Vector3 forward = local * Vector3.forward;
            _container.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            yield return null;
        }
        _updateCoroutine = null;
        ResetInteraction();
    }
}
