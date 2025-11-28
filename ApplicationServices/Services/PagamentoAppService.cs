using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Linq;

namespace ApplicationServices.Services
{
    public class PagamentoAppService : AppServiceBase<CONSULTA_PAGAMENTO>, IPagamentoAppService
    {
        private readonly IPagamentoService _baseService;
        private readonly IConfiguracaoService _confService;

        public PagamentoAppService(IPagamentoService baseService, IConfiguracaoService confService) : base(baseService)
        {
            _baseService = baseService;
            _confService = confService;
        }

        public CONSULTA_PAGAMENTO GetItemById(Int32 id)
        {
            CONSULTA_PAGAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public List<CONSULTA_PAGAMENTO> GetAllItens(Int32 idAss)
        {
            return _baseService.GetAllItens(idAss);
        }

        public Tuple<Int32, List<CONSULTA_PAGAMENTO>, Boolean> ExecuteFilterTuple(Int32? tipo, String nome, String favorecido, DateTime? inicio, DateTime? final, Int32? conferido, Int32 idAss)
        {
            try
            {
                List<CONSULTA_PAGAMENTO> objeto = new List<CONSULTA_PAGAMENTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(tipo, nome, favorecido, inicio, final, conferido, idAss);
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

        public PAGAMENTO_ANEXO GetAnexoById(Int32 id)
        {
            PAGAMENTO_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Int32 ValidateEditAnexo(PAGAMENTO_ANEXO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnexo(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PAGAMENTO_NOTA_FISCAL GetNotaById(Int32 id)
        {
            PAGAMENTO_NOTA_FISCAL lista = _baseService.GetNotaById(id);
            return lista;
        }

        public Int32 ValidateEditNota(PAGAMENTO_NOTA_FISCAL item)
        {
            try
            {
                // Persiste
                return _baseService.EditNota(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PAGAMENTO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            PAGAMENTO_ANOTACAO lista = _baseService.GetAnotacaoById(id);
            return lista;
        }

        public Int32 ValidateEditAnotacao(PAGAMENTO_ANOTACAO item)
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

        public List<TIPO_PAGAMENTO> GetAllTipos(Int32 idAss)
        {
            List<TIPO_PAGAMENTO> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public Int32 ValidateCreate(CONSULTA_PAGAMENTO item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.COPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddCOPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONSULTA_PAGAMENTO>(item),
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

        public Int32 ValidateEdit(CONSULTA_PAGAMENTO item, CONSULTA_PAGAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                String linha = "Id:" + item.COPA_CD_ID + "|Nome:" + item.COPA_NM_NOME + "|Data:" + item.COPA_DT_PAGAMENTO.ToString() + "|Valor:" + item.COPA_VL_VALOR.ToString();
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtCOPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = linha,
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

        public Int32 ValidateEditConfirma(CONSULTA_PAGAMENTO item, CONSULTA_PAGAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                String linha = "Id:" + item.COPA_CD_ID + "|Nome:" + item.COPA_NM_NOME + "|Data:" + item.COPA_DT_PAGAMENTO.ToString() + "|Valor:" + item.COPA_VL_VALOR.ToString();
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "VfdCOPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = linha,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                Int32 volta = _baseService.EditConfirma(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(CONSULTA_PAGAMENTO item, CONSULTA_PAGAMENTO itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(CONSULTA_PAGAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.COPA_IN_ATIVO = 0;

                // Monta Log
                String linha = "Id:" + item.COPA_CD_ID + "|Nome:" + item.COPA_NM_NOME + "|Data:" + item.COPA_DT_PAGAMENTO.ToString() + "|Valor:" + item.COPA_VL_VALOR.ToString();
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCOPA",
                    LOG_TX_REGISTRO = linha,
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


    }
}
