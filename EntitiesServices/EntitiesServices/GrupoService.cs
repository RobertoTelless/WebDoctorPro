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
    public class GrupoService : ServiceBase<GRUPO_PAC>, IGrupoService
    {
        private readonly IGrupoRepository _baseRepository;
        private readonly ILogRepository _logRepository;
        private readonly IGrupoContatoRepository _contRepository;
        private readonly IPacienteRepository _cliRepository;
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public GrupoService(IGrupoRepository baseRepository, ILogRepository logRepository, IGrupoContatoRepository contRepository, IPacienteRepository cliRepository) : base(baseRepository)
        {
            _baseRepository = baseRepository;
            _logRepository = logRepository;
            _contRepository = contRepository;
            _cliRepository = cliRepository;
        }

        public GRUPO_PAC CheckExist(GRUPO_PAC conta, Int32 idAss)
        {
            GRUPO_PAC item = _baseRepository.CheckExist(conta, idAss);
            return item;
        }

        public GRUPO_PACIENTE CheckExistContato(GRUPO_PACIENTE conta)
        {
            GRUPO_PACIENTE item = _contRepository.CheckExist(conta);
            return item;
        }

        public GRUPO_PAC GetItemById(Int32 id)
        {
            GRUPO_PAC item = _baseRepository.GetItemById(id);
            return item;
        }

        public GRUPO_PACIENTE GetContatoById(Int32 id)
        {
            return _contRepository.GetItemById(id);
        }

        public List<GRUPO_PAC> GetAllItens(Int32 idAss)
        {
            return _baseRepository.GetAllItens(idAss);
        }

        public List<PACIENTE> FiltrarContatos(GRUPO_PAC grupo, Int32 idAss)
        {
            return _cliRepository.FiltrarContatos(grupo, idAss);
        }

        public List<GRUPO_PAC> GetAllItensAdm(Int32 idAss)
        {
            return _baseRepository.GetAllItensAdm(idAss);
        }

        public Int32 Create(GRUPO_PAC item, LOG log)
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

        public Int32 Create(GRUPO_PAC item)
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


        public Int32 Edit(GRUPO_PAC item, LOG log)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    item.TIPO_PACIENTE = null;
                    item.SEXO = null;
                    item.UF = null;
                    item.USUARIO = null;
                    GRUPO_PAC obj = _baseRepository.GetById(item.GRUP_CD_ID);
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

        public Int32 Edit(GRUPO_PAC item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    GRUPO_PAC obj = _baseRepository.GetById(item.GRUP_CD_ID);
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

        public Int32 Delete(GRUPO_PAC item, LOG log)
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

        public Int32 CreateContato(GRUPO_PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    _contRepository.Add(item);
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

        public Int32 EditContato(GRUPO_PACIENTE item)
        {
            using (DbContextTransaction transaction = Db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    GRUPO_PACIENTE obj = _contRepository.GetById(item.GRCL_CD_ID);
                    _contRepository.Detach(obj);
                    _contRepository.Update(item);
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
