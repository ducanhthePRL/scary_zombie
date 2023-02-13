using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Purchasing.Security;
using System.Collections;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

public class IAPManager : SingletonMonoDontDestroy<IAPManager>, IStoreListener
{
    private string productId;
    private string productName;
    private string price;
    private string currency;

    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public UnityAction OnBuyDone = null, OnBuyFail = null;

    private bool isBuying = false;
    private CrossPlatformValidator m_Validator = null;
    private Action OnInitDone = null;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void Init(Action action)
    {
        if (IsInitialized()) return;
        OnInitDone = action;
        StopAllCoroutines();
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    private void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        var catalog = ProductCatalog.LoadDefaultCatalog();
        foreach (var product in catalog.allValidProducts)
        {
            if (product.allStoreIDs.Count > 0)
            {
                var ids = new IDs();
                foreach (var storeID in product.allStoreIDs)
                {
                    ids.Add(storeID.id, storeID.store);
                }
                builder.AddProduct(product.id, product.type, ids);

            }
            else
            {
                builder.AddProduct(product.id, product.type);
            }

        }
        // Continue adding the non-consumable product.

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 


        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.

        UnityPurchasing.Initialize(this, builder);
        //try
        //{
        //    var options = new InitializationOptions().SetEnvironmentName("production");
        //    UnityServices.InitializeAsync(options).ContinueWith(task => Debug.LogError("Init unity services success"));
        //}
        //catch (Exception exception)
        //{
        //    // An error occurred during initialization.
        //    Debug.LogError("Init unity services failed : " + exception.StackTrace);
        //}
    }


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public bool CheckBought(string ID)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(ID);
            if (product != null && product.hasReceipt)
            {
                return true;
            }
        }
        return false;
    }


    public void BuyProductID(string productId, string product_name, string price, string currency)
    {
        if (isBuying) return;
        if (!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(product_name))
            Debug.LogError("buy productId : " + productId + " - product_name : " + product_name);
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            isBuying = true;
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            Hashtable hashtable = new Hashtable();
            hashtable.Add("package_id", product != null && !string.IsNullOrEmpty(product.definition.id) ? product.definition.id : "");
            hashtable.Add("package_name", string.IsNullOrEmpty(product_name) ? "" : product_name);
            hashtable.Add("package_price", string.IsNullOrEmpty(price) ? "" : price);
            hashtable.Add("package_currency", string.IsNullOrEmpty(currency) ? "" : currency);
            if (product != null)
                this.productId = product.definition.id;
            this.productName = product_name;
            this.price = price;
            this.currency = currency;
            if (product != null && product.availableToPurchase)
            {
                //Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                FirebaseManager.instance.LogEvent("start_purchase_item_iap", hashtable);
                //NoticeManager.Instance.LogNotice(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product, productId);
            }
            // Otherwise ...
            else
            {
                isBuying = false;
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                FirebaseManager.instance.LogEvent("purchase_item_iap_not_available", hashtable);
                //NoticeManager.Instance.LogNotice("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
            popUpNotice.OnSetTextOneButton("Error", "Something went wrong!");
            isBuying = false;
            Debug.Log("BuyProductID FAIL. Not initialized.");
            //NoticeManager.Instance.LogNotice("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            //NoticeManager.Instance.LogNotice("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                if (result) {
                    UserDatas.SetBoughtRemoveAds(CheckBought("iap.removeads"), true);
                }
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        //Debug.Log("OnInitialized: PASS");
        //NoticeManager.Instance.LogNotice("OnInitialized: PASS");
        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
        InitializeValidator();
        UserDatas.SetBoughtRemoveAds(CheckBought("iap.removeads"), false);
        OnInitDone?.Invoke();
        OnInitDone = null;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitDone?.Invoke();
        OnInitDone = null;
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        //NoticeManager.Instance.LogNotice("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;

        var isPurchaseValid = IsPurchaseValid(product);
        isBuying = false;
        if (isPurchaseValid)
        {
            if (OnBuyDone != null)
            {
                OnBuyDone.Invoke();
                OnBuyDone = null;
            }

            if (!string.IsNullOrEmpty(productId) && args.purchasedProduct.definition.id.Equals(productId))
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("package_id", product != null && !string.IsNullOrEmpty(productId) ? productId : "");
                hashtable.Add("package_name", string.IsNullOrEmpty(productName) ? "" : productName);
                hashtable.Add("package_price", string.IsNullOrEmpty(price) ? "" : price);
                hashtable.Add("package_currency", string.IsNullOrEmpty(currency) ? "" : currency);
                FirebaseManager.instance.LogEvent("event_purchase_package", hashtable);
            }
        }
        else
        {
            OnBuyDone = null;
            if (OnBuyFail != null)
            {
                OnBuyFail.Invoke();
                OnBuyFail = null;
            }
        }
        return PurchaseProcessingResult.Complete;
    }

    private void InitializeValidator()
    {
        if (IsCurrentStoreSupportedByValidator())
        {
#if !UNITY_EDITOR
            m_Validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
#endif
        }
        else
        {
            //PopupNotice popupNotice = PanelManager.Show<PopupNotice>();
            Debug.LogError("Store is not supported");
        }
    }

    private bool IsPurchaseValid(Product product)
    {
#if UNITY_EDITOR
        return true;
#else
        //If we the validator doesn't support the current store, we assume the purchase is valid
        if (IsCurrentStoreSupportedByValidator())
        {
            try
            {
                var result = m_Validator.Validate(product.receipt);
                //The validator returns parsed receipts.
                //LogReceipts(result);
            }
            //If the purchase is deemed invalid, the validator throws an IAPSecurityException.
            catch (IAPSecurityException reason)
            {
                Debug.Log($"Invalid receipt: {reason}");
                return false;
            }
        }

        return true;
#endif
    }

    private bool IsCurrentStoreSupportedByValidator()
    {
        //The CrossPlatform validator only supports the GooglePlayStore and Apple's App Stores.
        return IsGooglePlayStoreSelected() || IsAppleAppStoreSelected();
    }

    private bool IsGooglePlayStoreSelected()
    {
        var currentAppStore = StandardPurchasingModule.Instance().appStore;
        return currentAppStore == AppStore.GooglePlay;
    }

    private bool IsAppleAppStoreSelected()
    {
        var currentAppStore = StandardPurchasingModule.Instance().appStore;
        return currentAppStore == AppStore.AppleAppStore ||
               currentAppStore == AppStore.MacAppStore;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        isBuying = false;
        OnBuyDone = null;
        if (OnBuyFail != null)
        {
            OnBuyFail.Invoke();
            OnBuyFail = null;
        }

        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

    }
}