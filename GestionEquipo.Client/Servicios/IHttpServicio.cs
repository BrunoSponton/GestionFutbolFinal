
namespace GestionEquipo.Client.Servicios
{
    public interface IHttpServicio
    {
        Task<HttpRespuesta<TResp>> Post<T, TResp>(string url, T entidad);
    }
}