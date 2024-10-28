using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.AnswerViewModels
{
	public class CreateAnswerDTO
	{
		public string AnswerText { get; set; }
		public bool IsCorrect { get; set; }
	}
}
