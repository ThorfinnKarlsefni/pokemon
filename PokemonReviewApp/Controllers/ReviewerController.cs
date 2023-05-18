using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;
        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _mapper = mapper;
            _reviewerRepository = reviewerRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.GetReviewerExists(reviewerId))
                return NotFound();

            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews")]
        public IActionResult GetReviewOfReviewer(int reviewerId)
        {
            if (!_reviewerRepository.GetReviewerExists(reviewerId))
                return NotFound();

            var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewByReviewer(reviewerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            if (reviewerCreate == null)
                return BadRequest(ModelState);

            var country = _reviewerRepository.GetReviewers().Where(r => r.LastName.Trim().ToUpper() == reviewerCreate.LastName.Trim().ToUpper()).FirstOrDefault();
            if (country != null)
            {
                ModelState.AddModelError("", "Country alread exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

            if (!_reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Something went wrong while  savin");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerDto reviewerUpdate)
        {
            if (reviewerId == null)
                return BadRequest(ModelState);

            if (reviewerId != reviewerUpdate.Id)
                return BadRequest(ModelState);

            if (!_reviewerRepository.GetReviewerExists(reviewerId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Reviewer>(reviewerUpdate);

            if (!_reviewerRepository.UpdateReviewer(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong updating owner");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.GetReviewerExists(reviewerId))
                return BadRequest(ModelState);

            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewer = _mapper.Map<Reviewer>(reviewerToDelete);

            if (!_reviewerRepository.DeleteReviewer(reviewer))
            {
                ModelState.AddModelError("", "Something went wrong deleting reviewer");
            }

            return NoContent();
        }
    }
}

