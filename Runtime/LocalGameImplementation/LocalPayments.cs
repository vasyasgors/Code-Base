using CodeBase.Infrastructure;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace CodeBase
{
    public class LocalPayments : IPayments
    {
        public Task<bool> ConsumePlayerPurchase(bool forceAll = false)
        {
            throw new NotImplementedException();
        }

        public Task<TProduct> GetProductById<TProduct>(string pruductID)
        {
            throw new NotImplementedException();
        }

        public TProductConfig GetProductConfigByPurchase<TPurchase, TProductConfig>(TPurchase purchase)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> Init<TResult, TProductConfig>(List<TProductConfig> allProducts)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsPurchased(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<TProductList> LoadCatalog<TProductList>()
        {
            throw new NotImplementedException();
        }

        public Task<TPurchase[]> LoadPlayerPurchases<TPurchase>()
        {
            throw new NotImplementedException();
        }

        public void StartPurchase<TPurchase>(string productID, Action<TPurchase> onPurchased = null, Action onError = null)
        {
            throw new NotImplementedException();
        }
    }

}

