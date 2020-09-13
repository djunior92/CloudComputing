
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage.RetryPolicies;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Agenda.Data
{
    public class ContatoStorageService
    {
        private readonly string conexao;
        private readonly ContatoRepository repository;

        public ContatoStorageService(string conn, ContatoRepository rep)
        {
            conexao = conn;
            repository = rep;
        }

        private async Task<string> EnviarBlobAsync(int id, FileData file)
        {
            string resultado = null;

            try
            {
                CloudStorageAccount conta = CloudStorageAccount.Parse(conexao);
                CloudBlobClient clienteblob = conta.CreateCloudBlobClient();
                CloudBlobContainer containerblob = clienteblob.GetContainerReference(Constantes.NomeContainer);

                if (await containerblob.CreateIfNotExistsAsync())
                {
                    await containerblob.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        });
                }

                CloudBlockBlob arquivoblob = containerblob.GetBlockBlobReference(file.Name);
                arquivoblob.Properties.ContentType = file.ContentType;

                file.Stream.Seek(0, SeekOrigin.Begin);

                await arquivoblob.UploadFromStreamAsync(file.Stream);

                resultado = arquivoblob.Uri.ToString();
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return resultado;
        }

        private async Task<bool> CriarMensagemAsync(int id, string url)
        {
            bool resultado = false;

            try
            {
                var info = new InfoImagem()
                {
                    Id = id,
                    Url = url
                };

                var conteudojson = JsonConvert.SerializeObject(info);

                CloudStorageAccount conta = CloudStorageAccount.Parse(conexao);

                CloudQueueClient clientequeue = conta.CreateCloudQueueClient();
                clientequeue.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

                CloudQueue fila = clientequeue.GetQueueReference(Constantes.NomeFila);
                await fila.CreateIfNotExistsAsync();

                CloudQueueMessage mensagem = new CloudQueueMessage(conteudojson);
                await fila.AddMessageAsync(mensagem);

                resultado = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            return resultado;
        }

        public async Task<bool> Upload(int id, FileData file)
        {
            bool resultado = false;

            if (file?.Stream == null || file?.Lenght == 0)
                throw new Exception("Imagem inválida");

            try
            {
                string nomearquivo = $"{id}{Path.GetExtension(file.Name)}";
                file.Name = nomearquivo;

                string url = await EnviarBlobAsync(id, file);

                if (url == null)
                    return resultado;

                await CriarMensagemAsync(id, url);

                Contato modelo = await repository.GetAsync(id);
                modelo.UrlFoto = url;
                modelo = await repository.SaveAsync(modelo);

                if (modelo != null)
                    resultado = true;
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            return resultado;

        }

        public async Task<bool> GerarThumbnail(InfoImagem info)
        {
            bool resultado = false;
            string enderecothumb = null;

            FileData file = new FileData()
            {
                Name = Path.GetFileNameWithoutExtension(info.Url) + "_Thumb.png",
                ContentType = "image/png"
            };

            try
            {
                HttpClient cliente = new HttpClient();
                MemoryStream imgStream = new MemoryStream();

                using (var foto = await cliente.GetStreamAsync(info.Url).ConfigureAwait(false))
                {
                    await foto.CopyToAsync(imgStream);

                    using (MemoryStream output = new MemoryStream())
                    {
                        ImageHelper.MakeThumbnail(imgStream, output);

                        if (output == null)
                            throw new NullReferenceException("Não foi possível processar o arquivo!");

                        file.Stream = output;

                        enderecothumb = await EnviarBlobAsync(info.Id, file);
                    }
                }

                if (enderecothumb != null)
                {
                    Contato modelo = await repository.GetAsync(info.Id);
                    modelo.UrlThumb = enderecothumb;
                    modelo = await repository.SaveAsync(modelo);
                    if (modelo != null)
                        resultado = true;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
            return resultado;
        }
    }
}
