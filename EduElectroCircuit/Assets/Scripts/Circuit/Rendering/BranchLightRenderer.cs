using System.Collections;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BranchLightRenderer: MonoBehaviour
{
    [SerializeField] private GameObject _branchIN;
    [SerializeField] private GameObject _branchOUT;
    
    [SerializeField] private float _animDuration = 2.5f;
    private BranchConectionState _branchInState;
    private BranchConectionState _branchOutState;


    public void Awake() {
        if(_branchIN != null)
        {
            _branchInState = new BranchConectionState();
            _branchInState.InitBranchConectionState(_branchIN, _animDuration, this);
            _branchIN.GetComponent<Node_Branch>().InitLightRenderer(this);
        }
        _branchOutState = new BranchConectionState();
        _branchOutState.InitBranchConectionState(_branchOUT, _animDuration, this);

        _branchOUT.GetComponent<Node_Branch>().InitLightRenderer(this);

    }

    public void SetBranchConectionLight(bool[] branchConnected, bool? nodeConnected, NodeType type)
    {
        switch(type)
        {
            case NodeType.BRANCH_OUT:
                _branchOutState.UpdateConectionState(branchConnected, nodeConnected, type);
                _branchOutState.CheckNodeStateChanged();
            break;
            case NodeType.BRANCH_IN:
                if(_branchIN != null)
                {
                    _branchInState.UpdateConectionState(branchConnected, nodeConnected, type);
                    _branchInState.CheckNodeStateChanged();
                }
            break;
        }
    }
}