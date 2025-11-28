using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IProdutoAppService : IAppServiceBase<PRODUTO>
    {
        Int32 ValidateCreate(PRODUTO perfil, USUARIO usuario);
        Int32 ValidateEdit(PRODUTO perfil, PRODUTO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(PRODUTO item, PRODUTO itemAntes);
        Int32 ValidateEditEspec(PRODUTO perfil, String texto, USUARIO usuario);
        Int32 ValidateDelete(PRODUTO perfil, USUARIO usuario);
        Int32 ValidateReativar(PRODUTO perfil, USUARIO usuario);

        List<PRODUTO> GetAllItens(Int32 idAss);
        List<PRODUTO> GetAllItensAdm(Int32 idAss);
        PRODUTO GetItemById(Int32 id);
        PRODUTO GetByNome(String nome, Int32 idAss);
        PRODUTO CheckExist(PRODUTO conta, Int32 idAss);
        PRODUTO CheckExist(String codigo, Int32 idAss);
        List<CATEGORIA_PRODUTO> GetAllTipos(Int32 idAss);
        List<SUBCATEGORIA_PRODUTO> GetAllSubs(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        Tuple<Int32, List<PRODUTO>, Boolean> ExecuteFilterTuple(Int32? catId, Int32? subId, String nome, String marca, String codigo,  Int32? tipo, Int32? composto, DateTime? data, Int32 idAss);
        PRODUTO_ANEXO GetAnexoById(Int32 id);
        PRODUTO_ANOTACAO GetAnotacaoById(Int32 id);
        PRODUTO CheckExistNome(String nome, Int32 idAss);
        PRODUTO CheckExistCodigo(String codigo, Int32 idAss);
        List<PRODUTO> GetAllItensUltimas(Int32 idAss, Int32 linhas);
        Int32 ValidateEditAnexo(PRODUTO_ANEXO item);
        MOVIMENTO_ANOTACAO GetAnotacaoMovimentoById(Int32 id);
        Int32 ValidateEditAnotacaoMovimento(MOVIMENTO_ANOTACAO item);

        List<PRODUTO_FALHA> GetAllFalhas(Int32 idAss);
        Int32 ValidateEditFalha(PRODUTO_FALHA item);
        Int32 ValidateCreateFalha(PRODUTO_FALHA item);
        Int32 ValidateEditAnotacao(PRODUTO_ANOTACAO item);

        PRODUTO_CUSTO CheckExistCusto(PRODUTO_CUSTO conta, Int32 idAss);
        PRODUTO_CUSTO GetCustoById(Int32 id);
        Int32 ValidateEditCusto(PRODUTO_CUSTO item);
        Int32 ValidateCreateCusto(PRODUTO_CUSTO item, Int32 idAss);

        PRODUTO_PRECO_VENDA CheckExistVenda(PRODUTO_PRECO_VENDA conta, Int32 idAss);
        PRODUTO_PRECO_VENDA GetPrecoVendaById(Int32 id);
        Int32 ValidateEditPrecoVenda(PRODUTO_PRECO_VENDA item, Int32 idProd);
        Int32 ValidateCreatePrecoVenda(PRODUTO_PRECO_VENDA item, Int32 idProd, Int32 idAss);

        PRODUTO_CONCORRENTE GetConcorrenteById(Int32 id);
        Int32 ValidateEditConcorrente(PRODUTO_CONCORRENTE item);
        Int32 ValidateCreateConcorrente(PRODUTO_CONCORRENTE item);

        MOVIMENTO_ESTOQUE_PRODUTO GetMovimentoById(Int32 id);
        Int32 ValidateEditMovimento(MOVIMENTO_ESTOQUE_PRODUTO item);
        Int32 ValidateCreateMovimento(MOVIMENTO_ESTOQUE_PRODUTO item, USUARIO usuario);
        Int32 ValidateCreateMovimentoCompraManual(MOVIMENTO_ESTOQUE_PRODUTO item, CONTA_BANCO_LANCAMENTO lanc, USUARIO usuario);

        PRODUTO_LOG GetLogById(Int32 id);
        Int32 ValidateEditLog(PRODUTO_LOG item);
        Int32 ValidateCreateLog(PRODUTO_LOG item);

        PRODUTO_ESTOQUE_FILIAL GetEstoqueFilialById(Int32 id);
        Int32 ValidateEditEstoqueFilial(PRODUTO_ESTOQUE_FILIAL item);
        Int32 ValidateCreateEstoqueFilial(PRODUTO_ESTOQUE_FILIAL item);
        List<PRODUTO_ESTOQUE_FILIAL> GetAllEstoqueFilial(Int32 idAss);

        PRODUTO_ESTOQUE_HISTORICO GetEstoqueHistoricoById(Int32 id);
        Int32 ValidateEditEstoqueHistorico(PRODUTO_ESTOQUE_HISTORICO item);
        Int32 ValidateCreateEstoqueHistorico(PRODUTO_ESTOQUE_HISTORICO item, Int32 idAss);

        Tuple<Int32, List<MOVIMENTO_ESTOQUE_PRODUTO>, Boolean> ExecuteFilterTupleMovimento(Int32? es, Int32? tipo, Int32? resp, DateTime? data, Int32 idAss);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllMovimentos(Int32 idAss);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllMovimentosAdm(Int32 idAss);

        Tuple<Int32, List<PRODUTO>, Boolean> ExecuteFilterTupleEstoque(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? situacao, DateTime? data, Int32 idAss);

    }
}
