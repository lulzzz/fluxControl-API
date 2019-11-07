using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.DataModels;
using FluxControlAPI.Models.DataModels.BusinessRule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FluxControlAPI.Controllers
{
    [ApiController]
    [Authorize("Bearer", Roles = "Manager, Administrator")]
    [Route("API/[controller]")]
    public class CompanyController : ControllerBase
    {
        [HttpPost]
        [Route("Add")]
        public ActionResult Add([FromBody] Company company)
        {
            try
            {
                using (var companyDAO = new CompanyDAO())
                    if (companyDAO.Add(company) != 0)
                    {
                        return StatusCode(201, "Adicionada");
                    }

                return StatusCode(304, new { Message = "Não adicionada" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha" });
            }
        }

        [HttpGet]
        [Route("Get/{id}")]
        public ActionResult Get(int id)
        {
            try
            {
                using (var companyDAO = new CompanyDAO())
                    return StatusCode(200, companyDAO.Get(id));
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro ao obter esta empresa" });
            }

        }

        [HttpGet]
        [Route("Load")]
        public ActionResult Load()
        {
            try
            {
                using (var companyDAO = new CompanyDAO())
                    return StatusCode(200, companyDAO.Load());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha" });
            }

        }

        [HttpPatch]
        [Route("Change/{id}")]
        public ActionResult Change(int id, [FromBody] Company company)
        {
            using (var companyDAO = new CompanyDAO())
                if (companyDAO.Change(id, company))
                    return StatusCode(200, new { Message = "Alterado com sucesso" });

            return StatusCode(304, new { Message = "Não alterado" });
        }

        [HttpDelete]
        [Route("Remove/{id}")]
        public ActionResult Remove(int id)
        {
            try
            {
                using (var companyDAO = new CompanyDAO())
                    if (companyDAO.Remove(id))
                        return StatusCode(200, new { Message = "Removido" });

                return StatusCode(304, new { Message = "Não Removido" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha" });
            }
        }
    }
}