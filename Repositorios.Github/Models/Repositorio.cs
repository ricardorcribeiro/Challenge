using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Repositorios.Github.Models
{
    public class Repositorio
    {
        public int Id { get; set; }

        [DisplayName("Nome dos Repositórios")]
        public string Nome { get; set; }
    }
}