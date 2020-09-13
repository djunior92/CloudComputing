using Agenda.Data;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using Microsoft.Extensions.DependencyInjection;
using ContatoCW12.WebJob;
using Microsoft.Extensions.Configuration;

namespace ContatoCW12.WebJob
{
    public class Functions
    {
        public async static Task GerarThumbnailAsync([QueueTrigger(Constantes.NomeFila)] string message, ILogger logger)
        {
            try
            {
                InfoImagem info = JsonConvert.DeserializeObject<InfoImagem>(message);

                if (info.Id == 0)
                {
                    logger.LogError("Mensagem inválida");
                    return;
                }

                var rep = (ContatoRepository) ActivatorUtilities.CreateInstance(Program.ServiceProvider, typeof(ContatoRepository));

                var config = Program.Configuration;
                string storagecon = config.GetValue<string>(Constantes.AzureStorageConfigName);
                var service = new ContatoStorageService(storagecon, rep);

                var resposta = await service.GerarThumbnail(info);

                if (resposta == false)
                    logger.LogInformation("Não foi possível gerar o thumbnail");
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
            }
            
        }
    }
}
