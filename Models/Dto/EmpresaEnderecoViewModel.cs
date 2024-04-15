using System.Collections.Generic;
using AppPesquisaCnpj.Models;

namespace AppPesquisaCnpj.Models
{
    public class EmpresaEnderecoViewModel
    {
        public Empresa empresa { get; set; }
        public List<Endereco> Enderecos { get; set; }
    }
}
