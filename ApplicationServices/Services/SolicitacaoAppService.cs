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
    public class SolicitacaoAppService : AppServiceBase<SOLICITACAO>, ISolicitacaoAppService
    {
        private readonly ISolicitacaoService _baseService;

        public SolicitacaoAppService(ISolicitacaoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_EXAME> GetAllTipos(Int32 idAss)
        {
            return _baseService.GetAllTipos(idAss);
        }

        public List<SOLICITACAO> GetAllItens(Int32 idAss)
        {
            List<SOLICITACAO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public SOLICITACAO CheckExist(SOLICITACAO conta, Int32 idAss)
        {
            SOLICITACAO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<SOLICITACAO> GetAllItensAdm(Int32 idAss)
        {
            List<SOLICITACAO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public SOLICITACAO GetItemById(Int32 id)
        {
            SOLICITACAO item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<SOLICITACAO>, Boolean> ExecuteFilter(Int32? tipo, String titulo, String descricao, Int32 idAss)
        {
            try
            {
                List<SOLICITACAO> objeto = new List<SOLICITACAO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, titulo, descricao, idAss);
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

        public Int32 ValidateCreate(SOLICITACAO item, USUARIO usuario)
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
                item.SOLI_IN_ATIVO = 1;

                // Monta Log
                DTO_Solicitacao_Modelo dto = MontarSolicitacaoModeloDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Modelo de Solicitação - Inclusão",
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

        public Int32 ValidateEdit(SOLICITACAO item, SOLICITACAO itemAntes, USUARIO usuario)
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
                DTO_Solicitacao_Modelo dto = MontarSolicitacaoModeloDTO(item.SOLI_CD_ID);
                DTO_Solicitacao_Modelo dtoAntes = MontarSolicitacaoModeloDTOObj(itemAntes);
                String json = JsonConvert.SerializeObject(dto, settings);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "Modelo de Solicitação - Alteração",
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

        public Int32 ValidateDelete(SOLICITACAO item, USUARIO usuario)
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

                // Acerta campos
                item.SOLI_IN_ATIVO = 0;

                // Monta Log
                DTO_Solicitacao_Modelo dto = MontarSolicitacaoModeloDTO(item.SOLI_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Modelo de Solicitação - Exclusão",
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

        public Int32 ValidateReativar(SOLICITACAO item, USUARIO usuario)
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
                item.SOLI_IN_ATIVO = 1;

                // Monta Log
                DTO_Solicitacao_Modelo dto = MontarSolicitacaoModeloDTO(item.SOLI_CD_ID);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Modelo de Solicitação - Reativação",
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

        public DTO_Solicitacao_Modelo MontarSolicitacaoModeloDTO(Int32 soliId)
        {
            using (var context = new CRMSysDBEntities())
            {
                var soliDTO = context.SOLICITACAO
                    .Where(l => l.SOLI_CD_ID == soliId)
                    .Select(l => new DTO_Solicitacao_Modelo
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        SOLI_CD_ID = l.SOLI_CD_ID,
                        SOLI_DS_DESCRICAO = l.SOLI_DS_DESCRICAO,
                        SOLI_IN_ATIVO = l.SOLI_IN_ATIVO,
                        SOLI_NM_INDICACAO = l.SOLI_NM_INDICACAO,
                        SOLI_NM_TITULO = l.SOLI_NM_TITULO,
                        TIEX_CD_ID = l.TIEX_CD_ID,
                    })
                    .FirstOrDefault();
                return soliDTO;
            }
        }

        public DTO_Solicitacao_Modelo MontarSolicitacaoModeloDTOObj(SOLICITACAO antes)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var soliDTO = new DTO_Solicitacao_Modelo()
                {
                    ASSI_CD_ID = antes.ASSI_CD_ID,
                    USUA_CD_ID = antes.USUA_CD_ID,
                    SOLI_CD_ID = antes.SOLI_CD_ID,
                    SOLI_DS_DESCRICAO = antes.SOLI_DS_DESCRICAO,
                    SOLI_IN_ATIVO = antes.SOLI_IN_ATIVO,
                    SOLI_NM_INDICACAO = antes.SOLI_NM_INDICACAO,
                    SOLI_NM_TITULO = antes.SOLI_NM_TITULO,
                    TIEX_CD_ID = antes.TIEX_CD_ID,
                };
                return soliDTO;
            }
        }

    }
}
