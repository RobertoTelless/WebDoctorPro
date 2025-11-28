using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IProdutoService : IServiceBase<PRODUTO>
    {
        Int32 Create(PRODUTO perfil);
        Int32 Create(PRODUTO perfil, LOG log);
        Int32 Edit(PRODUTO perfil, LOG log);
        Int32 Edit(PRODUTO perfil);
        Int32 Delete(PRODUTO perfil, LOG log);

        PRODUTO CheckExist(PRODUTO conta, Int32 idAss);
        PRODUTO CheckExist(String codigo, Int32 idAss);
        PRODUTO GetItemById(Int32 id);
        PRODUTO GetByNome(String nome, Int32 idAss);
        List<PRODUTO> GetAllItens(Int32 idAss);
        List<PRODUTO> GetAllItensAdm(Int32 idAss);
        PRODUTO_ANEXO GetAnexoById(Int32 id);
        PRODUTO_ANOTACAO GetAnotacaoById(Int32 id);
        PRODUTO CheckExistNome(String nome, Int32 idAss);
        PRODUTO CheckExistCodigo(String codigo, Int32 idAss);
        MOVIMENTO_ANOTACAO GetAnotacaoMovimentoById(Int32 id);

        List<CATEGORIA_PRODUTO> GetAllTipos(Int32 idAss);
        List<SUBCATEGORIA_PRODUTO> GetAllSubs(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<PRODUTO> ExecuteFilter(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? composto, DateTime? data, Int32 idAss);
        List<PRODUTO> GetAllItensUltimas(Int32 idAss, Int32 linhas);

        List<PRODUTO_FALHA> GetAllFalhas(Int32 idAss);
        Int32 EditFalha(PRODUTO_FALHA item);
        Int32 CreateFalha(PRODUTO_FALHA item);

        Int32 EditAnotacao(PRODUTO_ANOTACAO item);
        Int32 EditAnexo(PRODUTO_ANEXO item);
        Int32 EditAnotacaoMovimento(MOVIMENTO_ANOTACAO item);

        PRODUTO_CUSTO CheckExistCusto(PRODUTO_CUSTO conta, Int32 idAss);
        PRODUTO_CUSTO GetCustoById(Int32 id);
        Int32 EditCusto(PRODUTO_CUSTO item);
        Int32 CreateCusto(PRODUTO_CUSTO item);

        PRODUTO_PRECO_VENDA CheckExistVenda(PRODUTO_PRECO_VENDA conta, Int32 idAss);
        PRODUTO_PRECO_VENDA GetPrecoVendaById(Int32 id);
        Int32 EditPrecoVenda(PRODUTO_PRECO_VENDA item);
        Int32 CreatePrecoVenda(PRODUTO_PRECO_VENDA item);

        PRODUTO_CONCORRENTE GetConcorrenteById(Int32 id);
        Int32 EditConcorrente(PRODUTO_CONCORRENTE item);
        Int32 CreateConcorrente(PRODUTO_CONCORRENTE item);

        MOVIMENTO_ESTOQUE_PRODUTO GetMovimentoById(Int32 id);
        Int32 EditMovimento(MOVIMENTO_ESTOQUE_PRODUTO item);
        Int32 CreateMovimento(MOVIMENTO_ESTOQUE_PRODUTO item, LOG log);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllMovimentos(Int32 idAss);
        List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllMovimentosAdm(Int32 idAss);
        Int32 CreateMovimentoCompraManual(MOVIMENTO_ESTOQUE_PRODUTO item, CONTA_BANCO_LANCAMENTO lanc, LOG log);

        PRODUTO_LOG GetLogById(Int32 id);
        Int32 EditLog(PRODUTO_LOG item);
        Int32 CreateLog(PRODUTO_LOG item);

        PRODUTO_ESTOQUE_FILIAL GetEstoqueFilialById(Int32 id);
        Int32 EditEstoqueFilial(PRODUTO_ESTOQUE_FILIAL item);
        Int32 CreateEstoqueFilial(PRODUTO_ESTOQUE_FILIAL item);
        List<PRODUTO_ESTOQUE_FILIAL> GetAllEstoqueFilial(Int32 idAss);

        PRODUTO_ESTOQUE_HISTORICO GetEstoqueHistoricoById(Int32 id);
        Int32 EditEstoqueHistorico(PRODUTO_ESTOQUE_HISTORICO item);
        Int32 CreateEstoqueHistorico(PRODUTO_ESTOQUE_HISTORICO item);

        List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilterMovimento(Int32? es, Int32? tipo, Int32? resp, DateTime? data, Int32 idAss);
        List<PRODUTO> ExecuteFilterEstoque(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? situacao, DateTime? data, Int32 idAss);

    }
}
