using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppPesquisaCnpj.Data;
using AppPesquisaCnpj.Models;
using System.Net.Http.Json;
using System.Text.Json;

public class EmpresasController : Controller
{
    public readonly AppDbContext dbContext;
    public readonly IHttpClientFactory _httpClientFactory;
    public EmpresasController(AppDbContext context,IHttpClientFactory httpClientFactory)
    {
        dbContext = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ActionResult> Index()
{
    var empresas = await dbContext.Empresas.ToListAsync();
    var enderecos = await dbContext.Enderecos.ToListAsync();

    var viewModel = empresas.Select(e => new EmpresaEnderecoViewModel {
        empresa = e,
        Enderecos = enderecos.Where(end => end.EmpresaId == e.id).ToList()
    }).ToList();

    return View(viewModel);
}

[HttpGet]
public async Task<ActionResult> Details(int id)
{
    var empresa = await dbContext.Empresas.FirstOrDefaultAsync(e => e.id == id);   
    
    if (empresa == null)
    {
        return NotFound();
    }
    var enderecos = await dbContext.Enderecos
                                   .Where(end => end.EmpresaId == id)
                                   .ToListAsync();

    var viewModel = new EmpresaEnderecoViewModel
    {
        empresa = empresa,
        Enderecos = enderecos
    };

    return View(viewModel);
}    

    public ActionResult Pesquisar()
    {
        return View();
    }
    
    [HttpPost]
public async Task<ActionResult> ConsultarCNPJ(string cnpj)
{
    var client = _httpClientFactory.CreateClient();
    
    
    
    var cleanCNPJ = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
    var url = $"https://www.receitaws.com.br/v1/cnpj/{cleanCNPJ}";

    HttpResponseMessage response = await client.GetAsync(url);
var enderecos = new List<Endereco>();
    if (response.IsSuccessStatusCode)
{
    var jsonResponse = await response.Content.ReadAsStringAsync();
    
    using (JsonDocument jsonDoc = JsonDocument.Parse(jsonResponse))
    {
        var empresa = new Empresa
        {
            cnpj = jsonDoc.RootElement.GetProperty("cnpj").GetString(),
            nome = jsonDoc.RootElement.GetProperty("nome").GetString(),
            atividadePrincipal = jsonDoc.RootElement.GetProperty("atividade_principal").EnumerateArray().Select(x => x.GetProperty("code").GetString()).FirstOrDefault(),
            
        };

        var endereco = new Endereco{
            logradouro = jsonDoc.RootElement.GetProperty("logradouro").GetString(),
            numero = jsonDoc.RootElement.GetProperty("numero").GetString(),
            complemento = jsonDoc.RootElement.GetProperty("complemento").GetString(),
            municipio = jsonDoc.RootElement.GetProperty("municipio").GetString(),
            bairro = jsonDoc.RootElement.GetProperty("bairro").GetString(),
            uf = jsonDoc.RootElement.GetProperty("uf").GetString(),
            cep = jsonDoc.RootElement.GetProperty("cep").GetString(),
        };
enderecos.Add(endereco);
var viewModel = new EmpresaEnderecoViewModel
    {
        empresa = empresa,
        Enderecos = enderecos
    };
        return View("Detalhes", viewModel);
    }
}  

    return View("Error");
}

[HttpPost]
public async Task<IActionResult> Salvar(Empresa empresa, List<Endereco> enderecos)
{
    if (ModelState.IsValid)
    {
        try
        {
            if (empresa.id == 0)
            {
                dbContext.Empresas.Add(empresa);
            }
            else
            {
                dbContext.Empresas.Update(empresa);
            }

            if (empresa.id != 0 && empresa.enderecos != null)
            {
                var existingAddresses = dbContext.Enderecos.Where(e => e.EmpresaId == empresa.id);
                dbContext.Enderecos.RemoveRange(existingAddresses);
            }

            // Adiciona os novos endereços
            foreach (var endereco in enderecos)
            {
                endereco.EmpresaId = empresa.id; 
                dbContext.Enderecos.Add(endereco);
            }

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Não foi possível salvar os dados. Tente novamente, e se o problema persistir, veja o log de erros.");
        }
    }
    return View();
}

}
