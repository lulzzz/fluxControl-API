using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.Datas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FluxControlAPI.Controllers
{
    [ApiController]
    // [Authorize("Bearer", Roles = "Administrator")]
    [Route("API/[controller]")]
    public class ProviderController : ControllerBase
    {

        #region Rules

        [HttpGet]
        [Route("Rules/Get")]
        public ActionResult GetRules()
        {
            try
            {
                Rules rules = null;

                using (var rulesDAO = new RulesDAO())
                    rules = rulesDAO.Get();

                if (rules != null)
                    return StatusCode(200, rules);

                return StatusCode(404, new { Message = "Regras não encontradas" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno ao carregar regras" });
            }
        }

        [HttpPut]
        [Route("Rules/Update")]
        public ActionResult UpdateRules([FromBody] Rules rules)
        {
            try
            {

                using (var providerDAO = new RulesDAO())
                    if (providerDAO.Update(rules))
                        return StatusCode(200, new { Message = "Regras atualizadas" });

                return StatusCode(304, new { Message = "Regras não atualizadas" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno ao atualizar regras" });
            }
            
        }

        #endregion
    }
}