using DictionaryBackend.Data;
using DictionaryBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DictionaryBackend.Controllers
{
    [ApiController]
    [Route("api/words")]
    public class WordController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext dataContext = dataContext;

        [HttpGet]
        public OkObjectResult Get() => Ok(dataContext.Words.Include(x => x.Associations).Select(MapWord));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var word = await dataContext.Words.Include(x => x.Associations).FirstOrDefaultAsync(x => x.Id == id);
            if (word == null)
                return NotFound();

            return Ok(MapWord(word));
        }

        [HttpPost]
        public async Task<IActionResult> Post(WordDTO dto) => Ok(await AddToDB(dto));

        [HttpPost("range")]
        public async Task<IActionResult> Post(IEnumerable<WordSimpleDTO> simpleDtos)
        {
            IEnumerable<WordDTO> dtos = simpleDtos.Select(x => new WordDTO(0, x.Name, x.Associations.GroupBy(x => x).Select(x => new AssociationDTO(0, x.Key, x.Count()))));
            List<int> result = [];
            foreach (var dto in dtos)
                result.Add(await AddToDB(dto));

            return Ok(result);
        }

        [HttpPost("rough")]
        public async Task<IActionResult> Post(IEnumerable<string> words)
        {
            IEnumerable<WordDTO> dtos = words.Select(x => new WordDTO(0, x, []));
            List<int> result = [];
            foreach (var dto in dtos)
                result.Add(await AddToDB(dto));

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var word = await dataContext.Words.FirstOrDefaultAsync(x => x.Id == id);
            if (word == null)
                return NotFound();

            dataContext.Words.Remove(word);
            await dataContext.SaveChangesAsync();
            return Ok();
        }


        private async Task<int> AddToDB(WordDTO dto)
        {

            var word = new Word
            {
                Id = dto.Id,
                Name = dto.Name,
            };

            await dataContext.Words.AddAsync(word);
            await dataContext.SaveChangesAsync();

            IEnumerable<Association> newAssociations = dto.Associations.Select(x => new Association()
            {
                Name = x.Name,
                Count = x.Count,
                WordId = word.Id
            });

            await dataContext.Associations.AddRangeAsync(newAssociations);
            await dataContext.SaveChangesAsync();
            return word.Id;
        }

        private WordDTO MapWord(Word word) => new(
                word.Id,
                word.Name,
                word.Associations.Select(association => new AssociationDTO(
                     association.Id,
                     association.Name,
                     association.Count
                ))
            );

        public record AssociationDTO(int Id, string Name, int Count);
        public record WordDTO(int Id, string Name, IEnumerable<AssociationDTO> Associations);

        public record WordSimpleDTO(string Name, IEnumerable<string> Associations);
    }
}
