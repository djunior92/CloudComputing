using Agenda.Data;
using ContatoCW12.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ContatoCW12.WebApp.Controllers
{
    public class ContatoController : Controller
    {
        private readonly ContatoRepository repository;
        private readonly ContatoStorageService service;
        
        public ContatoController(IConfiguration config, ContatoRepository rep)
        {
            repository = rep;

            var conn = config.GetValue<string>(Constantes.AzureStorageConfigName);
            service = new ContatoStorageService(conn, repository);
        }

        public async Task<IActionResult> Index()
        {
            List<Contato> lista = await repository.ListAsync();

            return View(lista);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult result;
            Contato model = null;

            try
            {
                if (id == null)
                    model = repository.CreateNew();
                else
                    model = await repository.GetAsync(id.Value);

                if (model == null)
                    return BadRequest("ID não encontrado");

                result = View(model);
            }
            catch (Exception)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao editar");
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Contato model)
        {
            IActionResult result;

            try
            {
                model = await repository.SaveAsync(model);

                if (model == null)
                    return BadRequest("Ocorreu um erro ao salvar");

                result = RedirectToAction("Index");
            }
            catch (Exception)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao editar");
            }

            return result;
        }

        public async Task<IActionResult> Upload(int id)
        {
            IActionResult result;

            try
            {
                Contato contato = await repository.GetAsync(id);
                if (contato == null)
                {
                    return NotFound();
                }

                var modelo = new UploadFileViewModel()
                {
                    Id = id
                };

                return View(modelo);
            }
            catch (Exception)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao enviar");
            }

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadFileViewModel model)
        {
            IActionResult result;

            if (!ModelState.IsValid)
                return View(model);

            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Arquivo inválido!");
                return View(model);
            }

            try
            {
                FileData fdata = new FileData()
                {
                    Name = model.File.FileName,
                    ContentType = model.File.ContentType,
                    Lenght = model.File.Length,
                    Stream = new MemoryStream()
                };

                await model.File.CopyToAsync(fdata.Stream);

                if (await service.Upload(model.Id, fdata))
                    return RedirectToAction("Index");

                result = View(model);
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message);
                result = StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao enviar a foto");
            }
            return result;
        }
    }
}
