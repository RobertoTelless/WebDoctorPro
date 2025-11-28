using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class TipoPagamentoAppService : AppServiceBase<TIPO_PAGAMENTO>, ITipoPagamentoAppService
    {
        private readonly ITipoPagamentoService _baseService;

        public TipoPagamentoAppService(ITipoPagamentoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<TIPO_PAGAMENTO> GetAllItens(Int32 idAss)
        {
            List<TIPO_PAGAMENTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_PAGAMENTO> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_PAGAMENTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_PAGAMENTO GetItemById(Int32 id)
        {
            TIPO_PAGAMENTO item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_PAGAMENTO CheckExist(TIPO_PAGAMENTO conta, Int32 idAss)
        {
            TIPO_PAGAMENTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_PAGAMENTO item, USUARIO usuario)
        {
            try
            {
                if (usuario != null)
                {
                    // Verifica existencia prévia
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }

                    // Completa objeto
                    item.TIPA_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddTIPA",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_PAGAMENTO>(item),
                        LOG_IN_SISTEMA = 6
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.TIPA_IN_ATIVO = 1;

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(TIPO_PAGAMENTO item)
        {
            try
            {

                // Persiste
                Int32 volta = _baseService.Create(item);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(TIPO_PAGAMENTO item, TIPO_PAGAMENTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtTIPA",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_PAGAMENTO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_PAGAMENTO>(itemAntes),
                    LOG_IN_SISTEMA = 6
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TIPO_PAGAMENTO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CONSULTA_PAGAMENTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TIPA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIPA",
                    LOG_TX_REGISTRO = item.TIPA_NM_PAGAMENTO,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_PAGAMENTO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIPA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaTIPA",
                    LOG_TX_REGISTRO = item.TIPA_NM_PAGAMENTO,
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
