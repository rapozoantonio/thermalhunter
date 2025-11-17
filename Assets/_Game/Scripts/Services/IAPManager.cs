using UnityEngine;

/// <summary>
/// In-App Purchase manager for premium content
/// Handles IAP initialization and purchase processing
/// </summary>
public class IAPManager : Singleton<IAPManager>
{
    [Header("IAP Settings")]
    [SerializeField] private bool enableIAP = true;

    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();

        if (enableIAP)
        {
            InitializePurchasing();
        }

        // Register with ServiceLocator
        ServiceLocator.Instance.Register<IAPManager>(this);
    }

    private void InitializePurchasing()
    {
        // Unity IAP initialization would go here
        // var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        // builder.AddProduct(Constants.IAP_REMOVE_ADS, ProductType.NonConsumable);
        // builder.AddProduct(Constants.IAP_PREMIUM_SCOPE_PACK, ProductType.NonConsumable);
        // builder.AddProduct(Constants.IAP_STARTER_PACK, ProductType.Consumable);
        // UnityPurchasing.Initialize(this, builder);

        Debug.Log("[IAPManager] Initializing IAP system");

        // Simulate initialization
        isInitialized = true;

        // Check for existing purchases
        CheckPremiumStatus();
    }

    private void CheckPremiumStatus()
    {
        // Check if user has already purchased premium features
        // This would check receipts in production
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            Debug.Log($"[IAPManager] Ads Removed: {saveManager.CurrentSave.adsRemoved}");
        }
    }

    // === Purchase Methods ===

    public void PurchaseRemoveAds()
    {
        BuyProductID(Constants.IAP_REMOVE_ADS);
    }

    public void PurchasePremiumScopes()
    {
        BuyProductID(Constants.IAP_PREMIUM_SCOPE_PACK);
    }

    public void PurchaseStarterPack()
    {
        BuyProductID(Constants.IAP_STARTER_PACK);
    }

    private void BuyProductID(string productId)
    {
        if (!isInitialized)
        {
            Debug.LogError("[IAPManager] IAP not initialized!");
            ShowPurchaseFailedDialog("Store not ready. Please try again.");
            return;
        }

#if UNITY_STANDALONE
        Debug.LogWarning("[IAPManager] IAP not available on PC");
        ShowPurchaseFailedDialog("In-app purchases are not available on PC.");
        return;
#endif

        Debug.Log($"[IAPManager] Initiating purchase: {productId}");

        // Unity IAP implementation would go here
        // Product product = storeController.products.WithID(productId);
        // if (product != null && product.availableToPurchase)
        // {
        //     storeController.InitiatePurchase(product);
        // }

        // Simulate purchase for development
        SimulatePurchase(productId);
    }

    private void SimulatePurchase(string productId)
    {
        Debug.Log($"[IAPManager] Simulating purchase: {productId}");

        // Simulate purchase delay
        this.InvokeDelayed(() =>
        {
            OnPurchaseComplete(productId, GetProductPrice(productId));
        }, 1f);
    }

    private decimal GetProductPrice(string productId)
    {
        switch (productId)
        {
            case Constants.IAP_REMOVE_ADS:
                return (decimal)Constants.IAP_REMOVE_ADS_PRICE;
            case Constants.IAP_PREMIUM_SCOPE_PACK:
                return (decimal)Constants.IAP_PREMIUM_SCOPES_PRICE;
            case Constants.IAP_STARTER_PACK:
                return (decimal)Constants.IAP_STARTER_PACK_PRICE;
            default:
                return 0m;
        }
    }

    private void OnPurchaseComplete(string productId, decimal price)
    {
        Debug.Log($"[IAPManager] Purchase completed: {productId}");

        // Process purchase
        switch (productId)
        {
            case Constants.IAP_REMOVE_ADS:
                ProcessRemoveAdsPurchase();
                break;

            case Constants.IAP_PREMIUM_SCOPE_PACK:
                ProcessPremiumScopesPurchase();
                break;

            case Constants.IAP_STARTER_PACK:
                ProcessStarterPackPurchase();
                break;
        }

        // Track analytics
        AnalyticsManager analytics = ServiceLocator.Instance.TryGet<AnalyticsManager>();
        analytics?.TrackPurchase(productId, price);
        analytics?.TrackIAPAttempt(productId, true);

        // Publish event
        EventBus.Publish(new PurchaseCompletedEvent
        {
            ProductID = productId,
            Price = price
        });

        ShowPurchaseSuccessDialog(productId);
    }

    private void ProcessRemoveAdsPurchase()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            saveManager.SetAdsRemoved(true);
        }

        Debug.Log("[IAPManager] Ads removed permanently");
    }

    private void ProcessPremiumScopesPurchase()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            saveManager.UnlockScope("MilitaryGrade");
            saveManager.UnlockScope("TacticalPro");
            saveManager.UnlockScope("EliteHunter");
        }

        Debug.Log("[IAPManager] Premium scopes unlocked");
    }

    private void ProcessStarterPackPurchase()
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager != null)
        {
            saveManager.AddCurrency(5000);
            saveManager.UnlockWeapon("AdvancedAirRifle");
            saveManager.UnlockScope("MidRangeThermal");
        }

        Debug.Log("[IAPManager] Starter pack granted");
    }

    // === UI Dialogs ===

    private void ShowPurchaseSuccessDialog(string productId)
    {
        string message = productId switch
        {
            Constants.IAP_REMOVE_ADS => "Ads have been removed permanently!",
            Constants.IAP_PREMIUM_SCOPE_PACK => "Premium scopes unlocked!",
            Constants.IAP_STARTER_PACK => "Starter pack granted!",
            _ => "Purchase successful!"
        };

        Debug.Log($"[IAPManager] {message}");
        // UIManager.Instance?.ShowDialog("Success!", message);
    }

    private void ShowPurchaseFailedDialog(string reason)
    {
        Debug.LogWarning($"[IAPManager] Purchase failed: {reason}");
        // UIManager.Instance?.ShowDialog("Purchase Failed", reason);
    }

    // === Query Methods ===

    public bool IsProductPurchased(string productId)
    {
        SaveManager saveManager = SaveManager.Instance;
        if (saveManager == null) return false;

        switch (productId)
        {
            case Constants.IAP_REMOVE_ADS:
                return saveManager.CurrentSave.adsRemoved;

            case Constants.IAP_PREMIUM_SCOPE_PACK:
                return saveManager.IsScopeUnlocked("MilitaryGrade");

            default:
                return false;
        }
    }

    public string GetProductPrice(string productId, bool formatted = true)
    {
        decimal price = GetProductPrice(productId);
        return formatted ? $"${price:F2}" : price.ToString();
    }

    // === IStoreListener (would be implemented) ===
    // public void OnInitialized(IStoreController controller, IExtensionProvider extensions) { }
    // public void OnInitializeFailed(InitializationFailureReason error) { }
    // public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) { }
    // public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) { }
}
