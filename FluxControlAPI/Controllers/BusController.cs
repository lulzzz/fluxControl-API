using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FluxControlAPI.Controllers
{
    
    [ApiController]
    [Authorize("Bearer")]
    [Route("API/Company/[controller]")]
    public class BusController : ControllerBase
    {
        #region CRUD

        [HttpPost]
        [Authorize(Roles = "Manager, Administrator")]
        [Route("Add")]
        public ActionResult Add([FromBody] Bus bus)
        {
            try
            {
                using (var busDAO = new BusDAO())
                    if (busDAO.Add(bus) != 0)
                    {
                        return StatusCode(201, "Adicionado à frota");
                    }

                return StatusCode(304, new { Message = "Não adicionado" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha" });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Operator, Manager, Administrator")]
        [Route("Get/{id}")]
        public ActionResult Get(int id)
        {
            try
            {
                using (var busDAO = new BusDAO())
                    return StatusCode(200, busDAO.Get(id));
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao obter este ônibus" });
            }

        }

        [HttpGet]
        [Authorize(Roles = "Operator, Manager, Administrator")]
        [Route("Load")]
        public ActionResult Load()
        {
            try
            {
                using (var busDAO = new BusDAO())
                    return StatusCode(200, busDAO.Load());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha" });
            }

        }

        [HttpPatch]
        [Authorize(Roles = "Manager, Administrator")]
        [Route("Change/{id}")]
        public ActionResult Change(int id, [FromBody] Bus bus)
        {
            using (var busDAO = new BusDAO())
                if (busDAO.Change(id, bus))
                    return StatusCode(200, new { Message = "Alterado com sucesso" });

            return StatusCode(304, new { Message = "Não alterado" });
        }

        [HttpDelete]
        [Authorize(Roles = "Manager, Administrator")]
        [Route("Remove/{id}")]
        public ActionResult Remove(int id)
        {
            try
            {
                using (var busDAO = new BusDAO())
                    if (busDAO.Remove(id))
                        return StatusCode(200, new { Message = "Removido" });

                return StatusCode(304, new { Message = "Não Removido" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha" });
            }
        }

        #endregion
    }
}