using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class EspecialidadeAppService : AppServiceBase<ESPECIALIDADE>, IEspecialidadeAppService
    {
        private readonly IEspecialidadeService _baseService;

        public EspecialidadeAppService(IEspecialidadeService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<ESPECIALIDADE> GetAllItens(Int32 idAss)
        {
            List<ESPECIALIDADE> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<ESPECIALIDADE> GetAllItensAdm(Int32 idAss)
        {
            List<ESPECIALIDADE> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public ESPECIALIDADE GetItemById(Int32 id)
        {
            ESPECIALIDADE item = _baseService.GetItemById(id);
            return item;
        }

        public ESPECIALIDADE CheckExist(ESPECIALIDADE conta, Int32 idAss)
        {
            ESPECIALIDADE item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(ESPECIALIDADE item, USUARIO usuario)
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
                    item.ESPE_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddESPE",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<ESPECIALIDADE>(item),
                        LOG_IN_SISTEMA = 6
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.ESPE_IN_ATIVO = 1;

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

        public Int32 ValidateCreate(ESPECIALIDADE item)
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

        public Int32 ValidateEdit(ESPECIALIDADE item, ESPECIALIDADE itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtESPE",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<ESPECIALIDADE>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<ESPECIALIDADE>(itemAntes),
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

        public Int32 ValidateDelete(ESPECIALIDADE item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.USUARIO.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.ESPE_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelESPE",
                    LOG_TX_REGISTRO = item.ESPE_NM_NOME,
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

        public Int32 ValidateReativar(ESPECIALIDADE item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.ESPE_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaESPE",
                    LOG_TX_REGISTRO = item.ESPE_NM_NOME,
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
