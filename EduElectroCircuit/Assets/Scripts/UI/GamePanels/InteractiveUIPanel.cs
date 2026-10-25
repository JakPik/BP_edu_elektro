using UnityEngine;
using UnityEngine.UIElements;

public abstract class InteractiveUIPanel: UIPanel, IInteractable
{
    protected Vector2 worldToPanel;
    protected UIButton currentUIElement;

    protected void SetLocalCoordConverion()
    {
        BoxCollider collider = display.GetComponent<BoxCollider>();

        float worldWidth = collider.size.x;
        float worldHeight = collider.size.y;

        Vector2 resolution = display.worldSpaceSize;

        float panelWidth = resolution.x;
        float panelHeight = resolution.y;

        worldToPanel.x = panelWidth / worldWidth;
        worldToPanel.y = panelHeight / worldHeight;
    }
    
    protected Vector2? GetPointOnPanel(Vector3 worldHitPoint, Camera camera)
    {
        IPanel panel = display.rootVisualElement.panel;

        if (panel == null || camera == null)
            return null;

        Vector3 localPoint =
            display.transform.InverseTransformPoint(worldHitPoint);


        return new Vector2(
            localPoint.x * worldToPanel.x,
            localPoint.y * worldToPanel.y
        );
    }

    public virtual void OnInteract(bool resetInteraction = false)
    {
        if(currentUIElement != null) {
            currentUIElement.ButtonInteraction(this);
        }
    }

    public virtual bool CanInteract() => currentUIElement != null;

    public virtual void DisplayInfo(bool display, RaycastHit hit = default, Camera camera = null)
    {
        throw new System.NotImplementedException();
    }

    public abstract void PanelButtonReset();
    public abstract void PanelButtonSave();
    public abstract void PanelButtonReload(bool circuitActive);
}

