using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using EntitiesServices.Work_Classes;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class LeadAppService : AppServiceBase<LEAD>, ILeadAppService
    {
        private readonly ILeadService _baseService;

        public LeadAppService(ILeadService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<LEAD> GetAllItens(Int32 idAss)
        {
            List<LEAD> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public LEAD CheckExist(LEAD conta, Int32 idAss)
        {
            LEAD item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<LEAD> GetAllItensAdm(Int32 idAss)
        {
            List<LEAD> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public LEAD GetItemById(Int32 id)
        {
            LEAD item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<LEAD>, Boolean> ExecuteFilter(DateTime? inicio, DateTime? final, String nome, String email, Int32? status, String cpf, String cnpj, String cidade, Int32? uf, Int32 idAss)
        {
            try
            {
                List<LEAD> objeto = new List<LEAD>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(inicio, final, nome, email,status, cpf, cnpj, cidade, uf, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(LEAD item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.LEAD_IN_ATIVO = 1;

                // Monta Log
                DTO_Lead dto = MontarLeadDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Lead - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(LEAD item, LEAD itemAntes, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Lead dto = MontarLeadDTOObj(item);
                DTO_Lead dtoAntes = MontarLeadDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Lead - Alteração",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = jsonAntes,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(LEAD item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Checa integridade
                if (item.CRM.Count() > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.LEAD_IN_ATIVO = 0;

                // Monta Log
                DTO_Lead dto = MontarLeadDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "lead - Exclusão",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(LEAD item, USUARIO usuario)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Verifica integridade referencial

                // Acerta campos
                item.LEAD_IN_ATIVO = 1;

                // Monta Log
                DTO_Lead dto = MontarLeadDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Lead - Reativação",
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                Int32 volta = _baseService.Edit(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public LEAD_ANEXO GetLeadAnexoById(Int32 id)
        {
            LEAD_ANEXO lista = _baseService.GetLeadAnexoById(id);
            return lista;
        }

        public Int32 ValidateEditLeadAnexo(LEAD_ANEXO item)
        {
            try
            {
                // Persiste
                return _baseService.EditLeadAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public LEAD_ANOTACAO GetAnotacaoById(Int32 id)
        {
            LEAD_ANOTACAO lista = _baseService.GetAnotacaoById(id);
            return lista;
        }

        public Int32 ValidateEditAnotacao(LEAD_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DTO_Lead MontarLeadDTOObj(LEAD antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Lead()
                {
                    LEAD_CD_ID = antes.LEAD_CD_ID,
                    LEAD_DT_DUMMY =  antes.LEAD_DT_DUMMY,
                    LEAD_DT_ENTRADA = antes.LEAD_DT_ENTRADA,
                    LEAD_DT_NASCIMENTO = antes.LEAD_DT_NASCIMENTO,
                    LEAD_EM_EMAIL = antes.LEAD_EM_EMAIL,
                    LEAD_IN_ATIVO = antes.LEAD_IN_ATIVO,
                    LEAD_IN_SISTEMA = antes.LEAD_IN_SISTEMA,
                    LEAD_IN_STATUS = antes.LEAD_IN_STATUS,
                    LEAD_NM_BAIRRO = antes.LEAD_NM_BAIRRO,
                    LEAD_NM_CIDADE = antes.LEAD_NM_CIDADE,
                    LEAD_NM_COMPLEMENTO = antes.LEAD_NM_COMPLEMENTO,
                    LEAD_NM_ENDERECO = antes.LEAD_NM_ENDERECO,
                    LEAD_NM_NOME = antes.LEAD_NM_NOME,
                    LEAD_NR_CELULAR = antes.LEAD_NR_CELULAR,
                    LEAD_NR_CEP = antes.LEAD_NR_CEP,
                    LEAD_NR_CNPJ = antes.LEAD_NR_CNPJ,
                    LEAD_NR_CPF = antes.LEAD_NR_CPF,
                    LEAD_NR_NUMERO = antes.LEAD_NR_NUMERO,
                    CRM1_CD_ID = antes.CRM1_CD_ID,
                    SEXO_CD_ID = antes.SEXO_CD_ID,
                    UF_CD_ID = antes.UF_CD_ID,
                    USUA_CD_ID = antes.USUA_CD_ID,
                };
                return mediDTO;
            }
        }

    }
}
