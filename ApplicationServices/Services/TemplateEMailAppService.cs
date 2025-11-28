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
    public class TemplateEMailAppService : AppServiceBase<TEMPLATE_EMAIL>, ITemplateEMailAppService
    {
        private readonly ITemplateEMailService _baseService;

        public TemplateEMailAppService(ITemplateEMailService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TEMPLATE_EMAIL> GetAllItens(Int32 idAss)
        {
            List<TEMPLATE_EMAIL> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public TEMPLATE_EMAIL CheckExist(TEMPLATE_EMAIL conta, Int32 idAss)
        {
            TEMPLATE_EMAIL item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TEMPLATE_EMAIL> GetAllItensAdm(Int32 idAss)
        {
            List<TEMPLATE_EMAIL> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TEMPLATE_EMAIL GetItemById(Int32 id)
        {
            TEMPLATE_EMAIL item = _baseService.GetItemById(id);
            return item;
        }

        public TEMPLATE_EMAIL GetByCode(String sigla, Int32 idAss)
        {
            TEMPLATE_EMAIL item = _baseService.GetByCode(sigla, idAss);
            return item;
        }

        public Tuple<Int32, List<TEMPLATE_EMAIL>, Boolean> ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss)
        {
            try
            {
                List<TEMPLATE_EMAIL> objeto = new List<TEMPLATE_EMAIL>();
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

        public Int32 ValidateCreate(TEMPLATE_EMAIL item, USUARIO usuario)
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
                item.TEEM_IN_ATIVO = 1;
                item.TEEM_TX_COMPLETO = item.TEEM_TX_CABECALHO +  item.TEEM_TX_CORPO + item.TEEM_TX_DADOS;

                // Monta Log
                DTO_ModeloEMail dto = MontarModeloEMailDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Modelo de E-MAil - Inclusão",
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

        public Int32 ValidateCreate(TEMPLATE_EMAIL item)
        {
            try
            {
                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TEMPLATE_EMAIL item, TEMPLATE_EMAIL itemAntes, USUARIO usuario)
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
                DTO_ModeloEMail dto = MontarModeloEMailDTO(item.TEEM_CD_ID);
                DTO_ModeloEMail dtoAntes = MontarModeloEMailDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                item.EMPRESA = null;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Modelo de E-MAil - Alteração",
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

        public Int32 ValidateDelete(TEMPLATE_EMAIL item, USUARIO usuario)
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
                item.TEEM_IN_ATIVO = 0;

                // Monta Log
                DTO_ModeloEMail dto = MontarModeloEMailDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                item.EMPRESA = null;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Modelo de E-MAil - Exclusão",
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

        public Int32 ValidateReativar(TEMPLATE_EMAIL item, USUARIO usuario)
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
                item.TEEM_IN_ATIVO = 1;

                // Monta Log
                DTO_ModeloEMail dto = MontarModeloEMailDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                item.EMPRESA = null;
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Modelo de E-MAil - Reativação",
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

        public DTO_ModeloEMail MontarModeloEMailDTO(Int32 mediId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = context.TEMPLATE_EMAIL
                    .Where(l => l.TEEM_CD_ID == mediId)
                    .Select(l => new DTO_ModeloEMail
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        TEEM_AQ_ARQUIVO = l.TEEM_AQ_ARQUIVO,
                        TEEM_IN_ANIVERSARIO = l.TEEM_IN_ANIVERSARIO,
                        TEEM_IN_ATIVO = l.TEEM_IN_ATIVO,
                        TEEM_IN_EDITAVEL = l.TEEM_IN_EDITAVEL,
                        TEEM_IN_IMAGEM = l.TEEM_IN_IMAGEM,
                        TEEM_IN_PESQUISA = l.TEEM_IN_PESQUISA,
                        TEEM_IN_SISTEMA = l.TEEM_IN_SISTEMA,
                        TEEM_SG_SIGLA = l.TEEM_SG_SIGLA,
                        TEEM_TX_CABECALHO = l.TEEM_TX_CABECALHO,
                        TEEM_TX_DADOS = l.TEEM_TX_DADOS,
                        EMPR_CD_ID = l.EMPR_CD_ID,
                        TEEM_CD_ID = l.TEEM_CD_ID,
                        TEEM_IN_FIXO = l.TEEM_IN_FIXO,
                        TEEM_IN_HTML = l.TEEM_IN_HTML,
                        TEEM_IN_ROBOT = l.TEEM_IN_ROBOT,
                        TEEM_LK_LINK = l.TEEM_LK_LINK,
                        TEEM_NM_NOME = l.TEEM_NM_NOME,
                        TEEM_TX_COMPLETO = l.TEEM_TX_COMPLETO,
                        TEEM_TX_CORPO = l.TEEM_TX_CORPO,
                    })
                    .FirstOrDefault();
                return mediDTO;
            }
        }

        public DTO_ModeloEMail MontarModeloEMailDTOObj(TEMPLATE_EMAIL antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_ModeloEMail()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    TEEM_AQ_ARQUIVO = antes.TEEM_AQ_ARQUIVO,
                    TEEM_IN_ANIVERSARIO = antes.TEEM_IN_ANIVERSARIO,
                    TEEM_IN_ATIVO = antes.TEEM_IN_ATIVO,
                    TEEM_IN_EDITAVEL = antes.TEEM_IN_EDITAVEL,
                    TEEM_IN_IMAGEM = antes.TEEM_IN_IMAGEM,
                    TEEM_IN_PESQUISA = antes.TEEM_IN_PESQUISA,
                    TEEM_IN_SISTEMA = antes.TEEM_IN_SISTEMA,
                    TEEM_SG_SIGLA = antes.TEEM_SG_SIGLA,
                    TEEM_TX_CABECALHO = antes.TEEM_TX_CABECALHO,
                    TEEM_TX_DADOS = antes.TEEM_TX_DADOS,
                    EMPR_CD_ID = antes.EMPR_CD_ID,
                    TEEM_CD_ID = antes.TEEM_CD_ID,
                    TEEM_IN_FIXO = antes.TEEM_IN_FIXO,
                    TEEM_IN_HTML = antes.TEEM_IN_HTML,
                    TEEM_IN_ROBOT = antes.TEEM_IN_ROBOT,
                    TEEM_LK_LINK = antes.TEEM_LK_LINK,
                    TEEM_NM_NOME = antes.TEEM_NM_NOME,
                    TEEM_TX_COMPLETO = antes.TEEM_TX_COMPLETO,
                    TEEM_TX_CORPO = antes.TEEM_TX_CORPO,
                };
                return mediDTO;
            }
        }
    }
}
