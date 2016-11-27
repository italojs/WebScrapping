using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using HtmlAgilityPack;

namespace WebScraping
{
    class Program
    {
        static void Main(string[] args)
        {
            //OBS: ignore a parte do cadastro no banco de dados, foi feito de uma forma bem simploria apenas para demosracao ignoando boas praticas.
            var baseURL = "http://www.freitasleiloesonline.com.br/";//Site
            
            var client = new HtmlWeb();//aqui criamos um objeto com a classe HtmlWeb que contem metodos para a extracao de  dados em paginas WEB

            var paginaMateriaisHome = client.Load(baseURL + "/homesite/filtro.asp?q=materiais");//Resto  da URL onde voce ira extrair os dados  //client.Load faz o dowloand/carrega a pagina
            
            var codUltimaPagina = paginaMateriaisHome.DocumentNode.SelectNodes("//*[@id='listaLotesPaginacao']/ul/li").Count;// procura em todo html uma tag parecida, e conta a quatidade de tags(pags np caso)
            Console.WriteLine(codUltimaPagina);

            for(int i = 1; i <= 2; i++)//i<codUltimaPagina
            {
                var paginaMateriais = client.Load(baseURL + "homesite/filtro.asp?q=materiais&txBuscar=&CodSubCategoria=&SubCategoria=&UF=&CodCondicao=&Condicao=&OptValores=0&LblValores=&pagina=" + i);
                var materiais = paginaMateriais.DocumentNode.SelectNodes("//div[@id='listaLotes']/ul/li");

                var listaMateriais = new List<TB_MATERIAL>();

                foreach (var item in materiais)
                {
                    var urlImage = item.SelectSingleNode("div[1]/img").Attributes["src"].Value;//procura a tag IMG dentro da primeira div(de materiais) e depois pega o atributo src desta tag
                    Console.WriteLine(urlImage);
                    item.SelectSingleNode("div[2]/h1").Remove();//remover H1, pesquisa e remove em seguida...

                    var descricao = item.SelectSingleNode("div[2]").InnerHtml;//com o innerHTML conseguimos pegar o texto dentro do elemento, no caso a div
                    Console.WriteLine(descricao);

                    var lanceInicial = item.SelectSingleNode("div[3]/div[1]").InnerHtml.Split(':').Last().Trim();//split quebra string apartir do caractere informafo e o trasforma em um array ,last pega o ultimo valor do Array   , trim retira espaços vazios  
                    Console.WriteLine(lanceInicial);

                    var maiorLance = item.SelectSingleNode("div[3]/div[2]").InnerHtml.Split(':').Last().Trim();//split quebra string apartir do caractere informafo e o trasforma em um array ,last pega o ultimo valor do Array, trim retira espaços vazios  
                    Console.WriteLine(maiorLance);

                    var qtdLances = item.SelectSingleNode("div[3]/div[3]").InnerHtml.Split(':').Last().Trim();//split quebra string apartir do caractere informafo e o trasforma em um array ,last pega o ultimo valor do Array , trim retira espaços vazios  
                    Console.WriteLine(qtdLances);

                    var tbMaterialMOD = new TB_MATERIAL();
 
                    tbMaterialMOD.URL_FOTO = urlImage;
                    tbMaterialMOD.DESCRICAO = descricao;
                    tbMaterialMOD.LANCE_INICIAL = lanceInicial;
                    tbMaterialMOD.MAIOR_LANCE = maiorLance;
                    tbMaterialMOD.QTD_LANCE = Convert.ToInt32(qtdLances);

                    listaMateriais.Add(tbMaterialMOD);

                }
                using (var conexao = new DB_LEILAOEntities())//using serve para no fim de tudo, ele fechar a conexao com o BD
                {
                    conexao.TB_MATERIAL.AddRange(listaMateriais);//AddRange cadastra tudo de uma vez (de uma lista)
                    conexao.SaveChanges();
                    

                }

            }





            Console.ReadKey();


        }
    }
}
