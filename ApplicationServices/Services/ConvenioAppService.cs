using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class ConvenioAppService : AppServiceBase<CONVENIO>, IConvenioAppService
    {
        private readonly IConvenioService _baseService;

        public ConvenioAppService(IConvenioService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<CONVENIO> GetAllItens(Int32 idAss)
        {
            List<CONVENIO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<CONVENIO> GetAllItensAdm(Int32 idAss)
        {
            List<CONVENIO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public CONVENIO GetItemById(Int32 id)
        {
            CONVENIO item = _baseService.GetItemById(id);
            return item;
        }

        public CONVENIO CheckExist(CONVENIO conta, Int32 idAss)
        {
            CONVENIO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(CONVENIO item, USUARIO usuario)
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
                    item.CONV_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddCONV",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<CONVENIO>(item),
                        LOG_IN_SISTEMA = 1
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.CONV_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(CONVENIO item, CONVENIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtCONV",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<CONVENIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<CONVENIO>(itemAntes),
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

        public Int32 ValidateDelete(CONVENIO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PACIENTE.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.CONV_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelCONV",
                    LOG_TX_REGISTRO = item.CONV_NM_NOME,
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

        public Int32 ValidateReativar(CONVENIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.CONV_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaCONV",
                    LOG_TX_REGISTRO = item.CONV_NM_NOME,
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
