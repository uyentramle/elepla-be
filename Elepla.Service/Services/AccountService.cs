using AutoMapper;
using Elepla.Repository.Interfaces;
using Elepla.Service.Common;
using Elepla.Service.Interfaces;
using Elepla.Service.Utils;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Services
{
    public class AccountService : BaseService, IAccountService
    {
        public AccountService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITimeService timeService,
            IPasswordService passwordHasher,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IMemoryCache cache)
            : base(unitOfWork, mapper, timeService, passwordHasher, emailSender, smsSender, cache)
        {
        }
    }
}
