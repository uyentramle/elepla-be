﻿using Elepla.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Repository.Interfaces
{
	public interface IQuestionBankRepository : IGenericRepository<QuestionBank>
	{
		Task<QuestionBank> GetByQuestionIdAsync(string id);
	}
}
