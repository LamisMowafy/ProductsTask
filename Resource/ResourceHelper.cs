using System.Reflection;
using System.Resources;

namespace Resource
{
    public class ResourceHelper : IResourceHelper
    {
        private ResourceManager ResourceManager { get; set; }
        public ResourceHelper()
        {
        }
        public ResourceHelper(string resourceName)
        {
            ResourceManager = new ResourceManager(string.Format("{0}.{1}", "ProductTask.Resources", resourceName), Assembly.GetExecutingAssembly());
        }
        public string? Product(string message)
        {
            return ProductResource.ResourceManager.GetString(message);
        }
        public string? User(string message)
        {
            return UserResource.ResourceManager.GetString(message);
        }
        public string? Promotion(string message)
        {
            return PromotionResource.ResourceManager.GetString(message);
        }
        public string? Shared(string message)
        {
            return SharedResources.ResourceManager.GetString(message);
        }
        public string? ProductPromotion(string message)
        {
            return ProductPromotionResource.ResourceManager.GetString(message);
        }
        
    }
}
