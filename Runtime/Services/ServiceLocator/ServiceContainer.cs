using System;
using System.Collections.Generic;


namespace CodeBase.Infractructure
{
    public class ServiceContainer
    {
        private static Dictionary<Type, IService> allService = new Dictionary<Type, IService>();

        public static void RegisterSingle<TType>(IService implementation)
        {
            if (allService.ContainsKey(typeof(TType)) == false)
                allService.Add(typeof(TType), implementation);
        }

        public static TType Single<TType>() where TType : IService
        {
            allService.TryGetValue(typeof(TType), out IService service);

            if (service == null)
            {
                throw new NullReferenceException($"Serivce { typeof(TType) } not exist in container!");
            }

            return (TType) service;

        }
        
        public static void Unregister<TType>()
        {
            allService.Remove(typeof(TType));
        }
    }
}