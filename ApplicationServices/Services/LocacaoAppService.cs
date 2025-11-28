using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using Newtonsoft.Json;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace ApplicationServices.Services
{
    public class LocacaoAppService : AppServiceBase<LOCACAO>, ILocacaoAppService
    {
        private readonly ILocacaoService _baseService;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public LocacaoAppService(ILocacaoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<TIPO_HISTORICO> GetAllTipos(Int32 idAss)
        {
            return _baseService.GetAllTipos(idAss);
        }

        public List<LOCACAO> GetAllItens(Int32 idAss)
        {
            List<LOCACAO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public LOCACAO CheckExist(LOCACAO conta, Int32 idAss)
        {
            LOCACAO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<LOCACAO> GetAllItensAdm(Int32 idAss)
        {
            List<LOCACAO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public LOCACAO GetItemById(Int32 id)
        {
            LOCACAO item = _baseService.GetItemById(id);
            return item;
        }

        public Tuple<Int32, List<LOCACAO>, Boolean> ExecuteFilter(String paciente, String prod, DateTime? inicio, DateTime? final, Int32? status, String numero, Int32 idAss)
        {
            try
            {
                List<LOCACAO> objeto = new List<LOCACAO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(paciente, prod, inicio, final, status, numero, idAss);
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

        public Int32 ValidateCreate(LOCACAO item, USUARIO usuario)
        {
            Db.Configuration.LazyLoadingEnabled = false;
            Db.Configuration.ProxyCreationEnabled = false;
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.LOCA_IN_ATIVO = 1;

                // Monta Log
                String json = JsonConvert.SerializeObject(item, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "AddLOCA",
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

        public Int32 ValidateEdit(LOCACAO item, LOCACAO itemAntes, USUARIO usuario)
        {
            // Monta DTO
            DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);

            // Configura serilização
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                // Monta Log
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EdtLOCA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = null,
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

        public DTO_Locacao MontarLocacaoDTO(Int32 locacaoId)
        {
            using (var context = new CRMSysDBEntities())
            {
                // A chave está em: Filtrar o item desejado ANTES do Select
                var locacaoDTO = context.LOCACAO
                    .Where(l => l.LOCA_CD_ID == locacaoId)
                    .Select(l => new DTO_Locacao
                    {
                        ASSI_CD_ID = l.ASSI_CD_ID,
                        LOCA_CD_ID = l.LOCA_CD_ID,
                        PACI_CD_ID = l.PACI_CD_ID,
                        PROD_CD_ID = l.PROD_CD_ID,
                        USUA_CD_ID = l.USUA_CD_ID,
                        LOCA_DT_INICIO = l.LOCA_DT_INICIO,
                        LOCA_NR_PRAZO = l.LOCA_NR_PRAZO,
                        LOCA_IN_QUANTIDADE = l.LOCA_IN_QUANTIDADE,
                        LOCA_VL_PARCELA = l.LOCA_VL_PARCELA,
                        LOCA_VL_TOTAL = l.LOCA_VL_TOTAL,
                        LOCA_IN_ATIVO = l.LOCA_IN_ATIVO,
                        LOCA_GU_GUID = l.LOCA_GU_GUID,
                        LOCA_IN_STATUS = l.LOCA_IN_STATUS,
                        LOCA_NR_NUMERO = l.LOCA_NR_NUMERO,
                        LOCA_NM_TITULO = l.LOCA_NM_TITULO,
                        LOCA_DS_DESCRICAO = l.LOCA_DS_DESCRICAO,
                        LOCA_NR_DIA = l.LOCA_NR_DIA,
                        LOCA_DT_CANCELAMENTO = l.LOCA_DT_CANCELAMENTO,
                        LOCA_DS_JUSTIFICATIVA = l.LOCA_DS_JUSTIFICATIVA,
                        LOCA_DT_ENCERRAMENTO = l.LOCA_DT_ENCERRAMENTO,
                        LOCA_DT_FINAL = l.LOCA_DT_FINAL,
                        LOCA_DT_RENOVACAO = l.LOCA_DT_RENOVACAO,
                        LOCA_IN_RENOVACOES = l.LOCA_IN_RENOVACOES,
                        LOCA_IN_RENOVACAO = l.LOCA_IN_RENOVACAO,
                        LOCA_NR_SERIE = l.LOCA_NR_SERIE,
                        LOCA_IN_GARANTIA = l.LOCA_IN_GARANTIA,
                        LOCA_DT_GARANTIA = l.LOCA_DT_GARANTIA,
                        LOCA_NR_GARANTIA = l.LOCA_NR_GARANTIA,
                        LOCA_DT_APROVACAO = l.LOCA_DT_APROVACAO,
                        LOCA_IN_CONTRATO = l.LOCA_IN_CONTRATO,
                        LOCA_XM_NOTA_FISCAL = l.LOCA_XM_NOTA_FISCAL,
                        LOCA_TK_TOKEN = l.LOCA_TK_TOKEN,
                        LOCA_AQ_ARQUIVO_QRCODE = l.LOCA_AQ_ARQUIVO_QRCODE,
                        LOCA_DT_EMISSAO = l.LOCA_DT_EMISSAO,
                        LOCA_IN_ASSINADO_DIGITAL = l.LOCA_IN_ASSINADO_DIGITAL,
                    })
                    .FirstOrDefault();
                return locacaoDTO;
            }
        }

        public Int32 ValidateDelete(LOCACAO item, USUARIO usuario)
        {
            // Monta DTO
            DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);

            // Configura serilização
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                // Checa integridade

                // Acerta campos
                item.LOCA_IN_ATIVO = 0;

                // Monta Log
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelLOCA",
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

        public Int32 ValidateReativar(LOCACAO item, USUARIO usuario)
        {
            // Monta DTO
            DTO_Locacao dto = MontarLocacaoDTO(item.LOCA_CD_ID);

            // Configura serilização
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.LOCA_IN_ATIVO = 1;

                // Monta Log
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaLOCA",
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

        public LOCACAO_ANEXO GetLocacaoAnexoById(Int32 id)
        {
            LOCACAO_ANEXO lista = _baseService.GetLocacaoAnexoById(id);
            return lista;
        }

        public Int32 ValidateEditLocacaoAnexo(LOCACAO_ANEXO item)
        {
            try
            {
                // Persiste
                return _baseService.EditLocacaoAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public LOCACAO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            LOCACAO_ANOTACAO lista = _baseService.GetAnotacaoById(id);
            return lista;
        }

        public Int32 ValidateEditAnotacao(LOCACAO_ANOTACAO item)
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

        public List<LOCACAO_PARCELA> GetAllParcelas(Int32 idAss)
        {
            return _baseService.GetAllParcelas(idAss);
        }

        public LOCACAO_PARCELA GetParcelaById(Int32 id)
        {
            LOCACAO_PARCELA lista = _baseService.GetParcelaById(id);
            return lista;
        }

        public Int32 ValidateEditParcela(LOCACAO_PARCELA item)
        {
            try
            {
                // Persiste
                return _baseService.EditParcela(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateParcela(LOCACAO_PARCELA item)
        {
            try
            {
                item.LOPA_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateParcela(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public LOCACAO_OCORRENCIA GetOcorrenciaById(Int32 id)
        {
            LOCACAO_OCORRENCIA lista = _baseService.GetOcorrenciaById(id);
            return lista;
        }

        public Int32 ValidateEditOcorrencia(LOCACAO_OCORRENCIA item)
        {
            try
            {
                // Persiste
                return _baseService.EditOcorrencia(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateOcorrencia(LOCACAO_OCORRENCIA item)
        {
            try
            {
                item.LOOC_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateOcorrencia(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<LOCACAO_HISTORICO> GetAllHistorico(Int32 idAss)
        {
            return _baseService.GetAllHistorico(idAss);
        }

        public LOCACAO_HISTORICO GetHistoricoById(Int32 id)
        {
            LOCACAO_HISTORICO lista = _baseService.GetHistoricoById(id);
            return lista;
        }

        public Int32 ValidateCreateHistorico(LOCACAO_HISTORICO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateHistorico(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<LOCACAO_HISTORICO>, Boolean> ExecuteFilterTupleHistorico(Int32? tipo, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss)
        {
            try
            {
                List<LOCACAO_HISTORICO> objeto = new List<LOCACAO_HISTORICO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterHistorico(tipo, paci, inicio, final, descricao, idAss);
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

        public Tuple<Int32, List<LOCACAO_PARCELA>, Boolean> ExecuteFilterTupleParcela(Int32? tipo, Int32? paci, DateTime? inicio, DateTime? final, String descricao, Int32 idAss)
        {
            try
            {
                List<LOCACAO_PARCELA> objeto = new List<LOCACAO_PARCELA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterParcela(tipo, paci, inicio, final, descricao, idAss);
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

        public List<TIPO_CONTRATO> GetAllTipoContrato(Int32 idAss)
        {
            return _baseService.GetAllTipoContrato(idAss);
        }

        public List<CONTRATO_LOCACAO> GetAllContratos(Int32 idAss)
        {
            List<CONTRATO_LOCACAO> lista = _baseService.GetAllContratos(idAss);
            return lista;
        }

        public CONTRATO_LOCACAO CheckExistContrato(CONTRATO_LOCACAO conta, Int32 idAss)
        {
            CONTRATO_LOCACAO item = _baseService.CheckExistContrato(conta, idAss);
            return item;
        }

        public CONTRATO_LOCACAO GetContratoById(Int32 id)
        {
            CONTRATO_LOCACAO item = _baseService.GetContratoById(id);
            return item;
        }

        public Int32 ValidateEditContrato(CONTRATO_LOCACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditContrato(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateContrato(CONTRATO_LOCACAO item)
        {
            try
            {
                item.COLO_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateContrato(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
