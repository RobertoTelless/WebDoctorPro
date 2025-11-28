using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class TipoValorServicoAppService : AppServiceBase<TIPO_SERVICO_CONSULTA>, ITipoValorServicoAppService
    {
        private readonly ITipoValorServicoService _baseService;

        public TipoValorServicoAppService(ITipoValorServicoService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<TIPO_SERVICO_CONSULTA> GetAllItens(Int32 idAss)
        {
            List<TIPO_SERVICO_CONSULTA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_SERVICO_CONSULTA> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_SERVICO_CONSULTA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_SERVICO_CONSULTA GetItemById(Int32 id)
        {
            TIPO_SERVICO_CONSULTA item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_SERVICO_CONSULTA CheckExist(TIPO_SERVICO_CONSULTA conta, Int32 idAss)
        {
            TIPO_SERVICO_CONSULTA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_SERVICO_CONSULTA item, USUARIO usuario)
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
                    item.SERV_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddSERV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_SERVICO_CONSULTA>(item),
                        LOG_IN_SISTEMA = 6
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.SERV_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(TIPO_SERVICO_CONSULTA item, TIPO_SERVICO_CONSULTA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtSERV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_SERVICO_CONSULTA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_SERVICO_CONSULTA>(itemAntes),
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

        public Int32 ValidateDelete(TIPO_SERVICO_CONSULTA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.VALOR_SERVICO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.SERV_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelSERV",
                    LOG_TX_REGISTRO = item.SERV_NM_SERVICO,
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

        public Int32 ValidateReativar(TIPO_SERVICO_CONSULTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.SERV_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaSERV",
                    LOG_TX_REGISTRO = item.SERV_NM_SERVICO,
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
