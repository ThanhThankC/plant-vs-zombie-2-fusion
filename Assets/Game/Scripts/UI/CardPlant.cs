using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SelectionCard))]
public class CardPlant : MonoBehaviour, IDraggableButton, ISelectableButton
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Image selectedFrame;

    [Header("Cooldown Mask")]
    [SerializeField] private GameObject cooldownMask;
    [SerializeField] private Image cooldownFillImage;

    [Header("Sun Mask")]
    [SerializeField] private GameObject sunMask;

    public SelectionCard SelectionCard { get; private set; }

    private PlantManager plantManager;
    private DragController dragController;
    private SunManager sunManager;
    private PlantData data;
    private PlantType plantType;

    private bool enableBattle;
    private float totalCooldownDuration;
    private float cooldownTimer;
    private bool isOnCooldown;

    private void Awake()
    {
        SelectionCard = GetComponent<SelectionCard>();
    }

    private void Start()
    {
        plantManager = PlantManager.Instance;
        dragController = DragController.Instance;
        sunManager = SunManager.Instance;
    }

    public void Init(PlantData data)
    {
        if (data == null) return;
        this.data = data;
        plantType = data.plantType;
        if (cardImage != null) cardImage.sprite = data.cardSprite;

    }

    public void EnableBattle()
    {
        enableBattle = true;
        dragController.OnDragEnd += Deselect;
        sunManager.OnSunChanged += OnSunChanged;
        StartCooldown(data.firstcooldown);
    }

    private void Update()
    {
        if (!isOnCooldown) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownFillImage != null && totalCooldownDuration != 0)
            cooldownFillImage.fillAmount = Mathf.Clamp01(cooldownTimer / totalCooldownDuration);

        if (cooldownTimer <= 0)
            EndCooldown();
    }

    private void OnDestroy()
    {
        if (!enableBattle) return;
        dragController.OnDragEnd -= Deselect;
        sunManager.OnSunChanged -= OnSunChanged;
    }

    private void StartCooldown(float duration)
    {
        if (!CanUse())
        {
            AudioManager.Instance.PlayCardDenied();
            return;
        }

        AudioManager.Instance.PlayCardSelect();

        isOnCooldown = true;
        cooldownTimer = duration;
        totalCooldownDuration = duration;

        if (cooldownMask != null) cooldownMask.SetActive(true);
        if (cooldownFillImage != null) cooldownFillImage.fillAmount = 1f;
    }

    private void EndCooldown()
    {
        if (!CanUse())
        {
            AudioManager.Instance.PlayCardDenied();
            return;
        }

        AudioManager.Instance.PlayCardSelect();

        isOnCooldown = false;
        cooldownTimer = 0;

        if (cooldownMask != null) cooldownMask.SetActive(false);
        if (cooldownFillImage != null) cooldownFillImage.fillAmount = 0f;
    }

    private void OnSunChanged(int currentSun) => UpdateSunMask(currentSun);

    private void UpdateSunMask(int currentSun)
    {
        if (sunMask == null || data == null) return;
        sunMask.SetActive(currentSun < data.sunCost);
    }

    private bool CanUse()
    {
        if (isOnCooldown) return false;
        if (sunManager.TotalSun < data.sunCost) return false;
        return true;
    }

    public void TriggerCooldown()
    {
        StartCooldown(data.cooldown);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!enableBattle) return;

        dragController.RefreshTool();
        plantManager.OnCardClicked(plantType);
        dragController.BeginDrag(ToolType.None);
        Select();
        plantManager.ToggleGhostPlantVisual(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!enableBattle) return;

        dragController.RefreshTool();
        plantManager.OnCardClicked(plantType);
        dragController.BeginDrag(ToolType.None);
        Select();
        plantManager.ToggleGhostPlantVisual(true);
    }

    public void OnDrag(PointerEventData eventData) 
    {
        if (!dragController.IsDragging) return;
        CanvasInputReceiver.Instance.OnDrag(eventData); 
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if (!dragController.IsDragging) return;
        CanvasInputReceiver.Instance.OnEndDrag(eventData);
    }

    public void Select() => selectedFrame?.gameObject.SetActive(true);

    public void Deselect() => selectedFrame?.gameObject.SetActive(false);
} 
