using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class ValorConvenioAppService : AppServiceBase<VALOR_CONVENIO>, IValorConvenioAppService
    {
        private readonly IValorConvenioService _baseService;
        private readonly IConvenioService _servService;

        public ValorConvenioAppService(IValorConvenioService baseService, IConvenioService servService) : base(baseService)
        {
            _baseService = baseService;
            _servService = servService;
        }


        public List<VALOR_CONVENIO> GetAllItens(Int32 idAss)
        {
            List<VALOR_CONVENIO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<VALOR_CONVENIO> GetAllItensAdm(Int32 idAss)
        {
            List<VALOR_CONVENIO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public VALOR_CONVENIO GetItemById(Int32 id)
        {
            VALOR_CONVENIO item = _baseService.GetItemById(id);
            return item;
        }

        public VALOR_CONVENIO CheckExist(VALOR_CONVENIO conta, Int32 idAss)
        {
            VALOR_CONVENIO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<CONVENIO> GetAllConvenios(Int32 idAss)
        {
            return _baseService.GetAllConvenios(idAss);
        }

        public Int32 ValidateCreate(VALOR_CONVENIO item, USUARIO usuario)
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
                    item.VACV_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddVACV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<VALOR_CONVENIO>(item),
                        LOG_IN_SISTEMA = 6
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.VACV_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(VALOR_CONVENIO item, VALOR_CONVENIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtVACV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<VALOR_CONVENIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<VALOR_CONVENIO>(itemAntes),
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

        public Int32 ValidateDelete(VALOR_CONVENIO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CONSULTA_RECEBIMENTO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.VACV_IN_ATIVO = 0;

                // Monta Log
                CONVENIO serv = _servService.GetItemById(item.CONV_CD_ID.Value);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelVACV",
                    LOG_TX_REGISTRO = serv.CONV_NM_NOME,
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

        public Int32 ValidateReativar(VALOR_CONVENIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.VACV_IN_ATIVO = 1;

                // Monta Log
                CONVENIO serv = _servService.GetItemById(item.CONV_CD_ID.Value);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaVACV",
                    LOG_TX_REGISTRO = serv.CONV_NM_NOME,
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
