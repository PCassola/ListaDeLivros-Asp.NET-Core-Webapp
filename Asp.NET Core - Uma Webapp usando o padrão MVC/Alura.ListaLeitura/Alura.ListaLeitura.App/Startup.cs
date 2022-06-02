using Alura.ListaLeitura.App.Negocio;
using Alura.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.App
{
    public class Startup
    {
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app)
        {
            var builder = new RouteBuilder(app);
            builder.MapRoute("Livros/ParaLer", LivrosParaLer);
            builder.MapRoute("Livros/Lendo", LivrosLendo);
            builder.MapRoute("Livros/Lidos", LivrosLidos);
            builder.MapRoute("Cadastro/NovoLivro/{nome}/{Autor}", NovoLivroParaLer);
            builder.MapRoute("Livros/Detalhes/{id:int}", ExibirDetalhes);
            builder.MapRoute("Cadastro/NovoLivro", ExibeFormulario);
            builder.MapRoute("Cadastro/Incluir", ProcessaFormulario);
            var rotas = builder.Build();
            //roteamento usando asp.net core
            app.UseRouter(rotas);

            //roteamento usando dicionary
            //app.Run(Roteamento);
        }

        //public Task Roteamento(HttpContext context)
        //{
        //    var _repo = new LivroRepositorioCSV();
        //    var caminhosAtendidas = new Dictionary<string, RequestDelegate>
        //    {
        //        //Livros/ParaLer
        //        {"/Livros/ParaLer", LivrosParaLer },
        //        //Livros/Lendo
        //        {"/Livros/Lendo", LivrosLendo },
        //        //Livros/Lidos
        //        {"/Livros/Lidos", LivrosLidos },
        //    };

        //    if (caminhosAtendidas.ContainsKey(context.Request.Path))
        //    {
        //        var metodo = caminhosAtendidas[context.Request.Path];
        //        return metodo.Invoke(context);
        //    }

        //    context.Response.StatusCode = 404;
        //    return context.Response.WriteAsync("Caminho inexistente.") ;
        //}

        public Task LivrosParaLer(HttpContext context)
        {
            var _repo = new LivroRepositorioCSV();
            var conteudoArquivo = CarregaArquivoHTML("para-ler");
            
            foreach(var livro in _repo.ParaLer.Livros)
            {
                conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#",$"<li>{livro.Titulo} - {livro.Autor}</li>#NOVO-ITEM#");
            }
            conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#", "");

            return context.Response.WriteAsync(conteudoArquivo);
        }
        public Task LivrosLendo(HttpContext context)
        {
            var _repo = new LivroRepositorioCSV();
            var conteudoArquivo = CarregaArquivoHTML("lendo");

            foreach (var livro in _repo.Lendo.Livros)
            {
                conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#", $"<li>{livro.Titulo} - {livro.Autor}</li>#NOVO-ITEM#");
            }
            conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#", "");

            return context.Response.WriteAsync(conteudoArquivo);
        }
        public Task LivrosLidos(HttpContext context)
        {
            var _repo = new LivroRepositorioCSV();
            var conteudoArquivo = CarregaArquivoHTML("lidos");

            foreach (var livro in _repo.Lidos.Livros)
            {
                conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#", $"<li>{livro.Titulo} - {livro.Autor}</li>#NOVO-ITEM#");
            }
            conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM#", "");

            return context.Response.WriteAsync(conteudoArquivo);
        }
        public Task NovoLivroParaLer(HttpContext context)
        {
            var livro = new Livro()
            {
                Titulo = Convert.ToString(context.GetRouteValue("nome")),
                Autor = Convert.ToString(context.GetRouteValue("autor")),
            };

            var repo = new LivroRepositorioCSV();
            repo.Incluir(livro);
            return context.Response.WriteAsync("O livro foi adicionado com sucesso!");
        }
        public Task ExibirDetalhes(HttpContext context)
        {

            try
            {
                int id = Convert.ToInt32(context.GetRouteValue("id"));
                var repo = new LivroRepositorioCSV();
                var livro = repo.Todos.First(l => l.Id == id);
                return context.Response.WriteAsync(livro.Detalhes());
            }
            catch
            {
                return context.Response.WriteAsync("Esse id não existe.");
            }

        }
        private Task ExibeFormulario(HttpContext context)
        {
            var html = CarregaArquivoHTML("formulario");
            return context.Response.WriteAsync(html);
        }

        private string CarregaArquivoHTML(string nomeArquivo)
        {
            var nomeCompletoArquivo = $"Html/{nomeArquivo}.html";
            using (var arquivo = File.OpenText(nomeCompletoArquivo))
            {
                return arquivo.ReadToEnd();
            }
        }

        public Task ProcessaFormulario(HttpContext context)
        {
            var livro = new Livro()
            {
                Titulo = context.Request.Form["titulo"].First(),
                Autor = context.Request.Form["autor"].First(),
            };

            var repo = new LivroRepositorioCSV();
            repo.Incluir(livro);
            return context.Response.WriteAsync("O livro foi adicionado com sucesso!");
        }
    }
}
