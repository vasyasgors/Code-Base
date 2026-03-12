using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace CodeBase.Infrastructure
{
    // Result, ProductConfig, Product, Purchase
    public interface IPayments : IService
    {
        Task<TResult> Init<TResult, TProductConfig>(List<TProductConfig> allProducts);
        Task<TPurchase[]> LoadPlayerPurchases<TPurchase>();
        void StartPurchase<TPurchase>(string productID, Action<TPurchase> onPurchased = null, Action onError = null);
        Task<bool> ConsumePlayerPurchase(bool forceAll = false);
        Task<TProductList> LoadCatalog<TProductList>();
        Task<TProduct> GetProductById<TProduct>(string pruductID);
        Task<bool> IsPurchased(string productId);
        TProductConfig GetProductConfigByPurchase<TPurchase, TProductConfig>(TPurchase purchase);
    }
}





