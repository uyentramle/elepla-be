﻿using Elepla.Domain.Entities;
using Elepla.Repository.Data;
using Elepla.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Repositories
{
    public class QuestionInExamRepository : IQuestionInExamRepository
    {
        private readonly AppDbContext _dbContext;

        public QuestionInExamRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}