using UnityEngine;

[CreateAssetMenu(fileName = "MultimeterSO", menuName = "ComponentData/MultimeterSO")]
public class MultimeterSO : ScriptableObject
{
    [SerializeField] public string multimeterName;
    [SerializeField] public string value;
    [SerializeField] public string type;
}
