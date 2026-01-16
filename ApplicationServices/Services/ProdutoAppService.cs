using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;
using ModelServices.Interfaces.Repositories;
using Newtonsoft.Json;

namespace ApplicationServices.Services
{
    public class ProdutoAppService : AppServiceBase<PRODUTO>, IProdutoAppService
    {
        private readonly IProdutoService _baseService;
        private readonly IEmpresaAppService _filService;

        public ProdutoAppService(IProdutoService baseService, IEmpresaAppService filService) : base(baseService)
        {
            _baseService = baseService;
            _filService = filService;
        }

        public PRODUTO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _baseService.GetAnotacaoById(id);
        }

        public MOVIMENTO_ANOTACAO GetAnotacaoMovimentoById(Int32 id)
        {
            return _baseService.GetAnotacaoMovimentoById(id);
        }

        public PRODUTO_ANEXO GetAnexoById(Int32 id)
        {
            return _baseService.GetAnexoById(id);
        }

        public List<PRODUTO> GetAllItens(Int32 idAss)
        {
            List<PRODUTO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<PRODUTO_ESTOQUE_FILIAL> GetAllEstoqueFilial(Int32 idAss)
        {
            List<PRODUTO_ESTOQUE_FILIAL> lista = _baseService.GetAllEstoqueFilial(idAss);
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllMovimentos(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseService.GetAllMovimentos(idAss);
            return lista;
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllMovimentosAdm(Int32 idAss)
        {
            List<MOVIMENTO_ESTOQUE_PRODUTO> lista = _baseService.GetAllMovimentosAdm(idAss);
            return lista;
        }

        public List<PRODUTO> GetAllItensUltimas(Int32 idAss, Int32 linhas)
        {
            List<PRODUTO> lista = _baseService.GetAllItensUltimas(idAss, linhas);
            return lista;
        }

        public List<PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            List<PRODUTO> lista = _baseService.GetAllItensAdm(idAss);
            return lista;
        }

        public PRODUTO GetItemById(Int32 id)
        {
            PRODUTO item = _baseService.GetItemById(id);
            return item;
        }

        public PRODUTO GetByNome(String nome, Int32 idAss)
        {
            PRODUTO item = _baseService.GetByNome(nome, idAss);
            return item;
        }

        public PRODUTO CheckExist(PRODUTO conta, Int32 idAss)
        {
            PRODUTO item = _baseService.CheckExist(conta, idAss);
            return item;
        }

        public PRODUTO CheckExist(String codigo, Int32 idAss)
        {
            PRODUTO item = _baseService.CheckExist(codigo, idAss);
            return item;
        }

        public PRODUTO CheckExistNome(String nome, Int32 idAss)
        {
            PRODUTO item = _baseService.CheckExistNome(nome, idAss);
            return item;
        }

        public PRODUTO CheckExistCodigo(String codigo, Int32 idAss)
        {
            PRODUTO item = _baseService.CheckExistCodigo(codigo, idAss);
            return item;
        }

        public List<CATEGORIA_PRODUTO> GetAllTipos(Int32 idAss)
        {
            List<CATEGORIA_PRODUTO> lista = _baseService.GetAllTipos(idAss);
            return lista;
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllSubs(Int32 idAss)
        {
            List<SUBCATEGORIA_PRODUTO> lista = _baseService.GetAllSubs(idAss);
            return lista;
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            List<UNIDADE> lista = _baseService.GetAllUnidades(idAss);
            return lista;
        }

        public Tuple<Int32, List<PRODUTO>, Boolean> ExecuteFilterTuple(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? composto, DateTime? data, Int32 idAss)
        {
            try
            {
                List<PRODUTO> objeto = new List<PRODUTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilter(catId, subId, nome, marca, codigo, tipo, composto, data, idAss);
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

        public Tuple<Int32, List<PRODUTO>, Boolean> ExecuteFilterTupleEstoque(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? situacao, DateTime? data, Int32 idAss)
        {
            try
            {
                List<PRODUTO> objeto = new List<PRODUTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterEstoque(catId, subId, nome, marca, codigo, tipo, situacao, data, idAss);
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

        public Int32 ValidateCreate(PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Verifica existencia prévia
                if (_baseService.CheckExist(item, usuario.ASSI_CD_ID) != null)
                {
                    return 1;
                }

                // Calcula Preco Promoção caso tenha desconto e vice-versa
                if (item.PROD_PC_DESCONTO != null & item.PROD_PC_DESCONTO > 0)
                {
                    item.PROD_VL_PRECO_PROMOCAO = item.PROD_VL_PRECO_VENDA * (1 - (item.PROD_PC_DESCONTO / 100));
                }
                if (item.PROD_PC_DESCONTO == 0 & item.PROD_VL_PRECO_PROMOCAO > 0)
                {
                    item.PROD_PC_DESCONTO = (1 - (item.PROD_VL_PRECO_PROMOCAO / item.PROD_VL_PRECO_VENDA)) * 100;
                }
                item.PROD_VL_PRECO_ANTERIOR = item.PROD_VL_PRECO_VENDA;
                item.PROD_VL_PRECO_MINIMO = item.PROD_VL_PRECO_VENDA;

                // Completa objeto
                item.PROD_IN_ATIVO = 1;
                item.ASSI_CD_ID = usuario.ASSI_CD_ID;
                item.PROD_IN_SISTEMA = 6;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };


                // Monta Log
                DTO_Produto dto = MontarProdutoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Material/Produto - Inclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
                };

                // Persiste produto
                Int32 volta = _baseService.Create(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DTO_Produto MontarProdutoDTOObj(PRODUTO l)
        {
            using (var context = new CRMSysDBEntities())
            {
                var mediDTO = new DTO_Produto()
                {
                    ASSI_CD_ID = l.ASSI_CD_ID,
                    CAPR_CD_ID = l.CAPR_CD_ID,
                    PROD_CD_CODIGO = l.PROD_CD_CODIGO,
                    PROD_CD_ID = l.PROD_CD_ID,
                    SCPR_CD_ID = l.SCPR_CD_ID,
                    TIEM_CD_ID = l.TIEM_CD_ID,
                    UNID_CD_ID = l.UNID_CD_ID,
                    PROD_AQ_FOTO = l.PROD_AQ_FOTO,
                    PROD_DS_DESCRICAO = l.PROD_DS_DESCRICAO,
                    PROD_DS_INFORMACOES = l.PROD_DS_INFORMACOES,
                    PROD_DT_ALTERACAO = l.PROD_DT_ALTERACAO,
                    PROD_DT_CADASTRO = l.PROD_DT_CADASTRO,
                    PROD_IN_ATIVO = l.PROD_IN_ATIVO,
                    PROD_IN_COMPOSTO = l.PROD_IN_COMPOSTO,
                    PROD_IN_FRACIONADO = l.PROD_IN_FRACIONADO,
                    PROD_IN_LOCACAO = l.PROD_IN_LOCACAO,
                    PROD_IN_PECA = l.PROD_IN_PECA,
                    PROD_IN_SISTEMA = l.PROD_IN_SISTEMA,
                    PROD_IN_TIPO_PRODUTO = l.PROD_IN_TIPO_PRODUTO,
                    PROD_IN_USUARIO_ALTERACAO = l.PROD_IN_USUARIO_ALTERACAO,
                    PROD_NM_FABRICANTE = l.PROD_NM_FABRICANTE,
                    PROD_NM_FORNECEDOR = l.PROD_NM_FORNECEDOR,
                    PROD_NM_MARCA = l.PROD_NM_MARCA,
                    PROD_NM_MODELO = l.PROD_NM_MODELO,
                    PROD_NM_NOME = l.PROD_NM_NOME,
                    PROD_NM_REFERENCIA_FABRICANTE = l.PROD_NM_REFERENCIA_FABRICANTE,
                    PROD_NR_BARCODE = l.PROD_NR_BARCODE,
                    PROD_NR_REFERENCIA = l.PROD_NM_REFERENCIA_FABRICANTE,
                    PROD_PC_DESCONTO = l.PROD_PC_DESCONTO,
                    PROD_TX_OBSERVACOES = l.PROD_TX_OBSERVACOES,
                    PROD_VL_CUSTO = l.PROD_VL_CUSTO,
                    PROD_VL_CUSTO_CONCORRENTE_MEDIO = l.PROD_VL_CUSTO_CONCORRENTE_MEDIO,
                    PROD_VL_CVM_PESO = l.PROD_VL_CVM_PESO,
                    PROD_VL_CVM_RECEITA = l.PROD_VL_CVM_RECEITA,
                    PROD_VL_CVM_UNITARIO = l.PROD_VL_CVM_UNITARIO,
                    PROD_VL_ESTOQUE_ATUAL = l.PROD_VL_ESTOQUE_ATUAL,
                    PROD_VL_ESTOQUE_CUSTO = l.PROD_VL_ESTOQUE_CUSTO,
                    PROD_VL_ESTOQUE_MAXIMO = l.PROD_VL_ESTOQUE_MAXIMO,
                    PROD_VL_ESTOQUE_MINIMO = l.PROD_VL_ESTOQUE_MINIMO,
                    PROD_VL_ESTOQUE_RESERVA = l.PROD_VL_ESTOQUE_RESERVA,
                    PROD_VL_ESTOQUE_TOTAL = l.PROD_VL_ESTOQUE_TOTAL,
                    PROD_VL_ESTOQUE_VENDA = l.PROD_VL_ESTOQUE_VENDA,
                    PROD_VL_FATOR_CORRECAO = l.PROD_VL_FATOR_CORRECAO,
                    PROD_VL_LOCACAO = l.PROD_VL_LOCACAO,
                    PROD_VL_LOCACAO_MULTA = l.PROD_VL_LOCACAO_MULTA,
                    PROD_VL_LOCACAO_PROMOCAO = l.PROD_VL_LOCACAO_PROMOCAO,
                    PROD_VL_LOCACAO_TAXAS = l.PROD_VL_LOCACAO_TAXAS,
                    PROD_VL_MARGEM_CONTRIBUICAO = l.PROD_VL_MARGEM_CONTRIBUICAO,
                    PROD_VL_MEDIA_VENDA_MENSAL = l.PROD_VL_MEDIA_VENDA_MENSAL,
                    PROD_VL_PRECO_ANTERIOR = l.PROD_VL_PRECO_ANTERIOR,
                    PROD_VL_PRECO_MINIMO = l.PROD_VL_PRECO_MINIMO,
                    PROD_VL_PRECO_PROMOCAO = l.PROD_VL_PRECO_PROMOCAO,
                    PROD_VL_PRECO_VENDA = l.PROD_VL_PRECO_VENDA,
                    PROD_VL_ULTIMO_CUSTO = l.PROD_VL_ULTIMO_CUSTO,
                };
                return mediDTO;
            }

        }

        public Int32 ValidateEdit(PRODUTO item, PRODUTO itemAntes, USUARIO usuario)
        {
            try
            {
                // Calcula Preco Promoção caso tenha desconto
                if (item.PROD_PC_DESCONTO != null & item.PROD_PC_DESCONTO > 0)
                {
                    item.PROD_VL_PRECO_PROMOCAO = item.PROD_VL_PRECO_VENDA * (1 - (item.PROD_PC_DESCONTO / 100));
                }
                item.PROD_VL_PRECO_MINIMO = item.PROD_VL_PRECO_VENDA;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                // Monta Log
                DTO_Produto dto = MontarProdutoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                DTO_Produto dtoAntes = MontarProdutoDTOObj(itemAntes);
                String jsonAntes = JsonConvert.SerializeObject(dtoAntes, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Material/Produto - Alteração",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_TX_REGISTRO_ANTES = jsonAntes,
                    LOG_IN_SISTEMA = 6
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

        public Int32 ValidateEditEspec(PRODUTO item, String descricao, USUARIO usuario)
        {
            try
            {
                // Calcula Preco Promoção caso tenha desconto
                if (item.PROD_PC_DESCONTO != null & item.PROD_PC_DESCONTO > 0)
                {
                    item.PROD_VL_PRECO_PROMOCAO = item.PROD_VL_PRECO_VENDA * (1 - (item.PROD_PC_DESCONTO / 100));
                }
                item.PROD_VL_PRECO_MINIMO = item.PROD_VL_PRECO_VENDA;

                // Serialização
                String prod = item.PROD_CD_ID.ToString() + "|" + item.CAPR_CD_ID.ToString() + "|" + item.SCPR_CD_ID.ToString() + "|" + item.PROD_IN_TIPO_PRODUTO.ToString() + "|" + item.PROD_IN_COMPOSTO.ToString() + "|" + item.PROD_NM_NOME.ToString() + item.PROD_IN_ATIVO.ToString();
                prod = Serialization.SerializeJSON<PRODUTO>(item);

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EdtPROD",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = prod,
                    LOG_TX_REGISTRO_ANTES = descricao,
                    LOG_IN_SISTEMA = 6

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

        public Int32 ValidateEdit(PRODUTO item, PRODUTO itemAntes)
        {
            try
            {
                // Calcula Preco Promoção caso tenha desconto
                if (item.PROD_PC_DESCONTO != null & item.PROD_PC_DESCONTO > 0)
                {
                    item.PROD_VL_PRECO_PROMOCAO = item.PROD_VL_PRECO_VENDA * (1 - (item.PROD_PC_DESCONTO / 100));
                }
                item.PROD_VL_PRECO_MINIMO = item.PROD_VL_PRECO_VENDA;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Checa integridade

                // Acerta campos
                item.PROD_IN_ATIVO = 0;

                // Configura serilização
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };


                // Monta Log
                DTO_Produto dto = MontarProdutoDTOObj(item);
                String json = JsonConvert.SerializeObject(dto, settings);
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "Material/Produto - Exclusão",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = json,
                    LOG_IN_SISTEMA = 6
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

        public Int32 ValidateReativar(PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.PROD_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReaPROD",
                    LOG_TX_REGISTRO = "Produto: " + item.PROD_NM_NOME,
                    LOG_IN_SISTEMA = 6

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

        public List<PRODUTO_FALHA> GetAllFalhas(Int32 idAss)
        {
            List<PRODUTO_FALHA> lista = _baseService.GetAllFalhas(idAss);
            return lista;
        }

        public Int32 ValidateEditFalha(PRODUTO_FALHA item)
        {
            try
            {
                // Persiste
                return _baseService.EditFalha(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateFalha(PRODUTO_FALHA item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateFalha(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PRODUTO_CUSTO GetCustoById(Int32 id)
        {
            return _baseService.GetCustoById(id);
        }

        public Int32 ValidateEditCusto(PRODUTO_CUSTO item)
        {
            try
            {
                // Persiste
                return _baseService.EditCusto(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateCusto(PRODUTO_CUSTO item, Int32 idAss)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateCusto(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PRODUTO_CUSTO CheckExistCusto(PRODUTO_CUSTO conta, Int32 idAss)
        {
            PRODUTO_CUSTO item = _baseService.CheckExistCusto(conta, idAss);
            return item;
        }

        public PRODUTO_PRECO_VENDA CheckExistVenda(PRODUTO_PRECO_VENDA conta, Int32 idAss)
        {
            PRODUTO_PRECO_VENDA item = _baseService.CheckExistVenda(conta, idAss);
            return item;
        }

        public PRODUTO_PRECO_VENDA GetPrecoVendaById(Int32 id)
        {
            return _baseService.GetPrecoVendaById(id);
        }

        public PRODUTO_ESTOQUE_FILIAL GetEstoqueFilialById(Int32 id)
        {
            return _baseService.GetEstoqueFilialById(id);
        }

        public Int32 ValidateEditPrecoVenda(PRODUTO_PRECO_VENDA item, Int32 idProd)
        {
            try
            {
                // Calcula Preco Promoção caso tenha desconto
                if (item.PRPV_PC_DESCONTO != null & item.PRPV_PC_DESCONTO > 0)
                {
                    item.PRPV_VL_PRECO_PROMOCAO = item.PRPV_VL_PRECO_VENDA * (1 - (item.PRPV_PC_DESCONTO / 100));
                }
                if (item.PRPV_PC_DESCONTO == 0 & item.PRPV_VL_PRECO_PROMOCAO > 0)
                {
                    item.PRPV_PC_DESCONTO = (1 - (item.PRPV_VL_PRECO_PROMOCAO / item.PRPV_VL_PRECO_VENDA)) * 100;
                }
                item.PRPV_VL_PRECO_EMBALAGEM = item.PRPV_VL_PRECO_VENDA;

                // Persiste
                return _baseService.EditPrecoVenda(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreatePrecoVenda(PRODUTO_PRECO_VENDA item, Int32 idProd, Int32 idAss)
        {
            try
            {
                if (item.PRPV_VL_PRECO_VENDA == null)
                {
                    return 1;
                }
                if (item.PRPV_VL_PRECO_PROMOCAO == null)
                {
                    item.PRPV_VL_PRECO_PROMOCAO = 0;
                }

                // Calcula Preco Promoção caso tenha desconto
                if (item.PRPV_PC_DESCONTO == null)
                {
                    item.PRPV_PC_DESCONTO = 0;
                }
                if (item.PRPV_PC_DESCONTO != null & item.PRPV_PC_DESCONTO > 0)
                {
                    item.PRPV_VL_PRECO_PROMOCAO = item.PRPV_VL_PRECO_VENDA * (1 - (item.PRPV_PC_DESCONTO / 100));
                }
                if (item.PRPV_PC_DESCONTO == 0 & item.PRPV_VL_PRECO_PROMOCAO > 0)
                {
                    item.PRPV_PC_DESCONTO = (1 - (item.PRPV_VL_PRECO_PROMOCAO / item.PRPV_VL_PRECO_VENDA)) * 100;
                }
                item.PRPV_VL_PRECO_EMBALAGEM = item.PRPV_VL_PRECO_VENDA;

                // Persiste
                Int32 volta = _baseService.CreatePrecoVenda(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PRODUTO_CONCORRENTE GetConcorrenteById(Int32 id)
        {
            PRODUTO_CONCORRENTE lista = _baseService.GetConcorrenteById(id);
            return lista;
        }

        public Int32 ValidateEditConcorrente(PRODUTO_CONCORRENTE item)
        {
            try
            {
                // Persiste
                return _baseService.EditConcorrente(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateConcorrente(PRODUTO_CONCORRENTE item)
        {
            try
            {
                // Persiste
                item.PRPF_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateConcorrente(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnotacao(PRODUTO_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnotacao(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnotacaoMovimento(MOVIMENTO_ANOTACAO item)
        {
            try
            {
                // Persiste
                return _baseService.EditAnotacaoMovimento(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetMovimentoById(Int32 id)
        {
            MOVIMENTO_ESTOQUE_PRODUTO lista = _baseService.GetMovimentoById(id);
            return lista;
        }

        public Int32 ValidateEditMovimento(MOVIMENTO_ESTOQUE_PRODUTO item)
        {
            try
            {
                // Persiste
                return _baseService.EditMovimento(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateMovimento(MOVIMENTO_ESTOQUE_PRODUTO item, USUARIO usuario)
        {
            try
            {
                // Monta log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "Estoque - Movimento - Criação",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MOVIMENTO_ESTOQUE_PRODUTO>(item),
                    LOG_IN_SISTEMA = 6

                };

                // Persiste
                item.MOEP_IN_ATIVO = 1;
                item.MOEP_IN_SISTEMA = 6;
                Int32 volta = _baseService.CreateMovimento(item, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateMovimentoCompraManual(MOVIMENTO_ESTOQUE_PRODUTO item, CONTA_BANCO_LANCAMENTO lanc, USUARIO usuario)
        {
            try
            {
                // Monta log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "AddMOVT",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<MOVIMENTO_ESTOQUE_PRODUTO>(item),
                    LOG_IN_SISTEMA = 6

                };

                // Persiste movimento
                item.MOEP_IN_ATIVO = 1;
                item.MOEP_IN_SISTEMA = 6;
                Int32 volta = _baseService.CreateMovimentoCompraManual(item, lanc, log);
                return log.LOG_CD_ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PRODUTO_LOG GetLogById(Int32 id)
        {
            PRODUTO_LOG lista = _baseService.GetLogById(id);
            return lista;
        }

        public Int32 ValidateEditLog(PRODUTO_LOG item)
        {
            try
            {
                // Persiste
                return _baseService.EditLog(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateLog(PRODUTO_LOG item)
        {
            try
            {
                // Persiste
                item.PRLG_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateLog(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEditAnexo(PRODUTO_ANEXO item)
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

        public Int32 ValidateEditEstoqueFilial(PRODUTO_ESTOQUE_FILIAL item)
        {
            try
            {
                // Persiste
                return _baseService.EditEstoqueFilial(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateEstoqueFilial(PRODUTO_ESTOQUE_FILIAL item)
        {
            try
            {
                // Persiste
                item.PREF_IN_ATIVO = 1;
                Int32 volta = _baseService.CreateEstoqueFilial(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public PRODUTO_ESTOQUE_HISTORICO GetEstoqueHistoricoById(Int32 id)
        {
            return _baseService.GetEstoqueHistoricoById(id);
        }

        public Int32 ValidateEditEstoqueHistorico(PRODUTO_ESTOQUE_HISTORICO item)
        {
            try
            {
                // Persiste
                return _baseService.EditEstoqueHistorico(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateCreateEstoqueHistorico(PRODUTO_ESTOQUE_HISTORICO item, Int32 idAss)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.CreateEstoqueHistorico(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<MOVIMENTO_ESTOQUE_PRODUTO>, Boolean> ExecuteFilterTupleMovimento(Int32? es, Int32? tipo, Int32? resp, DateTime? data, Int32 idAss)
        {
            try
            {
                List<MOVIMENTO_ESTOQUE_PRODUTO> objeto = new List<MOVIMENTO_ESTOQUE_PRODUTO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterMovimento(es, tipo, resp, data, idAss);
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
    }
}
