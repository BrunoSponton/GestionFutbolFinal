using System.Text.Json;
using System.Text;

namespace GestionEquipo.Client.Servicios
{
    public class HttpServicio : IHttpServicio
    {
        private readonly HttpClient http;

        public HttpServicio(HttpClient http)
        {
            this.http = http;
        }

        public async Task<HttpRespuesta<TResp>> Post<T, TResp>(string url, T entidad)
        {
            var enviarJson = JsonSerializer.Serialize(entidad);

            var enviarContent = new StringContent(enviarJson,
                                Encoding.UTF8,
                                "application/json");

            var response = await http.PostAsync(url, enviarContent);
            if (response.IsSuccessStatusCode)
            {
                var respuesta = await DesSerializar<TResp>(response);
                return new HttpRespuesta<TResp>(respuesta, false, response);
            }
            else
            {
                return new HttpRespuesta<TResp>(default, true, response);
            }
        }

        private async Task<T?> DesSerializar<T>(HttpResponseMessage response)
        {
            var respuestaStr = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(respuestaStr,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }
    }
}
