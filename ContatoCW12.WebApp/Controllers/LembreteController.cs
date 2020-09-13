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
    public class LembreteController : Controller
    {
        private readonly LembreteRepository repository;

        public LembreteController(IConfiguration config, LembreteRepository rep)
        {
            repository = rep;
     
        }

        public async Task<IActionResult> Index()
        {
            List<Lembrete> lista = await repository.ListAsync();

            return View(lista);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            IActionResult result;
            Lembrete model = null;

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
        public async Task<IActionResult> Edit(Lembrete model)
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

        
    }
}
