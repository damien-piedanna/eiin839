using System;
using System.ServiceModel;

namespace AuthentifiedAccessSOAP
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
    [ServiceContract]
    public interface IServiceAccess
    {
        [OperationContract]
        String GetWeather(string city);
    }
}
