using AutoMapper;
using Elepla.Repository.Interfaces;
using Elepla.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public abstract class BaseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly AppConfiguration _appConfiguration;

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration appConfiguration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appConfiguration = appConfiguration;
        }
    }
}
