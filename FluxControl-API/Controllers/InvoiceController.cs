using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxControlAPI.Models.BusinessRule;
using FluxControlAPI.Models.Datas;
using FluxControlAPI.Models.Datas.BusinessRule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FluxControlAPI.Controllers
{
    [ApiController]
    [Authorize("Bearer", Roles = "Administrator")]
    [Route("API/[controller]")]
    public class InvoiceController : ControllerBase
    {
        [HttpPost]
        [Route("GenerateInvoice/{companyId}")]
        public ActionResult GenerateInvoice(int companyId, [FromBody] DateTime begin)
        {
            try
            {
                Company company;
                Rules rules;

                using (var companyDAO = new CompanyDAO())
                    company = companyDAO.Get(companyId);

                if (company == null)
                    return StatusCode(404, new { Message = "Empresa não encontrada" });

                using (var providerDAO = new RulesDAO())
                    rules = providerDAO.Get();

                if (rules == null)
                    return StatusCode(404, new { Message = "Não há regras base para o cálculo da fatura" });

                using (var invoiceDAO = new InvoiceDAO())
                {
                    var invoice = new Invoice()
                    {
                        GenerationDate = DateTime.Now,
                        CompanyDebtor = company.Id,
                        TaxConsidered = rules.Tax,
                        IntervalMinutesConsidered = rules.IntervalMinutes
                    };

                    invoice.Id = invoiceDAO.Add(invoice);

                    if (invoice.Id != 0)
                    {
                        using (var flowRecordDAO = new FlowRecordDAO())
                        {
                            List<FlowRecord> records;
                            decimal total = 0;

                            flowRecordDAO.MarkCharge(invoice.Id, company.Id, begin, begin.AddDays(company.InvoiceInterval));
                            records = invoiceDAO.GetInvoiceRecords(invoice.Id);

                            Parallel.ForEach(records, (record) =>
                            {
                                
                                // Calcula o valor para o período de permanência desse Ônibus com base no intervalo de cobrança e o valor da tarifa
                                var permanence = record.Departure - record.Arrival;
                                total += rules.Tax * Convert.ToDecimal(Math.Floor(permanence.Value.TotalMinutes / rules.IntervalMinutes));

                                // Caso tenha permanecido menos que o intervalo de cobrança, é cobrado a taxa mínima
                                if (total <= 0)
                                    total += rules.Tax;
                                
                            });

                            if (records.Count <= 0 && invoiceDAO.Cancel(invoice.Id))
                                return StatusCode(304, new { Message = "Não há registros não faturados para este período" });
                            
                            if (invoiceDAO.SetInvoiceValue(invoice.Id, total))
                            {
                                invoice = invoiceDAO.Get(invoice.Id);
                                return StatusCode(200, invoice);
                            }

                        }
                    }

                    return StatusCode(304, new { Message = "A fatura não foi gerada" });
                }
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Erro interno ao gerar fatura" });
            }
        }

        [HttpGet]
        [Route("ChargeRegistration/{recordId}")]
        public ActionResult ChargeRegistration(int recordId)
        {
            Rules rules;
            FlowRecord record;

            using (var providerDAO = new RulesDAO())
                rules = providerDAO.Get();

            if (rules == null)
                return StatusCode(404, new { Message = "Não há regras base para o cálculo da fatura" });

            using (var flowRecordDAO = new FlowRecordDAO())
                record = flowRecordDAO.Get(recordId);

            if (record == null)
                return StatusCode(404, new { Message = "Registro não encontrado" });

            if (record.Departure != null)
            {
                var permanence = record.Departure - record.Arrival;

                var total = rules.Tax * Convert.ToDecimal(Math.Floor(permanence.Value.TotalMinutes / rules.IntervalMinutes));

                // Caso tenha permanecido menos que o intervalo de cobrança, é cobrado a taxa mínima
                if (total <= 0)
                    total += rules.Tax;

                return StatusCode(200, new Invoice()
                {
                    Id = 0,
                    GenerationDate = DateTime.Now,
                    TaxConsidered = rules.Tax,
                    IntervalMinutesConsidered = rules.IntervalMinutes,
                    CompanyDebtor = record.BusRegistered.BusCompany,
                    TotalCost = total
                });
            }

            return StatusCode(412, new { Message = "O registro deve estar fechado para ser tarifado." });
            
        }

        [HttpDelete]
        [Route("Cancel/{id}")]
        public ActionResult Cancel(int id)
        {
            try
            {
                using (var invoiceDAO = new InvoiceDAO())
                    if (invoiceDAO.Cancel(id))
                        return StatusCode(200, new { Message = "Cancelado" });

                return StatusCode(304, new { Message = "Não cancelado" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha ao remover" });
            }
        }

        #region CRUD

        [HttpGet]
        [Route("Get/{id}")]
        public ActionResult Get(int id)
        {
            try
            {
                using (var invoiceDAO = new InvoiceDAO())
                    return StatusCode(200, invoiceDAO.Get(id));
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha ao obter fatura" });
            }

        }

        [HttpGet]
        [Route("Load")]
        public ActionResult Load()
        {
            try
            {
                using (var invoiceDAO = new InvoiceDAO())
                    return StatusCode(200, invoiceDAO.Load());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha ao carregar faturas" });
            }

        }

        [HttpDelete]
        [Route("Remove/{id}")]
        public ActionResult Remove(int id)
        {
            try
            {
                using (var invoiceDAO = new InvoiceDAO())
                    if (invoiceDAO.Remove(id))
                        return StatusCode(200, new { Message = "Removido" });

                return StatusCode(304, new { Message = "Não Removido" });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Falha ao remover" });
            }
        }

        #endregion
    }
}