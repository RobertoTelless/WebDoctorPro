using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Linq;

namespace ApplicationServices.Services
{
    public class TipoValorConsultaAppService : AppServiceBase<TIPO_VALOR_CONSULTA>, ITipoValorConsultaAppService
    {
        private readonly ITipoValorConsultaService _baseService;

        public TipoValorConsultaAppService(ITipoValorConsultaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<TIPO_VALOR_CONSULTA> GetAllItens(Int32 idAss)
        {
            List<TIPO_VALOR_CONSULTA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_VALOR_CONSULTA> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_VALOR_CONSULTA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_VALOR_CONSULTA GetItemById(Int32 id)
        {
            TIPO_VALOR_CONSULTA item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_VALOR_CONSULTA CheckExist(TIPO_VALOR_CONSULTA conta, Int32 idAss)
        {
            TIPO_VALOR_CONSULTA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_VALOR_CONSULTA item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.TIVL_IN_ATIVO = 1;

                if (usuario != null)
                {
                    // Verifica existencia prévia
                    if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                    {
                        return 1;
                    }

                    // Completa objeto
                    item.TIVL_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddTIVL",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_VALOR_CONSULTA>(item),
                        LOG_IN_SISTEMA = 6
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.TIVL_IN_ATIVO = 1;


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

        public Int32 ValidateCreate(TIPO_VALOR_CONSULTA item)
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

        public Int32 ValidateEdit(TIPO_VALOR_CONSULTA item, TIPO_VALOR_CONSULTA itemAntes, USUARIO usuario)
        {
            try
            {
                // Verifica padrao e exxistencia
                List<TIPO_VALOR_CONSULTA> lista = _baseService.GetAllItens(usuario.ASSI_CD_ID);
                if (lista.Where(p => p.TIVL_NM_TIPO == item.TIVL_NM_TIPO & p.TIVL_CD_ID != item.TIVL_CD_ID).Count() > 0)
                {
                    return 1;
                }

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtTIVL",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_VALOR_CONSULTA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_VALOR_CONSULTA>(itemAntes),
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

        public Int32 ValidateDelete(TIPO_VALOR_CONSULTA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.VALOR_CONSULTA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TIVL_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIVL",
                    LOG_TX_REGISTRO = item.TIVL_NM_TIPO,
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

        public Int32 ValidateReativar(TIPO_VALOR_CONSULTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIVL_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaTIVL",
                    LOG_TX_REGISTRO = item.TIVL_NM_TIPO,
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
