using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using ModelServices.Interfaces.EntitiesServices;
using System.Data.Entity;
using System.Data;

namespace ModelServices.EntitiesServices
{
    public class PerfilService : ServiceBase<PERFIL>, IPerfilService
    {
        private readonly IPerfilRepository _perfilRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public PerfilService(IPerfilRepository perfilRepository, ILogRepository logRepository, IConfiguracaoRepository configuracaoRepository) : base(perfilRepository)
        {
            _perfilRepository = perfilRepository;
            _logRepository = logRepository;
            _configuracaoRepository = configuracaoRepository;

        }

        public PERFIL CheckExist(PERFIL conta, Int32? idAss)
        {
            PERFIL item = _perfilRepository.CheckExist(conta, idAss);
            return item;
        }

        public USUARIO GetUserProfile(PERFIL perfil)
        {
            USUARIO usuario = _perfilRepository.GetUserProfile(perfil);
            return usuario;
        }

        public PERFIL GetItemById(Int32? id)
        {
            PERFIL item = _perfilRepository.GetItemById(id);
            return item;
        }

        public PERFIL GetByName(String nome, Int32? idAss)
        {
            PERFIL item = _perfilRepository.GetByName(nome, idAss);
            return item;
        }

        public List<PERFIL> GetAllItens(Int32? idAss)
        {
            return _perfilRepository.GetAllItens(idAss);
        }

        public Int32 Create(PERFIL perfil, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _perfilRepository.Add(perfil);
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

        public Int32 Create(PERFIL perfil)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _perfilRepository.Add(perfil);
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


        public Int32 Edit(PERFIL perfil, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PERFIL obj = _perfilRepository.GetById(perfil.PERF_CD_ID);
                    _perfilRepository.Detach(obj);
                    _logRepository.Add(log);
                    _perfilRepository.Update(perfil);
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

        public Int32 Edit(PERFIL perfil)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    PERFIL obj = _perfilRepository.GetById(perfil.PERF_CD_ID);
                    _perfilRepository.Detach(obj);
                    _perfilRepository.Update(perfil);
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

        public Int32 Delete(PERFIL perfil, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _logRepository.Add(log);
                    _perfilRepository.Remove(perfil);
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
