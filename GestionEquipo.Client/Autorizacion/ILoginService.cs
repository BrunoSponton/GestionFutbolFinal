using GestionEquipo.Shared.DTO;

namespace GestionEquipo.Client.Autorizacion
{
    public interface ILoginService
    {
        Task Login(UserTokenDTO tokenDTO);
        Task Logout();
    }
}
