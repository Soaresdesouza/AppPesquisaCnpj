using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace AppPesquisaCnpj.Models;

public class Empresa
{
    [Key] 
    public int id { get; set; }

    [Required] 
    [StringLength(14)] 
    public string cnpj { get; set; }

    [Required] 
    [StringLength(100)] 
    public string nome { get; set; }
    [Required]
    [StringLength(200)] 
    [JsonPropertyName("atividade_principal")]
    public string atividadePrincipal { get; set; }

[JsonPropertyName("enderecos")]
    public virtual ICollection<Endereco> enderecos { get; set; }

    public Empresa()
    {
        enderecos = new HashSet<Endereco>();
    }
}
