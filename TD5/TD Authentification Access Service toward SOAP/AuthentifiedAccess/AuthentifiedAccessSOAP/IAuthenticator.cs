using System.ServiceModel;

namespace AuthentifiedAccessSOAP
{
    [ServiceContract]
    public interface IAuthenticator
    {
        [OperationContract]
        bool ValidateCredentials(string username, string password);
    }
}
