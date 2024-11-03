
namespace GestionEquipo.Client.Servicios
{
    public interface IHttpServicio
    {
        Task<HttpRespuesta<object>> Post<T>(string url, T entidad);
    }
}