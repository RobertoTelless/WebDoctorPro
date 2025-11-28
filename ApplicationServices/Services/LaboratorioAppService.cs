using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class LaboratorioAppService : AppServiceBase<LABORATORIO>, ILaboratorioAppService
    {
        private readonly ILaboratorioService _baseService;

        public LaboratorioAppService(ILaboratorioService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<LABORATORIO> GetAllItens(Int32 idAss)
        {
            List<LABORATORIO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<LABORATORIO> GetAllItensAdm(Int32 idAss)
        {
            List<LABORATORIO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public LABORATORIO GetItemById(Int32 id)
        {
            LABORATORIO item = _baseService.GetItemById(id);
            return item;
        }

        public LABORATORIO CheckExist(LABORATORIO conta, Int32 idAss)
        {
            LABORATORIO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public Int32 ValidateCreate(LABORATORIO item, USUARIO usuario)
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
                    item.LABS_IN_ATIVO = 1;

                    // Monta Log
                    LOG log = new LOG
                    {
                        LOG_DT_DATA = DateTime.Now,
                        ASSI_CD_ID = usuario.ASSI_CD_ID,
                        USUA_CD_ID = usuario.USUA_CD_ID,
                        LOG_NM_OPERACAO = "AddLABS",
                        LOG_IN_ATIVO = 1,
                        LOG_TX_REGISTRO = Serialization.SerializeJSON<LABORATORIO>(item),
                        LOG_IN_SISTEMA = 6
                    };

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.LABS_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(LABORATORIO item, LABORATORIO itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtLABS",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<LABORATORIO>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<LABORATORIO>(itemAntes),
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

        public Int32 ValidateDelete(LABORATORIO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.PACIENTE_EXAMES.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.LABS_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelLABS",
                    LOG_TX_REGISTRO = item.LABS_NM_NOME,
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

        public Int32 ValidateReativar(LABORATORIO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.LABS_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaLABS",
                    LOG_TX_REGISTRO = item.LABS_NM_NOME,
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

        public List<UF> GetAllUF()
        {
            return _baseService.GetAllUF();
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }
    }
}
