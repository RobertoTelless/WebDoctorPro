using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;

namespace ApplicationServices.Services
{
    public class EmpresaAppService : AppServiceBase<EMPRESA>, IEmpresaAppService
    {
        private readonly IEmpresaService _baseService;

        public EmpresaAppService(IEmpresaService baseService) : base(baseService)
        {
            _baseService = baseService;
        }

        public EMPRESA CheckExist(EMPRESA conta, Int32 idAss)
        {
            EMPRESA item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public List<EMPRESA> GetAllItens(Int32 idAss)
        {
            List<EMPRESA> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<EMPRESA> GetAllItensAdm(Int32 idAss)
        {
            List<EMPRESA> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public EMPRESA GetItemById(Int32 id)
        {
            EMPRESA item = _baseService.GetItemById(id);
            return item;
        }

        public EMPRESA GetItemByAssinante(Int32 id)
        {
            EMPRESA item = _baseService.GetItemByAssinante(id);
            return item;
        }

        public List<UF> GetAllUF()
        {
            List<UF> lista = _baseService.GetAllUF();
            return lista;
        }

        public UF GetUFbySigla(String sigla)
        {
            return _baseService.GetUFbySigla(sigla);
        }

        public EMPRESA_ANEXO GetAnexoById(Int32 id)
        {
            EMPRESA_ANEXO lista = _baseService.GetAnexoById(id);
            return lista;
        }

        public Tuple<Int32, List<EMPRESA>, Boolean> ExecuteFilterTuple(String nome, Int32 idAss)
        {
            try
            {
                List<EMPRESA> objeto = new List<EMPRESA>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(nome, idAss);
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

        //public List<MAQUINA> GetAllMaquinas(Int32 idAss)
        //{
        //    return _baseService.GetAllMaquinas(idAss);
        //}

        //public List<REGIME_TRIBUTARIO> GetAllRegimes()
        //{
        //    return _baseService.GetAllRegimes();
        //}

        public Int32 ValidateCreate(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Completa objeto
                item.EMPR_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.EMPR_DT_CADASTRO = DateTime.Today.Date;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddEMPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item),
                    LOG_IN_SISTEMA = 1

                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreate(EMPRESA item)
        {
            try
            {

                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log                
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_NM_OPERACAO = "EdtEMPR",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<EMPRESA>(itemAntes),
                    LOG_IN_SISTEMA = 2

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

        public Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes)
        {
            try
            {
                // Remonta empresa
                EMPRESA nova = new EMPRESA();
                nova.ASSI_CD_ID = item.ASSI_CD_ID;
                nova.EMPR_AQ_LOGO = item.EMPR_AQ_LOGO;
                nova.EMPR_CD_ID = item.EMPR_CD_ID;
                nova.EMPR_DT_CADASTRO = item.EMPR_DT_CADASTRO;
                nova.EMPR_IN_ATIVO = item.EMPR_IN_ATIVO;
                nova.EMPR_IN_CALCULADO = item.EMPR_IN_CALCULADO;
                nova.EMPR_IN_MATRIZ = item.EMPR_IN_MATRIZ;
                nova.EMPR_IN_OPERA_CARTAO = item.EMPR_IN_OPERA_CARTAO;
                nova.EMPR_IN_PAGA_COMISSAO= item.EMPR_IN_PAGA_COMISSAO;
                nova.EMPR_NM_BAIRRO = item.EMPR_NM_BAIRRO;
                nova.EMPR_NM_CIDADE = item.EMPR_NM_CIDADE;
                nova.EMPR_NM_COMPLEMENTO = item.EMPR_NM_COMPLEMENTO;
                nova.EMPR_NM_EMAIL = item.EMPR_NM_EMAIL;
                nova.EMPR_NM_ENDERECO = item.EMPR_NM_ENDERECO;
                nova.EMPR_NM_GERENTE = item.EMPR_NM_GERENTE;
                nova.EMPR_NM_NOME = item.EMPR_NM_NOME;
                nova.EMPR_NM_NUMERO = item.EMPR_NM_NUMERO;
                nova.EMPR_NM_OUTRA_MAQUINA = item.EMPR_NM_OUTRA_MAQUINA;
                nova.EMPR_NM_RAZAO = item.EMPR_NM_RAZAO;
                nova.EMPR_NR_CELULAR = item.EMPR_NR_CELULAR;
                nova.EMPR_NR_CEP = item.EMPR_NR_CEP;
                nova.EMPR_NR_CNPJ = item.EMPR_NR_CNPJ;
                nova.EMPR_NR_CPF = item.EMPR_NR_CPF;
                nova.EMPR_NR_INSCRICAO_ESTADUAL = item.EMPR_NR_INSCRICAO_ESTADUAL;
                nova.EMPR_NR_INSCRICAO_MUNICIPAL = item.EMPR_NR_INSCRICAO_MUNICIPAL;
                nova.EMPR_NR_TELEFONE = item.EMPR_NR_TELEFONE;
                nova.EMPR_PC_ANTECIPACAO = item.EMPR_PC_ANTECIPACAO;
                nova.EMPR_PC_CUSTO_ANTECIPACOES = item.EMPR_PC_CUSTO_ANTECIPACOES;
                nova.EMPR_PC_CUSTO_VARIAVEL_TOTAL = item.EMPR_PC_CUSTO_VARIAVEL_TOTAL;
                nova.EMPR_PC_CUSTO_VARIAVEL_VENDA = item.EMPR_PC_CUSTO_VARIAVEL_VENDA;
                nova.EMPR_PC_VENDA_CREDITO = item.EMPR_PC_VENDA_CREDITO;
                nova.EMPR_PC_VENDA_DEBITO = item.EMPR_PC_VENDA_DEBITO;
                nova.EMPR_PC_VENDA_DINHEIRO = item.EMPR_PC_VENDA_DINHEIRO;
                nova.EMPR_VL_COMISSAO_GERENTE = item.EMPR_VL_COMISSAO_GERENTE;
                nova.EMPR_VL_COMISSAO_OUTROS = item.EMPR_VL_COMISSAO_OUTROS;
                nova.EMPR_VL_COMISSAO_VENDEDOR = item.EMPR_VL_COMISSAO_VENDEDOR;
                nova.EMPR_VL_FUNDO_PROPAGANDA = item.EMPR_VL_FUNDO_PROPAGANDA;
                nova.EMPR_VL_FUNDO_SEGURANCA = item.EMPR_VL_FUNDO_SEGURANCA;
                nova.EMPR_VL_IMPOSTO_MEI = item.EMPR_VL_IMPOSTO_MEI;
                nova.EMPR_VL_IMPOSTO_OUTROS = item.EMPR_VL_IMPOSTO_OUTROS;
                nova.EMPR_VL_PATRIMONIO_LIQUIDO = item.EMPR_VL_PATRIMONIO_LIQUIDO;
                nova.EMPR_VL_ROYALTIES = item.EMPR_VL_ROYALTIES;
                nova.EMPR_VL_TAXA_MEDIA = item.EMPR_VL_TAXA_MEDIA;
                nova.EMPR_VL_TAXA_MEDIA_DEBITO = item.EMPR_VL_TAXA_MEDIA_DEBITO;
                nova.MAQN_CD_ID = item.MAQN_CD_ID;
                nova.RETR_CD_ID = item.RETR_CD_ID;
                nova.UF_CD_ID = item.UF_CD_ID;
                nova.TIPE_CD_ID = item.TIPE_CD_ID;

                // Persiste
                return _baseService.Edit(nova);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.EMPR_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelEMPR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item),
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

        public Int32 ValidateReativar(EMPRESA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.EMPR_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaEMPR",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<EMPRESA>(item),
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
        public Int32 ValidateEditAnexo(EMPRESA_ANEXO item)
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

        public EMPRESA_FILIAL GetFilialById(Int32 id)
        {
            EMPRESA_FILIAL lista = _baseService.GetFilialById(id);
            return lista;
        }

        public Int32 ValidateEditFilial(EMPRESA_FILIAL item)
        {
            try
            {
                // Persiste
                return _baseService.EditFilial(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateFilial(EMPRESA_FILIAL item)
        {
            try
            {
                item.EMFI_IN_ATIVO = 1;

                // Persiste
                Int32 volta = _baseService.CreateFilial(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public EMPRESA_FILIAL CheckExistFilial(EMPRESA_FILIAL tarefa, Int32 idAss)
        {
            EMPRESA_FILIAL item = _baseService.CheckExistFilial(tarefa, idAss);
            return item;
        }
    }
}
