using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;
using Repositorios.Github.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Repositorios.Github.Controllers
{
    public class HomeController : Controller
    {

        //variáveis populadas por valores no Web.config
        private static string client_id = new AppSettingsReader().GetValue("client_id", typeof(string)).ToString();
        private static string client_secret = new AppSettingsReader().GetValue("client_secret", typeof(string)).ToString();
        private static string urlAccess_token = new AppSettingsReader().GetValue("urlAccess_token", typeof(string)).ToString();
        private static string urlRepositorio = new AppSettingsReader().GetValue("urlRepositorio", typeof(string)).ToString();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sucesso(string code)
        {
            try
            {
               string result = ObterAccessToken(code);

                var repositoriosList = ObterListaRepositorios(result);

                return View(repositoriosList);

            }
            catch (Exception ex)
            {
                return View();
            }           
        }


        private string ObterAccessToken(string code) {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlAccess_token);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    client_id = client_id,
                    client_secret = client_secret,
                    code = code
                });

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }


        private List<Repositorio> ObterListaRepositorios(string acessToken)
        {
            List<Repositorio> repositoriosList = new List<Repositorio>();
            HttpWebRequest request = WebRequest.Create(urlRepositorio) as HttpWebRequest;
            request.UserAgent = "Lista de Repositorio";
            int inicioString = acessToken.IndexOf('=') + 1;
            int fimString = acessToken.IndexOf("&scope") - inicioString;
            request.Headers.Add("Authorization", "Bearer " + acessToken.Substring(inicioString, fimString));
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                dynamic repositorio = JsonConvert.DeserializeObject(reader.ReadToEnd());

                //estou usando a estrutura de repetição para mantar o objeto do modelo para retornar pra view 
                foreach (var item in repositorio)
                {
                    repositoriosList.Add(new Repositorio
                    {
                        Id = item.id,
                        Nome = item.name
                    });
                }

            }

            return repositoriosList;
        }
    }
}