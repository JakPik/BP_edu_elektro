using System;
using UnityEngine;


[CreateAssetMenu(fileName = "ReloadRequestEventChannel", menuName = "Events/ReloadRequestEventChannel")]
public class ReloadRequestEventChannel : GenericEventChannel<ReloadRequestEvent>
{
}