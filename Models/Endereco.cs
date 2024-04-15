using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppPesquisaCnpj.Models;

public class Endereco {
    [Key] 
    public int id { get; set; }
    [Required] 
    [StringLength(100)] 
    public string logradouro { get; set; }
    [Required] 
    [StringLength(50)] 
    public string numero { get; set; }
    
    [StringLength(100)] 
    public string? complemento { get; set; }
    [Required] 
    [StringLength(9)] 
    public string cep { get; set; }
    [Required] 
    [StringLength(100)] 
    public string bairro { get; set; }
    [Required] 
    [StringLength(100)] 
    public string municipio { get; set; }
    [Required] 
    [StringLength(2)] 
    public string uf { get; set; }

    [ForeignKey("Empresa")]
    public int EmpresaId { get; set; }
    public virtual Empresa Empresa { get; set; }
}
