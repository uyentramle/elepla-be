using Elepla.Service.Interfaces;
using Elepla.Service.Models.ViewModels.SubjectViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elepla.API.Controllers
{
    public class SubjectController : BaseController
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubjectAsync(string? keyword, int pageIndex = 0, int pageSize = 10)
        {
            var response = await _subjectService.GetAllSubjectAsync(keyword, pageIndex, pageSize);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectByIdAsync(string subjectId)
        {
            var response = await _subjectService.GetSubjectByIdAsync(subjectId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubjectAsync(CreateSubjectDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _subjectService.CreateSubjectAsync(model);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSubjectAsync(UpdateSubjectDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _subjectService.UpdateSubjectAsync(model);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSubjectAsync(string subjectId)
        {
            var response = await _subjectService.DeleteSubjectAsync(subjectId);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
