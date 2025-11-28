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
    public class TemplateSMSAppService : AppServiceBase<TEMPLATE_SMS>, ITemplateSMSAppService
    {
        private readonly ITemplateSMSService _baseService;

        public TemplateSMSAppService(ITemplateSMSService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TEMPLATE_SMS> GetAllItens(Int32 idAss)
        {
            List<TEMPLATE_SMS> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public TEMPLATE_SMS CheckExist(TEMPLATE_SMS conta, Int32 idAss)
        {
            TEMPLATE_SMS item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TEMPLATE_SMS> GetAllItensAdm(Int32 idAss)
        {
            List<TEMPLATE_SMS> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TEMPLATE_SMS GetItemById(Int32 id)
        {
            TEMPLATE_SMS item = _baseService.GetItemById(id);
            return item;
        }

        public TEMPLATE_SMS GetByCode(String sigla, Int32 idAss)
        {
            TEMPLATE_SMS item = _baseService.GetByCode(sigla, idAss);
            return item;
        }

        public Tuple<Int32, List<TEMPLATE_SMS>, Boolean> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss)
        {
            try
            {
                List<TEMPLATE_SMS> objeto = new List<TEMPLATE_SMS>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(sigla, nome, conteudo, idAss);
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

        public Int32 ValidateCreate(TEMPLATE_SMS item, USUARIO usuario)
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
                item.TSMS_IN_ATIVO = 1;

                // Monta Log
                DTO_ModeloSMS dto = MontarModeloSMSDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Modelo SMS - Inclusão",
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

        public Int32 ValidateEdit(TEMPLATE_SMS item, TEMPLATE_SMS itemAntes, USUARIO usuario)
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
                DTO_ModeloSMS dto = MontarModeloSMSDTO(item.TSMS_CD_ID);
                DTO_ModeloSMS dtoAntes = MontarModeloSMSDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                item.EMPRESA = null;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Modelo SMS - Alteração",
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

        public Int32 ValidateDelete(TEMPLATE_SMS item, USUARIO usuario)
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
                if (item.MENSAGENS.Count > 0)
                {
                    if (item.MENSAGENS.Where(p => p.MENS_IN_ATIVO == 1).ToList().Count > 0)
                    {
                        return 1;
                    }
                }

                // Acerta campos
                item.TSMS_IN_ATIVO = 0;

                // Monta Log
                item.EMPRESA = null;
                DTO_ModeloSMS dto = MontarModeloSMSDTO(item.TSMS_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Modelo SMS - Exclusão",
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

        public Int32 ValidateReativar(TEMPLATE_SMS item, USUARIO usuario)
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
                item.TSMS_IN_ATIVO = 1;

                // Monta Log
                DTO_ModeloSMS dto = MontarModeloSMSDTO(item.TSMS_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                item.EMPRESA = null;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Modelo SMS - Reativação",
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

        public DTO_ModeloSMS MontarModeloSMSDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.TEMPLATE_SMS
                    .Where(l => l.TSMS_CD_ID == mediId)
                    .Select(l => new DTO_ModeloSMS
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        TSMS_IN_ATIVO = l.TSMS_IN_ATIVO,
                        TSMS_CD_ID = l.TSMS_CD_ID,
                        TSMS_IN_EDITAVEL = l.TSMS_IN_EDITAVEL,
                        TSMS_IN_FIXO = l.TSMS_IN_FIXO,
                        TSMS_IN_ROBOT = l.TSMS_IN_ROBOT,
                        TSMS_LK_LINK = l.TSMS_LK_LINK,
                        TSMS_NM_NOME = l.TSMS_NM_NOME,
                        TSMS_NR_SISTEMA = l.TSMS_NR_SISTEMA,
                        TSMS_SG_SIGLA = l.TSMS_SG_SIGLA,
                        TSMS_TX_CORPO = l.TSMS_TX_CORPO,
                        EMPR_CD_ID = l.EMPR_CD_ID,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public DTO_ModeloSMS MontarModeloSMSDTOObj(TEMPLATE_SMS antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_ModeloSMS()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    TSMS_IN_ATIVO = antes.TSMS_IN_ATIVO,
                    TSMS_CD_ID = antes.TSMS_CD_ID,
                    TSMS_IN_EDITAVEL = antes.TSMS_IN_EDITAVEL,
                    TSMS_IN_FIXO = antes.TSMS_IN_FIXO,
                    TSMS_IN_ROBOT = antes.TSMS_IN_ROBOT,
                    TSMS_LK_LINK = antes.TSMS_LK_LINK,
                    TSMS_NM_NOME = antes.TSMS_NM_NOME,
                    TSMS_NR_SISTEMA = antes.TSMS_NR_SISTEMA,
                    TSMS_SG_SIGLA = antes.TSMS_SG_SIGLA,
                    TSMS_TX_CORPO = antes.TSMS_TX_CORPO,
                    EMPR_CD_ID = antes.EMPR_CD_ID,
                };
                return mediDTO;
            }
        }

    }
}
