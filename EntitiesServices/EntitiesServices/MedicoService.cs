using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class MedicoService : ServiceBase<MEDICOS>, IMedicoService
    {
        private readonly IMedicoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IEspecialidadeRepository _espRepository;
        private readonly IMedicoEnvioRepository _envRepository;
        private readonly IMedicoAnexoRepository _aneRepository;
        private readonly IMedicoAnotacaoRepository _anoRepository;
        private readonly ITipoEnvioRepository _tenRepository;
        private readonly IMedicoMensagemRepository _mmRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public MedicoService(IMedicoRepository baseRepository, ILogRepository logRepository, IEspecialidadeRepository espRepository, IMedicoEnvioRepository envRepository, IMedicoAnexoRepository aneRepository, IMedicoAnotacaoRepository anoRepository, ITipoEnvioRepository tenRepository, IMedicoMensagemRepository mmRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _espRepository = espRepository;
            _envRepository = envRepository;
            _aneRepository = aneRepository;
            _anoRepository = anoRepository;
            _tenRepository = tenRepository;
            _mmRepository = mmRepository;
        }

        public List<ESPECIALIDADE> GetAllEspec(Int32 idAss)
        {
            return _espRepository.GetAllItens(idAss);
        }

        public List<TIPO_ENVIO> GetAllTipos(Int32 idAss)
        {
            return _tenRepository.GetAllItens(idAss);
        }

        public MEDICOS GetItemById(Int32 id)
        {
            MEDICOS item = _baseRepository.GetItemById(id);
            return item;
        }

        public List<MEDICOS> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<MEDICOS> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public MEDICOS CheckExist(MEDICOS item, Int32 idAss)
        {
            MEDICOS volta = _baseRepository.CheckExist(item, idAss);
            return volta;
        }

        public List<MEDICOS> ExecuteFilter(Int32? espec, String nome, String crm, String email, Int32 idAss)
        {
            return _baseRepository.ExecuteFilter(espec, nome, crm, email, idAss);

        }

        public Int32 Create(MEDICOS item, LOG log)
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

        public Int32 Create(MEDICOS item)
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

        public Int32 Edit(MEDICOS item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MEDICOS obj = _baseRepository.GetById(item.MEDC_CD_ID);
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

        public Int32 Edit(MEDICOS item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MEDICOS obj = _baseRepository.GetById(item.MEDC_CD_ID);
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

        public Int32 Delete(MEDICOS item, LOG log)
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

        public MEDICOS_ENVIO GetEnvioById(Int32 id)
        {
            return _envRepository.GetItemById(id);
        }

        public List<MEDICOS_ENVIO> GetAllEnvio(Int32 idAss)
        {
            return _envRepository.GetAllItens(idAss);
        }

        public Int32 EditEnvio(MEDICOS_ENVIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.MEDICOS = null;
                    item.PACIENTE = null;
                    MEDICOS_ENVIO obj = _envRepository.GetById(item.MEEV_CD_ID);
                    _envRepository.Detach(obj);
                    _envRepository.Update(item);
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

        public Int32 CreateEnvio(MEDICOS_ENVIO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _envRepository.Add(item);
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

        public MEDICOS_ENVIO_ANEXO GetMedicoAnexoById(Int32 id)
        {
            return _aneRepository.GetItemById(id);
        }

        public Int32 EditMedicoAnexo(MEDICOS_ENVIO_ANEXO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MEDICOS_ENVIO_ANEXO obj = _aneRepository.GetById(item.MVAN_CD_ID);
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

        public MEDICOS_ENVIO_ANOTACAO GetAnotacaoById(Int32 id)
        {
            return _anoRepository.GetItemById(id);
        }

        public Int32 EditAnotacao(MEDICOS_ENVIO_ANOTACAO item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.USUARIO = null;
                    MEDICOS_ENVIO_ANOTACAO obj = _anoRepository.GetById(item.MEAT_CD_ID);
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

        public MEDICOS_MENSAGEM CheckExistTextoMensagem(MEDICOS_MENSAGEM item, Int32 idAss)
        {
            MEDICOS_MENSAGEM volta = _mmRepository.CheckExist(item, idAss);
            return volta;
        }

        public MEDICOS_MENSAGEM GetTextoMensagemById(Int32 id)
        {
            MEDICOS_MENSAGEM item = _mmRepository.GetItemById(id);
            return item;
        }

        public List<MEDICOS_MENSAGEM> GetAllTextoMensagem(Int32 idAss)
        {
            return _mmRepository.GetAllItens(idAss);
        }

        public Int32 EditTextoMensagem(MEDICOS_MENSAGEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    MEDICOS_MENSAGEM obj = _mmRepository.GetById(item.METX_CD_ID);
                    _mmRepository.Detach(obj);
                    _mmRepository.Update(item);
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

        public Int32 CreateTextoMensagem(MEDICOS_MENSAGEM item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _mmRepository.Add(item);
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

    }
}
