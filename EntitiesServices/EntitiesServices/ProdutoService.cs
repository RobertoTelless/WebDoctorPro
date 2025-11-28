using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class ProdutoService : ServiceBase<PRODUTO>, IProdutoService
    {
        private readonly IProdutoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly ICategoriaProdutoRepository _tipoRepository;
        private readonly IUnidadeRepository _unidRepository;
        private readonly ISubcategoriaProdutoRepository _subRepository;
        private readonly IProdutoAnexoRepository _aneRepository;
        private readonly IProdutoAnotacaoRepository _anoRepository;
        private readonly IProdutoFalhaRepository _flRepository;
        private readonly IProdutoCustoRepository _cusRepository;
        private readonly IProdutoPrecoVendaRepository _venRepository;
        private readonly IProdutoConcorrenteRepository _pcRepository;
        private readonly IMovimentacaoEstoqueRepository _movRepository;
        private readonly IProdutoLogRepository _prlRepository;
        private readonly IProdutoEstoqueFilialRepository _estRepository;
        private readonly IProdutoEstoqueHistoricoRepository _hisRepository;
        private readonly IMovimentoAnotacaoRepository _moanRepository;

        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public ProdutoService(IProdutoRepository baseRepository, ILogRepository logRepository, ICategoriaProdutoRepository tipoRepository, IUnidadeRepository unidRepository, ISubcategoriaProdutoRepository subRepository, IProdutoAnexoRepository aneRepository, IProdutoAnotacaoRepository anoRepository, IProdutoFalhaRepository flRepository, IProdutoCustoRepository cusRepository, IProdutoPrecoVendaRepository venRepository, IProdutoConcorrenteRepository pcRepository, IMovimentacaoEstoqueRepository movRepository, IProdutoLogRepository prlRepository, IProdutoEstoqueFilialRepository estRepository, IProdutoEstoqueHistoricoRepository hisRepository, IMovimentoAnotacaoRepository moanRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _tipoRepository = tipoRepository;
            _unidRepository = unidRepository;
            _subRepository = subRepository;
            _aneRepository = aneRepository;
            _anoRepository = anoRepository;
            _flRepository = flRepository;
            _cusRepository = cusRepository;
            _venRepository = venRepository;
            _pcRepository = pcRepository;
            _movRepository = movRepository;
            _prlRepository = prlRepository;
            _estRepository = estRepository;
            _hisRepository = hisRepository;
            _moanRepository = moanRepository;
        }

        public PRODUTO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public MOVIMENTO_ANOTACAO GetAnotacaoMovimentoById(Int32 id)
        {
            return _moanRepository.GetItemById(id);
        }

        public PRODUTO_ANEXO GetAnexoById(Int32 id)
        {
            return _aneRepository.GetItemById(id);
        }

        public PRODUTO CheckExist(PRODUTO conta, Int32 idAss)
        {
            PRODUTO item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public PRODUTO CheckExist(String codigo, Int32 idAss)
        {
            PRODUTO item = _baseRepository.CheckExist(codigo, idAss);
            return item;
        }

        public PRODUTO CheckExistNome(String nome, Int32 idAss)
        {
            PRODUTO item = _baseRepository.CheckExistNome(nome, idAss);
            return item;
        }

        public PRODUTO CheckExistCodigo(String codigo, Int32 idAss)
        {
            PRODUTO item = _baseRepository.CheckExistCodigo(codigo, idAss);
            return item;
        }

        public PRODUTO GetItemById(Int32 id)
        {
            PRODUTO item = _baseRepository.GetItemById(id);
            return item;
        }

        public PRODUTO GetByNome(String nome, Int32 idAss)
        {
            PRODUTO item = _baseRepository.GetByNome(nome, idAss);
            return item;
        }

        public List<PRODUTO> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public async Task<List<PRODUTO>> GetAllItensAsync(Int32 idAss)
        {
            return await _baseRepository.GetAllItensAsync(idAss);
        }

        public List<PRODUTO> GetAllItensUltimas(Int32 idAss, Int32 linhas)
        {
            List<PRODUTO> lista =  _baseRepository.GetAllItensUltimas(idAss, linhas);
            return lista;
        }

        public async Task<List<PRODUTO>> GetAllItensUltimasAsync(Int32 idAss, Int32 linhas)
        {
            List<PRODUTO> lista = await _baseRepository.GetAllItensUltimasAsync(idAss, linhas);
            return lista;
        }

        public List<PRODUTO> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public List<PRODUTO_ESTOQUE_FILIAL> GetAllEstoqueFilial(Int32 idAss)
        {
            return _estRepository.GetAllItens(idAss);
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllMovimentos(Int32 idAss)
        {
            return _movRepository.GetAllItens(idAss);
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> GetAllMovimentosAdm(Int32 idAss)
        {
            return _movRepository.GetAllItensAdm(idAss);
        }

        public List<CATEGORIA_PRODUTO> GetAllTipos(Int32 idAss)
        {
            return _tipoRepository.GetAllItens(idAss);
        }

        public List<SUBCATEGORIA_PRODUTO> GetAllSubs(Int32 idAss)
        {
            return _subRepository.GetAllItens(idAss);
        }

        public List<UNIDADE> GetAllUnidades(Int32 idAss)
        {
            return _unidRepository.GetAllItens(idAss);
        }

        public List<PRODUTO> ExecuteFilter(Int32? catId, Int32? subId, String nome, String marca, String codigo,  Int32? tipo, Int32? composto, DateTime? data, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(catId, subId, nome, marca, codigo, tipo, composto, data, idAss);

        }

        public List<PRODUTO> ExecuteFilterEstoque(Int32? catId, Int32? subId, String nome, String marca, String codigo, Int32? tipo, Int32? situacao, DateTime? data, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(catId, subId, nome, marca, codigo, tipo, situacao, data, idAss);

        }

        public Int32 Create(PRODUTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Create(PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public Int32 Edit(PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.TIPO_EMBALAGEM = null;
                    item.ASSINANTE = null;
                    item.USUARIO = null;
                    PRODUTO obj = _baseRepository.GetById(item.PROD_CD_ID);
                    _baseRepository.Detach(obj);
                    _logRepository.Add(log);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Edit(PRODUTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.CRM_PEDIDO_VENDA_ITEM = null;
                    PRODUTO obj = _baseRepository.GetById(item.PROD_CD_ID);
                    _baseRepository.Detach(obj);
                    _baseRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 Delete(PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _baseRepository.Remove(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditFalha(PRODUTO_FALHA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_FALHA obj = _flRepository.GetById(item.PRFA_CD_ID);
                    _flRepository.Detach(obj);
                    _flRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateFalha(PRODUTO_FALHA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _flRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<PRODUTO_FALHA> GetAllFalhas(Int32 idAss)
        {
            return _flRepository.GetAllItens();
        }

        public PRODUTO_CUSTO GetCustoById(Int32 id)
        {
            return _cusRepository.GetItemById(id);
        }

        public Int32 EditCusto(PRODUTO_CUSTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_CUSTO obj = _cusRepository.GetById(item.PRCU_CD_ID);
                    _cusRepository.Detach(obj);
                    _cusRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateCusto(PRODUTO_CUSTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _cusRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PRODUTO_PRECO_VENDA GetPrecoVendaById(Int32 id)
        {
            return _venRepository.GetItemById(id);
        }

        public PRODUTO_ESTOQUE_FILIAL GetEstoqueFilialById(Int32 id)
        {
            return _estRepository.GetItemById(id);
        }

        public Int32 EditPrecoVenda(PRODUTO_PRECO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_PRECO_VENDA obj = _venRepository.GetById(item.PRPV_CD_ID);
                    _venRepository.Detach(obj);
                    _venRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreatePrecoVenda(PRODUTO_PRECO_VENDA item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _venRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PRODUTO_CUSTO CheckExistCusto(PRODUTO_CUSTO conta, Int32 idAss)
        {
            PRODUTO_CUSTO item = _cusRepository.CheckExist(conta, idAss);
            return item;
        }

        public PRODUTO_PRECO_VENDA CheckExistVenda(PRODUTO_PRECO_VENDA conta, Int32 idAss)
        {
            PRODUTO_PRECO_VENDA item = _venRepository.CheckExist(conta, idAss);
            return item;
        }

        public PRODUTO_CONCORRENTE GetConcorrenteById(Int32 id)
        {
            return _pcRepository.GetItemById(id);
        }

        public Int32 EditConcorrente(PRODUTO_CONCORRENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_CONCORRENTE obj = _pcRepository.GetById(item.PRPF_CD_ID);
                    _pcRepository.Detach(obj);
                    _pcRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateConcorrente(PRODUTO_CONCORRENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _pcRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditAnotacao(PRODUTO_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    PRODUTO_ANOTACAO obj = _anoRepository.GetById(item.PRAT_CD_ID);
                    _anoRepository.Detach(obj);
                    _anoRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditAnotacaoMovimento(MOVIMENTO_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    MOVIMENTO_ANOTACAO obj = _moanRepository.GetById(item.MOAN_CD_ID);
                    _moanRepository.Detach(obj);
                    _moanRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public MOVIMENTO_ESTOQUE_PRODUTO GetMovimentoById(Int32 id)
        {
            return _movRepository.GetItemById(id);
        }

        public Int32 EditMovimento(MOVIMENTO_ESTOQUE_PRODUTO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MOVIMENTO_ESTOQUE_PRODUTO obj = _movRepository.GetById(item.MOEP_CD_ID);
                    _movRepository.Detach(obj);
                    _movRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateMovimento(MOVIMENTO_ESTOQUE_PRODUTO item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _movRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
            
        public Int32 CreateMovimentoCompraManual(MOVIMENTO_ESTOQUE_PRODUTO item, CONTA_BANCO_LANCAMENTO lanc, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _movRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PRODUTO_LOG GetLogById(Int32 id)
        {
            return _prlRepository.GetItemById(id);
        }

        public Int32 EditLog(PRODUTO_LOG item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_LOG obj = _prlRepository.GetById(item.PRLG_CD_ID);
                    _prlRepository.Detach(obj);
                    _prlRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateLog(PRODUTO_LOG item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _prlRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditAnexo(PRODUTO_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_ANEXO obj = _aneRepository.GetById(item.PRAN_CD_ID);
                    _aneRepository.Detach(obj);
                    _aneRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 EditEstoqueFilial(PRODUTO_ESTOQUE_FILIAL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.EMPRESA_FILIAL = null;
                    item.PRODUTO = null;
                    PRODUTO_ESTOQUE_FILIAL obj = _estRepository.GetById(item.EMFI_CD_ID);
                    _estRepository.Detach(obj);
                    _estRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateEstoqueFilial(PRODUTO_ESTOQUE_FILIAL item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _estRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public PRODUTO_ESTOQUE_HISTORICO GetEstoqueHistoricoById(Int32 id)
        {
            return _hisRepository.GetItemById(id);
        }

        public Int32 EditEstoqueHistorico(PRODUTO_ESTOQUE_HISTORICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PRODUTO_ESTOQUE_HISTORICO obj = _hisRepository.GetById(item.PREH_CD_ID);
                    _hisRepository.Detach(obj);
                    _hisRepository.Update(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public Int32 CreateEstoqueHistorico(PRODUTO_ESTOQUE_HISTORICO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _hisRepository.Add(item);
                    transaction.Commit();
                    return 0;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<MOVIMENTO_ESTOQUE_PRODUTO> ExecuteFilterMovimento(Int32? es, Int32? tipo, Int32? resp, DateTime? data, Int32 idAss)
        {
            return _movRepository.ExecuteFilter(es, tipo, resp, data, idAss);

        }
    }
}
