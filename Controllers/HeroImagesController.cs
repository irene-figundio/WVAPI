using AI_Integration.DataAccess.Database.Models;
using AI_Integration.DataAccess.Database.Repositories.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AI_Integration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HeroImagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public HeroImagesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HeroImage>>> GetHeroImages()
        {
            return await _unitOfWork.Query<HeroImage>()
                .Where(h => h.IsDeleted != true)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHeroImage(int id)
        {
            var hero = await _unitOfWork.Repository<HeroImage>().GetByIdAsync(id);
            if (hero == null) return NotFound();

            hero.IsDeleted = true;
            hero.DeletionDate = DateTime.UtcNow;
            _unitOfWork.Repository<HeroImage>().Update(hero);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}
