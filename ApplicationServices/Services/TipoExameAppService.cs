using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class TipoExameAppService : AppServiceBase<TIPO_EXAME>, ITipoExameAppService
    {
        private readonly ITipoExameService _baseService;

        public TipoExameAppService(ITipoExameService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<TIPO_EXAME> GetAllItens(Int32 idAss)
        {
            List<TIPO_EXAME> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<TIPO_EXAME> GetAllItensAdm(Int32 idAss)
        {
            List<TIPO_EXAME> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public TIPO_EXAME GetItemById(Int32 id)
        {
            TIPO_EXAME item = _baseService.GetItemById(id);
            return item;
        }

        public TIPO_EXAME CheckExist(TIPO_EXAME conta, Int32 idAss)
        {
            TIPO_EXAME item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(TIPO_EXAME item, USUARIO usuario)
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
                    item.TIEX_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddTIEX",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_EXAME>(item),
                        LOG_IN_SISTEMA = 1
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.TIEX_IN_ATIVO = 1;

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

        public Int32 ValidateCreate(TIPO_EXAME item)
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


        public Int32 ValidateEdit(TIPO_EXAME item, TIPO_EXAME itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtTIEX",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<TIPO_EXAME>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<TIPO_EXAME>(itemAntes),
                    LOG_IN_SISTEMA = 1
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(TIPO_EXAME item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PACIENTE_EXAMES.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.TIEX_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelTIEX",
                    LOG_TX_REGISTRO = item.TIEX_NM_NOME,
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(TIPO_EXAME item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.TIEX_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaTIEX",
                    LOG_TX_REGISTRO = item.TIEX_NM_NOME,
                    LOG_IN_SISTEMA = 1

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
