using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Linq;

namespace ApplicationServices.Services
{
    public class ValorConsultaAppService : AppServiceBase<VALOR_CONSULTA>, IValorConsultaAppService
    {
        private readonly IValorConsultaService _baseService;

        public ValorConsultaAppService(IValorConsultaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public List<VALOR_CONSULTA> GetAllItens(Int32 idAss)
        {
            List<VALOR_CONSULTA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<VALOR_CONSULTA> GetAllItensAdm(Int32 idAss)
        {
            List<VALOR_CONSULTA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public VALOR_CONSULTA GetItemById(Int32 id)
        {
            VALOR_CONSULTA item = _baseService.GetItemById(id);
            return item;
        }

        public VALOR_CONSULTA CheckExist(VALOR_CONSULTA conta, Int32 idAss)
        {
            VALOR_CONSULTA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<TIPO_VALOR_CONSULTA> GetAllTipos(Int32 idAss)
        {
            return _baseService.GetAllTipos(idAss);
        }

        public Int32 ValidateCreate(VALOR_CONSULTA item, USUARIO usuario)
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

                    // Verifica padrao
                    List<VALOR_CONSULTA> lista = _baseService.GetAllItens(usuario.ASSI_CD_ID);
                    if (item.VACO_IN_PADRAO == 1 & lista.Where(p => p.VACO_IN_PADRAO == 1).Count() > 0)
                    {
                        return 2;
                    }

                    // Completa objeto
                    item.VACO_IN_ATIVO = 1;
                    item.VACO_NM_EXIBE = item.VACO_NM_NOME + " - R$ " + CrossCutting.Formatters.DecimalFormatter(item.VACO_NR_VALOR.Value);

                    // Persiste
                    Int32 volta = _baseService.Create(item);
                }
                else
                {
                    // Completa objeto
                    item.VACO_IN_ATIVO = 1;

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


        public Int32 ValidateEdit(VALOR_CONSULTA item, VALOR_CONSULTA itemAntes, USUARIO usuario)
        {
            try
            {
                // Verifica padrao
                List<VALOR_CONSULTA> lista = _baseService.GetAllItens(usuario.ASSI_CD_ID);
                if (item.VACO_IN_PADRAO == 1 & lista.Where(p => p.VACO_IN_PADRAO == 1 & p.VACO_CD_ID != item.VACO_CD_ID).Count() > 0)
                {
                    return 2;
                }

                // Completa objeto
                item.VACO_NM_EXIBE = item.VACO_NM_NOME + " - R$ " + CrossCutting.Formatters.DecimalFormatter(item.VACO_NR_VALOR.Value);

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(VALOR_CONSULTA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.CONSULTA_RECEBIMENTO.Count > 0)
                {
                    return 1;
                }
                if (item.PACIENTE_CONSULTA.Count > 0)
                {
                    return 1;
                }

                // Acerta campos
                item.VACO_IN_ATIVO = 0;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(VALOR_CONSULTA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.VACO_IN_ATIVO = 1;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<VALOR_CONSULTA_MATERIAL> GetAllConsultaMaterial(Int32 idAss)
        {
            return _baseService.GetAllConsultaMaterial(idAss);
        }

        public VALOR_CONSULTA_MATERIAL GetConsultaMaterialById(Int32 id)
        {
            VALOR_CONSULTA_MATERIAL lista = _baseService.GetConsultaMaterialById(id);
            return lista;
        }

        public Int32 ValidateEditConsultaMaterial(VALOR_CONSULTA_MATERIAL item)
        {
            try
            {
                // Persiste
                return _baseService.EditConsultaMaterial(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateConsultaMaterial(VALOR_CONSULTA_MATERIAL item)
        {
            try
            {
                item.VCMA_IN_ATIVO = 1;

                // Verifica existencia
                Int32 cd = item.PROD_CD_ID;
                List<VALOR_CONSULTA_MATERIAL> mats = _baseService.GetAllConsultaMaterial(item.ASSI_CD_ID);
                mats = mats.Where(p => p.PROD_CD_ID == item.PROD_CD_ID || p.VACO_CD_ID == item.VACO_CD_ID).ToList();
                if (mats.Count() > 0)
                {
                    return 1;
                }

                // Persiste
                Int32 volta = _baseService.CreateConsultaMaterial(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
