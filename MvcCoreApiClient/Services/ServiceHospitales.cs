using MvcCoreApiClient.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MvcCoreApiClient.Services
{
    public class ServiceHospitales
    {
        //NECESITAMOS LA URL DE ACCESO AL SEVICIO, SOLO LA URL
        private string ApiUrl;
        //NECESITAMOS INDICAR A NUESTRO SERVICE QUE LEEMOS CODIGO JSON
        private MediaTypeWithQualityHeaderValue header;

        public ServiceHospitales(IConfiguration configuration)
        {
            this.ApiUrl = configuration.GetValue<string>("ApiUrls:ApiHospitales");
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<Hospital> FindHospitalAsync(int idHospital)
        {
            using(HttpClient client = new HttpClient())
            {
                string request = "api/hospitales/" + idHospital;
                client.BaseAddress = new Uri(this.ApiUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    //SI LAS PROPIEDADES SE LLAMAN IGUAL A LA LECTURA DE JSON, 
                    //NO ES NECESARIO MAPEAR CON LA DECORACION [JSONPROPERTY]
                    //Y NO LEEREMOS CON NEWTONSOFT
                    Hospital data = await response.Content.ReadAsAsync<Hospital>();
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }

        //CREAMOS UN METODO ASINCRONO PARA LEER LOS HOSPITALES
        public async Task<List<Hospital>> GetHospitalesAsync()
        {
            //SE UTILIZA LA CLASE HTTCLIENT PARA LAS PETICIONES AL SERVIDOR
            using(HttpClient client = new HttpClient())
            {
                //NECESITAMOS UNA PETICION
                string request = "api/hospitales";
                //INDICAMOS LA URL BASE PARA ACCEDER AL SEVICIO API
                client.BaseAddress = new Uri(this.ApiUrl);
                //COMO ES POSIBLE QUE SE CRUCEN PETICIONES ENTRE METODOS CON DISTINTAS INFORMACIONES,
                //DEBEMOS LIMPIAR LOS HEADER COMO NOMA
                client.DefaultRequestHeaders.Clear();
                //CREAMOS UN NUEVO HEADER PARA INDICAR QUE LEEREMOS JSON
                client.DefaultRequestHeaders.Accept.Add(this.header);
                //HACEMOS LA PETICION AL SERVICIO (GET) Y CAPTURAMOS LA RESPUESTA
                HttpResponseMessage response = await client.GetAsync(request);
                //EN LA RESPUESTA, SE OFRECEN DISTINTOS STATUS CODE
                if (response.IsSuccessStatusCode)
                {
                    //DESCARGAMOS EL JSON COMO STRING
                    string json = await response.Content.ReadAsStringAsync();
                    //UTILIZAREMOS NWETON PARA RECUPERAR LOS DATOS SERIALIZADOS DE JSON A LIST<HOSPITAL>
                    List<Hospital> data = JsonConvert.DeserializeObject<List<Hospital>>(json);
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
