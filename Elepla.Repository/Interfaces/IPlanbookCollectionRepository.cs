﻿using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
    public interface IPlanbookCollectionRepository : IGenericRepository<PlanbookCollection>
    {
		Task<bool> CheckPlanbookCollectionIsSavedExistByTeacherId(string teacherId);
        Task<bool> CheckCollectionByName(string collectionName, string teacherId);
    }
}
